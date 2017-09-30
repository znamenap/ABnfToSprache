using System;
using NUnit.Framework;
using Sprache;

namespace AbnfToSprache.Tests
{
    [TestFixture]
    public class NumericTests
    {
        [TestCase("%d13")]
        [TestCase("%d13")]
        [TestCase("%b1101")]
        [TestCase("%x0D")]
        public void ParseNumVal(string input)
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.NumVal.TryParse(input);

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<Characters>(actual);
            Assert.That(actualValue.Value, Is.EqualTo("\r"));
        }

        [TestCase("%d13.10")]
        [TestCase("%d13.10")]
        [TestCase("%b1101.1010")]
        [TestCase("%x0D.0A")]
        public void ParseNumValContinuation(string input)
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.NumVal.TryParse(input);

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<Characters>(actual);
            Assert.That(actualValue.Value, Is.EqualTo(Environment.NewLine));
        }

        [TestCase("%d13.10.13.10")]
        [TestCase("%d13.10.13.10")]
        [TestCase("%b1101.1010.1101.1010")]
        [TestCase("%x0D.0A.0D.0A")]
        public void ParseNumValContinuationDouble(string input)
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.NumVal.TryParse(input);

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<Characters>(actual);
            Assert.That(actualValue.Value, Is.EqualTo(Environment.NewLine + Environment.NewLine));
        }

        [TestCase("%b1010-1101")]
        [TestCase("%d10-13")]
        [TestCase("%x0A-0D")]
        public void TestNumValValidRange(string input)
        {
            var actual = ABNFParser.NumVal.TryParse(input);

            var actualValue = ParseResult.ValueInstanceOf<CharacterRange>(actual);
            Assert.That(actualValue.Start, Is.EqualTo("\n"));
            Assert.That(actualValue.Stop, Is.EqualTo("\r"));
        }

        [TestCase("%b1010.1101.1010.1101")]
        [TestCase("%d10.13.10.13")]
        [TestCase("%x0A.0D.0A.0D")]
        public void TestNumValValidContinuation(string input)
        {
            var actual = ABNFParser.NumVal.TryParse(input);

            var actualValue = ParseResult.ValueInstanceOf<Characters>(actual);
            Assert.That(actualValue.Value, Is.EqualTo("\n\r\n\r"));
        }
    }
}
