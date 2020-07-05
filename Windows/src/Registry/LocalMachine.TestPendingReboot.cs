using System;
using System.Linq;
using System.Management;

namespace NerdyMishka.Windows.Registry
{
    public static partial class LocalMachine
    {
        [Flags]
        public enum RebootTypes : int
        {
            /// <summary>Auto Update.</summary>
            AutoUpdate = 1,

            /// <summary>Component Service.</summary>
            ComponentService = 2,

            /// <summary>Computer Name.</summary>
            ComputerName = 4,

            /// <summary>Domain Join.</summary>
            DomainJoin = 8,

            /// <summary>File Pending.</summary>
            FilePending = 16,

            /// <summary>System Center.</summary>
            SystemCenter = 32,

            /// <summary>All.</summary>
            All = AutoUpdate | ComputerName | ComponentService | DomainJoin | FilePending | SystemCenter,
        }

        public static bool TestForPendingReboot(RebootTypes type = RebootTypes.All)
        {
            bool reboot = false;
            bool isAll = type == RebootTypes.All;
            if (isAll || type.HasFlag(RebootTypes.AutoUpdate))
                reboot = TestAutoUpdatePendingReboot();

            if (!reboot && (isAll || type.HasFlag(RebootTypes.ComponentService)))
                reboot = TestServicingPendingReboot();

            if (!reboot && (isAll || type.HasFlag(RebootTypes.FilePending)))
                reboot = TestFilePendingReboot();

            if (!reboot && (isAll || type.HasFlag(RebootTypes.ComputerName)))
                reboot = TestComputerRenamePendingReboot();

            if (!reboot && (isAll || type.HasFlag(RebootTypes.DomainJoin)))
                reboot = TestDomainJoinPendingReboot();

            if (!reboot && (isAll || type.HasFlag(RebootTypes.SystemCenter)))
                reboot = TestSystemCenterPendingReboot();

            return reboot;
        }

        private static bool TestSystemCenterPendingReboot()
        {
            string computerName = Environment.GetEnvironmentVariable("COMPUTERNAME");
            string scope = $"\\\\{computerName}\\root\\CCM\\ClientSDK";
            string query = $"SELECT * FROM CCM_ClientUtilities WHERE Name=\"{computerName}\"";

            using (var searcher = new ManagementObjectSearcher(scope, query))
            {
                foreach (var obj in searcher.Get())
                {
                    var value = obj.GetPropertyValue("DetermineifRebootPending");
                    if (value != null && value is bool boolean)
                        return boolean;

                    value = obj.GetPropertyValue("IsHardRebootPending");
                    if (value != null && value is bool boolean1)
                        return boolean1;

                    value = obj.GetPropertyValue("RebootPending");
                    if (value != null && value is bool boolean2)
                        return boolean2;
                }
            }

            return false;
        }

        private static bool TestFilePendingReboot()
        {
            const string filePending = "HKLM:/SYSTEM/CurrentControlSet/Control/Session Manager/PendingFileRenameOperations";
            return RegistryUtil.GetValueAsInt32(filePending, -1) == 1;
        }

        private static bool TestDomainJoinPendingReboot()
        {
            const string path = "HKLM:/SYSTEM/CurrentControlSet/Services/Netlogon";
            using (var subKey = RegistryUtil.OpenSubKey(path))
            {
                if (subKey == null)
                    return false;

                return subKey
                        .GetValueNames()
                        .Any(o => string.Equals(o, "JoinDomain", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(o, "AvoidSpnSet", StringComparison.OrdinalIgnoreCase));
            }
        }

        private static bool TestComputerRenamePendingReboot()
        {
            const string path = "HKLM:/SYSTEM/CurrentControlSet/Control/ComputerName/ComputerName";
            const string activePath = "HKLM:/SYSTEM/CurrentControlSet/Control/ComputerName/ActiveComputerName";
            string computerName = RegistryUtil.GetValueAsString(path);
            string activeComputerName = RegistryUtil.GetValueAsString(activePath);

            if (string.IsNullOrWhiteSpace(activeComputerName))
                return false;

            return !string.Equals(computerName, activeComputerName, StringComparison.OrdinalIgnoreCase);
        }

        private static bool TestAutoUpdatePendingReboot()
        {
            const string path = "HKLM:/SOFTWARE/Microsoft/Windows/CurrentVersion/WindowsUpdate/Auto Update";
            using (var subKey = RegistryUtil.OpenSubKey(path))
            {
                if (subKey == null)
                    return false;

                return subKey.GetValueNames()
                                 .Any(o => string.Equals(o, "RebootRequired", StringComparison.OrdinalIgnoreCase));
            }
        }

        private static bool TestServicingPendingReboot()
        {
            const string servicing = "HKLM:/SOFTWARE/Microsoft/Windows/CurrentVersion/Component Based Servicing";
            using (var subKey = RegistryUtil.OpenSubKey(servicing))
            {
                if (subKey == null)
                    return false;

                return subKey.GetValueNames().Any(o =>
                               string.Equals(o, "RebootPending", StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}