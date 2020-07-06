using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.Api.KeePass.Package
{
    public static class RandomByteGeneratorFactory
    {
        private static readonly List<Type> Engines = new List<Type>()
        {
            null,
            null,
            typeof(Salsa20RandomByteGenerator),
            typeof(ChaCha20RandomByteGenerator),
        };

        public static IRandomByteGeneratorEngine GetGenerator(int id)
        {
            var type = Engines[id];
            return (IRandomByteGeneratorEngine)Activator.CreateInstance(type);
        }
    }
}
