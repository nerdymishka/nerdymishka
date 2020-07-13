using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NerdyMishka.Api.KeePass;
using NerdyMishka.Api.KeePass.Package;

namespace NerdyMishka.Api.KeePass.Serialization
{
    public class SerializerContext
    {
        public SerializerContext()
        {
            this.BinaryMap = new MemoryProtectedBytesMap();
        }

        public IRandomByteGenerator RandomByteGenerator { get; set; }

        public MemoryProtectionOptions MemoryProtection { get; set; }

        public byte DatabaseCompression { get; set; }

        public MemoryProtectedBytesMap BinaryMap { get; }

        public IDictionary<Type, Type> Mappings { get; internal set; }
    }
}
