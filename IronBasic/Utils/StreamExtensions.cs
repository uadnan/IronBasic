using System;
using System.IO;
using System.Linq;
using System.Text;
using IronBasic.Compilor;

namespace IronBasic.Utils
{
    internal static class StreamExtensions
    {
        public static string Read(this Stream stream, int length)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                var current = stream.ReadByte();
                if (current == -1)
                    break;

                builder.Append((char)current);
            }

            return builder.ToString();
        }

        public static string ReadToEnd(this Stream stream)
        {
            var builder = new StringBuilder();
            while (true)
            {
                var current = stream.ReadByte();
                if (current == -1)
                    break;

                builder.Append((char)current);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Parse line number and leave pointer at first char of line.
        /// </summary>
        public static int ReadLineNumber(this Stream stream)
        {
            var word = stream.Read(2);
            if (word == "\0\0" || word.Length < 2)
            {
                if (word.Length > 0)
                    stream.Seek(-1 * word.Length, SeekOrigin.Current);

                return -1;
            }

            word = stream.Read(2);
            if (word.Length < 2)
            {
                stream.Seek(-1 * word.Length - 2, SeekOrigin.Current);
                return -1;
            }

            return word.ToUnsignedInteger();
        }

        public static int SkipRead(this Stream stream, params int[] toSkip)
        {
            while (true)
            {
                var value = stream.ReadByte();
                if (value == -1 || !toSkip.Contains(value))
                    return value;
            }
        }

        public static int SkipPeek(this Stream stream, params int[] toSkip)
        {
            var value = stream.SkipRead(toSkip);
            if (value != -1)
                stream.Seek(-1, SeekOrigin.Current);

            return value;
        }

        /// <summary>
        /// Skip until character is in findrange.
        /// </summary>
        /// <remarks>
        /// String literal and comments will be respected
        /// </remarks>
        public static void AwareSkip(this Stream stream, int[] findRange, bool breakOnFirstChar=true)
        {
            var literal = false;
            var remark = false;

            while (true)
            {
                var current = stream.ReadByte();
                if (current == -1)
                    break;
                if (current == '"')
                    literal = !literal;
                else if (current == Token.KeywordRem[0])
                    remark = true;
                else if (current == '\0')
                {
                    literal = false;
                    remark = false;
                }

                if (literal || remark)
                    continue;

                if (Array.IndexOf(findRange, current) >= 0)
                {
                    if (breakOnFirstChar)
                    {
                        stream.Seek(-1, SeekOrigin.Current);
                        break;
                    }

                    breakOnFirstChar = true;
                }

                //  if not breakOnFirstChar, current needs to be properly processed.
                if (current == '\0')
                {
                    var value = stream.Read(2);
                    if (value.Length < 2 || value == "\0\0")
                        break;

                    stream.Read(2);
                }
                else if (Tokens.PlusByteTokens.ContainsKey((char)current))
                    stream.Read(Tokens.PlusByteTokens[(char)current]);
            }
        }

        public static string ReadLine(this Stream stream)
        {
            var builder = new StringBuilder();
            int value;
            while (true)
            {
                value = stream.ReadByte();
                if (value == -1)
                    break;

                if (value == '\r')
                {
                    var next = stream.ReadByte();
                    if (next != -1 && next != '\n')
                        stream.Seek(-1, SeekOrigin.Current);

                    break;
                }

                if (value == '\n')
                    break;

                builder.Append((char)value);
            }

            if (value != -1)
                stream.Seek(-1, SeekOrigin.Current);

            return builder.ToString();
        }

        public static void Write(this Stream stream, string value)
        {
            foreach (var ch in value)
            {
                stream.WriteByte((byte)ch);
            }
        }

        public static int SkipWhitespace(this Stream stream)
        {
            return SkipPeek(stream, Constants.Whitepsace);
        }
    }
}