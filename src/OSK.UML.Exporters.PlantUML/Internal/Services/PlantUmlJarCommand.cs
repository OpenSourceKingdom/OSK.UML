using Microsoft.Extensions.Logging;
using OSK.Utilities.JarRunner;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.UML.Exporters.PlantUML.Internal.Services
{
    public sealed class PlantUmlJarCommand(string domainName, string domainModel,
        string outputDirectory, ILogger<PlantUmlJarCommand> logger) : JarCommand<byte[]>
    {
        #region Variables

        private string _outputFilePath = string.Empty;
        private string _outputImagePath = string.Empty;

        #endregion

        #region Overrides

        protected override async ValueTask<byte[]> GetResultAsync(Process process, CancellationToken cancellationToken)
        {
            return await File.ReadAllBytesAsync(_outputImagePath, cancellationToken);
        }

        protected override IEnumerable<string> GetArgumentList()
        {
            string[] arguments = ["-failfast2", "-DPLANTUML_LIMIT_SIZE=8192", _outputFilePath, $"-o {outputDirectory}"];
            logger.LogDebug("PlantUml Arguments: {args}", string.Join(", ", arguments));

            return arguments;
        }

        protected override async ValueTask PrepareAsync(CancellationToken cancellationToken = default)
        {
            Directory.CreateDirectory(outputDirectory);

            _outputFilePath = Path.Combine(outputDirectory, $"{domainName}.txt");
            _outputImagePath = Path.Combine(outputDirectory, $"{domainName}.png");

            logger.LogDebug("Output directory: {directory}, text file path: {textFilePath}  Image Path: {imagePath}",
                outputDirectory, _outputFilePath, _outputImagePath);

            await File.WriteAllTextAsync(_outputFilePath, domainModel, cancellationToken);
        }

        #endregion
    }
}
