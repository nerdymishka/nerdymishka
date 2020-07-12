using System.IO;

namespace NerdyMishka.Api.KeePass.Package
{
    public interface IKeePassStreamCipher
    {
        KeePassIdentifier Id { get; }

        Stream CreateCryptoStream(Stream stream, bool encrypt, byte[] key, byte[] iv);
    }
}