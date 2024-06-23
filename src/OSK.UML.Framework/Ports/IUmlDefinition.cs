using OSK.UML.Framework.Models;
using System.Collections.Generic;

namespace OSK.UML.Framework.Ports
{
    public interface IUmlDefinition
    {
        IEnumerable<IUmlInterpreter> GetUmlInterpreters();
        bool MethodParameterTypeIsFirst { get; }

        bool IsNameSpace(string text);
        bool IsVisibility(string text);
        bool IsModifier(string text);
        bool TryGetUmlConstructType(string text, out UmlConstructType constructType);
    }
}
