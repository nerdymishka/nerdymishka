using System;
using System.Collections.Generic;

namespace NerdyMishka.Api.KeePass
{
    public class PackageInfo
    {
        public PackageInfo()
        {
            var now = DateTime.Now;
            this.RecycleBinId = KeePassIdentifier.Empty;
            this.EntryTemplatesGroupId = KeePassIdentifier.Empty;
            this.LastSelectedGroupId = KeePassIdentifier.Empty;
            this.LastTopVisibleGroupId = KeePassIdentifier.Empty;
            this.MaintenanceHistoryDays = 365;
            this.MasterKeyChangeForce = -1;
            this.MasterKeyChangeRec = -1;
            this.HistoryMaxItems = 10;
            this.HistoryMaxSize = 6291456;
            this.DatabaseNameChanged = now;
            this.DatabaseDescriptionChanged = now;
            this.DefaultUserNameChanged = now;
            this.EntryTemplatesGroupChanged = now;
            this.MemoryProtection = MemoryProtectionOptions.Create();
            this.Binaries = new List<BinaryInfo>();
            this.CustomIcons = new List<KeePassIcon>();
        }

        public string Generator { get; set; }

        public string DatabaseName { get; set; }

        public DateTime DatabaseNameChanged { get; set; }

        public string DatabaseDescription { get; set; }

        public DateTime DatabaseDescriptionChanged { get; set; }

        public string DefaultUserName { get; set; }

        public DateTime DefaultUserNameChanged { get; set; }

        public int MaintenanceHistoryDays { get; set; }

        public string Color { get; set; }

        public DateTime MasterKeyChanged { get; set; }

        public int MasterKeyChangeRec { get; set; }

        public int MasterKeyChangeForce { get; set; }

        public MemoryProtectionOptions MemoryProtection { get; set; }

        public bool RecycleBinEnabled { get; set; }

        public KeePassIdentifier RecycleBinId { get; set; }

        public DateTime RecycleBinChanged { get; set; }

        public KeePassIdentifier EntryTemplatesGroupId { get; set; }

        public DateTime EntryTemplatesGroupChanged { get; set; }

        public int HistoryMaxItems { get; set; }

        public int HistoryMaxSize { get; set; }

        public KeePassIdentifier LastSelectedGroupId { get; set; }

        public KeePassIdentifier LastTopVisibleGroupId { get; set; }

        public List<BinaryInfo> Binaries { get; }

        public List<KeePassIcon> CustomIcons { get; }

        public SortedDictionary<string, string> CustomData { get; }
    }
}
