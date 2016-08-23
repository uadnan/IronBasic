using IronBasic.Compilor.IO;
using System;
using System.IO;

namespace IronBasic.Runtime
{
    public class BytecodeStream : MemoryStream
    {
        public void Clear()
        {
            var buffer = GetBuffer();
            Array.Clear(buffer, 0, buffer.Length);
            Position = 0;
            SetLength(0);
        }

        public void SkipUntilLineEnd()
        {
            this.AwareSkip(new int[]
            {
                '\0'
            });
        }
    }
}