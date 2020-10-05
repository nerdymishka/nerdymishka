using System;

namespace NerdyMishka.Text
{
    public interface ITextTransform
    {
        ReadOnlySpan<char> Transform(ReadOnlySpan<char> input);
    }
}