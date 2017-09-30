using System;
using NUnit.Framework;
using Sprache;

namespace AbnfToSprache.Tests
{
    [TestFixture]
    public class ConcatenationTests
    {
        [Test]
        public void TestWithRepeatCountAndElement()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.Concatenation.TryParse("1\"string a\" 1\"string b\"");

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<SequenceExpression>(actual);
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Expressions, Is.Not.Null);
            Assert.That(actualValue.Expressions[0], Is.InstanceOf<RepetitionExpression>());
            Assert.That(actualValue.Expressions[0].As<RepetitionExpression>().Repeat.As<RepeatCountExpression>().Count, Is.EqualTo(1));
            Assert.That(actualValue.Expressions[0].As<RepetitionExpression>().Element.As<Terminal<string>>().Value, Is.EqualTo("string a"));
            Assert.That(actualValue.Expressions[1], Is.InstanceOf<RepetitionExpression>());
            Assert.That(actualValue.Expressions[1].As<RepetitionExpression>().Repeat.As<RepeatCountExpression>().Count, Is.EqualTo(1));
            Assert.That(actualValue.Expressions[1].As<RepetitionExpression>().Element.As<Terminal<string>>().Value, Is.EqualTo("string b"));
        }

        [Test]
        public void TestWithRepeatRangeAndElement()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.Concatenation.TryParse("1*3<s-t-r-i-n-g> *<s-t-r-i-n-g>");

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<SequenceExpression>(actual);
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Expressions, Is.Not.Null);
            Assert.That(actualValue.Expressions[0], Is.InstanceOf<RepetitionExpression>());
            Assert.That(actualValue.Expressions[0].As<RepetitionExpression>().Repeat.As<RepeatRangeExpression>().AtLeast, Is.EqualTo(1));
            Assert.That(actualValue.Expressions[0].As<RepetitionExpression>().Repeat.As<RepeatRangeExpression>().AtMost, Is.EqualTo(3));
            Assert.That(actualValue.Expressions[0].As<RepetitionExpression>().Element.As<ProseValExpression>().Name, Is.EqualTo("s-t-r-i-n-g"));
            Assert.That(actualValue.Expressions[1], Is.InstanceOf<RepetitionExpression>());
            Assert.That(actualValue.Expressions[1].As<RepetitionExpression>().Repeat.As<RepeatRangeExpression>().AtLeast, Is.EqualTo(0));
            Assert.That(actualValue.Expressions[1].As<RepetitionExpression>().Repeat.As<RepeatRangeExpression>().AtMost, Is.EqualTo(int.MaxValue));
            Assert.That(actualValue.Expressions[1].As<RepetitionExpression>().Element.As<ProseValExpression>().Name, Is.EqualTo("s-t-r-i-n-g"));
        }

        [Test]
        public void TestWithRepeatRangeAndElementSeparatedByMoreWsps()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.Concatenation.TryParse("1*3<s-t-r-i-n-g> ; comment  "+Environment.NewLine+"\t *<s-t-r-i-n-g>");

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<SequenceExpression>(actual);
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Expressions, Is.Not.Null);
            Assert.That(actualValue.Expressions[0], Is.InstanceOf<RepetitionExpression>());
            Assert.That(actualValue.Expressions[0].As<RepetitionExpression>().Repeat.As<RepeatRangeExpression>().AtLeast, Is.EqualTo(1));
            Assert.That(actualValue.Expressions[0].As<RepetitionExpression>().Repeat.As<RepeatRangeExpression>().AtMost, Is.EqualTo(3));
            Assert.That(actualValue.Expressions[0].As<RepetitionExpression>().Element.As<ProseValExpression>().Name, Is.EqualTo("s-t-r-i-n-g"));
            Assert.That(actualValue.Expressions[1], Is.InstanceOf<SequenceExpression>());

            var innerExpressions = actualValue.Expressions[1].As<SequenceExpression>().Expressions;
            Assert.That(innerExpressions[0].As<CommentExpression>().Value, Is.EqualTo(" comment  "));
            Assert.That(innerExpressions[1], Is.InstanceOf<RepetitionExpression>());
            Assert.That(innerExpressions[1].As<RepetitionExpression>().Repeat.As<RepeatRangeExpression>().AtLeast, Is.EqualTo(0));
            Assert.That(innerExpressions[1].As<RepetitionExpression>().Repeat.As<RepeatRangeExpression>().AtMost, Is.EqualTo(int.MaxValue));
            Assert.That(innerExpressions[1].As<RepetitionExpression>().Element.As<ProseValExpression>().Name, Is.EqualTo("s-t-r-i-n-g"));
        }

        [Test]
        public void TestWithElementOnly()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.Concatenation.TryParse("\"string\"");

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<SequenceExpression>(actual);
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Expressions, Is.Not.Null);
            Assert.That(actualValue.Expressions[0], Is.InstanceOf<Terminal<string>>());
            Assert.That(actualValue.Expressions[0].As<Terminal<string>>().Value, Is.EqualTo("string"));
        }
    }
}