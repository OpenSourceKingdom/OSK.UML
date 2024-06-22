using OSK.Parsing.FileTokens;
using OSK.Parsing.FileTokens.Handlers;
using OSK.Parsing.FileTokens.Ports;
using OSK.UML.Framework.Definitions;
using OSK.UML.Framework.Ports;
using OSK.UML.Ports;

namespace OSK.UML
{
    public static class UmlFileReader
    {
        public static IUmlParser OpenRead(string filePath)
            => OpenRead(filePath, DefaultTokenStateHandler.Instance);

        public static IUmlParser OpenRead(string filePath,
            ITokenStateHandler tokenStateHandler)
            => OpenRead(filePath, tokenStateHandler, DefaultUmlDefinition.Instance);

        public static IUmlParser OpenRead(string filePath, ITokenStateHandler tokenStateHandler,
            IUmlDefinition umlDefinition)
        {
            var tokenReader = FileTokenParser.OpenRead(filePath, tokenStateHandler);
            return new Internal.Services.UmlParser(tokenReader, umlDefinition);
        }
    }
}
