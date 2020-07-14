using System.Diagnostics.CodeAnalysis;

namespace NerdyMishka.Api.KeePass.Package
{
    [SuppressMessage(
        "Microsoft.Design",
        "CA1028: Enum storage should be Int32",
        Justification = "By Design. Values are stored as a a byte")]
    public enum HeaderField : byte
    {
        /// <summary>End of Header field.</summary>
        EndOfHeader = 0,

        /// <summary>Comment field.</summary>
        Comment = 1,

        /// <summary> The UUID of the Cipher for the database.</summary>
        PackageCipherId = 2,

        /// <summary>The database compression type (normal or g zipped).</summary>
        PackageCompression = 3,

        /// <summary>
        /// The seed used to generate the Cipher key.
        /// </summary>
        PackageCipherKey = 4,

        /// <summary>
        /// The set used to generate bytes for the Cipher key.
        /// </summary>
        AesKdfPassword = 5,

        /// <summary>
        /// The number of interations for the Encryption engine to execute.
        /// </summary>
        AesKdfIterations = 6,

        /// <summary>
        /// The number of interations for the Encryption engine to execute.
        /// </summary>
        PackageCipherIV = 7,

        /// <summary>
        /// Random bytes header field.
        /// </summary>
        RandomBytesCryptoKey = 8,

        /// <summary>
        /// The header byte mark field.
        /// </summary>
        StreamStartByteMarker = 9,

        /// <summary>
        /// The random bytes cryptography type field.
        /// </summary>
        RandomBytesCryptoType = 10,

        /// <summary>Key Derivation Function Parameters.</summary>
        KdfParameters = 11,

        /// <summary>Package level custom data e.g. variant dictionary.</summary>
        PackageCustomData = 12,
    }
}
