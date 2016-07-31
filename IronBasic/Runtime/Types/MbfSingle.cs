using System;

namespace IronBasic.Runtime.Types
{
    public class MbfSingle : MbfFloat
    {
        public const byte DigitCount = 7;
        public const byte MantissaBits = 24;
        public const byte ByteSize = 4;
        public const uint CarryMask = 0xffffff00;
        public const byte Bias = TrueBias + MantissaBits;

        public static readonly MbfSingle Zero = FromBytes(0x00, 0x00, 0x00, 0x00);
        public static readonly MbfSingle Half = FromBytes(0x00, 0x00, 0x00, 0x80);
        public static readonly MbfSingle One = FromBytes(0x00, 0x00, 0x00, 0x81);
        public static readonly MbfSingle Two = FromBytes(0x00, 0x00, 0x00, 0x82);
        public static readonly MbfSingle Ten = FromBytes(0x00, 0x00, 0x00, 0x84);
        public static readonly MbfSingle Max = FromBytes(0xff, 0xff, 0x7f, 0xff);
        public static readonly MbfSingle E = FromBytes(0x54, 0xf8, 0x2d, 0x82);
        public static readonly MbfSingle Pi = FromBytes(0xdb, 0x0f, 0x49, 0x82);
        public static readonly MbfSingle Log2 = FromBytes(0x16, 0x72, 0x31, 0x80);

        public static readonly MbfSingle MinValue = FromBytes(0xff, 0x23, 0x74, 0x94); // 999999.9, highest float  less than 10e+6
        public static readonly MbfSingle MaxValue = FromBytes(0x7f, 0x96, 0x18, 0x98); // 9999999, highest float less than 10e+7
    
        public MbfSingle(bool isNegitive = false, long mantisa = 0, byte exponent = 0)
            : base(DigitCount, MantissaBits, ByteSize, CarryMask, isNegitive, mantisa, exponent)
        {
        }

        public static MbfSingle FromInt64(long value)
        {
            var single = new MbfSingle(value < 0, Math.Abs(value) << 8, TrueBias + 24);
            single.Normalize();
            return single;
        }

        public static MbfSingle FromBytes(byte[] bytes)
        {
            if (bytes?.Length != 4)
                throw new FormatException("Invalid bytes format. Must be of length 4");

            return FromBytes(bytes[0], bytes[1], bytes[2], bytes[3]);
        }

        public static MbfSingle FromBytes(byte b1, byte b2, byte b3, byte b4)
        {
            // put mantissa in form . 1 f1 f2 f3 ... f23
            // internal representation has four bytes, last byte is carry for intermediate results
            // put mantissa in form . 1 f1 f2 f3 ... f55
            // internal representation has seven bytes, last bytes are carry for intermediate results
            var mantisa = (long)Math.Pow((b3 | 0x80)*0x100, ByteSize - 2);

            // 0, 1
            mantisa += b1;
            mantisa += (byte)(b2 * 0x100);
            mantisa <<= 8;

            return new MbfSingle(b3 >= 0x80, mantisa, b4);
        }

        public override object Clone()
        {
            return new MbfSingle(IsNegitive, Mantissa, Exponent);
        }

        protected override char TypeSign { get; } = '!';

        protected override char ExponentSign { get; } = 'E';

        protected override MbfFloat GetZero()
        {
            return Zero;
        }

        protected override MbfFloat GetMax()
        {
            return Max;
        }

        public MbfSingle Truncate()
        {
            return FromInt64(ToInt64());
        }

        public MbfSingle Floor()
        {
            if (IsZero)
                return Zero;

            var mbf = (MbfFloat)Truncate();
            if (mbf.IsNegitive && !Equals(mbf))
                mbf -= One;

            return (MbfSingle)mbf;
        }

        public override MbfSingle ToSingle()
        {
            return this;
        }
    }
}