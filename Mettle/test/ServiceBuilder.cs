using System;
using Mettle;

namespace Tests
{
    public class ServiceBuilder : IServiceProviderFactory
    {
        public IServiceProvider CreateProvider()
        {
            var provider = new SimpleServiceProvider();
            return provider;
        }
    }

    public class ServiceBuilder2 : IServiceProviderFactory
    {
        public IServiceProvider CreateProvider()
        {
            var provider = new SimpleServiceProvider();
            provider.AddTransient(typeof(UnitTestData), (s) => { return new UnitTestData(); });

            return provider;
        }
    }

    public class UnitTestData
    {
        public string Name { get; set; } = "test";
    }
}