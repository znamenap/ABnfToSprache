using System;
using NUnit.Framework;
using Sprache;

namespace AbnfToSprache.Tests
{
    [TestFixture]
    public class CommentTests
    {
        const string Content = "comment content";

        [Test]
        public void TestSimpleCommentWithEndLine()
        {
            string input = string.Format(";{0}\r\n", Content);

            // = Setup
            // = Exercise
            var actual = ABNFParser.Comment.TryParse(input);

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<CommentExpression>(actual);
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Value, Is.EqualTo(Content));
        }

        [Test]
        public void TestSimpleCommentWithoutEndLine()
        {
            // = Setup
            string input = string.Format(";{0}",Content);

            // = Exercise
            var actual = ABNFParser.Comment.TryParse(input);

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<CommentExpression>(actual);
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Value, Is.EqualTo(Content));
        }

        [Test]
        public void TestParseCommentOrNewLineOnComment()
        {
            // = Setup
            var input = string.Format(";{0}", Content);

            // = Exercise
            var actual = ABNFParser.CommentOrNewLine.TryParse(input);

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<CommentExpression>(actual);
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Value, Is.EqualTo(Content));
        }
        
        [Test]
        public void TestParseCommentOrNewLineOnNewLine()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.CommentOrNewLine.TryParse(Environment.NewLine);

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<Terminal<string>>(actual);
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Value, Is.EqualTo(Environment.NewLine));
        }
        
        [Test]
        public void TestCommentOrWhitepsaceWithSpace()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.CWsp.TryParse(" ");

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<Terminal<char>>(actual);
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Value, Is.EqualTo(' '));
        }

        [Test]
        public void TestCommentOrWhitepsaceWithTab()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.CWsp.TryParse("\t");

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<Terminal<char>>(actual);
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Value, Is.EqualTo('\t'));
        }

        [Test]
        public void TestCommentOrWhitepsaceWithCommentAndWhitespace()
        {
            // = Setup
            // = Exercise
            var actual = ABNFParser.CWsp.TryParse(";"+Content+Environment.NewLine+"\t");

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<CommentExpression>(actual);
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Value, Is.EqualTo(Content));
        }

        [Test]
        public void TestCommentOrWhitepsaceWithCommentWithoutWhitespace()
        {
            // = Setup
            var input = ";" + Content + Environment.NewLine;
            // = Exercise
            var actual = ABNFParser.CWsp.TryParse(input);

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<CommentExpression>(actual);
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue.Value, Is.EqualTo(Content));
        }
    }
}
