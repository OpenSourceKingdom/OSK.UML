using System.Collections.Generic;

namespace OSK.UML.Models
{
    public class UmlValueList : UmlComponent
    {
        public IEnumerable<string> Values { get; set; }
    }
}
