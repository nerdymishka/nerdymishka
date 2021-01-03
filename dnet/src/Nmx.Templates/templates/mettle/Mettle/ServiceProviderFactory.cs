using System;
using Mettle;

// ReSharper disable CheckNamespace
namespace Tests
{
    public class ServiceProviderFactory : IServiceProviderFactory
    {
        public IServiceProvider CreateProvider()
        {
            var provider = new Mettle.SimpleServiceProvider();

            return provider;
        }
    }
}