using System.Diagnostics.CodeAnalysis;

namespace NerdyMishka.Api.KeePass.Package
{
    [SuppressMessage("Microsoft.Naming",
        "CA1710:Identifiers should have correct suffix",
        Justification = "It's a dictionary")]
    public class VariantDictionary : VariantDictionaryBase<VariantDictionary>
    {
        public VariantDictionary()
        {
        }
    }
}