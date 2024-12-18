using OSK.Hexagonal.MetaData;
using OSK.UML.Ports;

namespace OSK.UML.Exporters.PlantUML.Ports
{
    [HexagonalPort(HexagonalPort.Primary)]
    public interface IPlantUmlExporter : IUmlExporter
    {
    }
}
