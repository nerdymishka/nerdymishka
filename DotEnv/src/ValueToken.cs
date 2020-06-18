using System;
using System.Text;

namespace NerdyMishka.Text.DotEnv
{
    public class ValueToken : Token
    {
        public ValueToken(Span<char> value)
        {
            this.Kind = TokenKind.Text;
            this.Value = value.ToArray();
        }

        public ValueToken(StringBuilder builder)
        {
            this.Kind = TokenKind.Text;
            var set = new char[builder.Length];
            builder.CopyTo(0, set, 0, set.Length);
            this.Value = set;
        }

        public ValueToken(Memory<char> value)
        {
            this.Kind = TokenKind.Text;
            this.Value = value.ToArray();
        }
    }
}