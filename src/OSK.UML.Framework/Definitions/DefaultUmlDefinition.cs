using OSK.UML.Framework.Models;

namespace OSK.UML.Framework.Definitions
{
    public class DefaultUmlDefinition : UmlDefinition
    {
        #region Static

        public static DefaultUmlDefinition Instance => new();

        #endregion

        public DefaultUmlDefinition()
            : base(nameSpaceKeyWord: "namespace",
                constructIdentifiers: [
                    new UmlConstructIdentifier()
                    {
                        Name = "struct",
                        ConstructType = UmlConstructType.Object
                    },
                    new UmlConstructIdentifier()
                    {
                        Name = "class",
                        ConstructType = UmlConstructType.Object
                    },
                    new UmlConstructIdentifier()
                    {
                        Name = "interface",
                        ConstructType = UmlConstructType.Object
                    },
                    new UmlConstructIdentifier()
                    {
                        Name = "enum",
                        ConstructType = UmlConstructType.ValueList
                    }],
                visibilityKeyWords: [ "public", "private", "internal", "protected" ],
                modifierKeyWords: [
                    "abstract", "readonly", "required", "static",
                    "record", "override", "sealed", "virtual",
                    "unsafe", "volatile", "async", "extern",
                    "required" 
                ],
                methodParameterTypeIsFirst: true,
                umlSyntaxTemplates: [
                    new UmlSyntaxTemplate(SyntaxRuleType.Visibility, SyntaxRuleType.Modifiers,
                        SyntaxRuleType.Type, SyntaxRuleType.Name, SyntaxRuleType.Parameters)
                    {
                        ElementType = UmlElementType.Method
                    },
                    new UmlSyntaxTemplate(SyntaxRuleType.Visibility, SyntaxRuleType.Modifiers,
                        SyntaxRuleType.Type, SyntaxRuleType.Name)
                    {
                        ElementType = UmlElementType.Variable
                    },
                    new UmlSyntaxTemplate(SyntaxRuleType.Visibility, SyntaxRuleType.Modifiers,
                        SyntaxRuleType.ConstructIdentifier, SyntaxRuleType.Name,
                        SyntaxRuleType.Inheritance)
                    {
                        ElementType = UmlElementType.Construct
                    },
                    new UmlSyntaxTemplate(SyntaxRuleType.NameSpace, SyntaxRuleType.Name)
                    {
                        ElementType = UmlElementType.NameSpace
                    }])
        {
        }
    }
}
