using System;
using System.Text;

namespace NerdyMishka.Text
{
    public class UnderscoreTransform : ITextTransform
    {
        public bool ConvertSpaces { get; set; }

        public ReadOnlySpan<char> Transform(ReadOnlySpan<char> identifier)
        {
            var sb = new StringBuilder();
            var i = 0;
            foreach (var c in identifier)
            {
                if (i == 0 && char.IsUpper(c))
                {
                    sb.Append(char.ToLowerInvariant(c));
                    i++;
                    continue;
                }

                if (c == '-' || (c == ' ' && this.ConvertSpaces))
                {
                    sb.Append('_');
                    i++;
                    continue;
                }

                if (char.IsUpper(c))
                {
                    if (char.IsLetterOrDigit(identifier[i - 1]))
                    {
                        sb.Append('_');
                    }

                    sb.Append(char.ToLowerInvariant(c));
                    i++;
                    continue;
                }

                sb.Append(c);
                i++;
            }

            var set = new char[sb.Length];
            sb.CopyTo(0, set, 0, set.Length);
            return set.AsSpan();
        }
    }
}