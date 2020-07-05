namespace NerdyMishka.Security.Cryptography
{
    /// <summary>
    /// The type of action for the delegate <see cref="NerdyMishka.Security.Cryptography.DataProtectionAction"/>.
    /// </summary>
    public enum MemoryProtectionActionType
    {
        /// <summary>Encrypt data</summary>
        Encrypt,

        /// <summary>Decrypt data.</summary>
        Decrypt,
    }
}