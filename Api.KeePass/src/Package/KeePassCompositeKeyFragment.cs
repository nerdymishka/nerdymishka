using System;
using NerdyMishka.Security;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.Api.KeePass.Package
{
    public class KeePassCompositeKeyFragment : ICompositeKeyFragment
    {
        private MemoryProtectedBytes bytes;
        private bool isDisposed = false;

        ~KeePassCompositeKeyFragment()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public ReadOnlySpan<byte> ToReadOnlySpan()
        {
            return this.bytes.ToArray();
        }

        protected void SetData(ReadOnlySpan<byte> data)
        {
            this.bytes = new MemoryProtectedBytes(data);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
                return;

            if (disposing)
            {
                if (this.bytes != null)
                    this.bytes.Dispose();
            }

            this.isDisposed = true;
        }
    }
}