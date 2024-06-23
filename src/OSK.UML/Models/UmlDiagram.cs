using System.Collections.Generic;

namespace OSK.UML.Models
{
    public class UmlDiagram
    {
        public string Name { get; set; }

        public IEnumerable<UmlComponent> UmlObjects { get; set; }

        public IEnumerable<UmlAssociation> UmlAssociations { get; set; }
    }
}
