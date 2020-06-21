using System;
using System.Security.Cryptography;
using NerdyMishka.Util.Binary;

namespace NerdyMishka.Security.Cryptography
{
    internal class Salsa20Transform : ICryptoTransform
    {
        // https://dotnetfiddle.net/Bh4ijW
        private static readonly uint[] Sigma = new uint[] { 0x61707865, 0x3320646E, 0x79622D32, 0x6B206574 };
        private static readonly uint[] Tau = new uint[] { 0x61707865, 0x3120646E, 0x79622D36, 0x6B206574 };
        private uint[] state;
        private uint[] stateBuffer = new uint[16];
        private int rounds = 10;
        private bool isDisposed = false;
        private bool skipXor = false;
        private int bytesRemaining = 0;
        private byte[] bitSet = new byte[64];

        public Salsa20Transform(byte[] key, byte[] iv, int rounds, bool skipXor)
        {
            this.state = CreateState(key, iv);
            this.rounds = rounds;
            this.skipXor = skipXor;
        }

        ~Salsa20Transform()
        {
            this.Dispose(false);
        }

        public bool CanReuseTransform
        {
            get
            {
                return false;
            }
        }

        public bool CanTransformMultipleBlocks
        {
            get
            {
                return true;
            }
        }

        public int InputBlockSize
        {
            get
            {
                return 64;
            }
        }

        public int OutputBlockSize
        {
            get
            {
                return 64;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            this.CheckDisposed();

            int bytesTransformed = 0;
            int internalOffset = 0;
            if (this.bytesRemaining > 0)
                internalOffset = 64 - this.bytesRemaining;

            while (inputCount > 0)
            {
                if (this.bytesRemaining == 0)
                {
                    AddRotateXor(this.state, this.stateBuffer, this.bitSet, this.rounds);
                    this.bytesRemaining = 64;
                    internalOffset = 0;
                }

                var length = Math.Min(this.bytesRemaining, inputCount);

                if (this.skipXor)
                {
                    Array.Copy(this.bitSet, internalOffset, outputBuffer, outputOffset, length);
                }
                else
                {
                    for (int i = 0; i < length; i++)
                        outputBuffer[outputOffset + i] = (byte)(inputBuffer[inputOffset + i] ^ this.bitSet[i]);
                }

                this.bytesRemaining -= length;
                bytesTransformed += length;
                inputCount -= length;
                outputOffset += length;
                inputOffset += length;
            }

            return bytesTransformed;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            this.CheckDisposed();

            byte[] output = new byte[inputCount];
            this.TransformBlock(inputBuffer, inputOffset, inputCount, output, 0);
            return output;
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                Array.Clear(this.stateBuffer, 0, this.stateBuffer.Length);
                Array.Clear(this.state, 0, this.state.Length);
                this.stateBuffer = null;
                this.state = null;
                this.isDisposed = true;
            }
        }

        private static void AddRotateXor(uint[] state, uint[] buffer, byte[] output, int rounds)
        {
            Array.Copy(state, buffer, 16);
            var v = buffer;

            for (var i = 0; i < rounds; i++)
            {
                v[4] ^= BitShift.RotateLeft32(v[0] + v[12], 7);
                v[8] ^= BitShift.RotateLeft32(v[4] + v[0], 9);
                v[12] ^= BitShift.RotateLeft32(v[8] + v[4], 13);
                v[0] ^= BitShift.RotateLeft32(v[12] + v[8], 18);
                v[9] ^= BitShift.RotateLeft32(v[5] + v[1], 7);
                v[13] ^= BitShift.RotateLeft32(v[9] + v[5], 9);
                v[1] ^= BitShift.RotateLeft32(v[13] + v[9], 13);
                v[5] ^= BitShift.RotateLeft32(v[1] + v[13], 18);
                v[14] ^= BitShift.RotateLeft32(v[10] + v[6], 7);
                v[2] ^= BitShift.RotateLeft32(v[14] + v[10], 9);
                v[6] ^= BitShift.RotateLeft32(v[2] + v[14], 13);
                v[10] ^= BitShift.RotateLeft32(v[6] + v[2], 18);
                v[3] ^= BitShift.RotateLeft32(v[15] + v[11], 7);
                v[7] ^= BitShift.RotateLeft32(v[3] + v[15], 9);
                v[11] ^= BitShift.RotateLeft32(v[7] + v[3], 13);
                v[15] ^= BitShift.RotateLeft32(v[11] + v[7], 18);
                v[1] ^= BitShift.RotateLeft32(v[0] + v[3], 7);
                v[2] ^= BitShift.RotateLeft32(v[1] + v[0], 9);
                v[3] ^= BitShift.RotateLeft32(v[2] + v[1], 13);
                v[0] ^= BitShift.RotateLeft32(v[3] + v[2], 18);
                v[6] ^= BitShift.RotateLeft32(v[5] + v[4], 7);
                v[7] ^= BitShift.RotateLeft32(v[6] + v[5], 9);
                v[4] ^= BitShift.RotateLeft32(v[7] + v[6], 13);
                v[5] ^= BitShift.RotateLeft32(v[4] + v[7], 18);
                v[11] ^= BitShift.RotateLeft32(v[10] + v[9], 7);
                v[8] ^= BitShift.RotateLeft32(v[11] + v[10], 9);
                v[9] ^= BitShift.RotateLeft32(v[8] + v[11], 13);
                v[10] ^= BitShift.RotateLeft32(v[9] + v[8], 18);
                v[12] ^= BitShift.RotateLeft32(v[15] + v[14], 7);
                v[13] ^= BitShift.RotateLeft32(v[12] + v[15], 9);
                v[14] ^= BitShift.RotateLeft32(v[13] + v[12], 13);
                v[15] ^= BitShift.RotateLeft32(v[14] + v[13], 18);
            }

            for (int i = 0; i < 16; ++i)
            {
                v[i] += state[i];
                output[i << 2] = (byte)v[i];
                output[(i << 2) + 1] = (byte)(v[i] >> 8);
                output[(i << 2) + 2] = (byte)(v[i] >> 16);
                output[(i << 2) + 3] = (byte)(v[i] >> 24);
            }

            state[8]++;
            if (state[8] == 0)
                state[9]++;
        }

        private static uint[] CreateState(byte[] key, byte[] iv)
        {
            int offset = key.Length - 16;
            uint[] expand = key.Length == 32 ? Sigma : Tau;
            uint[] state = new uint[16];

            // key
            state[1] = LittleEndianBitConverter.ToUInt32(key, 0);
            state[2] = LittleEndianBitConverter.ToUInt32(key, 4);
            state[3] = LittleEndianBitConverter.ToUInt32(key, 8);
            state[4] = LittleEndianBitConverter.ToUInt32(key, 12);

            // key offset
            state[11] = LittleEndianBitConverter.ToUInt32(key, offset + 0);
            state[12] = LittleEndianBitConverter.ToUInt32(key, offset + 4);
            state[13] = LittleEndianBitConverter.ToUInt32(key, offset + 8);
            state[14] = LittleEndianBitConverter.ToUInt32(key, offset + 12);

            // sigma / tau
            state[0] = expand[0];
            state[5] = expand[1];
            state[10] = expand[2];
            state[15] = expand[3];

            state[6] = LittleEndianBitConverter.ToUInt32(iv, 0);
            state[7] = LittleEndianBitConverter.ToUInt32(iv, 4);
            state[8] = 0;
            state[9] = 0;

            return state;
        }

        private void CheckDisposed()
        {
            if (this.isDisposed)
                throw new ObjectDisposedException("ICryptoTransform is already disposed");
        }
    }
}