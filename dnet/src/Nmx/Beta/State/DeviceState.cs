using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nmx.Beta.State
{
    public class DeviceState
    {
        private JsonSerializerSettings serializerSettings;

        public DeviceState()
        {
            this.serializerSettings = new JsonSerializerSettings()
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Formatting = Formatting.Indented,
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            };
        }

        public static DeviceState Self { get; } = new DeviceState();

        public JObject Root { get; private set; } = new JObject();

        public void Save()
        {
            var configurationStore = this.GetStoreLocation();
            if (!Directory.Exists(configurationStore))
                Directory.CreateDirectory(configurationStore);

            // TODO: use streams
            var json = JsonConvert.SerializeObject(this.Root, this.serializerSettings);
            File.WriteAllText($"{configurationStore}/device_state.json", json);
        }

        public void Load()
        {
            var configurationStore = this.GetStoreLocation();
            if (!Directory.Exists(configurationStore))
                return;

            var stateFile = Path.Combine(configurationStore, "device_state.json");
            if (!File.Exists(stateFile))
                return;

            var json = File.ReadAllText(stateFile);
            this.Root = JObject.Parse(json);
        }

        protected virtual string GetStoreLocation()
        {
            string configurationStore = string.Empty;
            if (OperatingSystem.IsWindows())
            {
                configurationStore = Environment.GetEnvironmentVariable("ALLUSERSPROFILE");
                configurationStore = Path.Combine(configurationStore, "nmx", "etc");
            }
            else
            {
                configurationStore = "/etc/nmx";
            }

            return configurationStore;
        }
    }
}
