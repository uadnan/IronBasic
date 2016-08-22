using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace IronBasic.Runtime.Types
{
    /// <summary>
    /// Floating-point number in Microsoft Binary Format.
    /// <see cref="http://www.experts-exchange.com/Programming/Languages/Pascal/Delphi/Q_20245266.html"/>
    /// <see cref="http://www.boyet.com/Articles/MBFSinglePrecision.html"/>
    /// </summary>
    public abstract class MbfFloat : ICloneable, IMbfFloat
    {
        public const byte TrueBias = 128;

        protected MbfFloat(byte mbfDigitsCount, byte mbfMantissaBits, byte mbfByteSize, ulong mbfCarryMask,
            bool isNegitive = false, BigInteger mantisa = default(BigInteger), byte exponent = 0)
        {
            MbfDigitCount = mbfDigitsCount;
            MbfMantissaBits = mbfMantissaBits;
            MbfByteSize = mbfByteSize;
            MbfBias = (byte) (TrueBias + mbfMantissaBits);
            MbfCarryMask = mbfCarryMask;

            IsNegitive = isNegitive;
            Mantissa = mantisa;
            Exponent = exponent;
        }

        public byte MbfDigitCount { get; }

        public byte MbfMantissaBits { get; }

        public byte MbfByteSize { get; }

        public byte MbfBias { get; }

        public ulong MbfCarryMask { get; }

        public bool IsNegitive { get; private set; }

        public BigInteger Mantissa { get; private set; }

        public byte Exponent { get; private set; }

        public bool IsZero => Exponent == 0;

        public abstract object Clone();

        protected abstract char TypeSign { get; }

        protected abstract char ExponentSign { get; }

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
                bytes.Add((byte) (mantisa & 0xff));
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
            if (Mantissa >= BigInteger.Pow(0x100, MbfByteSize))
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
            if (Exponent < MbfBias)
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

        protected abstract MbfFloat GetZero();

        protected abstract MbfFloat GetMax();

        public abstract MbfSingle ToSingle();

        /// <summary>
        /// Bring float to normal form.
        /// </summary>
        public void Normalize()
        {
            // zero mantissa -> make zero
            if (Mantissa == 0 || Exponent == 0)
            {
                var zero = GetZero();
                IsNegitive = zero.IsNegitive;
                Mantissa = zero.Mantissa;
                Exponent = zero.Exponent;

                return;
            }

            // are these correct?
            while (Mantissa <= (ulong)Math.Pow(2, MbfMantissaBits + 8 - 1)) // 0x7fffffffffffffff: # < 2**63
            {
                // Undeflow
                if (Exponent > 0)
                    Exponent -= 1;

                Mantissa <<= 1;
            }

            while (Mantissa > (ulong)Math.Pow(2, MbfMantissaBits + 8)) // 0xffffffffffffffff: # 2**64 or 0x100**8
            {
                // Overflow
                if (Exponent == 0xff)
                {
                    var max = GetMax();
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

        public override bool Equals(object obj)
        {
            var mbf = obj as MbfFloat;
            if (mbf == null)
                return false;

            return Equals(mbf);
        }

        protected bool Equals(MbfFloat mbf)
        {
            if (IsZero)
                return mbf.IsZero;

            return mbf.IsNegitive == IsNegitive &&
                   mbf.Exponent == Exponent &&
                   mbf.Mantissa == Mantissa;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MbfDigitCount.GetHashCode();
                hashCode = (hashCode*397) ^ MbfMantissaBits.GetHashCode();
                hashCode = (hashCode*397) ^ MbfByteSize.GetHashCode();
                hashCode = (hashCode*397) ^ MbfBias.GetHashCode();
                hashCode = (hashCode*397) ^ MbfCarryMask.GetHashCode();
                // ReSharper disable NonReadonlyMemberInGetHashCode
                hashCode = (hashCode*397) ^ IsNegitive.GetHashCode();
                hashCode = (hashCode*397) ^ Mantissa.GetHashCode();
                hashCode = (hashCode*397) ^ Exponent.GetHashCode();
                // ReSharper restore NonReadonlyMemberInGetHashCode
                return hashCode;
            }
        }

        public static MbfFloat operator +(MbfFloat left, MbfFloat right)
        {
            if (left.IsZero)
                return (MbfFloat) right.Clone();

            if (right.IsZero)
                return (MbfFloat) left.Clone();

            // ensure right has largest exponent
            if (left.Exponent > right.Exponent)
            {
                var temp = right;
                right = left;
                left = temp;
            }

            left = (MbfFloat) left.Clone();

            // denormalise left to match exponents
            while (left.Exponent < right.Exponent)
            {
                left.Exponent += 1;
                left.Mantissa >>= 1;
            }

            // add mantissas, taking sign into account
            if (left.IsNegitive == right.IsNegitive)
                left.Mantissa += right.Mantissa;
            else
            {
                if (left.Mantissa > right.Mantissa)
                    left.Mantissa -= right.Mantissa;
                else
                {
                    left.Mantissa = right.Mantissa - left.Mantissa;
                    left.IsNegitive = right.IsNegitive;
                }
            }

            left.Normalize();
            return left;
        }

        public static MbfFloat operator -(MbfFloat left, MbfFloat right)
        {
            right = (MbfFloat) right.Clone();
            right.IsNegitive = !right.IsNegitive;

            return left + right;
        }

        public static MbfFloat operator *(MbfFloat left, MbfFloat right)
        {
            if (left.IsZero)
                return left;

            if (right.IsZero)
                return right;

            var self = (MbfFloat) left.Clone();
            self.Exponent += (byte) (right.Exponent - right.MbfBias - 8);
            self.IsNegitive = self.IsNegitive != right.IsNegitive;
            self.Mantissa = self.Mantissa*right.Mantissa;

            self.Normalize();
            return self;
        }

        public static MbfFloat operator /(MbfFloat left, MbfFloat right)
        {
            if (right.IsZero)
                throw new DivideByZeroException();

            if (left.IsZero)
                return (MbfFloat)left.Clone();

            // signs
            left = (MbfFloat)left.Clone();
            left.IsNegitive = left.IsNegitive != right.IsNegitive;

            // subtract exponentials
            left.Exponent -= (byte)(right.Exponent - right.MbfBias - 8);

            // long division of mantissas
            var leftMantissa = left.Mantissa;
            var rightMantissa = right.Mantissa;

            left.Mantissa = 0;
            left.Exponent += 1;
            while (rightMantissa > 0)
            {
                left.Mantissa <<= 1;
                left.Exponent -= 1;

                if (leftMantissa > rightMantissa)
                {
                    leftMantissa -= rightMantissa;
                    left.Mantissa += 1;
                }

                rightMantissa >>= 1;
            }

            left.Normalize();
            return left;
        }

        public MbfFloat Round()
        {
            var self = (MbfFloat) Clone();
            if (Exponent - MbfBias > 0)
                self.Mantissa = self.Mantissa * (ulong)Math.Pow(2, self.Exponent - MbfBias);
            else
                self.Mantissa = self.Mantissa * (ulong)Math.Pow(2, -self.Exponent + MbfBias);

            self.Exponent = MbfBias;
            // carry bit set? then round up (moves exponent on overflow)
            self.ApplyCarry();
            self.Normalize();
            return self;
        }

        public MbfFloat Square()
        {
            return this * this;
        }

        public override string ToString()
        {
            return ToString(false, false);
        }

        public static string GetDigits(long number, int digits, bool removeTrailing=true)
        {
            var power10 = (long)Math.Pow(10L, digits - 1);
            var builder = new StringBuilder();

            while (power10 >= 1)
            {
                int digit = '0';
                while (number >= power10)
                {
                    digit += 1;
                    number -= power10;
                }

                builder.Append((char) digit);
                power10 /= 10;
            }

            if (removeTrailing)
            {
                // remove trailing zeros
                while (builder.Length > 1 && builder[builder.Length - 1] == '0')
                {
                    builder.Remove(builder.Length - 1, 1);
                }
            }

            return builder.ToString();
        }

        private static string ScientificNotation(string digits, int exponent10, char exponentSign = 'E',
            int digitsToDot = 1, bool forceDot = false)
        {
            var builder = new StringBuilder();
            if (digits.Length > digitsToDot)
            {
                builder.Append(digits.Substring(0, digitsToDot));
                builder.Append('.');
                builder.Append(digits.Substring(digitsToDot));
            }
            else if (digits.Length == digitsToDot && forceDot)
            {
                builder.Append(digits);
                builder.Append('.');
            }
            else
                builder.Append(digits);

            var exponent = exponent10 - digitsToDot + 1;
            builder.Append(exponentSign);
            builder.Append(exponent < 0 ? '-' : '+');

            builder.Append(GetDigits(Math.Abs(exponent), 2, false));
            return builder.ToString();
        }

        private static string DecimalNotation(string digits, int exponent10, string typeSign ="!", bool forceDot = false)
        {
            var builder = new StringBuilder();
            exponent10 += 1;

            if (exponent10 >= digits.Length)
            {
                builder.Append(digits);
                builder.Append(new string('0', exponent10 - digits.Length));
                if (forceDot)
                    builder.Append('.');

                if (!forceDot || typeSign == "#")
                    builder.Append(typeSign);
            }

            else if (exponent10 > 0)
            {
                builder.Append(digits.Substring(0, exponent10));
                builder.Append('.');
                builder.Append(digits.Substring(exponent10));

                if (typeSign == "#")
                    builder.Append(typeSign);
            }
            else
            {
                builder.Append('.');
                builder.Append(new string('0', -exponent10));
                builder.Append(digits);

                if (typeSign == "#")
                    builder.Append(typeSign);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Convert MbfFloat to .NET string.
        /// </summary>
        /// <param name="screen">screen=True (ie PRINT) - leading space, no type sign</param>
        /// <param name="write"></param>
        /// <returns></returns>
        public string ToString(bool screen, bool write)
        {
            // zero exponent byte means zero
            if (IsZero)
            {
                string value;
                if (screen && !write)
                    value = " 0";
                else if (write)
                    value = "0";
                else
                    value = "0" + TypeSign;

                return value;
            }

            var builder = new StringBuilder();

            // print sign
            if (IsNegitive)
                builder.Append("-");
            else if (screen && !write)
                    builder.Append(' ');

            var mbf = new MbfFloatBuilder(this);
            int exponent10;

            var number = mbf.BringToRange(out exponent10);
            var digits = GetDigits(number, MbfDigitCount);

            // exponent for scientific notation
            exponent10 += mbf.MbfDigitCount - 1;
            if (exponent10 > mbf.MbfDigitCount - 1 || digits.Length - exponent10 > mbf.MbfDigitCount + 1)
            {
                // use scientific notation
                builder.Append(ScientificNotation(digits, exponent10, ExponentSign));
            }
            else
            {
                var typeSign = string.Empty;
                // use decimal notation
                if (!screen && !write)
                    typeSign = TypeSign.ToString();

                builder.Append(DecimalNotation(digits, exponent10, typeSign));
            }

            return builder.ToString();
        }
    }
}