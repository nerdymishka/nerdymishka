using System;
using System.Linq;

namespace NerdyMishka.Api.KeePass
{
    public class KeePassGroup : IKeePassGroup, IEquatable<KeePassGroup>
    {
        private MoveableList<IKeePassEntry> entries;

        private MoveableList<IKeePassGroup> groups;

        private CustomDataDictionary customData;

        private IKeePassAuditFields auditFields = new KeePassAuditFields();

        private IKeePassPackage package;

        private IKeePassGroup parent;

        public bool IsExpanded { get; set; }

        public string DefaultAutoTypeSequence { get; set; }

        public bool? EnableAutoType { get; set; }

        public bool? EnableSearching { get; set; }

        public KeePassIdentifier LastTopVisibleEntryId { get; set; }

        public IKeePassGroup Parent
        {
            get => this.parent;
            protected internal set
            {
                this.parent = value;
            }
        }

        public IKeePassPackage Package
        {
            get => this.package;
            protected internal set
            {
                this.package = value;
                this.Groups.Package = value;
                this.Entries.Package = value;
            }
        }

        IKeePassPackage IKeePassChild.Package
        {
            get => this.Package;
            set => this.Package = value;
        }

        IKeePassGroup IKeePassChild.Parent
        {
            get => this.Parent;
            set => this.Parent = value;
        }

        public MoveableList<IKeePassEntry> Entries
        {
            get
            {
                if (this.entries == null)
                    this.entries = new MoveableList<IKeePassEntry>(this.Package, this);

                return this.entries;
            }
        }

        public MoveableList<IKeePassGroup> Groups
        {
            get
            {
                if (this.groups == null)
                    this.groups = new MoveableList<IKeePassGroup>(this.Package, this);

                return this.groups;
            }
        }

        public CustomDataDictionary CustomData
        {
            get
            {
                if (this.customData == null)
                    this.customData = new CustomDataDictionary();

                return this.customData;
            }
        }

        public KeePassIdentifier Id { get; set; }

        public KeePassIdentifier CustomIconUuid { get; set; }

        public int IconId { get; set; }

        public IKeePassAuditFields AuditFields => this.auditFields;

        public string Name { get; set; }

        public string Notes { get; set; }

        public IKeePassGroup CopyTo(IKeePassGroup destinationGroup)
        {
            throw new NotImplementedException();
        }

        public IKeePassEntry Entry(int index)
        {
            if (index < 0 || index > this.Entries.Count - 1)
                return default;

            return this.Entries[index];
        }

        public IKeePassEntry Entry(
            string name,
            StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (this.Entries.Count == 0)
                return default;

            return this.Entries.FirstOrDefault(
                o => o.Name != null &&
                string.Equals(o.Name, name, comparison));
        }

        public override bool Equals(object obj)
        {
            if (obj is KeePassGroup other)
                return this.Equals(other);

            return false;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            if (this.Name != null)
                hash += 17 + this.Name.GetHashCode();

            if (this.entries != null)
                hash += this.entries.GetHashCode();

            if (this.groups != null)
                hash += this.groups.GetHashCode();

            hash += this.Id.GetHashCode();

            return hash;
        }

        public bool Equals(KeePassGroup other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            // TODO: implement equality for KeePassGroup
            return false;
        }

        public void ExportTo(IKeePassGroup destination)
        {
            throw new NotImplementedException();
        }

        public IKeePassGroup Group(int index)
        {
            if (index < 0 || index > this.Groups.Count - 1)
                return default;

            return this.Groups[index];
        }

        public IKeePassGroup Group(
            string name,
            StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (this.Groups.Count == 0)
                return default;

            return this.Groups.FirstOrDefault(
                    o => o.Name != null &&
                    string.Equals(o.Name, name, comparison));
        }

        public void MergeTo(IKeePassGroup destination, bool overwrite = false, bool ignoreGroups = false)
        {
            throw new NotImplementedException();
        }
    }
}