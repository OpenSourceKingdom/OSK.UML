using System.Collections.Generic;

namespace OSK.UML.Models
{
    public class UmlMethod : UmlConstructMember
    {
        public IEnumerable<UmlParameter> Parameters { get; set; }
    }
}
