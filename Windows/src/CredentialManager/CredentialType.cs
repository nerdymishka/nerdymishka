using System;
using System.Diagnostics.CodeAnalysis;

namespace NerdyMishka.Windows.CredentialManager
{
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1028: Enum storage should be Int32", Justification = "Required")]
    public enum CredentialType : uint
    {
        /// <summary>Generic.</summary>
        Generic = 1,

        /// <summary>Domain Password.</summary>
        DomainPassword = 2,

        /// <summary>Domain Certificate.</summary>
        DomainCertificate = 3,

        /// <summary>Domain Visible Password.</summary>
        DomainVisiblePassword = 4,

        /// <summary>Generic Certificate.</summary>
        GenericCertificate = 5,

        /// <summary>Domain Extended.</summary>
        DomainExtended = 6,

        /// <summary>Maximum.</summary>
        Maximum = 7,

        /// <summary>Maximum Extended.</summary>
        MaximumEx = Maximum + 1000,
    }
}