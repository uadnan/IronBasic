using IronBasic.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronBasic.Tests.Types
{
    [TestClass]
    public class MbfDoubleTests
    {
        [TestMethod]
        public void TestMbfDoubleConstants()
        {
            MbfFloatAsserts.AreEqual(MbfDouble.Zero, 9223372036854775808UL, 0, false);
            MbfFloatAsserts.AreEqual(MbfDouble.Half, 9223372036854775808UL, 128, false);
            MbfFloatAsserts.AreEqual(MbfDouble.One, 9223372036854775808UL, 129, false);
            MbfFloatAsserts.AreEqual(MbfDouble.Two, 9223372036854775808UL, 130, false);
            MbfFloatAsserts.AreEqual(MbfDouble.Ten, 11529215046068469760UL, 132, false);
            MbfFloatAsserts.AreEqual(MbfDouble.Max, 18446744073709551360UL, 255, false);
            MbfFloatAsserts.AreEqual(MbfDouble.E, 12535862302449814272UL, 130, false);
            MbfFloatAsserts.AreEqual(MbfDouble.Pi, 14488038916154245632UL, 130, false);
            MbfFloatAsserts.AreEqual(MbfDouble.Log2, 12786308645202655744UL, 128, false);

            // Limits
            MbfFloatAsserts.AreEqual(MbfDouble.MaxValue, 10239999999999999744UL, 182, false);
            MbfFloatAsserts.AreEqual(MbfDouble.MinValue, 16383999999999999744UL, 178, false);
        }
    }
}