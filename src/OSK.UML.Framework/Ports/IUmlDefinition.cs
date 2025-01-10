using OSK.Hexagonal.MetaData;
using OSK.UML.Framework.Models;
using System.Collections.Generic;

namespace OSK.UML.Framework.Ports
{
    /// <summary>
    /// A Uml definition describes the different attributes a Uml diagram can possess.
    /// </summary>
    [HexagonalIntegration(HexagonalIntegrationType.IntegrationOptional)]
    public interface IUmlDefinition
    {
        /// <summary>
        /// Returns a list of <see cref="IUmlInterpreter"/> that can help to interpret the data parsed from a file token reader using this definition
        /// </summary>
        /// <returns>The Uml interpreters for this definition</returns>
        IEnumerable<IUmlInterpreter> GetUmlInterpreters();

        /// <summary>
        /// Whether the parameters to a method are described using (Type name, ..) or (name Type, ...)
        /// </summary>
        bool MethodParameterTypeIsFirst { get; }

        /// <summary>
        /// Determines if the text represents a namespace schema
        /// </summary>
        /// <param name="text">The string text that was parsed</param>
        /// <returns>Whether the text is a representation of a namespace</returns>
        bool IsNameSpace(string text);
        
        /// <summary>
        /// Determines if the text represents a visbility scope. Visibility is the accessibility for a given class, function, or other property to outside callers.
        /// </summary>
        /// <param name="text">The string text that was parsed</param>
        /// <returns>Whether the text is a representation of a visibility attribute</returns>
        bool IsVisibility(string text);

        /// <summary>
        /// Determines if the text represents a modifier attribute. Modifiers are additional descriptive syntax that helps to define aspects of a property or other memebr
        /// </summary>
        /// <param name="text">The string text that was parsed.</param>
        /// <returns>Whether the text is a representation of a modifier attribute</returns>
        bool IsModifier(string text);

        /// <summary>
        /// Attempts to create a <see cref="UmlConstructType"/> from a given text and returning if the creation was successful
        /// </summary>
        /// <param name="text">The text that was pased</param>
        /// <param name="constructType">The construct type, if the parsing was successful</param>
        /// <returns>Whether the parsing to the <see cref="UmlConstructType"/> was successful and the value returned is a valid result</returns>
        bool TryGetUmlConstructType(string text, out UmlConstructType constructType);
    }
}
