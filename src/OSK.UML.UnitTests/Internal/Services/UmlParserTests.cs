using Moq;
using OSK.Parsing.FileTokens.Models;
using OSK.Parsing.FileTokens.Ports;
using OSK.UML.Framework.Models;
using OSK.UML.Framework.Ports;
using OSK.UML.Internal.Services;
using OSK.UML.Models;
using OSK.UML.Ports;
using Xunit;

namespace OSK.UML.UnitTests.Internal.Services
{
    public class UmlParserTests
    {
        #region Variables

        private readonly IList<IUmlInterpreter> _interpreterList;

        private readonly Mock<IFileTokenReader> _mockTokenReader;
        private readonly IUmlParser _umlParser;

        #endregion

        #region Constructors

        public UmlParserTests()
        {
            _interpreterList = new List<IUmlInterpreter>();

            _mockTokenReader = new Mock<IFileTokenReader>();
            var mockDefinition = new Mock<IUmlDefinition>();
            mockDefinition.Setup(m => m.GetUmlInterpreters())
                .Returns(_interpreterList);

            _umlParser = new UmlParser(_mockTokenReader.Object, mockDefinition.Object);
        }

        #endregion

        #region ParseUmlAsync

        [Fact]
        public async Task ParseUmlAsync_FileTokenReaderReturnsEndOfFile_ReturnsEmptyList()
        {
            // Arrange
            _mockTokenReader.Setup(m => m.ReadTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FileToken(FileTokenType.EndOfFile));

            // Act
            var components = await _umlParser.ParseUmlAsync();

            // Assert
            Assert.Empty(components);
        }

        [Theory]
        [InlineData(FileTokenType.EndOfFile)]
        [InlineData(FileTokenType.ClosureEnd)]
        public async Task ParseUmlAsync_ValueListAndNameSpaceInterpretersSatisfied_DifferentEndTokensUsed_ReturnsExpectedUmlComponent(FileTokenType endFileTokenType)
        {
            // Arrange
            var nameSpace = "helloWorld";
            var enumName = "AbcEnum";
            var expectedValues = new[] { "Abc", "dEF", "hIj" };

            var mockNameSpaceInterpreter = new Mock<IUmlInterpreter>();
            mockNameSpaceInterpreter.SetupGet(m => m.InterpreterType)
                .Returns(UmlElementType.NameSpace);
            mockNameSpaceInterpreter.SetupGet(m => m.State)
                .Returns(UmlInterpreterState.Satisfied);
            mockNameSpaceInterpreter.Setup(m => m.GetElement())
                .Returns(new UmlElement()
                {
                    Name = nameSpace
                });

            _interpreterList.Add(mockNameSpaceInterpreter.Object);

            var mockValueListInterpreter = new Mock<IUmlInterpreter>();
            mockValueListInterpreter.SetupGet(m => m.InterpreterType)
                .Returns(UmlElementType.Construct);
            mockValueListInterpreter.SetupGet(m => m.State)
                .Returns(UmlInterpreterState.Satisfied);
            mockValueListInterpreter.Setup(m => m.GetElement())
                .Returns(new UmlElement()
                {
                    ConstructType = UmlConstructType.ValueList,
                    Name = enumName
                });

            _interpreterList.Add(mockValueListInterpreter.Object);

            var i = 0;
            var endTokenReached = false;
            _mockTokenReader.Setup(m => m.ReadTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    if (endTokenReached)
                    {
                        return new FileToken(FileTokenType.EndOfFile);
                    }

                    var token = i < 2
                     ? new FileToken(FileTokenType.Text, nameSpace.Select(c => (int)c).ToArray())
                     : i - 2 >= expectedValues.Length
                       ? new FileToken(endFileTokenType)
                       : new FileToken(FileTokenType.Text, expectedValues[i - 2].Select(c => (int)c).ToArray());

                    i++;
                    if (token.TokenType == endFileTokenType)
                    {
                        endTokenReached = true;
                    }

                    return token;
                });

            // Act
            var components = await _umlParser.ParseUmlAsync();

            // Assert
            Assert.Single(components);

            var element = (UmlValueList) components.First();

