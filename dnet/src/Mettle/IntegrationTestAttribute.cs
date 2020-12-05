using System.Runtime.InteropServices;
using Xunit.Sdk;

namespace Mettle
{
    [XunitTestCaseDiscoverer("Mettle.Xunit.Sdk.TestCaseDiscoverer", "Mettle")]
    public class IntegrationTestAttribute : TestCaseAttribute
    {
        public IntegrationTestAttribute()
        {
            this.Tags ??= "integration";
        }

        public OSPlatform[] Platforms { get; set; }
    }
}