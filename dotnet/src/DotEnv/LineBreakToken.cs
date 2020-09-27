using System;

namespace NerdyMishka.Text.DotEnv
{
    public class LineBreakToken : Token
    {
        public LineBreakToken()
        {
            this.Value = Environment.NewLine.ToCharArray();
            this.Kind = TokenKind.LineBreak;
        }
    }
}