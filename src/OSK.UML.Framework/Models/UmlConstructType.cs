namespace OSK.UML.Framework.Models
{
    public enum UmlConstructType
    {
        /// <summary>
        /// An object within some file based on user configuration. This is able to hold information relating to <see cref="UmlElementType.Method"/> and <see cref="UmlElementType.Variable"/>.
        /// </summary>
        Object,
        /// <summary>
        /// A list of string deliminated values based on user configuration. This closely matches what an enum would be in C#.
        /// </summary>
        ValueList
    }
}
