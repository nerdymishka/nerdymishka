using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace NerdyMishka.Util.Arrays
{
    public static class ArrayExtensions
    {
        public static bool SlowEquals(this ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
        {
            uint diff = (uint)left.Length ^ (uint)left.Length;
            for (int i = 0; i < left.Length; i++)
            {
                diff |= (uint)(left[i] ^ right[i]);
            }

            return diff == 0;
        }

        public static bool SlowEquals(this Span<byte> left, Span<byte> right)
        {
            uint diff = (uint)left.Length ^ (uint)left.Length;
            for (int i = 0; i < left.Length; i++)
            {
                diff |= (uint)(left[i] ^ right[i]);
            }

            return diff == 0;
        }

        public static T[] CopyTo<T>(this T[] array, IMemoryOwner<T> owner)
        {
            Check.NotNull(nameof(array), array);
            Check.NotNull(nameof(owner), owner);

            array.CopyTo(owner.Memory.Span);
            return array;
        }

        /// <summary>
        /// Clears the values of the array.
        /// </summary>
        /// <param name="array">The array to perform the clear operation against.</param>
        /// <param name="index">The start index. Defaults to 0.</param>
        /// <param name="length">The number of items to clear.</param>
        /// <typeparam name="T">The item type in the array.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear<T>(this T[] array, int index = 0, int? length = null)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (length == null)
                length = array.Length;

            System.Array.Clear(array, index, length.Value);
        }

        /// <summary>
        /// Creates new array of a given size and copies the items
        /// of the current array into the new array.
        /// </summary>
        /// <param name="array">The array to be resized.</param>
        /// <param name="length">The size of the new array.</param>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <returns>A new array with the given <paramref name="length" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Resize<T>(this T[] array, int length)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (length < 1)
                throw new ArgumentOutOfRangeException(nameof(length));

            var next = new T[length];
            Array.Copy(array, next, Math.Min(array.Length, next.Length));

            return next;
        }

        /// <summary>
        /// Copies the current array into a larger array.
        /// </summary>
        /// <param name="array">The array to copy.</param>
        /// <param name="growthSize">The number of items more than the current length.</param>
        /// <typeparam name="T">The type of items in the array.</typeparam>
        /// <returns>A resized array.</returns>
        public static T[] Grow<T>(this T[] array, int growthSize = 1)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));

            return Resize(array, array.Length + growthSize);
        }

        /// <summary>
        /// Grow the array by increments of the given block size.
        /// </summary>
        /// <param name="array">The array to grow.</param>
        /// <param name="blockSize">The increment size that the length must be a multiple.</param>
        /// <typeparam name="T">The array type.</typeparam>
        /// <returns>A larger copy of the given array.</returns>
        public static T[] GrowBy<T>(this T[] array, int blockSize = 16)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));

            return GrowBy(array, array.Length, blockSize);
        }

        /// <summary>
        /// Grow the array by increments of the given block size.
        /// </summary>
        /// <param name="array">The array to grow.</param>
        /// <param name="length">The minimum length that number of blocks must meet or exceed.</param>
        /// <param name="blockSize">The increment size that the length must be a multiple.</param>
        /// <typeparam name="T">The type of items in the array.</typeparam>
        /// <returns>A larger copy of the given array.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GrowBy<T>(this T[] array, int length, int blockSize = 16)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (blockSize < 1)
                throw new ArgumentOutOfRangeException(nameof(blockSize));

            int blocks = array.Length / blockSize;
            int size = blocks * blockSize;
            if (size <= length)
            {
                while (size < length)
                {
                    blocks++;
                    size = blocks * blockSize;
                }
            }

            return Resize(array, blocks * blockSize);
        }

        /// <summary>
        /// Creates a smaller copy of the given array.
        /// </summary>
        /// <param name="array">The array to copy.</param>
        /// <param name="shrinkRate">The number of items less than the current length.</param>
        /// <typeparam name="T">The type of items in the array.</typeparam>
        /// <returns>The smaller array.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Shrink<T>(this T[] array, int shrinkRate = 1)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));

            return Resize(array, array.Length - shrinkRate);
        }

        /// <summary>
        /// Shrinks the array by increments of the given block size.
        /// </summary>
        /// <param name="array">The array to grow.</param>
        /// <param name="blockSize">The increment size that the length must be a multiple.</param>
        /// <typeparam name="T">The type of items in the array.</typeparam>
        /// <returns>A larger copy of the given array.</returns>
        public static T[] ShrinkBy<T>(this T[] array, int blockSize = 16)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));

            return ShrinkBy(array, array.Length, blockSize);
        }

        /// <summary>
        /// Shrinks the array by increments of the given block size.
        /// </summary>
        /// <param name="array">The array to grow.</param>
        /// <param name="length">The maxmimum length that number of blocks must meet or be less than.</param>
        /// <param name="blockSize">The increment size that the length must be a multiple.</param>
        /// <typeparam name="T">The array type.</typeparam>
        /// <returns>A larger copy of the given array.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ShrinkBy<T>(this T[] array, int length, int blockSize = 16)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (blockSize < 1)
                throw new ArgumentOutOfRangeException(nameof(blockSize));

            int blocks = array.Length / blockSize;
            int size = blocks * blockSize;
            if (size >= length)
            {
                while (size > length)
                {
                    blocks--;
                    size = blocks * blockSize;
                }
            }

            return Resize(array, blocks * blockSize);
        }
    }
}