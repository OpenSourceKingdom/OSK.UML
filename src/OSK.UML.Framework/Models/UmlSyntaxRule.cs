namespace OSK.UML.Framework.Models
{
    public class UmlSyntaxRule
    {
        public SyntaxRuleType RuleType { get; set; }

        public bool AllowMultipleTokens { get; set; }

        public bool Optional { get; set; }
    }
}
