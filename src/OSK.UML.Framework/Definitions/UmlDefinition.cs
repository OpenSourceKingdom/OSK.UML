using OSK.UML.Framework.Internal.Services;
using OSK.UML.Framework.Models;
using OSK.UML.Framework.Ports;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.UML.Framework.Definitions
{
    public abstract class UmlDefinition(
        string nameSpaceKeyWord,
        IEnumerable<UmlConstructIdentifier> constructIdentifiers,
        IEnumerable<string> visibilityKeyWords,
        IEnumerable<string> modifierKeyWords,
        bool methodParameterTypeIsFirst,
        IEnumerable<UmlSyntaxTemplate> umlSyntaxTemplates) : IUmlDefinition
    {
        #region Variables

        private readonly IEnumerable<UmlSyntaxTemplate> _umlSyntaxTemplates = umlSyntaxTemplates ?? throw new ArgumentNullException(nameof(umlSyntaxTemplates));
        private readonly Dictionary<string, UmlConstructType> _constructIdentifierLookup = constructIdentifiers?.ToDictionary(c => c.Name, c => c.ConstructType) ?? throw new ArgumentNullException(nameof(constructIdentifiers));
        private readonly HashSet<string> _modifierKeyWords = modifierKeyWords?.ToHashSet() ?? throw new ArgumentNullException(nameof(modifierKeyWords));
        private readonly HashSet<string> _visibilityKeyWords = visibilityKeyWords?.ToHashSet() ?? throw new ArgumentNullException(nameof(visibilityKeyWords));

        #endregion
        
        #region IUmlDefinition

        public IEnumerable<IUmlInterpreter> GetUmlInterpreters()
            => _umlSyntaxTemplates.Select(t => new UmlInterpreter(t, this));

        public bool MethodParameterTypeIsFirst => methodParameterTypeIsFirst;

        public bool IsNameSpace(string text)
        {
            return nameSpaceKeyWord != null && string.Equals(nameSpaceKeyWord, text, StringComparison.Ordinal);
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
