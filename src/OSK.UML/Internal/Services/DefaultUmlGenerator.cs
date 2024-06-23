using OSK.Functions.Outputs.Abstractions;
using OSK.Functions.Outputs.Logging.Abstractions;
using OSK.Parsing.FileTokens.Handlers;
using OSK.UML.Models;
using OSK.UML.Options;
using OSK.UML.Ports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.UML.Internal.Services
{
    internal class DefaultUmlGenerator(IUmlParserFactory umlParserFactory,
        IOutputFactory<DefaultUmlGenerator> outputFactory) : IUmlGenerator
    {
        #region IUmlGenerator

        public async Task<IOutput<UmlGenerationResult>> GenerateUmlAsync(string path, string domainName, UmlGenerationOptions options,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(nameof(path));
            }
            if (string.IsNullOrWhiteSpace(domainName))
            {
                throw new ArgumentException(nameof(domainName));
            }

            var isDirectory = Directory.Exists(path);
            if (isDirectory)
            {
                var parseDirectoryResult = await ParseDirectoryAsync(path, options, cancellationToken);
                return parseDirectoryResult.IsSuccessful ? outputFactory.Success(new UmlGenerationResult()
                {
                    TotalUmlObjectsDiscovered = parseDirectoryResult.Value.Item1.Count(),
                    TotalFilesChecked = parseDirectoryResult.Value.Item2,
                    UmlDiagram = new UmlDiagram()
                    {
                        Name = domainName,
                        UmlObjects = parseDirectoryResult.Value.Item1,
                        UmlAssociations = GetAssociations(parseDirectoryResult.Value.Item1)
                    }
                }) : parseDirectoryResult.AsType<UmlGenerationResult>();
            }

            var isFile = File.Exists(path);
            if (isFile)
            {
                var parseFileResult = await ParseFileAsync(path, cancellationToken);
                return parseFileResult.IsSuccessful ? outputFactory.Success(new UmlGenerationResult()
                {
                    TotalFilesChecked = 1,
                    TotalUmlObjectsDiscovered = parseFileResult.Value.Count(),
                    UmlDiagram = new UmlDiagram()
                    {
                        Name = domainName,
                        UmlObjects = parseFileResult.Value,
                        UmlAssociations = GetAssociations(parseFileResult.Value)
                    }
                }) : parseFileResult.AsType<UmlGenerationResult>();
            }

            return outputFactory.Error<UmlGenerationResult>(HttpStatusCode.NotFound, $"The path {path} was not a valid path to a file or directory",
                "OSK.UML");
        }

        #endregion

        #region Helpers

        private async Task<IOutput<Tuple<IEnumerable<UmlComponent>, int>>> ParseDirectoryAsync(string directoryPath,
            UmlGenerationOptions options, CancellationToken cancellationToken)
        {
            var directoryPaths = Directory.GetFiles(directoryPath, $"*{options.FileExtensionPattern}", new EnumerationOptions()
            {
                RecurseSubdirectories = options.DirectoryDepth > 1,
            });

            var umlObjects = new List<UmlComponent>();
            var fileCount = 0;
            foreach (var path in directoryPaths)
            {
                var parseFileResult = await ParseFileAsync(path, cancellationToken);
                if (!parseFileResult.IsSuccessful)
                {
                    return parseFileResult.AsType<Tuple<IEnumerable<UmlComponent>, int>>();
                }

                fileCount++;
                umlObjects.AddRange(parseFileResult.Value);
            }

            return outputFactory.Success(new Tuple<IEnumerable<UmlComponent>, int>(umlObjects, fileCount));
        }

        private async Task<IOutput<IEnumerable<UmlComponent>>> ParseFileAsync(string filePath, CancellationToken cancellationToken)
        {
            using var parser = umlParserFactory.CreateParser(filePath, DefaultTokenStateHandler.Instance);
            var umlComponent = await parser.ParseUmlAsync(cancellationToken);

            return umlComponent == null
                ? outputFactory.BadRequest<IEnumerable<UmlComponent>>($"Unable to parse component from file {filePath}.")
                : outputFactory.Success(umlComponent);
        }

        private IEnumerable<UmlAssociation> GetAssociations(IEnumerable<UmlComponent> components)
        {
            var associations = new List<UmlAssociation>();
            var componentLookup = components.ToDictionary(c => c.Name, c => c);
            foreach (var construct in componentLookup.Values.OfType<UmlConstruct>())
            {
                var inheritanceAssociations = (construct.Inheritance ?? Enumerable.Empty<string>())
                    .Where(i => componentLookup.TryGetValue(i, out _))
                    .Select(i => new UmlAssociation()
                    {
                        Name = construct.Name,
                        AssociatedObjectType = i,
                        AssociationType = UmlAssociationType.Inheritance
                    });
                associations.AddRange(inheritanceAssociations);

                var variableAssociations = (construct.Variables ?? Enumerable.Empty<UmlVariable>())
                    .SelectMany(variable => GetAggregationTypes(variable.Type))
                    .Where(type => componentLookup.TryGetValue(type, out _))
                    .Select(type => new UmlAssociation()
                    {
                        Name = construct.Name,
                        AssociatedObjectType = type,
                        AssociationType = UmlAssociationType.Aggregation
                    });
                associations.AddRange(variableAssociations);
            }

            return associations;
        }

        private IEnumerable<string> GetAggregationTypes(string type)
        {
            type = type.TrimEnd('?');
            var underlyingTypes = type.Split(new char[] { ',', '<', '>' }, StringSplitOptions.RemoveEmptyEntries);

            return underlyingTypes;
        }

        #endregion
    }
}
