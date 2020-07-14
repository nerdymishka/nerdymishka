using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using NerdyMishka.Text;
using static NerdyMishka.Util.Binary.LittleEndianBitConverter;

namespace NerdyMishka.Api.KeePass.Package
{
    /// <summary>
    /// base variant dictionary variable data.
    /// </summary>
    /// <typeparam name="T">The dictionary type.</typeparam>
    /// <remarks>
    ///     <para>
    ///         The basic concepts of this variant dictionary come from
    ///         [kdbxweb](https://github.com/keeweb/kdbxweb/blob/master/lib/utils/var-dictionary.js).
    ///     </para>
    /// </remarks>
    [SuppressMessage("Microsoft.Naming",
        "CA1710:Identifiers should have correct suffix",
        Justification = "It's a dictionary")]
    public class VariantDictionaryBase<T> :
        IEnumerable<KeyValuePair<string, object>>,
        ICloneable<T>
        where T : VariantDictionaryBase<T>, new()
    {
        private const ushort Version = 0x0100;

        private const ushort Mask = 0xFF00;

        private readonly Dictionary<string, object> dictionary = new Dictionary<string, object>();

        [SuppressMessage("", "CA1720:", Justification = "By Design")]
        [SuppressMessage("", "CA1028:", Justification = "By Design")]
        public enum VariantType : byte
        {
            /// <summary>None.</summary>
            None = 0,

            /// <summary>unsigned int 32.</summary>
            UInt32 = 0x04,

            /// <summary>unsigned int 64.</summary>
            UInt64 = 0x05,

            /// <summary>boolean.</summary>
            Boolean = 0x08,

            /// <summary>int 32.</summary>
            Int32 = 0x0C,

            /// <summary>int 64.</summary>
            Int64 = 0x0D,

            /// <summary>UTF8 encoded string.</summary>
            String = 0x18,

            /// <summary>byte array.</summary>
            ByteArray = 0x42,
        }

        public int Count => this.dictionary.Count;

        public object this[string key]
        {
            get
            {
                if (this.dictionary.TryGetValue(key, out object value))
                    return value;

                return default;
            }

            set
            {
                this.Add(key, value);
            }
        }

        public bool IsTypeSupported(Type type)
        {
            if (type is null)
                return false;

            var name = type.Name;
            if (Enum.TryParse(name, out VariantType _) || name == "Byte[]")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a value to the dictionary.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>True, if the value is added; otherwise, false.</returns>
        public bool Add(string key, object value)
        {
            if (!string.IsNullOrWhiteSpace(key))
                return false;

            if (value is null)
                return false;

            if (this.IsTypeSupported(value.GetType()))
            {
                this.dictionary[key] = value;
                return true;
            }

            return false;
        }

        public T Clone()
        {
            var clone = new T();
            var map = clone.dictionary;
            foreach (var kv in this.dictionary)
            {
                if (kv.Value is byte[] byteArray)
                {
                    if (byteArray.Length == 0)
                    {
                        map[kv.Key] = Array.Empty<byte>();
                        continue;
                    }

                    var copy = new byte[byteArray.Length];
                    Array.Copy(byteArray, copy, byteArray.Length);
                    map[kv.Key] = copy;
                    continue;
                }

                map[kv.Key] = kv.Value;
            }

            return clone;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            => this.dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();

        public bool Remove(string key)
            => this.dictionary.Remove(key);

        public bool TryGetValue<TValue>(string key, out TValue value)
        {
            if (this.dictionary.TryGetValue(key, out object obj))
            {
                if (obj is TValue item)
                {
                    value = item;
                    return true;
                }

                if (obj is null)
                {
                    value = default;
                    return true;
                }

                var underlying = Nullable.GetUnderlyingType(typeof(T));
                if (underlying != null)
                {
                    obj = Convert.ChangeType(obj, underlying, CultureInfo.InvariantCulture);
                    value = (TValue)obj;
                    return true;
                }
            }

            value = default;
            return false;
        }

        public virtual byte[] Serialize()
        {
            using (var ms = new MemoryStream())
            {
                ms.Write(ToBytes(Version));

                foreach (var kv in this.dictionary)
                {
                    if (string.IsNullOrWhiteSpace(kv.Key))
                        continue;

                    var name = NerdyMishka.Text.Utf8Options.NoBom.GetBytes(kv.Key);
                    var value = kv.Value;
                    if (value is null)
                        continue;

                    VariantType type;
                    byte[] data;
                    switch (value)
                    {
                        case null:
                            continue;
                        case uint unsigned32:
                            type = VariantType.UInt32;
                            data = ToBytes(unsigned32);
                            break;
                        case ulong unsigned64:
                            type = VariantType.UInt64;
                            data = ToBytes(unsigned64);
                            break;
                        case bool bit:
                            type = VariantType.Boolean;
                            data = ToBytes(bit);
                            break;
                        case int signed32:
                            type = VariantType.Int32;
                            data = ToBytes(signed32);
                            break;
                        case long signed64:
                            type = VariantType.Int64;
                            data = ToBytes(signed64);
                            break;
                        case string text:
                            type = VariantType.String;
                            data = NerdyMishka.Text.Utf8Options.NoBom.GetBytes(text);
                            break;
                        case byte[] array:
                            type = VariantType.ByteArray;
                            data = array;
                            break;
                        default:
                            continue;
                    }

                    ms.WriteByte((byte)type);
                    ms.Write(ToBytes(name.Length));
                    ms.Write(name);
                    ms.Write(ToBytes(data.Length));
                    ms.Write(data);
                }

                ms.Write((byte)VariantType.None);

                return ms.ToArray();
            }
        }

        public virtual void Deserialize(byte[] data)
        {
            this.dictionary.Clear();

            using (var ms = new MemoryStream(data, false))
            {
                var version = ToUInt16(ms.ReadBytes(2));
                if ((version & Mask) > (Version & Mask))
                    throw new NotSupportedException($"Variant Dictionary ({version}) is not supported");

                while (ms.Position < ms.Length)
                {
                    byte variantTypeBit = (byte)ms.ReadByte();
                    if (variantTypeBit < 0)
                        throw new NotSupportedException($"Unknown Variant Dictionary Type ({variantTypeBit})");

                    if (variantTypeBit == 0)
                        break;

                    var variantType = (VariantType)variantTypeBit;
                    var keyLength = ToInt32(ms.ReadBytes(4));
                    var keyBytes = ms.ReadBytes(keyLength);
                    if (keyBytes.Length != keyLength)
                        throw new EndOfStreamException(
                            "Variant dictionary key length does not match:" +
                                $"expected ({keyLength}) actual ({keyBytes.Length})");

                    var key = NerdyMishka.Text.Utf8Options.NoBom.GetString(keyBytes);

                    var valueLength = ToInt32(ms.ReadBytes(4));
                    var valueBytes = ms.ReadBytes(valueLength);
                    if (valueLength != valueBytes.Length)
                        throw new EndOfStreamException(
                            "Variant dictionary value length does not match:" +
                                $"expected ({valueLength}) actual ({valueBytes.Length})");

                    switch (variantType)
                    {
                        case VariantType.UInt32:
                            if (valueLength == 4)
                                this[key] = ToUInt32(valueBytes);
                            break;
                        case VariantType.UInt64:
                            if (valueLength == 8)
                                this[key] = ToUInt64(valueBytes);
                            break;
                        case VariantType.Boolean:
                            if (valueLength == 1)
                                this[key] = ToBoolean(valueBytes);
                            break;
                        case VariantType.Int32:
                            if (valueLength == 4)
                                this[key] = ToInt64(valueBytes);
                            break;
                        case VariantType.Int64:
                            if (valueLength == 8)
                                this[key] = ToInt64(valueBytes);
                            break;

                        case VariantType.String:
                            this[key] = Utf8Options.NoBom.GetString(valueBytes);
                            break;

                        case VariantType.ByteArray:
                            this[key] = valueBytes;
                            break;

                        default:
                            continue;
                    }
                }
            }
        }
    }
}