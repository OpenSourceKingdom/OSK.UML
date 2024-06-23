namespace OSK.UML.Models
{
    public class UmlGenerationResult
    {
        public int TotalFilesChecked { get; set; }

        public int TotalUmlObjectsDiscovered { get; set; }

        public UmlDiagram UmlDiagram { get; set; }
    }
}
