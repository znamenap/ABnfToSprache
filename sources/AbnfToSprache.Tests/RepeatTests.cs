using NUnit.Framework;
using Sprache;

namespace AbnfToSprache.Tests
{
    [TestFixture]
    public class RepeatTests
    {
        [Test]
        public void TestValidRepeatCount()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.RepeatCount.TryParse("1");

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<RepeatCountExpression>(actual);
            Assert.That(actualValue.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestValidCount()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.Repeat.TryParse("1");

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<RepeatCountExpression>(actual);
            Assert.That(actualValue.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestValidRepeatRange()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.RepeatRange.TryParse("1*5");

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<RepeatRangeExpression>(actual);
            Assert.That(actualValue.AtLeast, Is.EqualTo(1));
            Assert.That(actualValue.AtMost, Is.EqualTo(5));
        }

        [Test]
        public void TestValidRange()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.Repeat.TryParse("1*5");

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<RepeatRangeExpression>(actual);
            Assert.That(actualValue.AtLeast, Is.EqualTo(1));
            Assert.That(actualValue.AtMost, Is.EqualTo(5));
        }

        [Test]
        public void TestDefaultRange()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.Repeat.TryParse("*");

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<RepeatRangeExpression>(actual);
            Assert.That(actualValue.AtLeast, Is.EqualTo(0));
            Assert.That(actualValue.AtMost, Is.EqualTo(int.MaxValue));
        }

        [Test]
        public void TestLowerLimitSpecified()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.Repeat.TryParse("2*");

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<RepeatRangeExpression>(actual);
            Assert.That(actualValue.AtLeast, Is.EqualTo(2));
            Assert.That(actualValue.AtMost, Is.EqualTo(int.MaxValue));
        }

        [Test]
        public void TestUpperLimitSpecified()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.Repeat.TryParse("*4");

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<RepeatRangeExpression>(actual);
            Assert.That(actualValue.AtLeast, Is.EqualTo(0));
            Assert.That(actualValue.AtMost, Is.EqualTo(4));
        }
    }
}
