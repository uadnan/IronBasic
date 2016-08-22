using System;
using System.Collections.Generic;
using System.Numerics;

namespace IronBasic.Runtime.Types
{
    public class MbfFloatBuilder : IMbfFloat
    {
        public MbfFloatBuilder(bool isDouble)
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

        public MbfFloatBuilder(MbfFloat fp)
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
        /// Convert float to byte representation.
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            ApplyCarry();

            // extract bytes
            var bytes = new List<byte>();
            var mantisa = Mantissa;
            for (var i = 1; i < MbfByteSize; i++)
            {
                mantisa >>= 8;
                bytes.Add((byte)(mantisa & 0xff));
            }

            // append exponent byte
            bytes.Add(Exponent);

            // Apply sign
            bytes[bytes.Count - 2] &= 0x7f;

            if (IsNegitive)
                bytes[bytes.Count - 2] |= 0x80;

            return bytes.ToArray();
        }


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

        /// <summary>
        /// Discard the carry byte.
        /// </summary>
        protected void DiscardCarry()
        {
            Mantissa = Mantissa & 0xff;
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

        private long RoundInt64()
        {
            long mantisa;
            if (Exponent > MbfBias)
                mantisa = (long)(Mantissa << (Exponent - MbfBias));
            else
                mantisa = (long)(Mantissa >> (-Exponent + MbfBias));

            // carry bit set? then round up (affect mantissa only, note we can be bigger
            // than our byte_size allows)

            if ((mantisa & 0xff) > 0x7f)
                mantisa += 0x100;

            if (IsNegitive)
                return -(mantisa >> 8);

            return mantisa >> 8;
        }

        /// <summary>
        /// Convert to integer.
        /// </summary>
        /// <returns></returns>
        public long ToInt64(bool round = false)
        {
            if (round)
                return RoundInt64();

            return TruncateInt64();
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

        public void Negate()
        {
            IsNegitive = !IsNegitive;
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

            var bI = (BigInteger)Mantissa;

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

        public void Subtract(MbfFloat right)
        {
            right = (MbfFloat)right.Clone();
            right.Negate();
            Add(right);
        }

        public void Multiply(MbfFloat right)
        {            
            if (IsZero)
                return;

            if (right.IsZero)
            {
                IsNegitive = right.IsNegitive;
                Mantissa = right.Mantissa;
                Exponent = right.Exponent;
            }

            Exponent += (byte)(right.Exponent - right.MbfBias - 8);
            IsNegitive = IsNegitive != right.IsNegitive;
            Mantissa = Mantissa * right.Mantissa;

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


        private static bool AbsGreaterThen(IMbfFloat left, IMbfFloat right)
        {
            if (left.Exponent != right.Exponent)
                return left.Exponent > right.Exponent;

            return left.Mantissa > right.Mantissa;
        }

        /// <summary>
        /// Return exponentiation needed to bring float into range.
        /// </summary>
        public long BringToRange(out int exponent10)
        {
            var min = IsDouble ? (MbfFloat) MbfDouble.MinValue : MbfSingle.MinValue;
            var max = IsDouble ? (MbfFloat) MbfDouble.MaxValue : MbfSingle.MaxValue;

            exponent10 = 0;
            while (AbsGreaterThen(this, max))
            {
                Divide10();
                exponent10 += 1;
            }

            ApplyCarry();

            while (AbsGreaterThen(min, this))
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