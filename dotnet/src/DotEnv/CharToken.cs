namespace NerdyMishka.Text.DotEnv
{
    public class CharToken : Token
    {
        public CharToken(char value, TokenKind kind)
        {
            this.Value = new char[] { value };
            this.Kind = kind;
        }
    }
}