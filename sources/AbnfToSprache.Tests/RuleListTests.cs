using System;
using NUnit.Framework;
using Sprache;

namespace AbnfToSprache.Tests
{
    [TestFixture]
    public class RuleListTests
    {
        [TestCase("rule-name-A = base-a / base-b / (base-c-1 / base-c-2) ; comment " + "\r\n" + "\r\n"
            + "rule-name-B=*base-a/base-b/(base-c-1/base-c-2)  ; comment\r\n"
            + "rule-name-C=base-a/base-b/(base-c-1/base-c-2) ; comment\r\n")]
        public void TestThreeRules(string input)
        {
            // = Setup
            Console.WriteLine("[");
            Console.Write(input);
            Console.WriteLine("]");

            // = Exercise
            var actual = ABNFParser.RuleList.TryParse(input);

            // = Outcome
            var actualValue = ParseResult.ValueInstanceOf<SequenceExpression>(actual);

            Assert.That(actualValue.Expressions, Is.Not.Null);
            Assert.That(actualValue.Expressions.Count, Is.EqualTo(6));
            Assert.That(actualValue.Expressions[0], Is.Not.Null);
            var rule = actualValue.Expressions[0].As<RuleExpression>();
           
            Assert.That(rule, Is.Not.Null);
            Assert.That(rule.RuleName, Is.Not.Null);
            Assert.That(rule.RuleName, Is.EqualTo("rule-name-A"));
            Assert.That(rule.Elements, Is.InstanceOf<SequenceExpression>());
            Assert.That(rule.Elements.Expressions, Is.Not.Null);
            Assert.That(rule.Elements.Expressions.Count, Is.EqualTo(1));
            Assert.That(rule.Elements.Expressions[0], Is.Not.Null);
            Assert.That(rule.Elements.Expressions[0].As<AlternationExpression>(), Is.Not.Null);
            var alternations = rule.Elements.Expressions[0].As<AlternationExpression>().Alternations;
            Assert.That(alternations, Is.Not.Null);
            Assert.That(alternations.Count, Is.EqualTo(3));
            Assert.That(actualValue.Expressions[1], Is.Not.Null);
            Assert.That(actualValue.Expressions[1].As<CommentExpression>(), Is.Not.Null);
            Assert.That(actualValue.Expressions[1].As<CommentExpression>().Value, Is.Not.Null);
            Assert.That(actualValue.Expressions[1].As<CommentExpression>().Value, Is.EqualTo(" comment"));
        }
    }
}