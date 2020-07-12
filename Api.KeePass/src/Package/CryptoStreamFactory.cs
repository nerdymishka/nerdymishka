using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NerdyMishka.Api.KeePass.Package
{
    public class CryptoStreamFactory : IKeePassCryptoStreamFactory
    {
        private List<IKeePassCryptoStreamProvider> providers = new List<IKeePassCryptoStreamProvider>();

        public CryptoStreamFactory()
        {
            this.providers.Add(new AesCryptoStreamProvider());
        }

        public int Count => this.providers.Count;

        public void Add(IKeePassCryptoStreamProvider provider)
        {
            if (provider is null)
                throw new ArgumentNullException(nameof(provider));

            var existingProvider = this.Find(provider.Id);
            if (existingProvider != null)
                throw new ArgumentException("provider already added", nameof(provider));

            this.providers.Add(provider);
        }

        public Stream CreateCryptoStream(KeePassIdentifier identifier, Stream innerStream, bool encrypt, byte[] key, byte[] iv)
        {
            var provider = this.Find(identifier);
            if (provider is null)
                throw new ArgumentException("provider could not be found for identifier");

            return provider.CreateCryptoStream(innerStream, encrypt, key, iv);
        }

        public IKeePassCryptoStreamProvider Find(KeePassIdentifier id)
        {
            return this.providers.SingleOrDefault(o => o.Id == id);
        }
    }
}