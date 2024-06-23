using System.Collections.Generic;

namespace OSK.UML.Models
{
    public abstract class UmlComponent
    {
        public string Namespace { get; set; }

        public string Name { get; set; }

        public string Visibility { get; set; }

        public IEnumerable<string> Modifiers { get; set; }
    }
}
