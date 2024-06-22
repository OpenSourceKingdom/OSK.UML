using OSK.Functions.Outputs.Abstractions;
using OSK.UML.Models;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.UML.Ports
{
    public interface IUmlExporter
    {
        Task<IOutput> ExportAsync(string path, UmlDiagram diagram, CancellationToken cancellationToken = default);
    }
}
