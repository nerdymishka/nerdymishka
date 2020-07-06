using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.Api.KeePass.Package
{
    [SuppressMessage(
        "Microsoft.Naming",
        "CA1710:Identifiers should have correct suffix",
        Justification = "By Design.")]
    public class KeePassCompositeKey : CompositeKey
    {
        public override ReadOnlySpan<byte> AssembleKey()
        {
            using (var ms = new MemoryStream())
            {
                foreach (var fragment in this.Fragments)
                {
                    ms.Write(fragment.ToReadOnlySpan());
                }

                ReadOnlySpan<byte> bytes = ms.ToArray();
                return bytes.ToSHA256Hash();
            }
        }
    }
}