using OSK.Parsing.FileTokens.Ports;

namespace OSK.UML.Ports
{
    public interface IUmlParserFactory
    {
        IUmlParser CreateParser(string path, ITokenStateHandler tokenStateHandler);
    }
}
