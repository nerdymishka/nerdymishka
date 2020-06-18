namespace NerdyMishka.Text.DotEnv
{
    public enum TokenKind
    {
        None,

        NewLine,

        LineBreak,

        WhiteSpace,

        Comment,

        Text,

        Assign,

        SingleQuote,

        DoubleQuote,

        JsonStart,

        JsonEnd,
    }
}