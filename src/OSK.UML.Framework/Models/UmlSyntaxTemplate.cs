using System;
using System.Linq;

namespace OSK.UML.Framework.Models
{
    public class UmlSyntaxTemplate
    {
        public UmlElementType ElementType { get; set; }

        public UmlSyntaxRule[] Rules { get; private set; }

        public UmlSyntaxTemplate()
        {
            Rules = [];
        }

        public UmlSyntaxTemplate(params UmlSyntaxRule[] syntaxRules)
        {
            Rules = syntaxRules;
        }

        public UmlSyntaxTemplate(params SyntaxRuleType[] ruleTypes)
        {
            SetRules(ruleTypes);
            if (Rules == null)
            {
                throw new ArgumentNullException(nameof(ruleTypes));
            }
        }

        internal void SetRules(params SyntaxRuleType[] ruleTypes)
        {
            Rules = ruleTypes.Select(ruleType => new UmlSyntaxRule()
            {
                RuleType = ruleType,
                AllowMultipleTokens = ruleType == SyntaxRuleType.Modifiers
                 || ruleType == SyntaxRuleType.Parameters
                 || ruleType == SyntaxRuleType.Inheritance,
                Optional = ruleType == SyntaxRuleType.Modifiers
                 || ruleType == SyntaxRuleType.Parameters
                 || ruleType == SyntaxRuleType.Inheritance
            }).ToArray();
        }

        internal void SetRules(params UmlSyntaxRule[] rules)
        {
            Rules = rules;
        }
    }
}
