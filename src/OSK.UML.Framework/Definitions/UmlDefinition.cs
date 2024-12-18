using OSK.UML.Framework.Internal.Services;
using OSK.UML.Framework.Models;
using OSK.UML.Framework.Ports;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.UML.Framework.Definitions
{
    public abstract class UmlDefinition : IUmlDefinition
    {
        #region Variables

        private readonly string _nameSpaceKeyWord;
        private readonly bool _methodParameterTypeIsFirst;
        private readonly IEnumerable<UmlSyntaxTemplate> _umlSyntaxTemplates;
        private readonly Dictionary<string, UmlConstructType> _constructIdentifierLookup;
        private readonly HashSet<string> _modifierKeyWords;
        private readonly HashSet<string> _visibilityKeyWords;

        #endregion

        #region Constructors

        protected UmlDefinition(
            string nameSpaceKeyWord,
            IEnumerable<UmlConstructIdentifier> constructIdentifiers,
            IEnumerable<string> visibilityKeyWords,
            IEnumerable<string> modifierKeyWords,
            bool methodParameterTypeIsFirst,
            IEnumerable<UmlSyntaxTemplate> umlSyntaxTemplates)
        {
            _nameSpaceKeyWord = nameSpaceKeyWord;
            _constructIdentifierLookup = constructIdentifiers?.ToDictionary(c => c.Name, c => c.ConstructType) ?? throw new ArgumentNullException(nameof(constructIdentifiers));
            _visibilityKeyWords = visibilityKeyWords?.ToHashSet() ?? throw new ArgumentNullException(nameof(visibilityKeyWords));
            _modifierKeyWords = modifierKeyWords?.ToHashSet() ?? throw new ArgumentNullException(nameof(modifierKeyWords));
            _methodParameterTypeIsFirst = methodParameterTypeIsFirst;
            _umlSyntaxTemplates = umlSyntaxTemplates ?? throw new ArgumentNullException(nameof(umlSyntaxTemplates));
        }

        #endregion

        #region IUmlDefinition

        public IEnumerable<IUmlInterpreter> GetUmlInterpreters()
            => _umlSyntaxTemplates.Select(t => new UmlInterpreter(t, this));

        public bool MethodParameterTypeIsFirst => _methodParameterTypeIsFirst;

        public bool IsNameSpace(string text)
        {
            return _nameSpaceKeyWord != null && string.Equals(_nameSpaceKeyWord, text, StringComparison.Ordinal);
        }

        public bool IsVisibility(string text)
        {
            return _visibilityKeyWords.Contains(text);
        }

        public bool IsModifier(string text)
        {
            return _modifierKeyWords.Contains(text);
        }

        public bool TryGetUmlConstructType(string text, out UmlConstructType constructType)
        {
            return _constructIdentifierLookup.TryGetValue(text, out constructType);
        }

        #endregion
    }
}
