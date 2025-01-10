using OSK.Functions.Outputs.Abstractions;
using OSK.Hexagonal.MetaData;
using OSK.UML.Models;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.UML.Ports
{
    [HexagonalIntegration(HexagonalIntegrationType.IntegrationRequired)]
    public interface IUmlExporter
    {
        /// <summary>
        /// Exports the <see cref="UmlDiagram"/> to a file
        /// </summary>
        /// <param name="path">The file that diagram will be saved to</param>
        /// <param name="diagram">The diagram being saved</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>The output of the export call</returns>
        Task<IOutput> ExportAsync(string path, UmlDiagram diagram, CancellationToken cancellationToken = default);
    }
}
