using System;
using System.Numerics;

namespace IronBasic.Types
{
    /// <summary>
    /// Represents Double precision floating poin number in
    /// Microsoft Binary Format
    /// </summary>
    public sealed class MbfDouble : MbfFloat
    {
        public const byte DigitCount = 16;
        public const byte MantissaBits = 56;
        public const byte ByteSize = 8;
        public const ulong CarryMask = 0xffffffffffffff00;
        public const byte Bias = TrueBias + MantissaBits;

        public static readonly MbfDouble Zero = FromBytes(0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00);
        public static readonly MbfDouble Half = FromBytes(0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80);
        public static readonly MbfDouble One = FromBytes(0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x81);
        public static readonly MbfDouble Two = FromBytes(0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x82);
        public static readonly MbfDouble Ten = FromBytes(0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x84);
        public static readonly MbfDouble Max = FromBytes(0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x7f, 0xff);
        public static readonly MbfDouble E = FromBytes(0x4b, 0xbb, 0xa2, 0x58, 0x54, 0xf8, 0x2d, 0x82);
        public static readonly MbfDouble Pi = FromBytes(0xc2, 0x68, 0x21, 0xa2, 0xda, 0x0f, 0x49, 0x82);
        public static readonly MbfDouble Log2 = FromBytes(0x7a, 0xcf, 0xd1, 0xf7, 0x17, 0x72, 0x31, 0x80);

        public static readonly MbfDouble MinValue = FromBytes(0xff, 0xff, 0x9f, 0x31, 0xa9, 0x5f, 0x63, 0xb2);
        public static readonly MbfDouble MaxValue = FromBytes(0xff, 0xff, 0x03, 0xbf, 0xc9, 0x1b, 0x0e, 0xb6);

        public MbfDouble(bool isNegitive = false, BigInteger mantisa = default(BigInteger), byte exponent = 0)
            : base(DigitCount, MantissaBits, ByteSize, CarryMask, isNegitive, mantisa, exponent)
        {
        }

        public static MbfDouble FromInt64(long value)
        {
            var single = new MbfDouble(value < 0, (ulong)(Math.Abs(value) << 8), TrueBias + 24);
            single.Normalize();
            return single;
        }

        public static MbfDouble FromBytes(byte[] bytes)
        {
            if (bytes?.Length != ByteSize)
                throw new FormatException("Invalid bytes format. Must be of length 8");

            return FromBytes(bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5], bytes[6], bytes[7]);
        }

        public static MbfDouble FromBytes(byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7, byte b8)
        {
            // put mantissa in form . 1 f1 f2 f3 ... f23
            // internal representation has four bytes, last byte is carry for intermediate results
            // put mantissa in form . 1 f1 f2 f3 ... f55
            // internal representation has seven bytes, last bytes are carry for intermediate results
            var mantisa = (b7 | 0x80U) * (ulong)Math.Pow(0x100, ByteSize - 2);

            // 0, 1
            mantisa += b1;
            mantisa += b2 * 0x100U;
            mantisa += (ulong)(b3 * Math.Pow(0x100, 2));
            mantisa += (ulong)(b4 * Math.Pow(0x100, 3));
            mantisa += (ulong)(b5 * Math.Pow(0x100, 4));
            mantisa += (ulong)(b6 * Math.Pow(0x100, 5));

            mantisa <<= 8;
            return new MbfDouble(b7 >= 0x80, mantisa, b8);
        }

        public MbfDouble Truncate()
        {
            return FromInt64(ToInt64());
        }

        public MbfDouble Floor()
        {
            if (IsZero)
                return Zero;

            var mbf = (MbfFloat)Truncate();
            if (mbf.IsNegitive && !Equals(mbf))
                mbf -= One;

            return (MbfDouble)mbf;
        }

        public override object Clone()
        {
            return new MbfDouble(IsNegitive, Mantissa, Exponent);
        }

        protected override char TypeSign { get; } = '#';

        protected override char ExponentSign { get; } = 'D';

        protected override MbfFloat GetZero()
        {
            return Zero;
        }

        protected override MbfFloat GetMax()
        {
            return Max;
        }

        public override MbfSingle ToSingle()
        {
            var bytes = ToBytes();
            var mantisa = (ulong)Math.Pow((bytes[6] | 0x80) * 0x100, ByteSize - 2);

            // 0, 1
            mantisa += bytes[4];
            mantisa += (byte)(bytes[5] * 0x100);
            mantisa <<= 8;

            mantisa += bytes[3];

            var single = new MbfSingle(bytes[6] >= 0x80, mantisa, bytes[7]);
            single.Normalize();
            return single;
        }
    }
}