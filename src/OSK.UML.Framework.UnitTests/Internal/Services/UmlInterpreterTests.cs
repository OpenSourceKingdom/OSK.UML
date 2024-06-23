using Moq;
using OSK.Parsing.FileTokens.Models;
using OSK.UML.Framework.Internal.Services;
using OSK.UML.Framework.Models;
using OSK.UML.Framework.Ports;
using Xunit;

namespace OSK.UML.Framework.UnitTests.Internal.Services
{
    public class UmlInterpreterTests
    {
        #region Variables

        private readonly UmlSyntaxTemplate _syntaxTemplate;
        private readonly Mock<IUmlDefinition> _mockDefinition;
        private readonly IUmlInterpreter _interpreter;

        #endregion

        #region Constructors

        public UmlInterpreterTests()
        {
            _syntaxTemplate = new UmlSyntaxTemplate();
            _mockDefinition = new Mock<IUmlDefinition>();

            _interpreter = new UmlInterpreter(_syntaxTemplate, _mockDefinition.Object);
        }

        #endregion

        #region TotalRules

        [Fact]
        public void TotalRules_GetsValueFromTemplate()
        {
            // Arrrange
            _syntaxTemplate.SetRules(SyntaxRuleType.Parameters, SyntaxRuleType.Modifiers);

            // Act/Assert
            Assert.Equal(_syntaxTemplate.Rules.Length, _interpreter.TotalRules);
        }

        #endregion

        #region InterpreterType

        [Fact]
        public void InterpreterType_GetsValueFromTemplate()
        {
            // Arrange
            _syntaxTemplate.ElementType = UmlElementType.Construct;

            // Act/Assert
            Assert.Equal(_syntaxTemplate.ElementType, _interpreter.InterpreterType);
        }

        #endregion

        #region Reset

        #endregion

        #region AddFileToken

        [Theory]
        [InlineData(FileTokenType.Delimeter)]
        [InlineData(FileTokenType.Ignore)]
        public void AddFileToken_FileTokenCanBeIgnored_DoesNotAffectState(FileTokenType fileTokenType)
        {
            // Arrange/Act
            _interpreter.AddFileToken(new FileToken(fileTokenType));

            // Assert
            Assert.Equal(UmlInterpreterState.Initialized, _interpreter.State);
        }

        [Fact]
        public void AddFileToken_TemplateIsForMethod_FileTokenIsEndOfStatement_StateIsSetToInvalid()
        {
            // Arrange
            _syntaxTemplate.ElementType = UmlElementType.Method;

            // Act
            _interpreter.AddFileToken(new FileToken(FileTokenType.EndOfStatement));

            // Assert
            Assert.Equal(UmlInterpreterState.Invalid, _interpreter.State);
        }

        [Fact]
        public void AddFileToken_SyntaxTemplateHasNoRules_StateIsSetToInvalid()
        {
            // Arrange/Act
            _interpreter.AddFileToken(new FileToken(FileTokenType.Text));

            // Assert
            Assert.Equal(UmlInterpreterState.Invalid, _interpreter.State);
        }

        [Fact]
        public void AddFileToken_SyntaxRulesAreSatisfied_StateIsSetToSatisfied()
        {
            // Arrange
            _syntaxTemplate.SetRules(SyntaxRuleType.Name);

            // Act
            _interpreter.AddFileToken(new FileToken(FileTokenType.Text, 100));

            // Assert
            Assert.Equal(UmlInterpreterState.Satisfied, _interpreter.State);
        }

        [Fact]
        public void AddFileToken_SyntaxRulesWereSatisfiedButReadAnotherTokenThatIsInvalid_StateIsReset()
        {
            // Arrange
            _syntaxTemplate.SetRules(SyntaxRuleType.Name, SyntaxRuleType.Name);

            // Act
            _interpreter.AddFileToken(new FileToken(FileTokenType.Text, 100));
            var originalState = _interpreter.State;

            _interpreter.AddFileToken(new FileToken(FileTokenType.EndOfFile));

            // Assert
            Assert.Equal(UmlInterpreterState.Parsing, originalState);
            Assert.Equal(UmlInterpreterState.Initialized, _interpreter.State);
        }

