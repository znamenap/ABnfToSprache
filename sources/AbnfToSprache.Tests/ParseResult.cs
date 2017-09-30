using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Sprache;

namespace AbnfToSprache.Tests
{
    internal static class ParseResult
    {
        internal static T ValueInstanceOf<T>(IResult<ElementExpression> actual) where T : class
        {
            Assert.That(actual, Is.Not.Null, "Parser result is not null");
            Assert.That(actual.Remainder.AtEnd, Is.True, "Parsed till the end. Stopped at {0}. {1} Expected: {2}.", actual.Remainder, actual.Message,
                JoinExpectations(actual.Expectations.ToList()));
            Assert.That(actual.WasSuccessful, Is.True, "Was sucesfully parsed. Expected: {0}. {1}", JoinExpectations(actual.Expectations.ToList()), actual);
            Assert.That(actual.Value, Is.Not.Null, "Value is not null");
            Assert.That(actual.Value, Is.InstanceOf<T>(), "Is instance of {0}", typeof(T).FullName);
            return actual.Value.As<T>();
        }

        private static string JoinExpectations(IList<string> expectations)
        {
            var result = new StringBuilder();
            result.Append('\'');
            var count = expectations.Count();
            if (count == 0)
                result.Append("<nothing>");
            else if (count == 1)
                result.Append(expectations.Single());
            else if (count > 1)
                result.Append(string.Join("' or '", expectations));
            result.Append('\'');
            return result.ToString().Replace("\r\n","\\r\\n");
        }

        internal static T ValueInstanceOf<T>(IResult<string> actual) where T : class
        {
            Assert.That(actual, Is.Not.Null, "Parser result is not null");
            Assert.That(actual.Remainder.AtEnd, Is.True, "Parsed till the end. Stopped at {0}. Expected: {1}.", actual.Remainder,
                JoinExpectations(actual.Expectations.ToList()));
            Assert.That(actual.WasSuccessful, Is.True, "Was sucesfully parsed. Expected: {0}.", JoinExpectations(actual.Expectations.ToList()));
            Assert.That(actual.Value, Is.Not.Null, "Value is not null");
            Assert.That(actual.Value, Is.InstanceOf<T>(), "Is instance of {0}", typeof(T).FullName);
            return actual.Value as T;
        }
    }

}