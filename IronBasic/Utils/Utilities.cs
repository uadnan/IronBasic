using System;
using System.IO;
using System.Linq;
using System.Text;

namespace IronBasic.Utils
{
    internal static class Utilities
    {
        internal static string ToCharString(this int value)
        {
            return value < 0 ? string.Empty : ((char)value).ToString();
        }

        internal static Stream AsStream(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            var byteArray = str.ToCharArray().Select(x => (byte)x).ToArray();
            return new MemoryStream(byteArray);
        }

        public static int TryParseInt32(this string value, int fallbackValue = 0)
        {
            int intVal;
            return int.TryParse(value, out intVal) ? intVal : fallbackValue;
        }

        public static int TrimEnd(this StringBuilder builder, params int[] chars)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var trimmed = 0;
            while (builder.Length > 0 &&
                   chars.Contains(builder[builder.Length - 1]))
            {
                builder.Remove(builder.Length - 1, 1);
                trimmed++;
            }

            return trimmed;
        }
    }
}