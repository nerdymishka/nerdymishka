using System;

namespace NerdyMishka.Windows.Registry
{
    /// <summary>
    /// Local Machine registry settings.
    /// </summary>
    public static partial class LocalMachine
    {
        public static void AddAutoRunEntry(
            string name,
            string value,
            bool overwrite = false)
        {
            const string path = "HKLM:/SOFTWARE/Microsoft/Windows/CurrentVersion/run";
            if (!overwrite)
            {
                var entry = RegistryUtil.GetValue(path, name);
                if (entry != null)
                    throw new ArgumentException($"An entry for {name} already exists");
            }

            RegistryUtil.SetValue(path, name, value);
        }

        public static void AddAutoRunPowerShellCommandEntry(
            string name,
            string command,
            string powershellArgs,
            bool overwrite)
        {
            powershellArgs = powershellArgs ?? "-ExecutionPolicy ByPass -NonInteractive";
            var bytes = System.Text.Encoding.BigEndianUnicode.GetBytes(command);
            command = Convert.ToBase64String(bytes);
            var winDir = Environment.GetEnvironmentVariable("windir");
            var powershellExe = $"{winDir}\\system32\\WindowsPowerShell\\v1.0\\powershell.exe";
            var invocation = $"{powershellExe} {powershellArgs} -encoded {command}";
            AddAutoRunEntry(name, invocation, overwrite);
        }

        public static void AddAutoRunPowerShellScriptEntry(
            string name,
            string scriptFile,
            string powershellArgs,
            bool overwrite)
        {
            powershellArgs = powershellArgs ?? "-ExecutionPolicy ByPass -NonInteractive";
            var winDir = Environment.GetEnvironmentVariable("windir");
            var powershellExe = $"{winDir}\\system32\\WindowsPowerShell\\v1.0\\powershell.exe";
            var invocation = $"{powershellExe} {powershellArgs} -File \"{scriptFile}\"";
            AddAutoRunEntry(name, invocation, overwrite);
        }

        public static void RemoveAutoRunEntry(string name)
        {
            const string path = "HKLM:/SOFTWARE/Microsoft/Windows/CurrentVersion/run";
            using (var subKey = RegistryUtil.OpenSubKey(path))
            {
                subKey.DeleteValue(name);
            }
        }
    }
}