using System;
using IronBasic.Runtime;

namespace IronBasic.Utils
{
    /// <summary>
    /// Contains helper method for compatibility between GW-BSIC Number Type and .NET Number Types
    /// </summary>
    internal static class NumberCompatibility
    {
        public const int MinValue = -0x8000;
        public const int MaxSignedValue = 0x7fff;
        public const int MaxUnsignedValue = 0xffff;

        /// <summary>
        /// Convert <see cref="int"/> to GW-BASIC unsigned integer
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>2 byte GW-BAISC unsigned integer</returns>
        public static string ToBasicUnsignedInteger(this int value)
        {
            if (value > MaxUnsignedValue || value < MinValue)
                throw new ReplRuntimeException(ReplExceptionCode.Overflow);

            if (value < 0)
                value = 0x10000 + value;

            return new string(
                new[]
                {
                    (char)(value & 0xff),
                    (char)(value >> 8)
                }
            );
        }

        /// <summary>
        /// Convert <see cref="int"/> to GW-BASIC signed integer
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>2 byte GW-BAISC signed integer</returns>
        public static string ToBasicSignedInteger(this int value)
        {
            if (value > MaxSignedValue || value < MinValue)
                throw new ReplRuntimeException(ReplExceptionCode.Overflow);

            if (value < 0)
                value = 0x10000 + value;

            return new string(
                new[]
                {
                    (char)(value & 0xff),
                    (char)(value >> 8)
                }
            );
        }

        public static int ToUnsignedInteger(this string value)
        {
            if (value?.Length != 2)
                throw new FormatException("Invalid Basic Integer format");

            return 0x100 * value[1] + value[0];
        }
    }
}