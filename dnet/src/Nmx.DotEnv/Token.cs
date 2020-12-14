using System.Collections.Generic;
using System.Linq;

namespace NerdyMishka.Text.DotEnv
{
    public abstract class Token
    {
        private char[] set;

        public static Token None => new CharToken(char.MinValue, TokenKind.None);

        public static CharToken Assign => new CharToken('=', TokenKind.Assign);

        public static CharToken DoubleQuote => new CharToken('\"', TokenKind.DoubleQuote);

        public static CharToken SingleQuote => new CharToken('\"', TokenKind.SingleQuote);

        public static CharToken JsonStart => new CharToken('{', TokenKind.JsonStart);

        public static CharToken JsonEnd => new CharToken('}', TokenKind.JsonEnd);

        public static CharToken WhiteSpace => new CharToken(' ', TokenKind.WhiteSpace);

        public static CharToken CommentStart => new CharToken('#', TokenKind.Comment);

        public static NewLineToken NewLine => new NewLineToken();

        public static LineBreakToken LineBreak => new LineBreakToken();

        public TokenKind Kind { get; set; }

        public IReadOnlyList<char> Value
        {
            get => this.set;
            set
            {
                if (value == null || value.Count == 0)
                {
                    this.set = System.Array.Empty<char>();
                    return;
                }

                this.set = value.ToArray();
            }
        }

        protected internal void Clear()
        {
            System.Array.Clear(this.set, 0, this.set.Length);
            this.set = null;
        }
    }
}