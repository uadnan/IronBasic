using System;
using System.IO;
using System.Linq;
using IronBasic.Utils;

namespace IronBasic.Runtime
{
    /// <summary>
    /// Represents Bytecode Stream of <see cref="Program"/>
    /// </summary>
    public class BytecodeStream : MemoryStream
    {
        /// <summary>
        /// Clears all contents and resets the position to start
        /// </summary>
        public void Clear()
        {
            var buffer = GetBuffer();
            Array.Clear(buffer, 0, buffer.Length);
            Position = 0;
            SetLength(0);
        }

        /// <summary>
        /// Advance cursor position to end of line
        /// </summary>
        public void SkipUntilLineEnd()
        {
            this.AwareSkip(Constants.LineTerminators);
        }

        /// <summary>
        /// Skip whitespace, peek and raise error if not in range.
        /// </summary>
        /// <param name="exceptionToRaise">Exception to raise in case next char is not in range</param>
        /// <param name="acceptable">List of acceptable chars</param>
        public void Require(ReplExceptionCode exceptionToRaise = ReplExceptionCode.SyntaxError, params int[] acceptable)
        {
            var nextNonWhiteSpace = this.SkipWhitespace();
            if (!acceptable.Contains(nextNonWhiteSpace))
                throw new ReplRuntimeException(exceptionToRaise);
        }
    }
}