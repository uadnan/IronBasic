using IronBasic.Compilor.IO;
using System;
using System.IO;

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
            this.AwareSkip(new[]
            {
                -1,
                '\0'
            });
        }
    }
}