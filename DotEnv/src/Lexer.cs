using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NerdyMishka.Text.DotEnv
{
    public static class Lexer
    {
        public static IReadOnlyList<Token> Tokenize(string content)
        {
            if (content is null)
                throw new ArgumentNullException(nameof(content));

            using (var sr = new StringReader(content))
            {
                return Tokenize(sr);
            }
        }

        public static IReadOnlyList<Token> Tokenize(TextReader reader)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            Span<char> buffer = stackalloc char[1024];
            var last = char.MinValue;
            int bytesRead = 0;
            var sb = new System.Text.StringBuilder();
            var list = new List<Token>();

            Action<StringBuilder, Token> consume = (sb2, t) =>
            {
                if (sb2.Length > 0)
                {
                    list.Add(new ValueToken(sb2));
                    sb2.Clear();
                }

                list.Add(t);
            };

            do
            {
                bytesRead = reader.ReadBlock(buffer);
                for (var i = 0; i < bytesRead; i++)
                {
                    var c = buffer[i];
                    if (last == '\\' && c == 'n')
                    {
                        if (sb.Length > 1)
                        {
                            sb.Remove(sb.Length - 1, 1);
                            list.Add(new ValueToken(sb));
                        }

                        list.Add(Token.LineBreak);
                        sb.Clear();
                        last = c;
                        continue;
                    }

                    if (last == '\r' && c == '\n')
                    {
                        if (sb.Length > 1)
                        {
                            sb.Remove(sb.Length - 1, 1);
                            list.Add(new ValueToken(sb));
                        }

                        list.Add(Token.NewLine);
                        sb.Clear();
                        last = c;
                        continue;
                    }

                    switch (c)
                    {
                        case '=':
                            consume(sb, Token.Assign);
                            break;
                        case ' ':
                            consume(sb, Token.WhiteSpace);
                            break;
                        case '\'':
                            consume(sb, Token.SingleQuote);
                            break;
                        case '\"':
                            consume(sb, Token.DoubleQuote);
                            break;
                        case '{':
                            consume(sb, Token.JsonStart);
                            break;
                        case '}':
                            consume(sb, Token.JsonEnd);
                            break;
                        case '#':
                            consume(sb, Token.CommentStart);
                            break;
                        default:
                            sb.Append(c);
                            break;
                    }

                    last = c;
                }
            }
            while (bytesRead > 0);

            if (sb.Length > 0)
            {
                list.Add(new ValueToken(sb));
                sb.Clear();
            }

            sb = null;
            return list;
        }
    }
}