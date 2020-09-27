using System;
using System.Diagnostics.CodeAnalysis;

namespace NerdyMishka.Windows.CredentialManager
{
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1028: Enum storage should be Int32", Justification = "Required")]
    public enum CredentialFlag : uint
    {
        /// <summary>None.</summary>
        None = 0x0,

        /// <summary>Prompt Now.</summary>
        PromptNow = 0x2,

        /// <summary>Username Target.</summary>
        UsernameTarget = 0x4,
    }
}