using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Sprache;

namespace AbnfToSprache
{
    public static class ParserExtensions
    {
        public static readonly Parser<char> WhiteSpaceWithoutEol = 
            Parse.Char(ch =>
            {
                var isWs = char.IsWhiteSpace(ch);
                var cat = char.GetUnicodeCategory(ch);
                var isLs = cat == UnicodeCategory.LineSeparator;
                var crlf = ch == '\r' || ch == '\n';
                var result = isWs && !isLs && !crlf;
                return result;
            }, "whitespace not EOL");

        public static Parser<T> TokenWithoutEol<T>(this Parser<T> parser)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");

            return WhiteSpaceWithoutEol.Many()
                .SelectMany((leading => parser), (leading, item) => new { leading, item })
                .SelectMany(param0 => WhiteSpaceWithoutEol.Many(), (param0, trailing) => param0.item);
        }
        
    }

    abstract class ElementExpression
    {
        internal string Name { get; private set; }
        
        internal ElementExpression(string name)
        {
            Name = name;
        }

        internal T As<T>() where T : class
        {
            return this as T;
        }

        public override string ToString()
        {
            return string.Format("Name: {0}", Name);
        }
    }

    class RuleExpression : ElementExpression
    {
        internal string RuleName { get; private set; }

        internal SequenceExpression Elements { get; private set; }

        internal RuleExpression(string name, string ruleName, SequenceExpression rules)
            : base(name)
        {
            RuleName = ruleName;
            Elements = rules;
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, Elements: {1}", Name, Elements.Expressions.Count);
        }
    }

    class OrRuleExpression : ElementExpression
    {
        internal string RuleName { get; private set; }

        internal SequenceExpression Elements { get; private set; }

        internal OrRuleExpression(string name, string ruleName, SequenceExpression rules)
            : base(name)
        {
            RuleName = ruleName;
            Elements = rules;
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, Elements: {1}", Name, Elements.Expressions.Count);
        }
    }

    class CommentExpression : ElementExpression
    {
        internal string Value { get; private set; }

        internal CommentExpression(string content)
            : base("CommentExpression")
        {
            Value = content;
        }
    }

    class ProseValExpression : ElementExpression
    {
        internal ProseValExpression(string name)
            : base(name)
        {
        }
    }

    abstract class RepeatExpression : ElementExpression
    {
        internal RepeatExpression(string name)
            : base(name)
        {
        }
    }

    class RepeatCountExpression : RepeatExpression
    {
        internal int Count { get; private set; }

        internal RepeatCountExpression(int count)
            : base("repeat-count")
        {
            Count = count;
        }
    }

    class RepeatRangeExpression : RepeatExpression
    {
        internal int AtLeast { get; private set; }
        internal int AtMost { get; private set; }

        internal RepeatRangeExpression(int atLEast, int atMost)
            : base("repeat-range")
        {
            AtLeast = atLEast;
            AtMost = atMost;
        }
    }

    class Terminal : ElementExpression
    {
        internal Terminal(string name)
            : base(name)
        {
        }
    }

    class Terminal<T> : Terminal
    {
        internal T Value { get; private set; }

        internal Terminal(string name, T value)
            : base(name)
        {
            Value = value;
        }
    }

    class Characters : Terminal
    {
        internal Characters(string value)
            : base("NumericTerminal")
        {
            Value = value;
        }

        internal String Value { get; private set; }
    }

    class CharacterRange : Terminal
    {
        internal string Start { get; private set; }
        internal string Stop { get; private set; }

        internal CharacterRange(string start, string stop)
            : base("NumericRangeTerminal")
        {
            Start = start;
            Stop = stop;
        }
    }

    class Tminators
    {
        internal readonly static Terminal<string> EndOfLine = new Terminal<string>("end-of-line", Environment.NewLine);
    }

    /// <summary>
    /// Implements combinator parser for ABNFParser syntax. http://tools.ietf.org/html/rfc5234
    /// </summary>
    internal static class ABNFParser
    {
        internal static class Term
        {
            /// <summary>ALPHA = %x41-5A / %x61-7A ; A-Z / a-z</summary>
            internal static readonly Parser<char> Aplha =
                Parse.Letter;

            /// <summary>BIT = "0" / "1"</summary>
            internal static readonly Parser<char> Bit =
                Parse.Char('0').Or(Parse.Char('1'));

            /// <summary>CHAR = %x01-7F</summary>
            /// <remarks>any 7-bit US-ASCCII character, excluding NUL (i.e. depends on encoding)</remarks>
            internal static readonly Parser<char> Char =
                Parse.Char(c => c > (char)0x01 && c < (char)0x7F, "US ASCII Char");

            /// <summary>CRLF = CR LF</summary>
            /// <remarks>
            /// Internet standard newline.
            /// CR = %x0D; carriage return
            /// LF = %x0A; linefeed
            /// </remarks>
            internal static readonly Parser<IEnumerable<char>> CrLf =
                Parse.String(Environment.NewLine);

            /// <summary>DQUOTE = %x22</summary>
            /// <remarks>" (Double Quote)</remarks>
            internal static readonly Parser<char> DQuote =
                Parse.Char('"');

            /// <summary>DIGIT = %x30-39</summary>
            /// <remarks>0-9</remarks>
            internal static readonly Parser<char> Digit =
                Parse.Digit;

            /// <summary>HEXDIG = DIGIT / "A" / "B" / "C" / "D" / "E" / "F"</summary>
            internal static readonly Parser<char> HexDig =
                Digit.XOr(Parse.Chars("abcdefABCDEF"));

            /// VCHAR = %x21-7E ; visible (printing) characters
            internal static readonly Parser<char> VChar =
                Parse.Char(c => c > (char)0x21 && c < (char)0x7E, "US ASCII Printable Char");

            /// <summary>HTAB = %x09</summary>
            /// <remarks>horizontal tab</remarks>
            internal static readonly Parser<char> HTab =
                Parse.Char('\t');

            /// <summary>SP = %x20</summary>
            internal static readonly Parser<char> Space =
                Parse.Char(' ');

            /// <summary>WSP = SP / HTAB</summary>
            /// <remarks>white space</remarks>
            internal static readonly Parser<char> WSp =
                Space.XOr(HTab);

            internal static readonly Parser<char> Hyphen = 
                Parse.Char('-');

            internal static readonly Parser<char> PercentChar = 
                Parse.Char('%');

            internal static readonly Parser<Char> DotChar = 
                Parse.Char('.');
        }

        private static readonly Parser<char> BinSeq =
            from byteStr in Term.Bit.AtLeastOnce().Text()
            select Str2Byte2Char(byteStr, 2);

        private static readonly Parser<string> BinSeqCont =
            from d in Term.DotChar
            from bytes in Term.Bit.AtLeastOnce().Text().DelimitedBy(Term.DotChar)
            select Str2Byte2Str(bytes, 2);

        private static readonly Parser<string> BinRangeStop =
            from hyphen in Term.Hyphen
            from binStr in BinSeq.Once().Text()
            select binStr;

        /// <summary>bin-val = "b" 1*BIT [ 1*("." 1*BIT) / ("-" 1*BIT) ]</summary>
        private static readonly Parser<Terminal> BinVal =
            from b in Parse.Char('b')
            from seq in BinSeq.Once().Text()
            from optSeqCont in BinSeqCont.Optional()
            from optRangeStop in BinRangeStop.Optional()
            select optSeqCont.IsDefined ? new Characters(seq + optSeqCont.Get()) : 
                        optRangeStop.IsDefined ? (Terminal)new CharacterRange(seq, optRangeStop.Get()) :
                            new Characters(seq);

        private static readonly Parser<char> DecSeq =
            from byteStr in Term.Digit.AtLeastOnce().Text()
            select Str2Byte2Char(byteStr, 10);

        private static readonly Parser<string> DecSeqCont =
            from d in Term.DotChar
            from bytes in Term.Digit.AtLeastOnce().Text().DelimitedBy(Term.DotChar)
            select Str2Byte2Str(bytes, 10);

        private static readonly Parser<string> DecRangeStop =
            from hyphen in Term.Hyphen
            from binStr in DecSeq.Once().Text()
            select binStr;

        /// <summary>dec-val = "d" 1*DIGIT [ 1*("." 1*DIGIT) / ("-" 1*DIGIT) ]</summary>
        private static readonly Parser<Terminal> DecVal =
            from b in Parse.Char('d')
            from seq in DecSeq.Once().Text()
            from optSeqCont in DecSeqCont.Optional()
            from optRangeStop in DecRangeStop.Optional()
            select optSeqCont.IsDefined ? new Characters(seq + optSeqCont.Get()) : 
                        optRangeStop.IsDefined ? (Terminal)new CharacterRange(seq, optRangeStop.Get()) :
                            new Characters(seq);

        private static readonly Parser<char> HexSeq =
            from byteStr in Term.HexDig.AtLeastOnce().Text()
            select Str2Byte2Char(byteStr, 16);

        private static readonly Parser<string> HexSeqCont =
            from d in Term.DotChar
            from bytes in Term.HexDig.AtLeastOnce().Text().DelimitedBy(Term.DotChar)
            select Str2Byte2Str(bytes, 16);

        private static readonly Parser<string> HexRangeStop =
            from hyphen in Term.Hyphen
            from binStr in HexSeq.Once().Text()
            select binStr;

        /// <summary>hex-val        =  "x" 1*HEXDIG [ 1*("." 1*HEXDIG) / ("-" 1*HEXDIG) ]</summary>
        private static readonly Parser<Terminal> HexVal =
            from b in Parse.Char('x')
            from seq in HexSeq.Once().Text()
            from optSeqCont in HexSeqCont.Optional()
            from optRangeStop in HexRangeStop.Optional()
            select optSeqCont.IsDefined ? new Characters(seq + optSeqCont.Get()) : 
                        optRangeStop.IsDefined ? (Terminal)new CharacterRange(seq, optRangeStop.Get()) :
                            new Characters(seq);

        internal static readonly Parser<Terminal> NumVal =
            from p in Term.PercentChar
            from t in BinVal.XOr(DecVal).XOr(HexVal)
            select t;

        /// <summary>char-val = DQUOTE *(%x20-21 / %x23-7E) DQUOTE</summary>
        internal static readonly Parser<Terminal> CharVal =
            from quotedString in Parse.AnyChar.Except(Term.DQuote).Many().Contained(Term.DQuote, Term.DQuote).Text()
            select (Terminal) new Terminal<string>("CharVal", quotedString);

        /// <summary>prose-val = "<" *(%x20-3D / %x3F-7E) ">"</summary>
        /// <remarks>bracketed string of SP and VCHAR without angles prose description, to be used as last resort</remarks>
        internal static readonly Parser<ElementExpression> ProseVal =
            from labr in Parse.Char('<')
            from name in Parse.Ref(() => RuleName)
            from rabr in Parse.Char('>')
            select new ProseValExpression(name);

        private static readonly Parser<ElementExpression> NewLine =
            from el in Term.CrLf.Text()
            select new Terminal<string>("CrLf", el);

        private static readonly Parser<Terminal<char>> WSp =
            Term.WSp.Named("Term.Wsp").Select(i => new Terminal<char>("WSp", i));

        /// <summary>comment = ";" *(WSP / VCHAR) CRLF</summary>
        internal static readonly Parser<ElementExpression> Comment =
            from sc in Parse.Char(';')
            from content in (Term.WSp.Or(Term.VChar)).XMany().Text()
            from endline in Term.CrLf.Text().XMany().Optional() // .Optional() added from logical point of the view
            select new CommentExpression(content);

        /// <summary>c-nl = comment / CRLF</summary>
        /// <remarks>comment or newline</remarks>
        internal static readonly Parser<ElementExpression> CommentOrNewLine =
            Comment.Or(NewLine);

        /// <summary>c-nl-wsp = c-nl WSP </summary>
        private static readonly Parser<ElementExpression> CommentOrNewLineWsp =
            from cmt in CommentOrNewLine
            from wsp in WSp.Optional() // .Optional() added from logical point of the view
            select cmt;

        /// <summary>c-wsp = WSP / (c-nl WSP)</summary>
        internal static readonly Parser<ElementExpression> CWsp =
            WSp.Or(CommentOrNewLineWsp);

        // internal static readonly Parser<ElementExpression> CommentOrWhitepsace =
        /// <summary>
        /// rulename = ALPHA *(ALPHA / DIGIT / "-")
        /// </summary>
        internal static readonly Parser<string> RuleName =
            from first in Term.Aplha.Once()
            from rest in Term.Aplha.Or(Term.Digit).Except(Parse.Char('_')).XOr(Term.Hyphen).Many()
            select new string(first.Concat(rest).ToArray());

        internal static readonly Parser<RepeatExpression> RepeatCount =
            from count in Term.Digit.AtLeastOnce().Text()
            select new RepeatCountExpression(int.Parse(count));

        internal static readonly Parser<RepeatExpression> RepeatRange =
            from min in Term.Digit.XMany().Text()
            from star in Parse.Char('*')
            from max in Term.Digit.XMany().Text()
            select new RepeatRangeExpression(ParseIntWithDefault(min,0), ParseIntWithDefault(max,int.MaxValue));

        private static int ParseIntWithDefault(string n, int @default)
        {
            if (string.IsNullOrWhiteSpace(n))
                return @default;
            return int.Parse(n);
        }

        /// <summary>repeat = 1*DIGIT / (*DIGIT "*" *DIGIT)</summary>
        internal static readonly Parser<RepeatExpression> Repeat =
            RepeatRange.Or(RepeatCount);

        /// <summary>repetition = [repeat] element</summary>
        internal static readonly Parser<ElementExpression> Repetition =
            from r in Repeat.Optional()
            from e in ParseElement()
            select r.IsDefined ? new RepetitionExpression(r.Get(), e) : e;

        /// <summary> 1*c-wsp repetition </summary>
        internal static readonly Parser<ElementExpression> CWspRepetition =
            from cwsp in CWsp.AtLeastOnce()
            from r in Repetition.Once()
            select cwsp.Any(c => c is CommentExpression) ?
                new SequenceExpression(cwsp.Where(c => c is CommentExpression).Concat(r).ToList()) : r.First();

        /// <summary> concatenation = repetition *(1*c-wsp repetition) </summary>
        internal static readonly Parser<ElementExpression> Concatenation =
            from first in Repetition.Once()
            from rest in CWspRepetition.Many()
            select new SequenceExpression(first.Concat(rest).Select(t => t).ToList());

        /// <summary> *c-wsp "/" *c-wsp concatenation </summary>
        internal static readonly Parser<ElementExpression> CWspAlternation =
            from cwsp1 in CWsp.Many()
            from slash in Parse.Char('/')
            from cwsp2 in CWsp.Many()
            from concatenation in Concatenation.Once()
            select cwsp2.Any(c => c is CommentExpression) ?
                new SequenceExpression(cwsp2.Where(c => c is CommentExpression).Concat(concatenation).ToList())
                    : concatenation.First();

        /// <summary> alternation = concatenation *(*c-wsp "/" *c-wsp concatenation) </summary>
        internal static readonly Parser<ElementExpression> Alternation =
            from concat in Concatenation.Once()
            from rest in CWspAlternation.Many()
            select new AlternationExpression(concat.Concat(rest).Select(t => t).ToList());

        /// <summary>  *c-wsp alternation *c-wsp </summary>
        internal static readonly Parser<ElementExpression> CWspGroup = 
            from cwsp1 in CWsp.Many()
            from alt in Alternation.Once()
            from cwsp2 in CWsp.Many()
            select new GroupExpression(
                cwsp1.Where(c => c is CommentExpression).Concat(alt).Concat(cwsp2.Where(c => c is CommentExpression)).ToList());

        /// <summary> group = "(" *c-wsp alternation *c-wsp ")" </summary>
        internal static readonly Parser<ElementExpression> Group =
            CWspGroup.Contained(Parse.Char('('), Parse.Char(')'));

        /// <summary>  *c-wsp alternation *c-wsp </summary>
        internal static readonly Parser<ElementExpression> CWspOption =
            from cwsp1 in CWsp.Many()
            from alt in Alternation.Once()
            from cwsp2 in CWsp.Many()
            select new OptionExpression(
                cwsp1.Where(c => c is CommentExpression).Concat(alt).Concat(cwsp2.Where(c => c is CommentExpression)).ToList());

        /// <summary> option = "[" *c-wsp alternation *c-wsp "]" </summary>
        internal static readonly Parser<ElementExpression> Option =
            CWspOption.Contained(Parse.Char('['), Parse.Char(']'));

        /// <summary> elements = alternation *c-wsp </summary>
        internal static readonly Parser<ElementExpression> Elements =
            from alt in Alternation.Once()
            from cmts in CWsp.Many()
            select new SequenceExpression(alt.Concat(cmts)); // .Where(c => c is CommentExpression)).ToList());

        /// <summary>assignemnt-seq = ("=" / "=/")</summary>
        internal static readonly Parser<ElementExpression> AssignmentSeq =
            from eq in Parse.Char('=').Once().Text()
            from comb in Parse.Char('/').Optional()
            select new Terminal<string>("assignment", comb.IsEmpty ? eq : eq + comb.Get());

        /// <summary>basic rules definition and incremental alternatives</summary>
        /// <remarks>defined-as = *c-wsp ("=" / "=/") *c-wsp</remarks>
        internal static readonly Parser<ElementExpression> DefinedAs =
            from cwsp1 in CWsp.Many().Named("cwsp1")
            from op in AssignmentSeq.Named("AssignmentSeq")
            from cwsp2 in CWsp.Many().Named("cwsp2")
            select op;

        /// <summary>
        /// rule           =  rulename defined-as elements c-nl
        /// </summary>
        internal static readonly Parser<ElementExpression> Rule =
            from ruleName in RuleName
            from definedAs in DefinedAs
            from elements in Elements
            from commentOrNewLine in CommentOrNewLine.Named("commentOrNewLine")
            select definedAs.As<Terminal<string>>().Value.Equals("=") ?
                (ElementExpression) new RuleExpression("rule", ruleName, elements.As<SequenceExpression>()) :
                (ElementExpression) new OrRuleExpression("or-rule", ruleName, elements.As<SequenceExpression>());

        /// <summary>
        /// *c-wsp c-nl
        /// </summary>
        internal static readonly Parser<ElementExpression> CWspRuleList =
            from cmt in CWsp.XMany().Named("cmt")
            from cnl in CommentOrNewLine.Once().Named("cnl")
            select new SequenceExpression(cmt.Concat(cnl)); // .Where(i => i is CommentExpression)));

        //TODO: Continue here - fix multiple rules parsing. Comments annt in the result
        /// <summary>
        /// rulelist       =  1*( rule / (*c-wsp c-nl) )
        /// </summary>
        internal static readonly Parser<ElementExpression> RuleList =
            from rules in (Rule.Or(CWspRuleList)).AtLeastOnce()
            select new SequenceExpression(rules);

        /// <summary>element = rulename / group / option / char-val / num-val / prose-val</summary>
        internal static Parser<ElementExpression> ParseElement()
        {
            return RuleName.Select(t => new Terminal<string>("rulename", t)).XOr(Group).XOr(Option)
                    .XOr(CharVal).XOr(NumVal).XOr(ProseVal);
        }

        private static string Str2Byte2Str(IEnumerable<string> byteStrs, int fromBase)
        {
            var res = new StringBuilder();
            foreach (var byteStr in byteStrs)
            {
                res.Append(Str2Byte2Char(byteStr, fromBase));
            }
            return res.ToString();
        }

        private static char Str2Byte2Char(string byteStr, int fromBase)
        {
            return Convert.ToChar(Convert.ToByte(byteStr, fromBase));
        }
    }

    internal class AlternationExpression : ElementExpression
    {
        public IList<ElementExpression> Alternations { get; private set; }

        public AlternationExpression(IList<ElementExpression> alternations)
            : base("alternation")
        {
            Alternations = alternations;
        }
    }

    internal class SequenceExpression : ElementExpression
    {
        public IList<ElementExpression> Expressions { get; private set; }

        internal SequenceExpression(IEnumerable<ElementExpression> expressions)
            : base("sequence")
        {
            Expressions = new List<ElementExpression>(expressions);
        }

        internal SequenceExpression(IList<ElementExpression> expressions)
            : base("sequence")
        {
            Expressions = expressions;
        }
    }

    internal class GroupExpression : ElementExpression
    {
        public IList<ElementExpression> Expressions { get; private set; }

        internal GroupExpression(IList<ElementExpression> expressions)
            : base("group")
        {
            Expressions = expressions;
        }
    }

    internal class OptionExpression : ElementExpression
    {
        public IList<ElementExpression> Expressions { get; private set; }

        internal OptionExpression(IList<ElementExpression> expressions)
            : base("option")
        {
            Expressions = expressions;
        }
    }

    internal class RepetitionExpression : ElementExpression
    {
        public RepeatExpression Repeat { get; private set; }
        public ElementExpression Element { get; set; }

        public RepetitionExpression(RepeatExpression repeat, ElementExpression element)
            : base("repetition")
        {
            Repeat = repeat;
            Element = element;
        }
    }
}