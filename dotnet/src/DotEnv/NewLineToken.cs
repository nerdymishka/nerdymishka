using System;

namespace NerdyMishka.Text.DotEnv
{
    public class NewLineToken : Token
    {
        public NewLineToken()
        {
            this.Value = Environment.NewLine.ToCharArray();
            this.Kind = TokenKind.NewLine;
        }
    }
}
