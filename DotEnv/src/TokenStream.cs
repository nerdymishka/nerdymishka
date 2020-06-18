using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Text.DotEnv
{
    public class TokenStream : IDisposable
    {
        private IReadOnlyList<Token> tokens;

        private StringBuilder sb;

        private int position = -1;

        private Token current;

        public TokenStream(IReadOnlyList<Token> tokens)
        {
            this.sb = new StringBuilder(0);
            this.tokens = tokens;
            this.position = -1;
        }

        public int Count => this.tokens.Count;

        public Token Current => this.current;

        public int WordLength => this.sb.Length;

        public StringBuilder Builder => this.sb;

        public void Reset()
        {
            this.position = -1;
        }

        public Token Peek(int index)
        {
            if (index < 0 || index > this.Count)
                return null;

            return this.tokens[index];
        }

        public bool MoveNext()
        {
            if (++this.position >= this.Count)
                return false;

            this.current = this.tokens[this.position];
            return true;
        }

        public void Append(string value)
        {
            this.sb.Append(value);
        }

        public bool Consume(bool move = true)
        {
            // TODO: separate this out?
            this.sb.Append(this.current.Value);

            if (move)
                return this.MoveNext();

            return false;
        }

        public string GetWord(bool clear = true)
        {
            var word = this.sb.ToString();
            if (clear)
                this.sb.Clear();

            return word;
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
                foreach (var token in this.tokens)
                {
                    if (token is ValueToken)
                    {
                        token.Clear();
                    }
                }

                this.tokens = null;
                this.sb.Clear();
                this.sb = null;
            }
        }
    }
}