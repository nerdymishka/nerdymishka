using System;
using System.Text;

namespace NerdyMishka.Text
{
    public static class StringBuilderExtensions
    {
        public static int IndexOf(
            this StringBuilder builder,
            string searchText,
            StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return builder
                .ToReadOnlySpan()
                .IndexOf(searchText.AsSpan(), comparison);
        }

        public static char[] ToArray(this StringBuilder builder)
        {
            Check.NotNull(nameof(builder), builder);
            var set = new char[builder.Length];
            builder.CopyTo(0, set, 0, set.Length);
            return set;
        }

        public static ReadOnlySpan<char> ToReadOnlySpan(this StringBuilder builder)
        {
            Check.NotNull(nameof(builder), builder);
            var set = new char[builder.Length];
            builder.CopyTo(0, set, 0, set.Length);
            return set;
        }

        public static StringBuilder Append(
            this StringBuilder builder,
            Span<char> input)
        {
            Check.NotNull(nameof(builder), builder);

            for (var i = 0; i < input.Length; i++)
                builder.Append(input[i]);

            return builder;
        }

        public static StringBuilder Append(
            this StringBuilder builder,
            ReadOnlySpan<char> input)
        {
            Check.NotNull(nameof(builder), builder);

            for (var i = 0; i < input.Length; i++)
                builder.Append(input[i]);

            return builder;
        }

        public static StringBuilder Append(
            this StringBuilder builder,
            ReadOnlyMemory<char> input)
        {
            Check.NotNull(nameof(builder), builder);
            Check.NotNull(nameof(input), input);

            builder.Append(input.Span);
            return builder;
        }

        public static bool Contains(
            this StringBuilder builder,
            string searchText,
            StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return Contains(builder, searchText.AsSpan(), comparison);
        }

        public static bool Contains(
            this StringBuilder builder,
            ReadOnlySpan<char> searchText,
            StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return builder
                .ToReadOnlySpan()
                .Contains(searchText, comparison);
        }

        public static StringBuilder AppendTransform(
            this StringBuilder builder,
            string identifier,
            ITextTransform transformer)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            if (transformer is null)
                throw new ArgumentNullException(nameof(transformer));

            return builder.Append(
                transformer.Transform(identifier.AsSpan()).ToArray());
        }

        public static StringBuilder AppendQuote(
            this StringBuilder builder,
            string value,
            char leftQuote = '"',
            char rightQuote = '"')
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Append(leftQuote)
                    .Append(value)
                    .Append(rightQuote);
        }

        public static StringBuilder AppendQuote(
            this StringBuilder builder,
            string value,
            ITextTransform transformer,
            char leftQuote = '"',
            char rightQuote = '"')
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            if (transformer is null)
                throw new ArgumentNullException(nameof(transformer));

            return builder.Append(leftQuote)
                    .Append(transformer.Transform(value.AsSpan()).ToArray())
                    .Append(rightQuote);
        }
    }
}