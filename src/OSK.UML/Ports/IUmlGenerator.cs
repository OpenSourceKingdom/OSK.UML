using OSK.Functions.Outputs.Abstractions;
using OSK.UML.Models;
using OSK.UML.Options;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.UML.Ports
{
    public interface IUmlGenerator
    {
        Task<IOutput<UmlGenerationResult>> GenerateUmlAsync(string path, string domainName, UmlGenerationOptions options, CancellationToken cancellationToken = default);
    }
}
