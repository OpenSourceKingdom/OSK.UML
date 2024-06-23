using System.Collections.Generic;

namespace OSK.UML.Models
{
    public abstract class UmlConstructMember
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string Visibility { get; set; }

        public IEnumerable<string> Modifiers { get; set; }
    }
}
