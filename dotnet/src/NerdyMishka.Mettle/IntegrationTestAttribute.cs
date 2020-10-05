using Xunit.Sdk;

namespace Mettle
{
    [XunitTestCaseDiscoverer("Mettle.Xunit.Sdk.TestCaseDiscoverer", "NerdyMishka.Mettle")]
    public class IntegrationTestAttribute : TestCaseAttribute
    {
        public IntegrationTestAttribute()
        {
            this.Tags = this.Tags ?? "integration";
        }
    }
}