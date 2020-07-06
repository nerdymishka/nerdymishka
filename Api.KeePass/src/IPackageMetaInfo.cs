using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Api.KeePass
{
    public interface IPackageMetaInfo
    {
        string Generator { get; set; }

        string DatabaseName { get; set; }

        DateTime DatabaseNameChanged { get; set; }

        string DatabaseDescription { get; set; }

        DateTime DatabaseDescriptionChanged { get; set; }

        string DefaultUserName { get; set; }

        DateTime DefaultUserNameChanged { get; set; }

        int MaintenanceHistoryDays { get; set; }

        string Color { get; set; }

        DateTime MasterKeyChanged { get; set; }

        int MasterKeyChangeRec { get; set; }

        int MasterKeyChangeForce { get; set; }

        MemoryProtectionOptions MemoryProtection { get; set; }

        bool RecycleBinEnabled { get; set; }

        KeePassIdentifier RecycleBinId { get; set; }

        DateTime RecycleBinChanged { get; set; }

        KeePassIdentifier EntryTemplatesGroupId { get; set; }

        DateTime EntryTemplatesGroupChanged { get; set; }

        int HistoryMaxItems { get; set; }

        int HistoryMaxSize { get; set; }

        KeePassIdentifier LastSelectedGroupId { get; set; }

        KeePassIdentifier LastTopVisibleGroupId { get; set; }

        IList<BinaryInfo> Binaries { get; }

        IList<KeePassIcon> CustomIcons { get; }

        SortedDictionary<string, string> CustomData { get; }
    }
}
