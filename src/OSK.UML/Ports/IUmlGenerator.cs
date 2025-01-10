using OSK.Functions.Outputs.Abstractions;
using OSK.Hexagonal.MetaData;
using OSK.UML.Models;
using OSK.UML.Options;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.UML.Ports
{
    [HexagonalIntegration(HexagonalIntegrationType.LibraryProvided, HexagonalIntegrationType.ConsumerPointOfEntry)]
    public interface IUmlGenerator
    {
        /// <summary>
        /// Generates a <see cref="UmlGenerationResult"/> given a file path and a domain name for the target
        /// </summary>
        /// <param name="path">The path to either a directory or file that a </param>
        /// <param name="domainName">The domain name for the UML Diagram</param>
        /// <param name="options">Generation options for the result</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>The result of the generation</returns>
        Task<IOutput<UmlGenerationResult>> GenerateUmlAsync(string path, string domainName, UmlGenerationOptions options, CancellationToken cancellationToken = default);
    }
}
