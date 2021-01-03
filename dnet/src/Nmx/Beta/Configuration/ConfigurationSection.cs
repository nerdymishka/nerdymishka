using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Nmx.Beta.State
{
    public class ConfigurationSection
    {
        protected internal JObject Bag { get; private set; } = new JObject();

        protected void SetValue<T>(string propertyName, ref T currentValue, T value)
        {
            if (ReferenceEquals(currentValue, value) || currentValue.Equals(value))
                return;

            if (value is ConfigurationSection ss)
            {
                this.Bag[propertyName] = ss.Bag;
                currentValue = value;
                return;
            }

            this.Bag[propertyName] = JToken.FromObject(value);
            currentValue = value;
        }
    }
}
