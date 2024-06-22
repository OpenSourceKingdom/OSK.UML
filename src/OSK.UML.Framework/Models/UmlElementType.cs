namespace OSK.UML.Framework.Models
{
    public enum UmlElementType
    {
        /// <summary>
        /// A function body within a <see cref="UmlConstructType.Object"/>
        /// </summary>
        Method,
        /// <summary>
        /// A property or field within a <see cref="UmlConstructType.Object"/>
        /// </summary>
        Variable,
        /// <summary>
        /// An object of some kind within <see cref="UmlConstructType"/>
        /// </summary>
        Construct,
        /// <summary>
        /// The namespace for other elements that are being interpreted within the same scope
        /// </summary>
        NameSpace
    }
}
