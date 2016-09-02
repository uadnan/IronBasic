using System;
using System.Numerics;

namespace IronBasic.Types
{
    /// <summary>
    /// Helper class to parse floating point value
    /// </summary>
    internal class MbfFloatParser
    {
        public MbfFloatParser(bool isDouble)
        {
            IsDouble = isDouble;

            if (isDouble)
            {
                MbfDigitCount = MbfDouble.DigitCount;
                MbfMantissaBits = MbfDouble.MantissaBits;
                MbfByteSize = MbfDouble.ByteSize;
                MbfBias = MbfDouble.Bias;
                MbfCarryMask = MbfDouble.CarryMask;
            }
            else
            {
                MbfDigitCount = MbfSingle.DigitCount;
                MbfMantissaBits = MbfSingle.MantissaBits;
                MbfByteSize = MbfSingle.ByteSize;
                MbfBias = MbfSingle.Bias;
                MbfCarryMask = MbfSingle.CarryMask;
            }
        }

        public MbfFloatParser(MbfFloat fp)
            : this(fp is MbfDouble)
        {
            if (fp == null)
                throw new ArgumentNullException(nameof(fp));

            Mantissa = fp.Mantissa;
            Exponent = fp.Exponent;
            IsNegitive = fp.IsNegitive;
        }

        public byte MbfDigitCount { get; }

        public byte MbfMantissaBits { get; }

        public byte MbfByteSize { get; }

        public byte MbfBias { get; }

        public ulong MbfCarryMask { get; }

        public bool IsNegitive { get; set; }

        public BigInteger Mantissa { get; set; }

        public byte Exponent { get; set; }

        public bool IsZero => Exponent == 0;

        public bool IsDouble { get; }

        /// <summary>
        /// Apply the carry byte.
        /// </summary>
        private void ApplyCarry()
        {
            // carry bit set? then round up
            if ((Mantissa & 0xff) > 0x7f)
                Mantissa += 0x100;

            // overflow?
            if (Mantissa >= (ulong)Math.Pow(0x100, MbfByteSize))
            {
                Exponent += 1;
                Mantissa >>= 1;
            }

            // discard carry
            Mantissa ^= Mantissa & 0xff;
        }

        private long TruncateInt64()
        {
            var mantisa = Mantissa >> 8;
            long val;
            if (Exponent > MbfBias)
                val = (long)(mantisa << (Exponent - MbfBias));
            else
                val = (long)(mantisa >> (-Exponent + MbfBias));

            if (IsNegitive)
                return -val;

            return val;
        }

        /// <summary>
        /// Bring float to normal form.
        /// </summary>
        public void Normalize()
        {
            // zero mantissa -> make zero
            if (Mantissa == 0 || Exponent == 0)
            {
                var zero = IsDouble ? (MbfFloat)MbfDouble.Zero : MbfSingle.Zero;
                IsNegitive = zero.IsNegitive;
                Mantissa = zero.Mantissa;
                Exponent = zero.Exponent;

                return;
            }

            // are these correct?
            var expectedValue = BigInteger.Pow(2, MbfMantissaBits + 8 - 1);
            while (Mantissa <= expectedValue) // 0x7fffffffffffffff: # < 2**63
            {
                // Undeflow
                if (Exponent > 0)
                    Exponent -= 1;

                Mantissa <<= 1;
            }

            expectedValue = BigInteger.Pow(2, MbfMantissaBits + 8);
            while (Mantissa > expectedValue) // 0xffffffffffffffff: # 2**64 or 0x100**8
            {
                // Overflow
                if (Exponent == 0xff)
                {
                    var max = IsDouble ? (MbfFloat)MbfDouble.Max : MbfSingle.Max;
                    IsNegitive = max.IsNegitive;
                    Mantissa = max.Mantissa;
                    Exponent = max.Exponent;

                    throw new OverflowException();
                }

                Exponent += 1;
                Mantissa >>= 1;
            }
        }

        public MbfFloat ToMbfFloat()
        {
            if (IsDouble)
                return new MbfDouble(IsNegitive, Mantissa, Exponent);

            return new MbfSingle(IsNegitive, Mantissa, Exponent);
        }

