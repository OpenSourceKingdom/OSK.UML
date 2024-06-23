using OSK.Parsing.FileTokens.Ports;
using OSK.UML.Ports;

namespace OSK.UML.Internal.Services
{
    internal class UmlParserFactory : IUmlParserFactory
    {
        public IUmlParser CreateParser(string path, ITokenStateHandler tokenStateHandler)
        {
            return UmlFileReader.OpenRead(path, tokenStateHandler);
        }
    }
}
