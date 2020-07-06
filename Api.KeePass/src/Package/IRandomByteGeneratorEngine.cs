namespace NerdyMishka.Api.KeePass.Package
{
    public interface IRandomByteGeneratorEngine
    {
        int Id { get; }

        void Initialize(byte[] key);

        byte[] NextBytes(int count);
    }
}