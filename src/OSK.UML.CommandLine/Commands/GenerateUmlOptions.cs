using CommandLine;

namespace OSK.UML.CommandLine.Commands
{
    [Verb("generate", HelpText = "Generates a UML diagram based on the provided options and target.")]
    public class GenerateUmlOptions
    {
        [Option('p', "path", Required = true, HelpText = "Specifies the path to a file or directory to be parsed.")]
        public required string Path { get; set; }

        [Option("fileTypes", Default = new[] { ".cs" }, HelpText = "Filter for the types of files that will be included in the uml diagram.")]
        public required IEnumerable<string> FileTypes { get; set; }

        [Option('d', "directoryDepth", Default = 1, HelpText = "How deep should the generator look for files, within a directory, when creating the UML.")]
        public required int DirectoryDepth { get; set; }

        [Option('n', "name", Default = "domainModel", HelpText = "The name of the output file that will be generated for the UML diagram.")]
        public required string Name { get; set; }

        [Option('o', "outputDirectory", Default = ".", HelpText = "Sets the output path to use for the exported uml diagram.")]
        public required string OutputPath { get; set; }

        [Option('v', "verbose", Default = false, HelpText = "Enable verbose logging.")]
        public required bool EnableVerboseLogging { get; set; }

        [Option("plantUmlPath", Default = null, HelpText = "Override the path location for the plant uml jar file that will be used to create UML diagrams.")]
        public string? PlantUmlPathOverride { get; set; }
    }
}
