namespace NerdyMishka.Api.KeePass.Package
{
    public static class AesKdf
    {
        public const string IterationsParameterName = "R";
        public const string KeyParameterName = "S";

        public static readonly KeePassIdentifier Id =
            new KeePassIdentifier(new byte[]
            {
                0xC9, 0xD9, 0xF3, 0x9A, 0x62, 0x8A, 0x44, 0x60,
                0xBF, 0x74, 0x0D, 0x08, 0xC1, 0x8A, 0x4F, 0xEA,
            });
    }
}