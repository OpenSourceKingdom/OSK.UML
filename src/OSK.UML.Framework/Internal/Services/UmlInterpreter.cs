using OSK.Parsing.FileTokens.Models;
using OSK.UML.Framework.Models;
using OSK.UML.Framework.Ports;
using System;
using System.Collections.Generic;

namespace OSK.UML.Framework.Internal.Services
{
    internal class UmlInterpreter(UmlSyntaxTemplate template, IUmlDefinition definition)
        : IUmlInterpreter
    {
        #region Variables

        private int _ruleIndex = 0;

        private UmlInterpreterState _interpreterState = UmlInterpreterState.Initialized;
        private string _visibility = string.Empty;
        private string _type = string.Empty;
        private string _name = string.Empty;
        private List<string> _modifiers = [];
        private List<string> _inheritance = [];
        private List<UmlElementParameter> _parameters = [];
        private UmlConstructType? _constructType;

        private string _firstParameterText = null;

        #endregion

        #region IUmlInterpreter

        public int TotalRules => template.Rules.Length;

        public UmlInterpreterState State => _interpreterState;

        public UmlElementType InterpreterType => template.ElementType;

        public void Reset()
        {
            _ruleIndex = 0;
            _visibility = string.Empty;
            _type = string.Empty;
            _name = string.Empty;
            _modifiers = [];
            _inheritance = [];
            _parameters = [];
            _constructType = null;
            _firstParameterText = null;
            _interpreterState = UmlInterpreterState.Initialized;
        }

        public void AddFileToken(FileToken fileToken)
        {
            if ((_interpreterState != UmlInterpreterState.Initialized
                 && _interpreterState != UmlInterpreterState.Parsing)
                 || CanIgnoreToken(fileToken))
            {
                return;
            }
            if (template.ElementType == UmlElementType.Method &&
                fileToken.TokenType == FileTokenType.EndOfStatement)
            {
                _interpreterState = UmlInterpreterState.Invalid;
                return;
            }

            var currentRule = GetSyntaxRule(_ruleIndex);
            if (currentRule == null)
            {
                _interpreterState = UmlInterpreterState.Invalid;
                return;
            }

            var isTokenValidForRule = ValidateTokenForSyntax(fileToken, currentRule);
            if (!isTokenValidForRule && currentRule.Optional)
            {
                currentRule = GetSyntaxRule(_ruleIndex + 1);
                if (currentRule == null)
                {
                    _interpreterState = UmlInterpreterState.Satisfied;
                    return;
                }

                _ruleIndex++;
                isTokenValidForRule = ValidateTokenForSyntax(fileToken, currentRule);
            }
            if (!isTokenValidForRule)
            {
                Reset();
                return;
            }

            SetStateForValidTokenSyntax(fileToken, currentRule);
            _ruleIndex = currentRule.AllowMultipleTokens
                ? _ruleIndex
                : _ruleIndex + 1;

            _interpreterState = _ruleIndex >= template.Rules.Length
                ? UmlInterpreterState.Satisfied : UmlInterpreterState.Parsing;
        }

        public UmlElement GetElement()
        {
            if (_interpreterState != UmlInterpreterState.Satisfied)
            {
                return null;
            }

            return new UmlElement()
            {
                ConstructType = _constructType,
                UmlType = template.ElementType,
                Name = _name,
                Type = _type,
                Visibility = _visibility,
                Inheritance = _inheritance,
                Modifiers = _modifiers,
                Parameters = _parameters
            };
        }

        #endregion

        #region Helpers

        private bool CanIgnoreToken(FileToken fileToken)
            => fileToken == null || fileToken.TokenType switch
            {
                FileTokenType.Delimeter => true,
                FileTokenType.Ignore => true,
                _ => false
            };

        private void SetStateForValidTokenSyntax(FileToken token, UmlSyntaxRule rule)
        {
            switch (rule.RuleType)
            {
                case SyntaxRuleType.Visibility:
                    _visibility = token.ToString();
                    break;
                case SyntaxRuleType.Type:
                    _type = token.ToString();
                    break;
                case SyntaxRuleType.Name:
                    _name = token.ToString();
                    break;
                case SyntaxRuleType.ConstructIdentifier:
                    var tokenString = token.ToString();
                    if (definition.TryGetUmlConstructType(tokenString, out var identifier))
                    {
                        _type = tokenString;
                        _constructType = identifier;
                    }
                    break;
                case SyntaxRuleType.Modifiers:
                    _modifiers.Add(token.ToString());
                    break;
                case SyntaxRuleType.Inheritance:
                    _inheritance.Add(token.ToString());
                    break;
                case SyntaxRuleType.Parameters:
                    if (token.TokenType == FileTokenType.ClosureStart)
                    {
                        return;
                    }
                    if (_firstParameterText == null)
                    {
                        _firstParameterText = token.ToString();
                    }
                    else
                    {
                        _parameters.Add(new UmlElementParameter()
                        {
                            Name = definition.MethodParameterTypeIsFirst ? token.ToString() : _firstParameterText,
                            Type = definition.MethodParameterTypeIsFirst ? _firstParameterText : token.ToString(),
                        });
                        _firstParameterText = null;
                    }
                    break;
            }
        }

        private bool ValidateTokenForSyntax(FileToken fileToken, UmlSyntaxRule syntaxRule)
        {
            var matchesRule = syntaxRule.RuleType switch
            {
                SyntaxRuleType.NameSpace => fileToken.TokenType == FileTokenType.Text && definition.IsNameSpace(fileToken.ToString()),
                SyntaxRuleType.Parameters => fileToken.TokenType == FileTokenType.ClosureStart || fileToken.TokenType == FileTokenType.Text,
                SyntaxRuleType.Type => fileToken.TokenType == FileTokenType.Text,
                SyntaxRuleType.Name => fileToken.TokenType == FileTokenType.Text,
                SyntaxRuleType.Inheritance => fileToken.TokenType == FileTokenType.Text,
                SyntaxRuleType.Modifiers => definition.IsModifier(fileToken.ToString()),
                SyntaxRuleType.Visibility => definition.IsVisibility(fileToken.ToString()),
                SyntaxRuleType.ConstructIdentifier => definition.TryGetUmlConstructType(fileToken.ToString(), out _),
                _ => throw new InvalidOperationException($"Unexpected rule type was used: {syntaxRule.RuleType}.")
            };

            return matchesRule;
        }

        private UmlSyntaxRule GetSyntaxRule(int index)
            => index >= template.Rules.Length
             ? null : template.Rules[index];

        #endregion
    }
}
