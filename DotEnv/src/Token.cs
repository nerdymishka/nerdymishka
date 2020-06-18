using System;
using System.Buffers;
using System.Text;

namespace NerdyMishka.Text.DotEnv
{
    public abstract class Token
    {
        public TokenKind Kind { get; set; }

        public char[] Value { get; protected set; }

        public static Token None => new CharToken(Char.MinValue, TokenKind.None);

        public static CharToken Assign => new CharToken('=', TokenKind.Assign);

        public static CharToken DoubleQuote => new CharToken('\"', TokenKind.DoubleQuote);

        public static CharToken SingleQuote => new CharToken('\"', TokenKind.SingleQuote);

        public static CharToken JsonStart => new CharToken('{', TokenKind.JsonStart);

        public static CharToken JsonEnd => new CharToken('}', TokenKind.JsonEnd);

        public static CharToken WhiteSpace => new CharToken(' ', TokenKind.WhiteSpace);

        public static CharToken CommentStart => new CharToken('#', TokenKind.Comment);

        public static NewLineToken NewLine => new NewLineToken();

        public static LineBreakToken LineBreak => new LineBreakToken();
    }
}