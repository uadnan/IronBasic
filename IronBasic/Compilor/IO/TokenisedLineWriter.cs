using System.IO;

namespace IronBasic.Compilor.IO
{
    /// <summary>
    /// Writes program line in tokenized form
    /// </summary>
    internal class TokenisedLineWriter : LineWriter
    {
        public TokenisedLineWriter(Stream stream) : base(stream)
        {
        }

        public void WriteLineNumber(string lineNumber)
        {
            if (string.IsNullOrEmpty(lineNumber))
            {
                // direct line; internally, we need an anchor for the program pointer,
                // so we encode a ':'

                Write(':');
                return;
            }

            // terminates last line and fills up the first char in the buffer
            // (that would be the magic number when written to file)
            // in direct mode, we'll know to expect a line number if the output
            // starts with a  00
            Write("\0");

            // write line number. first two bytes are for internal use
            // & can be anything nonzero; we use this.

            Write($"\xC0\xDE{lineNumber}");
        }

        public void WriteHex(string value)
        {
            Write($"{Token.HexadecimalConstant}{value}");
        }

        public void WriteOctal(string value)
        {
            Write($"{Token.OctalConstant}{value}");
        }
    }
}