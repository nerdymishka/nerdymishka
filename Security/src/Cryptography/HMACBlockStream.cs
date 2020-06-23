using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using NerdyMishka.Text;
using NerdyMishka.Util.Arrays;
using NerdyMishka.Util.Collections;

namespace NerdyMishka.Security.Cryptography
{
    /// <summary>
    /// Hash-Based Message Authenticated Code Stream that adds authenticated codes
    /// using a given hash algorithm. This stream will write a block of bytes
    /// from the inner stream, the block size, and then write a computed
    /// hash with the given hash algorithm. The default encoding is UTF-8
    /// with no Byte Order Mark ("BOM").
    /// </summary>
    public class HMACBlockStream : System.IO.Stream
    {
        private Stream innerStream;

        private BinaryReader reader;

        private BinaryWriter writer;

        private bool endOfStream = false;

        private byte[] endOfStreamMarker = new byte[32];

        private HashAlgorithmName hashAlgorithm;

        private KeyedHashAlgorithmType keyedHashAlgorithm;

        private HashAlgorithm signer;

        private byte[] internalBuffer;

        private int expectedPosition = 0;

        private bool disposed;

        private int bufferOffset = 0;

        /// <summary>
        /// Initializes a new instance of <see cref="NerdyMishka.Security.Cryptography.HMACBlockStream"/>
        /// </summary>
        /// <param name="innerStream">The stream that will be read or written to.</param>
        /// <param name="write">If true, the stream will be written to; othewise, read from.</param>
        public HMACBlockStream(Stream innerStream, bool write = true)
            : this(innerStream, write, Utf8Options.NoBom, HashAlgorithmName.SHA256)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="NerdyMishka.Security.Cryptography.HMACBlockStream"/>
        /// </summary>
        /// <param name="innerStream">The stream that will be read or written to.</param>
        /// <param name="write">If true, the stream will be written to; othewise, read from.</param>
        /// <param name="encoding">The encoding that the stream should use.</param>
        /// <param name="keyedHashAlgorithm">The keyed hash algorithm used to create an authentication code.</param>
        /// <param name="key">The key for the keyed hash algorithm.</param>
        public HMACBlockStream(
            Stream innerStream,
            bool write,
            Encoding encoding,
            KeyedHashAlgorithmType keyedHashAlgorithm,
            byte[] key)
            : this(innerStream, write, encoding, HashAlgorithmName.SHA256)
        {
            this.keyedHashAlgorithm = keyedHashAlgorithm;
            var signer = KeyedHashAlgorithm.Create(keyedHashAlgorithm.ToString());
            signer.Key = key;
            this.signer = signer;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="NerdyMishka.Security.Cryptography.HMACBlockStream"/>
        /// </summary>
        /// <param name="innerStream">The stream that will be read or written to.</param>
        /// <param name="write">If true, the stream will be written to; othewise, read from.</param>
        /// <param name="encoding">The encoding that the stream should use.</param>
        /// <param name="hashAlgorithm">The hash algorithm used to create an authentication code.</param>
        public HMACBlockStream(Stream innerStream, bool write, Encoding encoding, HashAlgorithmName hashAlgorithm)
        {
            if (innerStream == null)
                throw new ArgumentNullException(nameof(innerStream));

            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            this.innerStream = innerStream;
            this.hashAlgorithm = hashAlgorithm;

            if (write)
                this.writer = new BinaryWriter(innerStream, encoding);
            else
                this.reader = new BinaryReader(innerStream, encoding);

            this.signer = HashAlgorithm.Create(this.hashAlgorithm.ToString());
        }

        /// <summary>
        /// Gets if consumers of this stream can read from it.
        /// </summary>
        public override bool CanRead => this.writer == null;

        /// <summary>
        /// Gets if the consumers of this stream can seek. Always False.
        /// </summary>
        public override bool CanSeek => false;

        /// <summary>
        /// Gets if consumers of this stream can write to it.
        /// </summary>
        public override bool CanWrite => this.writer != null;

        /// <summary>
        /// Gets
        /// </summary>
        public override long Length => this.innerStream.Length;

        /// <summary>
        /// Gets the current position of the stream.
        /// </summary>
        public override long Position
        {
            get
            {
                return this.innerStream.Position;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Flushes the write stream, if there is one.
        /// </summary>
        public override void Flush()
        {
            if (this.writer != null)
                this.writer.Flush();
        }

        /// <summary>
        /// Reads a give number of bytes (<paramref name="count"/>) starting at the given <paramref name="offset"/>.
        /// </summary>
        /// <param name="buffer">The buffer that filled with bytes.</param>
        /// <param name="offset">The offset from the position of the stream</param>
        /// <param name="count">The number of bytes to read to the buffer.</param>
        /// <returns>The number of bytes read.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this.reader == null)
                throw new InvalidOperationException("HMACStream cannot read");

            int progress = 0;

            while (progress < count)
            {
                if (this.internalBuffer == null)
                {
                    this.bufferOffset = 0;
                    this.internalBuffer = this.ReadNext();
                    if (this.internalBuffer == null)
                        return progress;
                }

                int l = Math.Min(this.internalBuffer.Length - this.bufferOffset, count);

                Array.Copy(this.internalBuffer, this.bufferOffset, buffer, offset, l);
                offset += l;
                this.bufferOffset += l;
                progress += l;

                if (this.bufferOffset == this.internalBuffer.Length)
                    this.internalBuffer = null;
            }

            return progress;
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="offset"> Not supported.</param>
        /// <param name="origin"> Not supported.</param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="value"> Not supported.</param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Writes the number of bytes from the buffer at the specified offset.
        /// </summary>
        /// <param name="buffer">That data to write to the stream.</param>
        /// <param name="offset">The offset from the position of the inner stream.</param>
        /// <param name="count">The number of bytes that should be written.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (this.writer == null)
                throw new InvalidOperationException($"HMACStream cannot write.");

            this.writer.Write(this.expectedPosition);
            this.expectedPosition++;

            var length = count - offset;
            var bytes = new byte[length];
            Array.Copy(buffer, bytes, length);

            var hashBytes = this.signer.ComputeHash(bytes);

            this.writer.Write(hashBytes);
            this.writer.Write(length);
            this.writer.Write(bytes);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">Determines if the object is disposed manually or gced.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:Do not catch general exception types", Justification = "By Design")]
        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
                return;

            this.disposed = true;
            if (disposing)
            {
                if (this.innerStream == null)
                    return;

                if (this.reader != null)
                {
                    try
                    {
                        this.reader.Dispose();
                    }
                    catch
                    {
                        // meh. coreclr throws an exception
                    }
                }

                if (this.writer != null)
                {
                    this.WriteEndOfStream();
                    this.Flush();
                    this.writer.Dispose();
                }

                this.innerStream?.Dispose();
                this.signer?.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Writes the end of the stream.
        /// </summary>
        protected virtual void WriteEndOfStream()
        {
            this.writer.Write(this.expectedPosition);
            this.writer.Write(new byte[32]);
            this.writer.Write(0);
        }

        private byte[] ReadNext()
        {
            if (this.endOfStream)
                return null;

            int actualPosition = this.reader.ReadInt32();
            if (this.expectedPosition != actualPosition)
                throw new Exception($"The stream's actual position {actualPosition} does not match the expected position {this.expectedPosition} ");

            this.expectedPosition++;
            byte[] expectedHash = this.reader.ReadBytes(this.signer.HashSize);
            int bufferSize = this.reader.ReadInt32();

            if (bufferSize == 0)
            {
                if (!this.endOfStreamMarker.EqualTo(expectedHash))
                    throw new Exception("invalid end-of-stream marker");

                this.endOfStream = true;
                return null;
            }

            byte[] decryptedBytes = this.reader.ReadBytes(bufferSize);
            byte[] actualHash = this.signer.ComputeHash(decryptedBytes);

            if (!expectedHash.EqualTo(actualHash))
                throw new Exception("The file is corrupted or has been tampered with.");

            expectedHash.Clear();
            actualHash.Clear();

            return decryptedBytes;
        }
    }
}