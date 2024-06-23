using OSK.Parsing.FileTokens.Models;
using OSK.UML.Framework.Models;

namespace OSK.UML.Framework.Ports
{
    public interface IUmlInterpreter
    {
        int TotalRules { get; }

        UmlElementType InterpreterType { get; }

        UmlInterpreterState State { get; }

        void Reset();

        void AddFileToken(FileToken fileToken);

        UmlElement? GetElement();
    }
}
