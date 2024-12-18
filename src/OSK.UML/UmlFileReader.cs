using OSK.Parsing.FileTokens;
using OSK.Parsing.FileTokens.Handlers;
using OSK.Parsing.FileTokens.Ports;
using OSK.UML.Framework.Definitions;
using OSK.UML.Framework.Ports;
using OSK.UML.Ports;

namespace OSK.UML
{
    /// <summary>
    /// Provides a utility to create <see cref="IUmlParser"/> objects
    /// </summary>
    public static class UmlFileReader
    {
        /// <summary>
        /// Create a <see cref="IUmlParser"/> that reads the file path using a default Token state handler and Uml definition
        /// </summary>
        /// <param name="filePath">The path to a file to parse</param>
        /// <returns>The parser targeted to the provided file path</returns>
        public static IUmlParser OpenRead(string filePath)
            => OpenRead(filePath, DefaultTokenStateHandler.Instance);

        /// <summary>
        /// Create a <see cref="IUmlParser"/> that reads the file path using a custom token state handler and a default uml definition
        /// </summary>
        /// <param name="filePath">The file path to parse</param>
        /// <param name="tokenStateHandler">The custom token state handler to use with the file parsing</param>
        /// <returns>The parser targeted to the provided file path</returns>
        public static IUmlParser OpenRead(string filePath,
            ITokenStateHandler tokenStateHandler)
            => OpenRead(filePath, tokenStateHandler, DefaultUmlDefinition.Instance);


        /// <summary>
        /// Create a <see cref="IUmlParser"/> that reads the file path using a custom uml definition and a custom token state handler
        /// </summary>
        /// <param name="filePath">The file path to parse</param>
        /// <param name="umlDefinition">The custom uml definition to interpret the uml diagram</param>
        /// <returns>The parser targeted to the provided file path</returns>
        public static IUmlParser OpenRead(string filePath,
            IUmlDefinition umlDefinition)
            => OpenRead(filePath, DefaultTokenStateHandler.Instance, umlDefinition);

        /// <summary>
        /// Create a <see cref="IUmlParser"/> that reads the file path using a custom uml definition and a custom token state handler
        /// </summary>
        /// <param name="filePath">The file path to parse</param>
        /// <param name="tokenStateHandler">The custom token state handler to use with the file parsing</param>
        /// <param name="umlDefinition">The custom uml definition to interpret the uml diagram</param>
        /// <returns>The parser targeted to the provided file path</returns>
        public static IUmlParser OpenRead(string filePath, ITokenStateHandler tokenStateHandler,
            IUmlDefinition umlDefinition)
        {
            var tokenReader = FileTokenParser.OpenRead(filePath, tokenStateHandler);
            return new Internal.Services.UmlParser(tokenReader, umlDefinition);
        }
    }
}
