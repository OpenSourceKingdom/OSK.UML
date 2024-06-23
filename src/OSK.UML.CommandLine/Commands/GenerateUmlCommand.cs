using Microsoft.Extensions.Logging;
using OSK.Functions.Outputs.Abstractions;
using OSK.UML.Options;
using OSK.UML.Ports;
using System.Diagnostics;

namespace OSK.UML.CommandLine.Commands
{
    public class GenerateUmlCommand(GenerateUmlOptions options,
        IUmlGenerator generator, IUmlExporter exporter,
        ILogger<GenerateUmlCommand> logger)
    {
        #region GenerateUmlCommand

        public async Task<IOutput> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Generating UML diagram for {path} with a depth of {depth}...", options.Path, options.DirectoryDepth);
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var generationResult = await generator.GenerateUmlAsync(options.Path, options.Name, new UmlGenerationOptions()
            {
                DirectoryDepth = options.DirectoryDepth,
                FileExtensionPattern = string.Join(",", options.FileTypes)
            }, cancellationToken);
            stopwatch.Stop();

            logger.LogDebug("Uml generation completed in {seconds} seconds.", Math.Round(stopwatch.Elapsed.TotalSeconds, 3));
            if (!generationResult.IsSuccessful)
            {
                logger.LogError("Uml generation failure: {error}", generationResult.GetErrorString());
                return generationResult;
            }
            logger.LogInformation("Uml generation successful. Parsed {umlObjectCount} uml objects from {fileCount} files discovered.", generationResult.Value.TotalUmlObjectsDiscovered, generationResult.Value.TotalFilesChecked);

            logger.LogDebug("Exporting UML diagram to {outputPath}...", options.OutputPath);

            stopwatch.Restart();
            var exportResult = await exporter.ExportAsync(options.OutputPath, generationResult.Value.UmlDiagram, cancellationToken);
            stopwatch.Stop();

            logger.LogDebug("Uml export completed in {seconds} seconds.", Math.Round(stopwatch.Elapsed.TotalSeconds, 3));
            if (!generationResult.IsSuccessful)
            {
                logger.LogError("Uml export failure: {error}", exportResult.GetErrorString());
                return generationResult;
            }
            logger.LogInformation("Uml export successful.");

            return exportResult;
        }

        #endregion
    }
}
