using OSK.Hexagonal.MetaData;
using OSK.Parsing.FileTokens.Models;
using OSK.UML.Framework.Models;

namespace OSK.UML.Framework.Ports
{
    /// <summary>
    /// Acts as interpreter of file tokens using a <see cref="IUmlDefinition"/>. The interpreter is meant to process file tokens and maintain state information for Uml parsing
    /// </summary>
    [HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
    public interface IUmlInterpreter
    {
        /// <summary>
        /// Represents the total number of <see cref="UmlSyntaxRule"/>s associated to the interpreter
        /// </summary>
        int TotalRules { get; }

        /// <summary>
        /// The specific <see cref="UmlElementType"/> the interpreter parses
        /// </summary>
        UmlElementType InterpreterType { get; }

        /// <summary>
        /// The current <see cref="UmlInterpreterState"/> state of this interpreter
        /// </summary>
        UmlInterpreterState State { get; }

        /// <summary>
        /// Resets the interpreter state so that file tokens can be processed from a fresh, initialized, state using <see cref="AddFileToken(FileToken)"/>
        /// </summary>
        void Reset();

        /// <summary>
        /// Adds a <see cref="FileToken"/> to the interpreter's state for processing
        /// </summary>
        /// <param name="fileToken">The file token that was parsed</param>
        void AddFileToken(FileToken fileToken);

        /// <summary>
        /// Attempts to retrieve the current parsed <see cref="UmlElement"/>, if the <see cref="UmlInterpreterState"/> is satisfied
        /// </summary>
        /// <returns>A <see cref="UmlElement"/> if the interpreter has completed processing or null if it hasn't</returns>
        UmlElement GetElement();
    }
}
