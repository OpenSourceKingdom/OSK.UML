using System.Collections.Generic;

namespace OSK.UML.Models
{
    public class UmlConstruct : UmlComponent
    {
        public string Type { get; set; }

        public IEnumerable<string> Inheritance { get; set; }

        public IEnumerable<UmlVariable> Variables { get; set; }

        public IEnumerable<UmlMethod> Methods { get; set; }
    }
}
