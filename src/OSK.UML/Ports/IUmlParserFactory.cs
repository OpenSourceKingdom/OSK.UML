using OSK.Parsing.FileTokens.Ports;

namespace OSK.UML.Ports
{
    public interface IUmlParserFactory
    {
        /// <summary>
        /// Create a <see cref="IUmlParser"/> for the specified path.
        /// </summary>
        /// <param name="path">The path to target the Uml Parser to</param>
        /// <param name="tokenStateHandler">The token state handler that will be used for parsing the Uml diagram</param>
        /// <returns>A Uml parser that uses the providedd token state handler</returns>
        IUmlParser CreateParser(string path, ITokenStateHandler tokenStateHandler);
    }
}
