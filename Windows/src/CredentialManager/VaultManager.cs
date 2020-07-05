using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace NerdyMishka.Windows.CredentialManager
{
    [CLSCompliant(false)]
    [SuppressMessage("", "SA1615:", Justification = "Return types are documented")]
    [SuppressMessage("", "SA1629:", Justification = "Documentation has periods")]
    public static class VaultManager
    {
        private static readonly bool PlatformSupported = Environment.OSVersion.Platform.HasFlag(PlatformID.Win32NT);

        /// <summary>
        /// Creates a new credential to save to the Windows Credential Store.
        /// </summary>
        /// <returns cref="VaultCredential">A vault credential with common default values set.</return>
        [CLSCompliant(false)]
        public static VaultCredential Create()
        {
            return new VaultCredential()
            {
                LastWritten = DateTime.UtcNow,
                Type = CredentialType.Generic,
                Flags = CredentialFlag.None,
                Persistence = Persistence.LocalMachine,
                Attributes = IntPtr.Zero,
            };
        }

        /// <summary>
        /// Writes a credential to the Windows Credential Store.
        /// </summary>
        /// <param name="credential">The credential to write to the store.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <c>credential</c> is null.</exception>
        [CLSCompliant(false)]
        public static void Write(VaultCredential credential)
        {
            Guard();
            if (credential == null)
                throw new ArgumentNullException(nameof(credential));

            var cred = credential;
            var length = (uint)cred.Data.Length;
            cred.LastWritten = DateTime.UtcNow;

            IntPtr data = Marshal.AllocHGlobal((int)length);
            if (length > 0)
            {
                var bytes = cred.GetBlob();
                Marshal.Copy(bytes, 0, data, (int)length);
            }

            var native = new NativeCredential()
            {
                AttributeCount = (uint)cred.AttributeCount,
                Comment = Marshal.StringToCoTaskMemUni(cred.Comment),
                CredentialBlob = data,
                CredentialBlobSize = length,
                Attributes = IntPtr.Zero,
                Flags = (uint)cred.Flags,
                Persist = (uint)cred.Persistence,
                TargetAlias = Marshal.StringToCoTaskMemUni(cred.Alias),
                TargetName = Marshal.StringToCoTaskMemUni(cred.Key),
                Type = (uint)cred.Type,
                UserName = Marshal.StringToCoTaskMemUni(cred.UserName),
            };

            var isSet = WriteCredential(ref native, 0);
            int errorCode = Marshal.GetLastWin32Error();
            if (isSet)
                return;

            throw new ExternalException($"Advapi32.dll -> CredWriteW failed to write credential. error code {errorCode}");
        }

        /// <summary>
        /// Reads a credential from the Windows Credential Store.
        /// </summary>
        /// <param name="key">The unique key of the entry to read.</param>
        /// <param name="type">The type of credential that should be deleted. Defaults to <c>Generic<c>.</param>
        /// <returns>A credential.</returns>
        public static VaultCredential Read(
            string key,
            CredentialType type = CredentialType.Generic)
        {
            Guard();
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            bool success = ReadCredential(key, type, 0, out IntPtr nativeCredentialPointer);
            int errorCode = Marshal.GetLastWin32Error();
            if (success)
            {
                using (var handle = new CredentialHandle(nativeCredentialPointer))
                {
                    return handle.AllocateCredential();
                }
            }

            string message;
            switch (errorCode)
            {
                case 1168:
                    // credential not found
                    return null;
                default:
                    message = $"Advapi32.dll -> CredWriteW failed to read credential. error code {errorCode}.";
                    throw new ExternalException(message);
            }
        }

        /// <summary>
        /// Deletes a credential from the Windows Credential Store .
        /// </summary>
        /// <param name="key">The unique key of the entry to delete .</param>
        /// <param name="type">The type of credential that should be deleted. Defaults to <c>Generic<c> .</param>
        /// <exception cref="System.ArgumentNullException">Thrown when key is empty or null .</exception>
        /// <exception cref="System.NotSupportedException">Thrown when called on a non Windows system .</exception>
        public static void Delete(string key, CredentialType type = CredentialType.Generic)
        {
            VaultManager.Guard();

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            bool success = DeleteCredential(key, type, 0);
            int errorCode = Marshal.GetLastWin32Error();

            if (success)
                return;

            throw new ExternalException($"Advapi32.dll -> CredDeleteW failed to delete credential. error code {errorCode}");
        }

        /// <summary>
        /// Returns a list of all the credentials of the user calling this action.
        /// </summary>
        /// <returns>A list of <see cref="VaultCredential[]" />.</returns>
        [CLSCompliant(false)]
        public static VaultCredential[] List()
        {
            Guard();
            int flags;

            if (Environment.OSVersion.Version.Major >= 6)
            {
                flags = 0x1;
            }
            else
            {
                string message = "Retrieving all credentials is only possible on Windows version Vista or later.";
                throw new Exception(message);
            }

            bool success = EnumerateCredentials(null, flags, out int count, out IntPtr nextCredentialsPointer);
            int errorCode = Marshal.GetLastWin32Error();

            if (success)
            {
                using (var handle = new CredentialHandle(nextCredentialsPointer))
                {
                    return handle.AllocateCredentials(count);
                }
            }

            throw new ExternalException($"Advapi32.dll -> CredEnumerateW failed to read credentials. error code {errorCode}");
        }

        private static void Guard()
        {
            if (!PlatformSupported)
                throw new NotSupportedException("The Windows Credentials Vault only exists on Windows systems.");
        }

        [DllImport("Advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool ReadCredential([In] string target, [In] CredentialType type, [In] int reservedFlag, out IntPtr credentialPtr);

        [DllImport("Advapi32.dll", EntryPoint = "CredWriteW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool WriteCredential([In] ref NativeCredential userCredential, [In] uint flags);

        [DllImport("Advapi32.dll", EntryPoint = "CredFree", SetLastError = true)]
        private static extern bool FreeCredential([In] IntPtr credentialPointer);

        [DllImport("Advapi32.dll", EntryPoint = "CredDeleteW", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool DeleteCredential([In] string target, [In] CredentialType type, [In] int reservedFlag);

        [DllImport("Advapi32.dll", EntryPoint = "CredEnumerateW", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool EnumerateCredentials([In] string filter, [In] int flags, out int count, out IntPtr credentialPtrs);

        internal class CredentialHandle : Microsoft.Win32.SafeHandles.CriticalHandleMinusOneIsInvalid
        {
            public CredentialHandle(IntPtr handle)
            {
                this.SetHandle(handle);
            }

            public VaultCredential AllocateCredential()
            {
                if (this.IsInvalid)
                    throw new InvalidOperationException($"{typeof(CriticalHandle).FullName} handle is invalid");

                return AllocateCredentialFromHandle(this.handle);
            }

            [SuppressMessage("", "CA1307: ", Justification = "NETSTANDARD2.0")]
            public VaultCredential[] AllocateCredentials(int count)
            {
                if (this.IsInvalid)
                    throw new InvalidOperationException("Invalid CriticalHandle!");

                var credentials = new VaultCredential[count];
                for (int i = 0; i < count; i++)
                {
                    IntPtr nextPointer = Marshal.ReadIntPtr(this.handle, i * IntPtr.Size);
                    var credential = AllocateCredentialFromHandle(nextPointer);

                    if (credential.Key.Contains(":target="))
                    {
                        var key = credential.Key;
                        key = key.Substring(key.IndexOf("=", StringComparison.OrdinalIgnoreCase) + 1);
                        credential.Key = key;
                    }

                    credentials[i] = credential;
                }

                return credentials;
            }

            protected override bool ReleaseHandle()
            {
                if (this.IsInvalid)
                    return false;

                FreeCredential(this.handle);
                this.SetHandleAsInvalid();
                return true;
            }

            private static VaultCredential AllocateCredentialFromHandle(IntPtr handle)
            {
#if NET45
                var native = new NativeCredential();
                Marshal.PtrToStructure(handle, native);
#else
                var native = Marshal.PtrToStructure<NativeCredential>(handle);
#endif
                var fileTime = (((long)native.LastWritten.dwHighDateTime) << 32) + native.LastWritten.dwLowDateTime;
                byte[] data = null;

                if (native.CredentialBlobSize > 0)
                {
                    data = new byte[native.CredentialBlobSize];
                    Marshal.Copy(native.CredentialBlob, data, 0, (int)native.CredentialBlobSize);
                }

                Memory<byte> ps = default;
                if (data != null)
                {
                    ps = new Memory<byte>(data);
                    Array.Clear(data, 0, data.Length);
                }

                return new VaultCredential()
                {
                    AttributeCount = (int)native.AttributeCount,
                    Attributes = native.Attributes,
                    Comment = Marshal.PtrToStringUni(native.Comment),
                    Flags = (CredentialFlag)native.Flags,
                    LastWritten = DateTime.FromFileTime(fileTime),
                    UserName = Marshal.PtrToStringUni(native.UserName),
                    Alias = Marshal.PtrToStringUni(native.TargetAlias),
                    Key = Marshal.PtrToStringUni(native.TargetName),
                    Length = (int)native.CredentialBlobSize,
                    Persistence = (Persistence)native.Persist,
                    Type = (CredentialType)native.Type,
                    Data = ps,
                };
            }
        }
    }
}