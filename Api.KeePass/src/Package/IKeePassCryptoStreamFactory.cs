using System.IO;

namespace NerdyMishka.Api.KeePass.Package
{
    public interface IKeePassCryptoStreamFactory
    {
        int Count { get; }

        void Add(IKeePassCryptoStreamProvider provider);

        IKeePassCryptoStreamProvider Find(KeePassIdentifier id);

        Stream CreateCryptoStream(
            KeePassIdentifier identifier,
            Stream innerStream,
            bool encrypt,
            byte[] key,
            byte[] iv);
    }
}