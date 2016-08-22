using IronBasic.Runtime.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronBasic.Tests.Runtime.Types
{
    [TestClass]
    public class MbfSingleTests
    {
        [TestMethod]
        public void TestMbfSingleConstants()
        {
            MbfFloatAsserts.AreEqual(MbfSingle.Zero, 2147483648UL, 0, false);
            MbfFloatAsserts.AreEqual(MbfSingle.Half, 2147483648UL, 128, false);
            MbfFloatAsserts.AreEqual(MbfSingle.One, 2147483648UL, 129, false);
            MbfFloatAsserts.AreEqual(MbfSingle.Two, 2147483648UL, 130, false);
            MbfFloatAsserts.AreEqual(MbfSingle.Ten, 2684354560UL, 132, false);
            MbfFloatAsserts.AreEqual(MbfSingle.Max, 4294967040UL, 255, false);
            MbfFloatAsserts.AreEqual(MbfSingle.E, 2918732800UL, 130, false);
            MbfFloatAsserts.AreEqual(MbfSingle.Pi, 3373259520UL, 130, false);
            MbfFloatAsserts.AreEqual(MbfSingle.Log2, 2977043968UL, 128, false);

            // Limits
            MbfFloatAsserts.AreEqual(MbfSingle.MaxValue, 2559999744UL, 152, false);
            MbfFloatAsserts.AreEqual(MbfSingle.MinValue, 4095999744UL, 148, false);
        }
    }
}