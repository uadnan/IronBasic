using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using IronBasic.Runtime.Types;

namespace IronBasic.Compilor.IO
{
    public class TokenisedLineReader : LineReader
    {
        private readonly IDictionary<string, string> _tokenKeywordMap;

        public TokenisedLineReader(string value, IDictionary<string, string> tokenKeywordMap) : base(value)
        {
            _tokenKeywordMap = tokenKeywordMap;
        }

        /// <summary>
        /// Parse line number and leave pointer at first char of line.
        /// </summary>
        public int ReadLineNumber()
        {
            var word = Read(2);
            if (word == "\0\0" || word.Length < 2)
            {
                if (word.Length > 0)
                    BaseStream.Seek(-1 * word.Length, SeekOrigin.Current);

                return -1;
            }

            word = Read(2);
            if (word.Length < 2)
            {
                BaseStream.Seek(-1 * word.Length - 2, SeekOrigin.Current);
                return -1;
            }

            return word.ToUnsignedInteger();
        }

        public int ReadUnsignedInteger()
        {
            var firstBit = Read();
            return 0x100 * Read() + firstBit;
        }

        // Read two's complement little-endian int token to Python integer
        private int ReadSignedInteger()
        {
            // 2's complement signed int, least significant byte first, sign bit is most significant bit
            var firstBit = Read();
            var lastBit = Read();

            var value = 0x100 * (lastBit & 0x7f) + firstBit;
            if ((lastBit & 0x80) == 0x80)
                return -0x8000 + value;

            return value;
        }

        private string ReadOctal()
        {
            var number = ReadUnsignedInteger();
            return "&O" + Convert.ToString(number, 8);
        }

        private string ReadHex()
        {
            var number = ReadUnsignedInteger();
            return "&H" + Convert.ToString(number, 16).ToUpper();
        }

        private string ReadByte()
        {
            var current = Read();
            if (current == -1)
                return string.Empty;

            return current.ToString();
        }

        public string ReadNumber()
        {
            var current = Read();

            if (current >= Token.Constant0 && current < Token.Constant10)
                return ((char)('0' + current - 0x11)).ToString();

            switch (current)
            {
                case Token.OctalConstant:
                    return ReadOctal();
                case Token.HexadecimalConstant:
                    return ReadHex();
                case Token.ByteConstant:
                    return ReadByte();
                case Token.Constant10:
                    return "10";
                case Token.IntegerConstant:
                    return ReadSignedInteger().ToString();
                case Token.FloatConstant:
                    return MbfSingle.FromBytes((byte)Read(), (byte)Read(), (byte)Read(), (byte)Read()).ToString(false, false);
                case Token.DoubleConstant:
                    var bytes = new byte[8];
                    BaseStream.Read(bytes, 0, 8);
                    return MbfDouble.FromBytes(bytes).ToString(false, false);
            }

            if (current > -1)
                BaseStream.Seek(-1, SeekOrigin.Current);

            return null;
        }


        public string ReadKeywords(out bool isComment)
        {
            isComment = false;

            // try for single-byte token or two-byte token
            // if no match, first char is passed unchanged
            var current = Read();
            string keyword;
            if (!_tokenKeywordMap.TryGetValue(current.ToCharString(), out keyword))
            {
                var currentStr = (char)current + ((char)Peek()).ToString();
                if (_tokenKeywordMap.TryGetValue(currentStr, out keyword))
                    Read();
                else
                    return current.ToCharString();
            }

            // when we're here, s is an actual keyword token.

            var builder = new StringBuilder();
            builder.Append(keyword);

            if (keyword == "'")
                isComment = true;
            else if (keyword == "REM")
            {
                var next = Read();
                if (next > -1)
                {
                    switch (next.ToCharString())
                    {
                        case Token.KeywordORem:
                            // if next char is token('), we have the special value REM'
                            // -- replaced by ' below.
                            builder.Append('\'');
                            break;
                        default:
                            BaseStream.Seek(-1, SeekOrigin.Current);
                            break;
                    }
                }

                isComment = true;
            }

            return builder.ToString();
        }
    }
}