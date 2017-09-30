using NUnit.Framework;
using Sprache;

namespace AbnfToSprache.Tests
{
    [TestFixture]
    public class RuleNameTests
    {
        [TestCase("formula")]
        [TestCase("expression")]
        [TestCase("ref-expression")]
        [TestCase("ref-expression21")]
        [TestCase("ref-expression-twenty-one")]
        public void TestParseSimpleRuleName(string input)
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.RuleName.TryParse(input);

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<string>(actual);
            Assert.That(actualValue, Is.Not.Null);
        }

        [TestCase("<formula>")]
        [TestCase("<ref-expression>")]
        [TestCase("<ref-expression21>")]
        [TestCase("<ref-expression-twenty-one>")]
        public void TestParseReferenceRuleName(string input)
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.ProseVal.TryParse(input);

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<ProseValExpression>(actual);
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Name, Is.EqualTo(input.Replace("<", "").Replace(">", "")));
        }

        [TestCase("1formula")]
        [TestCase("_formula")]
        // invalid case [TestCase("formula_likeThis")]
        public void TestParseInvalidRuleName(string input)
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.RuleName.TryParse(input);

            // = Outcome
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.WasSuccessful, Is.False);
        }
    }
}
