namespace NerdyMishka.Text.DotEnv
{
    public enum TokenKind
    {
        /// <summary>None.</summary>
        None,

        /// <summary>New Line.</summary>
        NewLine,

        /// <summary>Line Break.</summary>
        LineBreak,

        /// <summary>White Space.</summary>
        WhiteSpace,

        /// <summary>Comment.</summary>
        Comment,

        /// <summary>Text.</summary>
        Text,

        /// <summary>Assign.</summary>
        Assign,

        /// <summary>Single Quote.</summary>
        SingleQuote,

        /// <summary>Double Quote.</summary>
        DoubleQuote,

        /// <summary>Json Start.</summary>
        JsonStart,

        /// <summary>Json End.</summary>
        JsonEnd,
    }
}