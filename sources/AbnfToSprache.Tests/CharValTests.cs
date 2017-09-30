using NUnit.Framework;
using Sprache;

namespace AbnfToSprache.Tests
{
    [TestFixture]
    public class CharValTests
    {
        [TestCase("\"command string\"")]
        [TestCase("\"command string\"")]
        public void TestRegularLiteralString(string input)
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.CharVal.TryParse(input);

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<Terminal<string>>(actual);
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Value, Is.EqualTo("command string"));
        }
    }
}
