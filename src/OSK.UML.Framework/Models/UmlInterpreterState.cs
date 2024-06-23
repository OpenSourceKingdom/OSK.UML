namespace OSK.UML.Framework.Models
{
    public enum UmlInterpreterState
    {
        /// <summary>
        /// Signifies that the inerpeter has not yet received input for state information to be determined
        /// </summary>
        Initialized,
        /// <summary>
        /// The current state of the interpreter is in a bad state and a valid <see cref="UmlElement"/> can not be created from the current state.
        /// </summary>
        Invalid,
        /// <summary>
        /// The interpreter is currently parsing state information and a determination for whether a <see cref="UmlElement"/> can be created has yet to be made and more input is reqired to do so.
        /// </summary>
        Parsing,
        /// <summary>
        /// The interpreter is able to create a valid <see cref="UmlElement"/> based on the currnet information that has been provided. Further input while in this state could potentially invalidate the element and should be retrieved and the interpreter reset prior to reading more data.
        /// </summary>
        Satisfied
    }
}
