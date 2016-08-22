using System;
using System.Linq;
using System.Numerics;
using IronBasic.Runtime.Types;

namespace IronBasic.Compilor
{
    public static class GwNumberConversion
    {
        public const int MinValue = -0x8000;
        public const int MaxSignedValue = 0x7fff;
        public const int MaxUnsignedValue = 0xffff;

        /// <summary>
        /// Convert System.Int32 in range [-32768, 65535] to BASIC Integer.
        /// </summary>
        /// <param name="value"></param>
        public static string ToBasicUnsignedInteger(this int value)
        {
            if (value > MaxUnsignedValue || value < MinValue)
                throw new OverflowException();

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

        public static string ToBasicSignedInteger(this int value)
        {
            if (value > MaxSignedValue || value < MinValue)
                throw new OverflowException();

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

        public static MbfFloat ToMbf(this string value, bool allowNonNumbers = true)
        {
            var foundSign = false;
            var foundPoint = false;
            var foundExpression = false;

            var foundExpSign = false;
            var expressionNegitive = false;
            var negitive = false;

            var exp10 = 0;
            var exponent = 0;
            var mantissa = 0UL;
            var digits = 0;
            var zeros = 0;

            var isDouble = false;
            var isSingle = false;

            foreach (var c in value)
            {
                // ignore whitespace throughout (x = 1   234  56  .5  means x=123456.5 in gw!)
                if (Constants.AsciiWhitepsace.Contains(c))
                    continue;

                // determine sign
                if (!foundSign)
                {
                    foundSign = true;
                    // number has started; if no sign encountered here, sign must be pos.
                    if (c == '+' || c == '-')
                    {
                        negitive = c == '-';
                        continue;
                    }
                }

                // parse numbers and decimal points, until 'E' or 'D' is found
                if (!foundExpression)
                {
                    if (Constants.AsciiDigits.Contains(c))
                    {
                        mantissa *= 10;
                        mantissa += (ulong)(c - '0');

                        if (foundPoint)
                            exp10 -= 1;

                        // keep track of precision digits
                        if (mantissa != 0)
                        {
                            digits += 1;
                            if (foundPoint && c == '0')
                                zeros += 1;
                            else
                                zeros = 0;
                        }

                        continue;
                    }

                    if (c == '.')
                    {
                        foundPoint = true;
                        continue;
                    }

                    if ("DE".Contains(char.ToUpper(c)))
                    {
                        foundExpression = true;
                        isDouble = c == 'D' || c == 'd';
                        continue;
                    }

                    if (c == '!')
                    {
                        // makes it a single, even if more than eight digits specified
                        isSingle = true;
                        break;
                    }

                    if (c == '#')
                    {
                        isDouble = true;
                        break;
                    }

                    if (allowNonNumbers)
                        break;

                    return null;
                }

                if (!foundExpSign)
                {
                    foundExpSign = true;
                    // exponent has started; if no sign given, it must be pos.
                    if (c == '-' || c == '+')
                    {
                        expressionNegitive = c == '-';
                        continue;
                    }
                }

                if (Constants.AsciiDigits.Contains(c))
                {
                    exponent *= 10;
                    exponent += c - '0';
                    continue;
                }

                if (allowNonNumbers)
                    break;

                return null;
            }

            if (expressionNegitive)
                exp10 -= exponent;
            else
                exp10 += exponent;

            // eight or more digits means double, unless single override
            if (digits - zeros > 7 && !isSingle)
                isDouble = true;

            mantissa *= 0x100;
            var builder = new MbfFloatBuilder(isDouble)
            {
                IsNegitive = negitive,
                Exponent = isDouble ? MbfDouble.Bias : MbfSingle.Bias,
                Mantissa = (BigInteger)mantissa
            };

            builder.Normalize();
            while (exp10 < 0)
            {
                builder.Divide10();
                exp10 += 1;
            }

            while (exp10 > 0)
            {
                builder.Multiply10();
                exp10 += 1;
            }

            builder.Normalize();
            return builder.ToMbfFloat();
        }

        public static int ToUnsignedInteger(this string value)
        {
            if (value?.Length != 2)
                throw new FormatException("Invalid Basic Integer format");

            return 0x100 * value[1] + value[0];
        }
    }
}