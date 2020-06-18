using System;
using System.Collections.Generic;

namespace NerdyMishka.Text.DotEnv
{
    public class Parser : IDisposable
    {
        private TokenStream stream;

        private Dictionary<string, string> result = new Dictionary<string, string>();

        private ValueState valueState = ValueState.None;

        private string lastWord;

        public enum ValueState : int
        {
            None = 0,

            SingleQuote = 1,

            DoubleQuote = 2,

            Json = 3,
        }

        public enum ParserState : int
        {
            None = 0,

            Comment = 1,

            Key = 2,

            Value = 3,

            Multiline = 4,

            End = 400,
        }

        public IDictionary<string, string> Parse(IReadOnlyList<Token> tokens)
        {
            var state = ParserState.None;
            this.stream = new TokenStream(tokens);

            while (this.stream.MoveNext())
            {
                var token = this.stream.Current;
                switch (state)
                {
                    case ParserState.End:
                        {
                            var r = new Dictionary<string, string>(this.result);
                            this.result.Clear();
                            return r;
                        }

                    case ParserState.Comment:
                        state = this.MoveNextLine(token);
                        break;
                    case ParserState.None:
                        state = this.VisitNone(token);
                        break;
                    case ParserState.Key:
                        state = this.VisitKey(token);
                        break;
                    case ParserState.Multiline:
                        state = this.VisitMultiline(token);
                        break;
                    case ParserState.Value:
                        state = this.VisitValue(token);
                        break;
                }
            }

            var r2 = new Dictionary<string, string>(this.result);
            this.result.Clear();
            return r2;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
            if (dispose)
            {
                this.stream.Dispose();
                this.stream = null;
                this.lastWord = null;
                this.result = null;
            }
        }

        private ParserState VisitKey(Token token)
        {
            while (token.Kind != TokenKind.Assign)
            {
                if (!this.stream.Consume())
                    return ParserState.End;

                token = this.stream.Current;
                if (token.Kind == TokenKind.NewLine)
                {
                    this.result.Add(this.stream.GetWord().Trim(), null);
                    return ParserState.None;
                }
            }

            this.lastWord = this.stream.GetWord().Trim();
            return ParserState.Value;
        }

        private ParserState VisitDoubleQuote(Token token)
        {
            while (token.Kind != TokenKind.DoubleQuote)
            {
                // transform \n into line break
                if (token.Kind == TokenKind.LineBreak)
                {
                    if (!this.stream.Consume())
                        return ParserState.End;

                    token = this.stream.Current;
                    continue;
                }

                if (!this.stream.Consume())
                    return ParserState.End;

                token = this.stream.Current;
            }

            this.valueState = ValueState.None;
            this.result.Add(this.lastWord, this.stream.GetWord().TrimEnd('\"'));

            while (token.Kind != TokenKind.NewLine)
            {
                if (!this.stream.MoveNext())
                    return ParserState.End;

                token = this.stream.Current;
            }

            return ParserState.None;
        }

        private ParserState VisitSingleQuote(Token token)
        {
            while (token.Kind != TokenKind.SingleQuote)
            {
                // transform \n into line break
                if (token.Kind == TokenKind.LineBreak)
                {
                    if (!this.stream.Consume())
                        return ParserState.End;

                    token = this.stream.Current;
                    continue;
                }

                if (!this.stream.Consume())
                    return ParserState.End;

                token = this.stream.Current;
            }

            this.valueState = ValueState.None;
            this.result.Add(this.lastWord, this.stream.GetWord().TrimEnd('\''));

            while (token.Kind != TokenKind.NewLine)
            {
                if (!this.stream.MoveNext())
                    return ParserState.End;

                token = this.stream.Current;
            }

            return ParserState.None;
        }

        private ParserState VisitJson(Token token)
        {
            Token lastToken = null;
            while (this.stream.Consume())
            {
                var current = this.stream.Current;
                if (current.Kind == TokenKind.JsonEnd &&
                    lastToken.Kind == TokenKind.NewLine)
                {
                    this.stream.Consume();
                    var value = this.stream.GetWord();
                    this.result.Add(this.lastWord, value);

                    token = this.stream.Current;
                    while (this.stream.Consume())
                    {
                        token = this.stream.Current;
                        if (token.Kind == TokenKind.NewLine)
                        {
                            this.valueState = ValueState.None;
                            return ParserState.None;
                        }
                    }

                    return ParserState.End;
                }

                lastToken = current;
            }

            return ParserState.End;
        }

