using System;
using System.Diagnostics.CodeAnalysis;

namespace NerdyMishka.Windows.CredentialManager
{
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1028: Enum storage should be Int32", Justification = "Required")]
    public enum Persistence : uint
    {
        /// <summary>
        /// Session.
        /// </summary>
        Session = 1,

        /// <summary>
        /// Local Machine.
        /// </summary>
        LocalMachine = 2,

        /// <summary>
        /// Enterprise.
        /// </summary>
        Enterprise = 3,
    }
}