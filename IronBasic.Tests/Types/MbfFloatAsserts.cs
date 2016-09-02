using IronBasic.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronBasic.Tests.Types
{
    public static class MbfFloatAsserts
    {
        public static void AreEqual(MbfFloat value, ulong mantisa, byte exponent, bool isNegitive)
        {
            Assert.AreEqual(value.Mantissa, mantisa);
            Assert.AreEqual(value.Exponent, exponent);
            Assert.AreEqual(value.IsNegitive, isNegitive);
        }
    }
}