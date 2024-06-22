namespace OSK.UML.Exporters.PlantUML.Internal
{
    public static class PlantUmlConstants
    {
        public const string StartUml = "@startuml";
        public const string EndUml = "@enduml";

        public const string ExtensionUml_Dashed = "<|--";
        public const string CompositionUml_Dashed = "*--";
        public const string AggregationUml_Dashed = "o--";
        public const string ExtensionUml_Dotted = "<|..";
        public const string CompositionUml_Dotted = "*..";
        public const string AggregationUml_Dotted = "o..";

        public const string PrivateSymbolUml = "-";
        public const string PublicSymbolUml = "+";
        public const string ProtectedSymbolUml = "#";
    }
}
