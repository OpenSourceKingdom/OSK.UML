using OSK.Parsing.FileTokens.Models;
using OSK.Parsing.FileTokens.Ports;
using OSK.UML.Framework.Models;
using OSK.UML.Framework.Ports;
using OSK.UML.Models;
using OSK.UML.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.UML.Internal.Services
{
    internal class UmlParser(IFileTokenReader tokenReader, IUmlDefinition umlDefinition)
        : IUmlParser
    {
        #region Variables

        private string _nameSpace;

        #endregion

        #region IUmlParser

        public async Task<IEnumerable<UmlComponent>> ParseUmlAsync(CancellationToken cancellationToken = default)
        {
            var components = new List<UmlComponent>();

            while (true)
            {
                var component = await ReadUmlComponentAsync(cancellationToken);
                if (component == null)
                {
                    break;
                }

                components.Add(component);
            }

            return components;
        }

        public void Dispose()
        {
            tokenReader.Dispose();
        }

        #endregion

        #region Helpers

        private async Task<UmlComponent> ReadUmlComponentAsync(CancellationToken cancellationToken)
        {
            var interpreters = umlDefinition.GetUmlInterpreters();
            var constructMemberInterpreters = interpreters.Where(i => i.InterpreterType == UmlElementType.Method
             || i.InterpreterType == UmlElementType.Variable).ToArray();
            var constructInterpreters = interpreters.Where(i => i.InterpreterType == UmlElementType.Construct)
                .ToArray();
            var nameSpaceInterpreter = interpreters.FirstOrDefault(i => i.InterpreterType == UmlElementType.NameSpace);

            while (true)
            {
                var fileToken = await tokenReader.ReadTokenAsync(cancellationToken);
                if (fileToken.TokenType == FileTokenType.EndOfFile)
                {
                    return null;
                }
                switch (fileToken.TokenType)
                {
                    case FileTokenType.Text:
                    case FileTokenType.EndOfStatement:
                    case FileTokenType.ClosureStart:
                        if (nameSpaceInterpreter != null)
                        {
                            nameSpaceInterpreter.AddFileToken(fileToken);
                            if (nameSpaceInterpreter.State == UmlInterpreterState.Satisfied)
                            {
                                _nameSpace = nameSpaceInterpreter.GetElement()!.Name;
                                nameSpaceInterpreter = null;
                                continue;
                            }
                        }
                        IUmlInterpreter completedInterpreter = null;
                        foreach (var interpreter in constructInterpreters)
                        {
                            interpreter.AddFileToken(fileToken);
                            if (interpreter.State == UmlInterpreterState.Satisfied)
                            {
                                completedInterpreter = interpreter;
                                break;
                            }
                        }
                        if (completedInterpreter != null)
                        {
                            var umlElement = completedInterpreter.GetElement();
                            var component = umlElement!.ConstructType! switch
                            {
                                UmlConstructType.Object => await ReadUmlConstructAsync(umlElement, constructMemberInterpreters, cancellationToken),
                                UmlConstructType.ValueList => await ReadUmlValueListAsync(umlElement, cancellationToken),
                                _ => (UmlComponent)null
                            };

                            return component is null
                                ? throw new InvalidOperationException($"Unable to interpret a uml component with an unexpected file format.")
                                : component;
                        }
                        break;
                }
            }
        }

        private async Task<UmlConstruct> ReadUmlConstructAsync(UmlElement mainUmlElement,
            IUmlInterpreter[] memberInterpreters, CancellationToken cancellationToken)
        {
            var variables = new List<UmlVariable>();
            var methods = new List<UmlMethod>();
            var openClosures = 1;

            var completedInterpreters = new HashSet<int>();
            while (true)
            {
                var fileToken = await tokenReader!.ReadTokenAsync(cancellationToken);
                if (fileToken.TokenType == FileTokenType.ClosureEnd && openClosures == 0
                     || fileToken.TokenType == FileTokenType.EndOfFile)
                {
                    break;
                }

                switch (fileToken.TokenType)
                {
                    case FileTokenType.Assignment:
                        await tokenReader.ReadToEndTokenAsync(fileToken, cancellationToken);
                        break;
                    case FileTokenType.Text:
                    case FileTokenType.EndOfStatement:
                    case FileTokenType.ClosureStart:
                    case FileTokenType.ClosureEnd:
                        if (fileToken.TokenType == FileTokenType.ClosureStart)
                        {
                            openClosures++;
                        }
                        else if (fileToken.TokenType == FileTokenType.ClosureEnd)
                        {
                            openClosures--;
                        }

                        for (var i = 0; i < memberInterpreters.Length; i++)
                        {
                            var interpreter = memberInterpreters[i];
                            interpreter.AddFileToken(fileToken);
                            if (interpreter.State != UmlInterpreterState.Parsing)
                            {
                                completedInterpreters.Add(i);
                            }
                        }

                        if (completedInterpreters.Count == memberInterpreters.Length)
                        {
                            completedInterpreters.Clear();
                            IUmlInterpreter umlInterpreter = null;
                            foreach (var interpreter in memberInterpreters.Where(i => i.State == UmlInterpreterState.Satisfied))
                            {
                                if (umlInterpreter == null || interpreter.TotalRules > umlInterpreter.TotalRules)
                                {
                                    umlInterpreter = interpreter;
                                }
                            }
                            if (umlInterpreter != null)
                            {
                                var umlElement = umlInterpreter.GetElement();
                                switch (umlElement!.UmlType)
                                {
                                    case UmlElementType.Method:
                                        methods.Add(new UmlMethod()
                                        {
                                            Modifiers = umlElement.Modifiers,
                                            Name = umlElement.Name,
                                            Type = umlElement.Type,
                                            Visibility = umlElement.Visibility,
                                            Parameters = umlElement.Parameters
                                                .Select(p => new UmlParameter()
                                                {
                                                    Name = p.Name,
                                                    Type = p.Type
                                                })
                                        });
                                        break;
                                    case UmlElementType.Variable:
                                        variables.Add(new UmlVariable()
                                        {
                                            Modifiers = umlElement.Modifiers,
                                            Name = umlElement.Name,
                                            Visibility = umlElement.Visibility,
                                            Type = umlElement.Type
                                        });
                                        break;
                                }
                            }

                            foreach (var interpreter in memberInterpreters)
                            {
                                interpreter.Reset();
                            }
                        }
                        break;
                }
            }

            return new UmlConstruct()
            {
                Namespace = _nameSpace,
                Name = mainUmlElement.Name,
                Inheritance = mainUmlElement.Inheritance,
                Modifiers = mainUmlElement.Modifiers,
                Type = mainUmlElement.Type,
                Visibility = mainUmlElement.Visibility,
                Variables = variables,
                Methods = methods
            };
        }

        private async Task<UmlValueList> ReadUmlValueListAsync(UmlElement element, CancellationToken cancellationToken)
        {
            var values = new List<string>();
            while (true)
            {
                var fileToken = await tokenReader.ReadTokenAsync(cancellationToken);
                if (fileToken.TokenType == FileTokenType.ClosureEnd
                     || fileToken.TokenType == FileTokenType.EndOfFile)
                {
                    break;
                }
                if (fileToken.TokenType == FileTokenType.Text)
                {
                    values.Add(fileToken.ToString());
                }
            }

            return new UmlValueList()
            {
                Namespace = _nameSpace,
                Name = element.Name,
                Modifiers = element.Modifiers,
                Visibility = element.Visibility,
                Values = values
            };
        }

        #endregion
    }
}