using Moq;
using OSK.Functions.Outputs.Logging.Abstractions;
using OSK.Functions.Outputs.Mocks;
using OSK.Parsing.FileTokens.Ports;
using OSK.UML.Internal.Services;
using OSK.UML.Models;
using OSK.UML.Options;
using OSK.UML.Ports;
using System.Net;
using Xunit;

namespace OSK.UML.UnitTests.Internal.Services
{
    public class DefaultUmlGeneratorTests: IDisposable
    {
        #region Variables

        private string _testDirectory;

        private readonly Mock<IUmlParser> _mockParser;
        private readonly IOutputFactory<DefaultUmlGenerator> _outputFactory;
        private readonly IUmlGenerator _generator;

        #endregion

        #region Constructors

        public DefaultUmlGeneratorTests() 
        {
            _testDirectory = Path.Combine(".", "TestData");
            Directory.CreateDirectory(_testDirectory);

            _mockParser = new Mock<IUmlParser>();

            var mockParserFactory = new Mock<IUmlParserFactory>();
            mockParserFactory.Setup(m => m.CreateParser(It.IsAny<string>(), It.IsAny<ITokenStateHandler>()))
                .Returns(_mockParser.Object);

            _outputFactory = new MockOutputFactory<DefaultUmlGenerator>();
            _generator = new DefaultUmlGenerator(mockParserFactory.Object, _outputFactory);
        }

        #endregion

        #region Disposable

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }

            Assert.False(Directory.Exists(_testDirectory));
        }

        #endregion

        #region GenerateUmlAsync

        [Fact]
        public async Task GenerateUmlAsync_NullPath_ThrowsArgumentException()
        {
            // Arrange/Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _generator.GenerateUmlAsync(null, "Hi", new UmlGenerationOptions()));
        }

        [Fact]
        public async Task GenerateUmlAsync_NullDomainName_ThrowsArgumentException()
        {
            // Arrange/Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _generator.GenerateUmlAsync("Hi", null, new UmlGenerationOptions()));
        }

        [Fact]
        public async Task GenerateUmlAsync_ParserReturnsNull_ReturnsBadRequest()
        {
            // Arrange
            var path = Path.Combine(_testDirectory, "test");
            using var _ = File.CreateText(path);

            _mockParser.Setup(m => m.ParseUmlAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<UmlComponent>)null!);

            // Act
            var result = await _generator.GenerateUmlAsync(path, "hi", new UmlGenerationOptions());

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Equal(HttpStatusCode.BadRequest, result.Code.StatusCode);
        }

        [Fact]
        public async Task GenerateUmlAsync_PathNotFound_ReturnsNotFound()
        {
            // Arrange/Act
            var result = await _generator.GenerateUmlAsync("notreal", "hi", new UmlGenerationOptions());

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Equal(HttpStatusCode.NotFound, result.Code.StatusCode);
        }

        [Fact]
        public async Task GenerateUmlAsync_PathForSingleFile_ParserReturnsSuccessfully_ReturnsExpectedList()
        {
            // Arrange
            var path = Path.Combine(_testDirectory, "test");
            using var _ = File.CreateText(path);

            List<UmlComponent> expectedComponents = [ new UmlConstruct() { Name = "hi" }, new UmlValueList() { Name = "two" } ];
            _mockParser.Setup(m => m.ParseUmlAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedComponents);

            // Act
            var result = await _generator.GenerateUmlAsync(path, "hi", new UmlGenerationOptions());

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.Equal("hi", result.Value.UmlDiagram.Name);
            Assert.Equal(expectedComponents, result.Value.UmlDiagram.UmlObjects);
        }

        [Fact]
        public async Task GenerateUmlAsync_PathForDirectory_FilterForCSharpFiles_ParserReturnsSuccessfully_ReturnsExpectedList()
        {
            // Arrange
            var testFiles = 3;
            for (var i = 0; i < testFiles; i++)
            {
                var path = Path.Combine(_testDirectory, $"test{i}.cs");
                using var _ = File.CreateText(path);
            }

            List<UmlComponent> expectedComponents = [
                new UmlConstruct() { Name = "hi" }, new UmlValueList() { Name = "two" },
                new UmlConstruct() { Name = "hello" },
                new UmlValueList() { Name = "twoFor" }, new UmlValueList() { Name = "twoHelp" }
            ];
            var returnCount = 0;
            _mockParser.Setup(m => m.ParseUmlAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    if (returnCount == 0)
                    {
                        returnCount++;
                        return expectedComponents.Take(2);
                    }
                    if (returnCount == 1)
                    {
                        returnCount++;
                        return expectedComponents.Skip(2).Take(1);
                    }

                    return expectedComponents.Skip(3);
                });

            // Act
            var result = await _generator.GenerateUmlAsync(_testDirectory, "hi", new UmlGenerationOptions());

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.Equal("hi", result.Value.UmlDiagram.Name);
            Assert.Equal(expectedComponents, result.Value.UmlDiagram.UmlObjects);
        }

        [Fact]
        public async Task GenerateUmlAsync_PathForDirectory_FilterForAllFiles_ParserReturnsSuccessfully_ReturnsExpectedList()
        {
            // Arrange
            var testFiles = 3;
            for (var i = 0; i < testFiles; i++)
            {
                var extension = i == 0 ? ".cs"
                    : i == 1 ? ".txt"
                    : string.Empty;
                var path = Path.Combine(_testDirectory, $"test{i}{extension}");
                using var _ = File.CreateText(path);
            }

            List<UmlComponent> expectedComponents = [
                new UmlConstruct() { Name = "hi" }, new UmlValueList() { Name = "two" },
                new UmlConstruct() { Name = "hello" },
                new UmlValueList() { Name = "twoFor" }, new UmlValueList() { Name = "twoHelp" }
            ];
            var returnCount = 0;
            _mockParser.Setup(m => m.ParseUmlAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    if (returnCount == 0)
                    {
                        returnCount++;
                        return expectedComponents.Take(2);
                    }
                    if (returnCount == 1)
                    {
                        returnCount++;
                        return expectedComponents.Skip(2).Take(1);
                    }

                    return expectedComponents.Skip(3);
                });

            // Act
            var result = await _generator.GenerateUmlAsync(_testDirectory, "hi", new UmlGenerationOptions()
            {
                FileExtensionPattern = string.Empty
            });

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.Equal("hi", result.Value.UmlDiagram.Name);
            Assert.Equal(expectedComponents, result.Value.UmlDiagram.UmlObjects);
        }

        #endregion
    }
}