        public void Add(MbfFloat right, bool normalize = true)
        {
            if (right.IsZero)
                return;

            if (IsZero)
            {
                Mantissa = right.Mantissa;
                Exponent = right.Exponent;
                IsNegitive = right.IsNegitive;
                return;
            }

            // ensure right has largest exponent
            if (Exponent > right.Exponent)
            {
                var temp = ToMbfFloat();
                Mantissa = right.Mantissa;
                Exponent = right.Exponent;
                IsNegitive = right.IsNegitive;

                right = temp;
            }

            // denormalise left to match exponents
            while (Exponent < right.Exponent)
            {
                Exponent += 1;
                Mantissa >>= 1;
            }

            // add mantissas, taking sign into account
            if (IsNegitive == right.IsNegitive)
                Mantissa += right.Mantissa;
            else
            {
                if (Mantissa > right.Mantissa)
                    Mantissa -= right.Mantissa;
                else
                {
                    Mantissa = right.Mantissa - Mantissa;
                    IsNegitive = right.IsNegitive;
                }
            }

            if (normalize)
                Normalize();
        }

        public void Divide(MbfFloat right)
        {
            if (right.IsZero)
            {
                var max = IsDouble ? (MbfFloat)MbfDouble.Max : MbfSingle.Max;
                Exponent = max.Exponent;
                Mantissa = max.Exponent;
                
                throw new DivideByZeroException();
            }

            if (IsZero)
                return;

            // signs
            IsNegitive = IsNegitive != right.IsNegitive;

            // subtract exponentials
            Exponent -= (byte)(right.Exponent - right.MbfBias - 8);

            // long division of mantissas
            var leftMantissa = Mantissa;
            var rightMantissa = right.Mantissa;

            Mantissa = 0;
            Exponent += 1;
            while (rightMantissa > 0)
            {
                Mantissa <<= 1;
                Exponent -= 1;

                if (leftMantissa > rightMantissa)
                {
                    leftMantissa -= rightMantissa;
                    Mantissa += 1;
                }

                rightMantissa >>= 1;
            }

            Normalize();
        }

        public void Multiply10()
        {
            if (IsZero)
                return;

            var copy = IsDouble
                ? (MbfFloat)new MbfDouble(IsNegitive, Mantissa, (byte)(Exponent + 2))
                : new MbfSingle(IsNegitive, Mantissa, (byte)(Exponent + 2));

            Add(copy, false);
            Exponent += 1;
            Normalize();
        }

        public void Divide10()
        {
            Divide(IsDouble ? (MbfFloat)MbfDouble.Ten : MbfSingle.Ten);
        }


        private bool AbsGreaterThenMax(MbfFloat maxValue)
        {
            if (Exponent != maxValue.Exponent)
                return Exponent > maxValue.Exponent;

            return Mantissa > maxValue.Mantissa;
        }

        private bool AbsLessThenMin(MbfFloat minValue)
        {
            if (Exponent != minValue.Exponent)
                return Exponent < minValue.Exponent;

            return Mantissa < minValue.Mantissa;
        }

        /// <summary>
        /// Return exponentiation needed to bring float into range.
        /// </summary>
        public long BringToRange(out int exponent10)
        {
            var min = IsDouble ? (MbfFloat) MbfDouble.MinValue : MbfSingle.MinValue;
            var max = IsDouble ? (MbfFloat) MbfDouble.MaxValue : MbfSingle.MaxValue;

            exponent10 = 0;
            while (AbsGreaterThenMax(max))
            {
                Divide10();
                exponent10 += 1;
            }

            ApplyCarry();

            while (AbsLessThenMin(min))
            {
                Multiply10();
                exponent10 -= 1;
            }

            // round off carry byte before doing the decimal rounding
            // this brings results closer in line with GW-BASIC output

            ApplyCarry();

            // round to integer: first add one half
            Add(IsDouble ? (MbfFloat)MbfDouble.Half : MbfSingle.Half);

            // then truncate to int (this throws away carry)
            var number = TruncateInt64();
            if (IsNegitive)
                number += 1;

            return number;
        }
    }
}