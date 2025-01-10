using OSK.Hexagonal.MetaData;
using OSK.UML.Ports;

namespace OSK.UML.Exporters.PlantUML.Ports
{
    [HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
    public interface IPlantUmlExporter : IUmlExporter
    {
    }
}
