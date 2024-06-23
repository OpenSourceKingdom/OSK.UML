using System.Collections.Generic;

namespace OSK.UML.Framework.Models
{
    public class UmlElement
    {
        public string Name { get; set; }

        public UmlElementType UmlType { get; set; }

        public string Type { get; set; }

        public string Visibility { get; set; }

        public IEnumerable<string> Inheritance { get; set; }

        public IEnumerable<string> Modifiers { get; set; }

        public UmlConstructType? ConstructType { get; set; }

        public IEnumerable<UmlElementParameter> Parameters { get; set; }
    }
}
