using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Shell
{
    public class CommandOptions
    {
        private Dictionary<string, object> options = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        private string delimiter = " ";

        private bool quote = true;

        public override string ToString()
        {
            var initialParams = this.options.Keys.Where(o => o.StartsWith('{')).ToList();
            var sb = new StringBuilder();

            foreach (var p in initialParams)
                sb.Append(' ').Append(this.options[p]);

            foreach (var kv in this.options)
            {
                if (kv.Key.StartsWith('{'))
                    continue;

                var v = kv.Value;

                switch (v)
                {
                    case bool flag:
                        if (flag)
                            sb.Append(' ').Append(kv.Key);
                        break;

                    default:
                        sb.Append(' ')
                            .Append(kv.Key)
                            .Append(this.delimiter);

                        if (this.quote)
                            sb.Append('\"');

                        sb.Append(kv.Value);

                        if (this.quote)
                            sb.Append('\"');

                        break;
                }
            }

            return sb.ToString();
        }

        protected T Get<T>(string name)
        {
            if (!this.options.TryGetValue(name, out var value))
                return default;

            if (value is null)
                return default;

            return (T)value;
        }

        protected void Set<T>(string name, T value)
        {
            this.options[name] = value;
        }
    }
}
