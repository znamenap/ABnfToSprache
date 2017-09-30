using System;
using NUnit.Framework;
using Sprache;

namespace AbnfToSprache.Tests
{
    [TestFixture]
    public class DefinedAsTests
    {
        [TestCase("=")]
        [TestCase("   =")]
        [TestCase("   =   ")]
        public void TestEqualSimple(string input)
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.DefinedAs.TryParse(input);

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<Terminal<string>>(actual);
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Value, Is.EqualTo("="));
        }

        [TestCase("=/")]
        [TestCase("   =/")]
        [TestCase("   =/   ")]
        public void TestEqualWithCombineInSequence(string input)
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.DefinedAs.TryParse(input);

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<Terminal<string>>(actual);
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Value, Is.EqualTo("=/"));
        }

    }
}
