namespace OSK.UML.Framework.Models
{
    public class UmlConstructIdentifier
    {
        internal static UmlConstructIdentifier None = new UmlConstructIdentifier()
        {
            ConstructType = UmlConstructType.Object,
            Name = string.Empty
        };

        public string Name { get; set; }

        public UmlConstructType ConstructType { get; set; }
    }
}
