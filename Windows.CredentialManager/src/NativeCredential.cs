using System;
using System.Runtime.InteropServices;

namespace NerdyMishka.Windows.CredentialManager
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct NativeCredential
    {
        public uint Flags;

        public uint Type;

        public IntPtr TargetName;

        public IntPtr Comment;

        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;

        public uint CredentialBlobSize;

        public IntPtr CredentialBlob;

        public uint Persist;

        public uint AttributeCount;

        public IntPtr Attributes;

        public IntPtr TargetAlias;

        public IntPtr UserName;
    }
}