        [Fact]
        public void AddFileToken_FileTokenIsInvalidForOptionalRuleButSatisfiesNext_StateIsSatisfied()
        {
            // Arrange
            _mockDefinition.Setup(m => m.IsModifier(It.IsAny<string>()))
                .Returns(false);

            _syntaxTemplate.SetRules(SyntaxRuleType.Modifiers, SyntaxRuleType.Name);

            // Act
            _interpreter.AddFileToken(new FileToken(FileTokenType.Text));

            // Assert
            Assert.Equal(UmlInterpreterState.Satisfied, _interpreter.State);
        }

        [Fact]
        public void AddFileToken_FileTokenIsInvalidForOptionalRuleButIsLastRule_StateIsSatisfied()
        {
            // Arrange
            _mockDefinition.Setup(m => m.IsModifier(It.IsAny<string>()))
                .Returns(false);

            _syntaxTemplate.SetRules(SyntaxRuleType.Modifiers);

            // Act
            _interpreter.AddFileToken(new FileToken(FileTokenType.Text));

            // Assert
            Assert.Equal(UmlInterpreterState.Satisfied, _interpreter.State);
        }

        #endregion

        #region GetElement

        [Fact]
        public void GetElement_StateIsNotSatisfied_ReturnsNull()
        {
            // Arrange/Act
            var element = _interpreter.GetElement();

            // Assert
            Assert.Null(element);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetElement_StateIsSatisfied_ReturnsExpectedElement(bool paramTypeIsFirst)
        {
            // Arrange
            var modifier = "mod";
            var visibility = "vis";
            var name = "world";

            var parameterTypeText = "name";
            var parameterNameText = "value";

            _mockDefinition.Setup(m => m.IsModifier(It.IsAny<string>()))
                .Returns((string s) => s == modifier);
            _mockDefinition.Setup(m => m.IsVisibility(It.IsAny<string>()))
                .Returns((string s) => s == visibility);
            _mockDefinition.SetupGet(m => m.MethodParameterTypeIsFirst)
                .Returns(paramTypeIsFirst);

            _syntaxTemplate.SetRules(SyntaxRuleType.Modifiers, SyntaxRuleType.Visibility,
                SyntaxRuleType.Name, SyntaxRuleType.Parameters);

            // Act
            _interpreter.AddFileToken(new FileToken(FileTokenType.Text, modifier.Select(c => (int)c).ToArray()));
            _interpreter.AddFileToken(new FileToken(FileTokenType.Text, visibility.Select(c => (int)c).ToArray()));
            _interpreter.AddFileToken(new FileToken(FileTokenType.Text, name.Select(c => (int)c).ToArray()));

            _interpreter.AddFileToken(new FileToken(FileTokenType.Text, parameterTypeText.Select(c => (int)c).ToArray()));
            _interpreter.AddFileToken(new FileToken(FileTokenType.Text, parameterNameText.Select(c => (int)c).ToArray()));

            _interpreter.AddFileToken(new FileToken(FileTokenType.ClosureEnd));

            var element = _interpreter.GetElement();

            // Assert
            Assert.NotNull(element);
            Assert.Equal(UmlInterpreterState.Satisfied, _interpreter.State);
            Assert.Equal(name, element.Name);
            Assert.Single(element.Modifiers);
            Assert.Equal(modifier, element.Modifiers.First());
            Assert.Equal(visibility, element.Visibility);
            Assert.Single(element.Parameters);
            
            var parameter = element.Parameters.First();

            if (paramTypeIsFirst)
            {
                Assert.Equal(parameterTypeText, parameter.Type);
                Assert.Equal(parameterNameText, parameter.Name);
            }
            else
            {
                Assert.Equal(parameterTypeText, parameter.Name);
                Assert.Equal(parameterNameText, parameter.Type);
            }
        }

        #endregion
    }
}
