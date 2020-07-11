using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace NerdyMishka.Util.Data
{
    public static class DbDataReaderExtensions
    {
        public static bool GetBoolean(this DbDataRecord dr, string columnName)
        {
            Check.NotNull(nameof(dr), dr);
            int ordinal = dr.GetOrdinal(columnName);
            return dr.GetBoolean(ordinal);
        }

        public static byte GetByte(this DbDataRecord dr, string columnName)
        {
            Check.NotNull(nameof(dr), dr);
            int ordinal = dr.GetOrdinal(columnName);
            return dr.GetByte(ordinal);
        }

        public static long GetBytes(
            this DbDataRecord dr,
            string columnName,
            long offset,
            byte[] buffer,
            int bufferIndex,
            int length)
        {
            Check.NotNull(nameof(dr), dr);

            int ordinal = dr.GetOrdinal(columnName);
            return dr.GetBytes(ordinal, offset, buffer, bufferIndex, length);
        }

        public static char GetChar(this DbDataRecord dr, string columnName)
        {
            Check.NotNull(nameof(dr), dr);
            int ordinal = dr.GetOrdinal(columnName);
            return dr.GetChar(ordinal);
        }

        public static long GetChars(
            this DbDataRecord dr,
            string columnName,
            long offset,
            char[] buffer,
            int bufferIndex,
            int length)
        {
            Check.NotNull(nameof(dr), dr);

            int ordinal = dr.GetOrdinal(columnName);
            return dr.GetChars(ordinal, offset, buffer, bufferIndex, length);
        }

        public static DateTime GetDateTime(this DbDataRecord dr, string columnName)
        {
            Check.NotNull(nameof(dr), dr);
            int ordinal = dr.GetOrdinal(columnName);
            return dr.GetDateTime(ordinal);
        }

        public static decimal GetDecimal(this DbDataRecord dr, string columnName)
        {
            Check.NotNull(nameof(dr), dr);
            int ordinal = dr.GetOrdinal(columnName);
            return dr.GetDecimal(ordinal);
        }

        public static double GetDouble(this DbDataRecord dr, string columnName)
        {
            Check.NotNull(nameof(dr), dr);
            int ordinal = dr.GetOrdinal(columnName);
            return dr.GetDouble(ordinal);
        }

        public static float GetFloat(this DbDataRecord dr, string columnName)
        {
            Check.NotNull(nameof(dr), dr);
            int ordinal = dr.GetOrdinal(columnName);
            return dr.GetFloat(ordinal);
        }

        public static Type GetFieldType(this DbDataRecord dr, string columnName)
        {
            Check.NotNull(nameof(dr), dr);
            int ordinal = dr.GetOrdinal(columnName);
            return dr.GetFieldType(ordinal);
        }

        public static Guid GetGuid(this DbDataRecord dr, string columnName)
        {
            Check.NotNull(nameof(dr), dr);
            int ordinal = dr.GetOrdinal(columnName);
            return dr.GetGuid(ordinal);
        }

        public static short GetInt16(this DbDataRecord dr, string columnName)
        {
            Check.NotNull(nameof(dr), dr);
            int ordinal = dr.GetOrdinal(columnName);
            return dr.GetInt16(ordinal);
        }

        public static int GetInt32(this DbDataRecord dr, string columnName)
        {
            Check.NotNull(nameof(dr), dr);
            int ordinal = dr.GetOrdinal(columnName);
            return dr.GetInt32(ordinal);
        }

        public static long GetInt64(this DbDataRecord dr, string columnName)
        {
            Check.NotNull(nameof(dr), dr);
            int ordinal = dr.GetOrdinal(columnName);
            return dr.GetInt64(ordinal);
        }

        public static string GetString(this DbDataRecord dr, string columnName)
        {
            Check.NotNull(nameof(dr), dr);
            int ordinal = dr.GetOrdinal(columnName);
            return dr.GetString(ordinal);
        }

        public static object GetValue(this DbDataRecord dr, string columnName)
        {
            Check.NotNull(nameof(dr), dr);

            int ordinal = dr.GetOrdinal(columnName);
            return dr.GetValue(ordinal);
        }

        public static object IsDBNull(this DbDataRecord dr, string columnName)
        {
            Check.NotNull(nameof(dr), dr);

            int ordinal = dr.GetOrdinal(columnName);
            return dr.IsDBNull(ordinal);
        }

        public static T GetFieldValue<T>(this DbDataReader dr, string columnName)
        {
            Check.NotNull(nameof(dr), dr);

            int ordinal = dr.GetOrdinal(columnName);
            return dr.GetFieldValue<T>(ordinal);
        }

        public static Task<T> GetFieldValueAsync<T>(
            this DbDataReader dr,
            string columnName,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(nameof(dr), dr);

            int ordinal = dr.GetOrdinal(columnName);
            return dr.GetFieldValueAsync<T>(ordinal, cancellationToken);
        }
    }
}