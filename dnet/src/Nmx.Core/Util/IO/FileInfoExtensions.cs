using System;
using System.Globalization;
using System.IO;

namespace NerdyMishka.Util.IO
{
    public static class FileInfoExtensions
    {
        public static string FormatFileSize(
            this long length,
            ByteMeasurement format = ByteMeasurement.Auto)
        {
            string[] sizes = new string[] { "B", "KB", "MB", "GB", "TB", "PB" };
            double l = length;
            int order = 0;
            const int interval = 1024;
            while (l >= interval && order < (sizes.Length - 1))
            {
                l /= interval;
                order++;
                if (format != ByteMeasurement.Auto && order == (int)format)
                {
                    break;
                }
            }

            return string.Format(CultureInfo.InvariantCulture, "{0:0.##} {1}", l, sizes[order]);
        }

        public static string FormatFileSize(
            this FileInfo info,
            ByteMeasurement format = ByteMeasurement.Auto)
        {
            Check.ArgNotNull(info, nameof(info));

            return FormatFileSize(info.Length, format);
        }
    }
}