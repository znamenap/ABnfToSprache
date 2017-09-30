using NUnit.Framework;
using Sprache;

namespace AbnfToSprache.Tests
{
    [TestFixture]
    public class RepetitionTests
    {
        [Test]
        public void TestWithRepeatCountAndElement()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.Repetition.TryParse("1\"string\"");

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<RepetitionExpression>(actual);
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Element, Is.Not.Null);
            Assert.That(actualValue.Element, Is.InstanceOf<Terminal<string>>());
            Assert.That(actualValue.Element.As<Terminal<string>>().Value, Is.EqualTo("string"));
            Assert.That(actualValue.Repeat, Is.Not.Null);
            Assert.That(actualValue.Repeat, Is.InstanceOf<RepeatCountExpression>());
            Assert.That(actualValue.Repeat.As<RepeatCountExpression>().Count, Is.EqualTo(1));
        }

        [Test]
        public void TestWithRepeatRangeAndElement()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.Repetition.TryParse("1*3<s-t-r-i-n-g>");

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<RepetitionExpression>(actual);
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Element, Is.Not.Null);
            Assert.That(actualValue.Element, Is.InstanceOf<ProseValExpression>());
            Assert.That(actualValue.Element.As<ProseValExpression>().Name, Is.EqualTo("s-t-r-i-n-g"));
            Assert.That(actualValue.Repeat, Is.Not.Null);
            Assert.That(actualValue.Repeat, Is.InstanceOf<RepeatRangeExpression>());
            Assert.That(actualValue.Repeat.As<RepeatRangeExpression>().AtLeast, Is.EqualTo(1));
            Assert.That(actualValue.Repeat.As<RepeatRangeExpression>().AtMost, Is.EqualTo(3));
        }

        [Test]
        public void TestWithRepeatDefaultLeastRangeAndElement()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.Repetition.TryParse("*3<s-t-r-i-n-g>");

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<RepetitionExpression>(actual);
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Element, Is.Not.Null);
            Assert.That(actualValue.Element, Is.InstanceOf<ProseValExpression>());
            Assert.That(actualValue.Element.As<ProseValExpression>().Name, Is.EqualTo("s-t-r-i-n-g"));
            Assert.That(actualValue.Repeat, Is.Not.Null);
            Assert.That(actualValue.Repeat, Is.InstanceOf<RepeatRangeExpression>());
            Assert.That(actualValue.Repeat.As<RepeatRangeExpression>().AtLeast, Is.EqualTo(0));
            Assert.That(actualValue.Repeat.As<RepeatRangeExpression>().AtMost, Is.EqualTo(3));
        }

        [Test]
        public void TestWithRepeatAnyRangeAndElement()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.Repetition.TryParse("*<s-t-r-i-n-g>");

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<RepetitionExpression>(actual);
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Element, Is.Not.Null);
            Assert.That(actualValue.Element, Is.InstanceOf<ProseValExpression>());
            Assert.That(actualValue.Element.As<ProseValExpression>().Name, Is.EqualTo("s-t-r-i-n-g"));
            Assert.That(actualValue.Repeat, Is.Not.Null);
            Assert.That(actualValue.Repeat, Is.InstanceOf<RepeatRangeExpression>());
            Assert.That(actualValue.Repeat.As<RepeatRangeExpression>().AtLeast, Is.EqualTo(0));
            Assert.That(actualValue.Repeat.As<RepeatRangeExpression>().AtMost, Is.EqualTo(int.MaxValue));
        }

        [Test]
        public void TestWithElementOnly()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.Repetition.TryParse("\"string\"");

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<Terminal<string>>(actual);
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Value, Is.EqualTo("string"));
        }
    }
}
