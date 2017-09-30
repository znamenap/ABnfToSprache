﻿using System;
using NUnit.Framework;
using Sprache;

namespace AbnfToSprache.Tests
{
    [TestFixture]
    public class OptionTests
    {
        [Test]
        public void TestWithRepeatCountAndElement()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.Option.TryParse("[ \t 1\"string a\"/ 1\"string b\"/%x20]");

            // = Outcome
            var actualValueRoot = ParseResult.ValueInstanceOf<OptionExpression>(actual);
            Assert.That(actualValueRoot.Expressions[0], Is.InstanceOf<AlternationExpression>());
            var actualValue = actualValueRoot.Expressions[0].As<AlternationExpression>();
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Alternations, Is.Not.Null);
            Assert.That(actualValue.Alternations[0], Is.InstanceOf<SequenceExpression>());
            Assert.That(actualValue.Alternations[0].As<SequenceExpression>().Expressions[0], Is.InstanceOf<RepetitionExpression>());
            Assert.That(actualValue.Alternations[0].As<SequenceExpression>().Expressions[0].As<RepetitionExpression>().Repeat.As<RepeatCountExpression>().Count, Is.EqualTo(1));
            Assert.That(actualValue.Alternations[0].As<SequenceExpression>().Expressions[0].As<RepetitionExpression>().Element.As<Terminal<string>>().Value, Is.EqualTo("string a"));
            Assert.That(actualValue.Alternations[1].As<SequenceExpression>().Expressions[0], Is.InstanceOf<RepetitionExpression>());
            Assert.That(actualValue.Alternations[1].As<SequenceExpression>().Expressions[0].As<RepetitionExpression>().Repeat.As<RepeatCountExpression>().Count, Is.EqualTo(1));
            Assert.That(actualValue.Alternations[1].As<SequenceExpression>().Expressions[0].As<RepetitionExpression>().Element.As<Terminal<string>>().Value, Is.EqualTo("string b"));
            Assert.That(actualValue.Alternations[2].As<SequenceExpression>().Expressions[0], Is.InstanceOf<Characters>());
            Assert.That(actualValue.Alternations[2].As<SequenceExpression>().Expressions[0].As<Characters>().Value, Is.EqualTo(" "));
        }

        [Test]
        public void TestWithRepeatRangeAndElement()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.Option.TryParse("[1*3<s-t-r-i-n-g>/*<s-t-r-i-n-g>]");

            // = Outcome
            var actualValueRoot = ParseResult.ValueInstanceOf<OptionExpression>(actual);
            Assert.That(actualValueRoot.Expressions[0], Is.InstanceOf<AlternationExpression>());
            var actualValue = actualValueRoot.Expressions[0].As<AlternationExpression>();
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Alternations, Is.Not.Null);
            Assert.That(actualValue.Alternations[0], Is.InstanceOf<SequenceExpression>());
            Assert.That(actualValue.Alternations[0].As<SequenceExpression>().Expressions[0], Is.InstanceOf<RepetitionExpression>());
            Assert.That(actualValue.Alternations[0].As<SequenceExpression>().Expressions[0].As<RepetitionExpression>().Repeat.As<RepeatRangeExpression>().AtLeast, Is.EqualTo(1));
            Assert.That(actualValue.Alternations[0].As<SequenceExpression>().Expressions[0].As<RepetitionExpression>().Repeat.As<RepeatRangeExpression>().AtMost, Is.EqualTo(3));
            Assert.That(actualValue.Alternations[0].As<SequenceExpression>().Expressions[0].As<RepetitionExpression>().Element.As<ProseValExpression>().Name, Is.EqualTo("s-t-r-i-n-g"));
            Assert.That(actualValue.Alternations[1].As<SequenceExpression>().Expressions[0], Is.InstanceOf<RepetitionExpression>());
            Assert.That(actualValue.Alternations[1].As<SequenceExpression>().Expressions[0].As<RepetitionExpression>().Repeat.As<RepeatRangeExpression>().AtLeast, Is.EqualTo(0));
            Assert.That(actualValue.Alternations[1].As<SequenceExpression>().Expressions[0].As<RepetitionExpression>().Repeat.As<RepeatRangeExpression>().AtMost, Is.EqualTo(int.MaxValue));
            Assert.That(actualValue.Alternations[1].As<SequenceExpression>().Expressions[0].As<RepetitionExpression>().Element.As<ProseValExpression>().Name, Is.EqualTo("s-t-r-i-n-g"));
        }

        [Test]
        public void TestWithRepeatRangeAndElementSeparatedByMoreWsps()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.Option.TryParse("[1*3<s-t-r-i-n-g> / ; comment  " + Environment.NewLine + "\t *<s-t-r-i-n-g>]");

            // = Outcome
            var actualValueRoot = ParseResult.ValueInstanceOf<OptionExpression>(actual);
            Assert.That(actualValueRoot.Expressions[0], Is.InstanceOf<AlternationExpression>());
            var actualValue = actualValueRoot.Expressions[0].As<AlternationExpression>();
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Alternations, Is.Not.Null);
            Assert.That(actualValue.Alternations[0], Is.InstanceOf<SequenceExpression>());
            Assert.That(actualValue.Alternations[0].As<SequenceExpression>().Expressions[0], Is.InstanceOf<RepetitionExpression>());
            Assert.That(actualValue.Alternations[0].As<SequenceExpression>().Expressions[0].As<RepetitionExpression>().Repeat.As<RepeatRangeExpression>().AtLeast, Is.EqualTo(1));
            Assert.That(actualValue.Alternations[0].As<SequenceExpression>().Expressions[0].As<RepetitionExpression>().Repeat.As<RepeatRangeExpression>().AtMost, Is.EqualTo(3));
            Assert.That(actualValue.Alternations[0].As<SequenceExpression>().Expressions[0].As<RepetitionExpression>().Element.As<ProseValExpression>().Name, Is.EqualTo("s-t-r-i-n-g"));
            Assert.That(actualValue.Alternations[1], Is.InstanceOf<SequenceExpression>());

            var innerExpressions = actualValue.Alternations[1].As<SequenceExpression>().Expressions;
            Assert.That(innerExpressions[0].As<CommentExpression>().Value, Is.EqualTo(" comment  "));
            Assert.That(innerExpressions[1].As<SequenceExpression>().Expressions[0], Is.InstanceOf<RepetitionExpression>());
            Assert.That(innerExpressions[1].As<SequenceExpression>().Expressions[0].As<RepetitionExpression>().Repeat.As<RepeatRangeExpression>().AtLeast, Is.EqualTo(0));
            Assert.That(innerExpressions[1].As<SequenceExpression>().Expressions[0].As<RepetitionExpression>().Repeat.As<RepeatRangeExpression>().AtMost, Is.EqualTo(int.MaxValue));
            Assert.That(innerExpressions[1].As<SequenceExpression>().Expressions[0].As<RepetitionExpression>().Element.As<ProseValExpression>().Name, Is.EqualTo("s-t-r-i-n-g"));
        }

        [Test]
        public void TestWithElementOnly()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.Option.TryParse("[\"string\"]");

            // = Outcome
            var actualValueRoot = ParseResult.ValueInstanceOf<OptionExpression>(actual);
            Assert.That(actualValueRoot.Expressions[0], Is.InstanceOf<AlternationExpression>());
            var actualValue = actualValueRoot.Expressions[0].As<AlternationExpression>();
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Alternations, Is.Not.Null);
            Assert.That(actualValue.Alternations[0], Is.InstanceOf<SequenceExpression>());
            Assert.That(actualValue.Alternations[0].As<SequenceExpression>().Expressions[0], Is.InstanceOf<Terminal<string>>());
            Assert.That(actualValue.Alternations[0].As<SequenceExpression>().Expressions[0].As<Terminal<string>>().Value, Is.EqualTo("string"));
        }

        [Test]
        public void TestWithThreeElements()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.Option.TryParse("[\"string a\" \"string b\" \"string c\"]");

            // = Outcome
            var actualValueRoot = ParseResult.ValueInstanceOf<OptionExpression>(actual);
            Assert.That(actualValueRoot.Expressions[0], Is.InstanceOf<AlternationExpression>());
            var actualValue = actualValueRoot.Expressions[0].As<AlternationExpression>();
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Alternations, Is.Not.Null);
            Assert.That(actualValue.Alternations[0], Is.InstanceOf<SequenceExpression>());
            Assert.That(actualValue.Alternations[0].As<SequenceExpression>().Expressions[0], Is.InstanceOf<Terminal<string>>());
            Assert.That(actualValue.Alternations[0].As<SequenceExpression>().Expressions[0].As<Terminal<string>>().Value, Is.EqualTo("string a"));
            Assert.That(actualValue.Alternations[0].As<SequenceExpression>().Expressions[1], Is.InstanceOf<Terminal<string>>());
            Assert.That(actualValue.Alternations[0].As<SequenceExpression>().Expressions[1].As<Terminal<string>>().Value, Is.EqualTo("string b"));
            Assert.That(actualValue.Alternations[0].As<SequenceExpression>().Expressions[2], Is.InstanceOf<Terminal<string>>());
            Assert.That(actualValue.Alternations[0].As<SequenceExpression>().Expressions[2].As<Terminal<string>>().Value, Is.EqualTo("string c"));
        }
    }
}