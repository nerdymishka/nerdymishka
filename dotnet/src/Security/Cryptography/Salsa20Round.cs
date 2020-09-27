namespace NerdyMishka.Security.Cryptography
{
    /// <summary>
    /// The number of rounds that Salsa can perform.
    /// </summary>
    public enum Salsa20Round
    {
        /// <summary>8 rounds.</summary>
        Eight = 8,

        /// <summary>10 rounds.</summary>
        Ten = 10,

        /// <summary>12 rounds.</summary>
        Twelve = 12,

        /// <summary>20 rounds.</summary>
        Twenty = 20,
    }
}