        private ParserState VisitMultiline(Token token)
        {
            switch (this.valueState)
            {
                case ValueState.DoubleQuote:
                    return this.VisitDoubleQuote(token);
                case ValueState.SingleQuote:
                    return this.VisitSingleQuote(token);
                case ValueState.Json:
                    return this.VisitJson(token);
                default:
                    throw new NotSupportedException($"unsupported value state {this.valueState}");
            }
        }

        private ParserState VisitValue(Token token)
        {
            if (this.stream.WordLength == 0 && this.valueState.HasFlag(ValueState.None))
            {
                switch (token.Kind)
                {
                    case TokenKind.NewLine:
                        this.result.Add(this.lastWord, null);
                        return ParserState.None;
                    case TokenKind.WhiteSpace:
                        return ParserState.Value;
                    case TokenKind.DoubleQuote:
                        this.valueState = ValueState.DoubleQuote;
                        if (!this.stream.MoveNext())
                            return ParserState.End;
                        token = this.stream.Current;
                        break;
                    case TokenKind.SingleQuote:
                        this.valueState = ValueState.SingleQuote;
                        if (!this.stream.MoveNext())
                            return ParserState.End;
                        token = this.stream.Current;
                        break;
                    case TokenKind.JsonStart:
                        return this.VisitJson(token);
                }
            }

            if (this.valueState != ValueState.None)
            {
                if (token.Kind == TokenKind.NewLine)
                {
                    this.stream.Consume();
                    return this.VisitMultiline(token);
                }

                switch (this.valueState)
                {
                    case ValueState.DoubleQuote:
                        return this.VisitDoubleQuoteLine(token);
                    case ValueState.SingleQuote:
                        return this.VisitSingleQuoteLine(token);
                }
            }

            while (token.Kind != TokenKind.NewLine)
            {
                if (!this.stream.Consume())
                    return ParserState.End;

                token = this.stream.Current;
            }

            this.result.Add(this.lastWord, this.stream.GetWord().Trim());
            return ParserState.None;
        }

        private ParserState VisitDoubleQuoteLine(Token token)
        {
            while (token.Kind != TokenKind.DoubleQuote)
            {
                if (token.Kind == TokenKind.LineBreak)
                {
                    if (!this.stream.Consume())
                        return ParserState.End;

                    token = this.stream.Current;
                    continue;
                }

                if (token.Kind == TokenKind.NewLine)
                {
                    this.stream.Consume();
                    token = this.stream.Current;
                    return this.VisitDoubleQuote(token);
                }

                if (!this.stream.Consume())
                    return ParserState.End;

                token = this.stream.Current;
            }

            this.result.Add(this.lastWord, this.stream.GetWord());
            return this.MoveNextLine(token);
        }

        private ParserState VisitSingleQuoteLine(Token token)
        {
            while (token.Kind != TokenKind.SingleQuote)
            {
                if (token.Kind == TokenKind.LineBreak)
                {
                    this.stream.Append(Environment.NewLine);
                    if (!this.stream.MoveNext())
                        return ParserState.End;

                    token = this.stream.Current;
                    continue;
                }

                if (token.Kind == TokenKind.NewLine)
                {
                    this.stream.Consume();
                    token = this.stream.Current;
                    return this.VisitSingleQuote(token);
                }

                if (!this.stream.Consume())
                    return ParserState.End;

                token = this.stream.Current;
            }

            this.result.Add(this.lastWord, this.stream.GetWord());
            return this.MoveNextLine(token);
        }

        private ParserState MoveNextLine(Token token)
        {
            while (token.Kind != TokenKind.NewLine)
            {
                if (!this.stream.MoveNext())
                    return ParserState.End;

                token = this.stream.Current;
            }

            return ParserState.None;
        }

        private ParserState VisitNone(Token token)
        {
            switch (token.Kind)
            {
                case TokenKind.Comment:
                    return ParserState.Comment;
                case TokenKind.WhiteSpace:
                case TokenKind.NewLine:
                    return ParserState.None;
                case TokenKind.Text:
                    this.stream.Consume(false);
                    return ParserState.Key;
                default:
                    {
                        throw new NotSupportedException(token.Kind.ToString());
                    }
            }
        }
    }
}