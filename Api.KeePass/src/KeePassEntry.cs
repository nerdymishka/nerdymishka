using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.Api.KeePass
{
    [SuppressMessage(
        "Microsoft.Usage",
        "CA2227: Collection properties should be read only",
        Justification = "Setters are internal protected.")]
    [SuppressMessage(
        "Microsoft.Design",
        "CA1056:URI properties should not be strings",
        Justification = "Uri may not be a correctly formated URI")]
    public class KeePassEntry : IKeePassEntry,
        IEquatable<KeePassEntry>,
        IEquatable<IKeePassEntry>
    {
        private List<string> tags;

        private MoveableList<IKeePassEntry> history;

        private MemoryProtectedTextDictionary strings;

        private MemoryProtectedBytesDictionary binaries;

        private CustomDataDictionary customData;

        public string ForegroundColor { get; set; }

        public string BackgroundColor { get; set; }

        public string OverrideUrl { get; set; }

        public IList<string> Tags
        {
            get
            {
                if (this.tags == null)
                    this.tags = new List<string>();

                return this.tags;
            }

            internal protected set
            {
                if (value is null)
                    this.tags = null;
                else
                    this.tags = new List<string>(value);
            }
        }

        public MemoryProtectedTextDictionary Strings
        {
            get
            {
                if (this.strings == null)
                    this.strings = new MemoryProtectedTextDictionary();

                return this.strings;
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

        public KeePassAutoType AutoType { get; set; }

        public MoveableList<IKeePassEntry> History
        {
            get
            {
                if (this.history == null)
                    this.history = new MoveableList<IKeePassEntry>();

                return this.history;
            }

            internal protected set
            {
                if (value is null)
                    this.history = null;
                else
                    this.history = new MoveableList<IKeePassEntry>(value);
            }
        }

        IKeePassAutoType IKeePassEntry.AutoType
        {
            get => this.AutoType;
            set
            {
                if (value is null)
                {
                    this.AutoType = null;
                    return;
                }

                KeePassAssociation association = null;
                if (value.Association != null)
                {
                    var assoc = value.Association;
                    association = new KeePassAssociation()
                    {
                        KeystrokeSequence = assoc.KeystrokeSequence,
                        Window = assoc.Window,
                    };
                }

                this.AutoType = new KeePassAutoType()
                {
                    DataTransferObfuscation = value.DataTransferObfuscation,
                    Enabled = value.Enabled,
                    Association = association,
                };
            }
        }

        MoveableList<IKeePassEntry> IKeePassEntry.History => this.History;

        public bool IsHistorical { get; set; }

        public MemoryProtectedBytesDictionary Binaries
        {
            get
            {
                if (this.binaries == null)
                    this.binaries = new MemoryProtectedBytesDictionary();

                return this.binaries;
            }
        }

        public KeePassIdentifier CustomIconUuid { get; set; }

        public KeePassIdentifier Id { get; set; }

        public int IconId { get; set; }

        public KeePassAuditFields AuditFields { get; protected internal set; }

        IKeePassAuditFields IKeePassNode.AuditFields => this.AuditFields;

        public virtual string Name
        {
            get
            {
                if (this.strings != null)
                    return this.strings.ReadAsString(KeePassEntryFieldNames.Name);

                return default;
            }

            set
            {
                this.Strings.SetValue(KeePassEntryFieldNames.Name, value.AsSpan());
            }
        }

        public IKeePassEntry CopyTo(IKeePassEntry destination, bool cleanHistory = false)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            int hash = 0;

            if (this.strings != null)
                hash += 1 * 31 * this.strings.GetHashCode();

            if (this.binaries != null)
                hash += 2 * 31 * this.binaries.GetHashCode();

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (obj is KeePassEntry other)
                return this.Equals(other);

            return false;
        }

        public bool Equals(KeePassEntry other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            // TODO: implement equals for KeePassEntry
            return false;
        }

        public bool Equals(IKeePassEntry other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (other is KeePassEntry entry)
                return this.Equals(entry);

            return false;
        }
    }
}