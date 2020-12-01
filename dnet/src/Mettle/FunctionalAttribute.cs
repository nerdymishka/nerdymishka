using System;
using System.Runtime.InteropServices;
using Xunit.Sdk;

namespace Mettle
{
    [XunitTestCaseDiscoverer("Mettle.Xunit.Sdk.TestCaseDiscoverer", "Mettle")]
    public class FunctionalAttribute : TestCaseAttribute
    {
        public FunctionalAttribute()
        {
            this.Tags ??= "functional";
        }

        public OSPlatform[] Platforms { get; set; }
    }
}