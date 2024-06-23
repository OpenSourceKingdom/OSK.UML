namespace OSK.UML.Models
{    
    // Comment 1
    /// <summary>
    /// Comment 2
    /// </summary>
    /*
     * Comment 3 
     */
    public class UmlAssociation
    {
        public string Name { get; set; }

        public string AssociatedObjectType { get; set; }

        public UmlAssociationType AssociationType { get; set; }
    }
}
