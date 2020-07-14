using System.Diagnostics.CodeAnalysis;

namespace NerdyMishka.Api.KeePass.Package
{
    [SuppressMessage("Microsoft.Naming",
        "CA1710:Identifiers should have correct suffix",
        Justification = "It's a special dictionary")]
    public class KdfParameters : VariantDictionaryBase<KdfParameters>
    {
        private const string Identifier = "$UUID";

        public KdfParameters()
        {
        }

        public KdfParameters(KeePassIdentifier id)
        {
            this.Id = id;
            this.Add(Identifier, id.ToArray());
        }

        public KeePassIdentifier Id { get; private set; }

        public override void Deserialize(byte[] data)
        {
            base.Deserialize(data);
            if (!this.TryGetValue(Identifier, out byte[] value) || value.Length != 16)
            {
                this.Id = KeePassIdentifier.Empty;
                return;
            }

            this.Id = value;
        }
    }
}
