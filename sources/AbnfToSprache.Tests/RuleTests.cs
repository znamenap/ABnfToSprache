using NUnit.Framework;
using Sprache;

namespace AbnfToSprache.Tests
{
    [TestFixture]
    public class RuleTests
    {
        [TestCase("rule-name = base-a / base-b / (base-c-1 / base-c-2) \r\n; comment\r\n")]
        [TestCase("rule-name=*base-a/base-b/(base-c-1/base-c-2); comment\r\n")]
        [TestCase("rule-name=base-a/base-b/(base-c-1/base-c-2)\r\n; comment\r\n")]
        public void TestSimpleRule(string input)
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.Rule.TryParse(input);

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<RuleExpression>(actual);
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.RuleName, Is.Not.Null);
            Assert.That(actualValue.RuleName, Is.EqualTo("rule-name"));
            Assert.That(actualValue.Elements, Is.InstanceOf<SequenceExpression>());
            Assert.That(actualValue.Elements.Expressions, Is.Not.Null);
            Assert.That(actualValue.Elements.Expressions.Count, Is.EqualTo(2));
            Assert.That(actualValue.Elements.Expressions[0], Is.Not.Null);
            Assert.That(actualValue.Elements.Expressions[0].As<AlternationExpression>(), Is.Not.Null);
            var alternations = actualValue.Elements.Expressions[0].As<AlternationExpression>().Alternations;
            Assert.That(alternations, Is.Not.Null);
            Assert.That(alternations.Count, Is.EqualTo(3));
            Assert.That(actualValue.Elements.Expressions[1], Is.Not.Null);
            Assert.That(actualValue.Elements.Expressions[1].As<CommentExpression>(), Is.Not.Null);
            Assert.That(actualValue.Elements.Expressions[1].As<CommentExpression>().Value, Is.Not.Null);
            Assert.That(actualValue.Elements.Expressions[1].As<CommentExpression>().Value, Is.EqualTo(" comment"));
        }

        [TestCase("rule-name =/ base-a / base-b / (base-c-1 / base-c-2) ; comment \r\n")]
        [TestCase("rule-name=/base-a/base-b/(base-c-1/base-c-2); comment\r\n")]
        [TestCase("rule-name=/base-a/base-b/(base-c-1/base-c-2) ; comment\r\n")]
        public void TestOrRule(string input)
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.Rule.TryParse(input);

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<OrRuleExpression>(actual);
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.RuleName, Is.Not.Null);
            Assert.That(actualValue.RuleName, Is.EqualTo("rule-name"));
            Assert.That(actualValue.Elements, Is.InstanceOf<SequenceExpression>());
            Assert.That(actualValue.Elements.Expressions, Is.Not.Null);
            Assert.That(actualValue.Elements.Expressions.Count, Is.EqualTo(2));
            Assert.That(actualValue.Elements.Expressions[0], Is.Not.Null);
            Assert.That(actualValue.Elements.Expressions[0].As<AlternationExpression>(), Is.Not.Null);
            var alternations = actualValue.Elements.Expressions[0].As<AlternationExpression>().Alternations;
            Assert.That(alternations, Is.Not.Null);
            Assert.That(alternations.Count, Is.EqualTo(3));
            Assert.That(actualValue.Elements.Expressions[1], Is.Not.Null);
            Assert.That(actualValue.Elements.Expressions[1].As<CommentExpression>(), Is.Not.Null);
            Assert.That(actualValue.Elements.Expressions[1].As<CommentExpression>().Value, Is.Not.Null);
            Assert.That(actualValue.Elements.Expressions[1].As<CommentExpression>().Value, Is.EqualTo(" comment"));
        }

    }
}
