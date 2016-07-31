using System.Diagnostics;
using IronBasic.Compilor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronBasic.Tests.Compilor
{
    [TestClass]
    public class TokeniserTests
    {
        private static readonly string[] FormattedLines =
        {
            "10 PRINT \"Hello Wolrd\"",
            "20 PRINT#1, ASC(STR$(1))"
        };

        [TestMethod]
        public void TestFormattedLines()
        {
            var tokeniser = new Tokeniser(Grammar.All);
            foreach (var line in FormattedLines)
            {
                var tokenizedLine = tokeniser.Tokenise(line);
                var detokenisedLine = tokeniser.Detokenise(tokenizedLine).Text;

                Assert.IsTrue(line == detokenisedLine, $"Either tokenisation or detokenisation of '{line}' has some serious issues");
            }
        }
    }
}