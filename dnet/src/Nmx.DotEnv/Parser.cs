using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NerdyMishka.Text.DotEnv
{
    public class Parser : IDisposable
    {
        private TokenStream stream;

        private Dictionary<string, string> result = new Dictionary<string, string>();

        private ValueStates valueState = ValueStates.None;

        private string lastWord;

        [Flags]
        public enum ValueStates : int
        {
            /// <summary>
            /// None.
            /// </summary>
            None = 0,

            /// <summary>
            /// Single Quote.
            /// </summary>
            SingleQuote = 1,

            /// <summary>
            /// Double Quote.
            /// </summary>
            DoubleQuote = 2,

            /// <summary>
            /// JSON.
            /// </summary>
            Json = 3,
        }

        public enum ParserState : int
        {
            /// <summary>
            /// None.
            /// </summary>
            None = 0,

            /// <summary>
            /// Comment.
            /// </summary>
            Comment = 1,

            /// <summary>
            /// Key.
            /// </summary>
            Key = 2,

            /// <summary>
            ///  Value.
            /// </summary>
            Value = 3,

            /// <summary>
            ///  Multi-line.
            /// </summary>
            Multiline = 4,

            /// <summary>
            /// End.
            /// </summary>
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

            this.valueState = ValueStates.None;
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

            this.valueState = ValueStates.None;
            this.result.Add(this.lastWord, this.stream.GetWord().TrimEnd('\''));

            while (token.Kind != TokenKind.NewLine)
            {
                if (!this.stream.MoveNext())
                    return ParserState.End;

                token = this.stream.Current;
            }

            return ParserState.None;
        }

        [SuppressMessage("", "IDE0059:", Justification = "By Design")]
        private ParserState VisitJson()
        {
            Token lastToken = null;
            while (this.stream.Consume())
            {
                var currentToken = this.stream.Current;
                if (currentToken.Kind == TokenKind.JsonEnd &&
                    lastToken != null && lastToken.Kind == TokenKind.NewLine)
                {
                    this.stream.Consume();
                    var value = this.stream.GetWord();
                    this.result.Add(this.lastWord, value);

                    currentToken = this.stream.Current;
                    while (this.stream.Consume())
                    {
                        currentToken = this.stream.Current;
                        if (currentToken.Kind == TokenKind.NewLine)
                        {
                            this.valueState = ValueStates.None;
                            return ParserState.None;
                        }
                    }

                    return ParserState.End;
                }

                lastToken = currentToken;
            }

            return ParserState.End;
        }

        private ParserState VisitMultiline(Token token)
        {
            switch (this.valueState)
            {
                case ValueStates.DoubleQuote:
                    return this.VisitDoubleQuote(token);
                case ValueStates.SingleQuote:
                    return this.VisitSingleQuote(token);
                case ValueStates.Json:
                    return this.VisitJson();
                default:
                    throw new NotSupportedException($"unsupported value state {this.valueState}");
            }
        }

        private ParserState VisitValue(Token token)
        {
            if (this.stream.WordLength == 0 && this.valueState.HasFlag(ValueStates.None))
            {
                switch (token.Kind)
                {
                    case TokenKind.NewLine:
                        this.result.Add(this.lastWord, null);
                        return ParserState.None;
                    case TokenKind.WhiteSpace:
                        return ParserState.Value;
                    case TokenKind.DoubleQuote:
                        this.valueState = ValueStates.DoubleQuote;
                        if (!this.stream.MoveNext())
                            return ParserState.End;
                        token = this.stream.Current;
                        break;
                    case TokenKind.SingleQuote:
                        this.valueState = ValueStates.SingleQuote;
                        if (!this.stream.MoveNext())
                            return ParserState.End;
                        token = this.stream.Current;
                        break;
                    case TokenKind.JsonStart:
                        return this.VisitJson();
                }
            }

            if (this.valueState != ValueStates.None)
            {
                if (token.Kind == TokenKind.NewLine)
                {
                    this.stream.Consume();
                    return this.VisitMultiline(token);
                }

                switch (this.valueState)
                {
                    case ValueStates.DoubleQuote:
                        return this.VisitDoubleQuoteLine(token);
                    case ValueStates.SingleQuote:
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