            Assert.Equal(nameSpace, element.Namespace);
            Assert.Equal(enumName, element.Name);
            Assert.Equal(expectedValues, element.Values);
        }

        [Fact]
        public async Task ParseUmlAsync_ObjectWithoutNameSpaceSatsified_ReturnsExpectedUmlComponent()
        {
            var objectName = "AbcObject";
            var visibility = "whatAWorld";
            var expectedModifiers = new[] { "Abc", "dEF", "hIj" };
            var expectedInheritance = new[] { "super" };

            var mockObjectInterpreter = new Mock<IUmlInterpreter>();
            mockObjectInterpreter.SetupGet(m => m.InterpreterType)
                .Returns(UmlElementType.Construct);
            mockObjectInterpreter.SetupGet(m => m.State)
                .Returns(UmlInterpreterState.Satisfied);
            mockObjectInterpreter.Setup(m => m.GetElement())
                .Returns(new UmlElement()
                {
                    Visibility = visibility,
                    Modifiers = expectedModifiers,
                    Inheritance = expectedInheritance,
                    ConstructType = UmlConstructType.Object,
                    Name = objectName
                });

            _interpreterList.Add(mockObjectInterpreter.Object);

            var mockMemberInterpreter = new Mock<IUmlInterpreter>();
            mockMemberInterpreter.SetupGet(m => m.InterpreterType)
                .Returns(UmlElementType.Method);
            mockMemberInterpreter.SetupGet(m => m.State)
                .Returns(UmlInterpreterState.Satisfied);

            var methodElement = new UmlElement()
            {
                Name = "Method",
                UmlType = UmlElementType.Method,
                Parameters = [
                    new UmlElementParameter()
                    {
                        Name = "Hi",
                        Type = "two"
                    }
                ],
                Visibility = "cotton",
                Type = "Fields",
                Modifiers = [ "WorldBest" ]
            };
            var variableElement = new UmlElement()
            {
                Name = "Variable",
                UmlType = UmlElementType.Variable,
                Modifiers = [ "CottonWood" ],
                Type = "woops"
            };
            var returningMemberIndex = 0;
            mockMemberInterpreter.Setup(m => m.GetElement())
                .Returns(() =>
                {
                    var element = returningMemberIndex == 0
                        ? methodElement
                        : variableElement;

                    returningMemberIndex++;
                    return element;
                });

            _interpreterList.Add(mockMemberInterpreter.Object);

            var memberIndex = 0;
            _mockTokenReader.Setup(m => m.ReadTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    if (memberIndex < 3)
                    {
                        memberIndex++;
                        return new FileToken(FileTokenType.Text);
                    }

                    var token = memberIndex == 3
                        ? new FileToken(FileTokenType.Assignment)
                        : new FileToken(FileTokenType.EndOfFile);
                    memberIndex++;

                    return token;
                });

            // Act
            var result = await _umlParser.ParseUmlAsync();

            // Assert
            Assert.Single(result);

            var umlObject = (UmlConstruct)result.First();

            Assert.Equal(objectName, umlObject.Name);
            Assert.Equal(expectedModifiers, umlObject.Modifiers);
            Assert.Equal(expectedInheritance, umlObject.Inheritance);
            Assert.Equal(visibility, umlObject.Visibility);
            Assert.Null(umlObject.Namespace);

            Assert.Single(umlObject.Variables);
            var variable = umlObject.Variables.First();
            
            Assert.Equal(variableElement.Name, variable.Name);
            Assert.Equal(variableElement.Type, variable.Type);
            Assert.Equal(variable.Visibility, variable.Visibility);
            Assert.Equal(variableElement.Modifiers, variable.Modifiers);

            Assert.Single(umlObject.Methods);
            var method = umlObject.Methods.First();

            Assert.Equal(methodElement.Name, method.Name);
            Assert.Equal(methodElement.Type, method.Type);
            Assert.Equal(methodElement.Visibility, method.Visibility);
            Assert.Equal(methodElement.Modifiers, method.Modifiers);

            _mockTokenReader.Verify(m => m.ReadToEndTokenAsync(It.IsAny<FileToken>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion
    }
}
