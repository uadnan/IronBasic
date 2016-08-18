using System.Numerics;

namespace IronBasic.Runtime.Types
{
    public interface IMbfFloat
    {
        byte Exponent { get; }

        bool IsNegitive { get; }

        bool IsZero { get; }

        BigInteger Mantissa { get; }

        byte MbfBias { get; }

        byte MbfByteSize { get; }

        ulong MbfCarryMask { get; }

        byte MbfDigitCount { get; }

        byte MbfMantissaBits { get; }
    }
}