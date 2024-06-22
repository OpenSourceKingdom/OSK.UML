using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OSK.Functions.Outputs.Abstractions;
using OSK.Functions.Outputs.Logging.Abstractions;
using OSK.UML.Exporters.PlantUML.Options;
using OSK.UML.Exporters.PlantUML.Ports;
using OSK.UML.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.UML.Exporters.PlantUML.Internal.Services
{
    internal class PlantUmlExporter(IOutputFactory<PlantUmlExporter> outputFactory,
        ILogger<PlantUmlJarCommand> logger,
        IOptions<PlantUmlOptions> options) : IPlantUmlExporter
    {
        #region IPlantUmlExporter

        public async Task<IOutput> ExportAsync(string outputPath, UmlDiagram diagram, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                throw new ArgumentException(nameof(outputPath));
            }
            if (diagram == null)
            {
                throw new ArgumentNullException(nameof(diagram));
            }

            var umlString = ToUmlString(diagram, cancellationToken);
            var jarCommand = new PlantUmlJarCommand(diagram.Name, umlString, outputPath, logger);

            await jarCommand.ExecuteAsync(options.Value.JarFilePath, cancellationToken);
            return outputFactory.Success();
        }

        #endregion

        #region Helpers

        private string ToUmlString(UmlDiagram diagram, CancellationToken cancellationToken)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(PlantUmlConstants.StartUml);

            foreach (var component in diagram.UmlObjects)
            {
                cancellationToken.ThrowIfCancellationRequested();
                WriteUmlComponent(stringBuilder, component);
            }
            foreach (var association in diagram.UmlAssociations)
            {
                cancellationToken.ThrowIfCancellationRequested();
                WriteUmlAssociation(stringBuilder, association);
            }

            stringBuilder.AppendLine(PlantUmlConstants.EndUml);

            return stringBuilder.ToString();
        }

        private void WriteUmlComponent(StringBuilder builder, UmlComponent component)
        {
            var valueList = component as UmlValueList;
            var construct = component as UmlConstruct;
            var componentType = valueList == null
                ? construct!.Type
                : "enum";

            builder.AppendLine($"{componentType} {component.Name} {{");

            if (valueList != null)
            {
                foreach (var value in valueList.Values)
                {
                    builder.AppendLine(value);
                }
            }
            else if (construct != null)
            {
                foreach (var variable in construct.Variables)
                {
                    builder.AppendLine($"{variable.Type} {variable.Name}");
                }
                foreach (var method in construct.Methods)
                {
                    var parameterList = string.Join(",", method.Parameters.Select(p => $"{p.Type} {p.Name}"));
                    builder.AppendLine($"{method.Type} {method.Name}({parameterList})");
                }
            }

            builder.AppendLine("}");
        }

        private void WriteUmlAssociation(StringBuilder builder, UmlAssociation association)
        {
            var uml = association.AssociationType switch
            {
                UmlAssociationType.Inheritance => $"{association.AssociatedObjectType} {PlantUmlConstants.ExtensionUml_Dashed} {association.Name}",
                UmlAssociationType.Aggregation => $"{association.AssociatedObjectType} {PlantUmlConstants.AggregationUml_Dashed} {association.Name}",
                _ => throw new InvalidOperationException($"PlantUml does not support associations of type {association.AssociationType}.")
            };

            builder.AppendLine(uml);
        }

        #endregion
    }
}
