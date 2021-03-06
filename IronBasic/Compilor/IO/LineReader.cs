﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using IronBasic.Utils;

namespace IronBasic.Compilor.IO
{
    /// <summary>
    /// Base class to read program line
    /// </summary>
    internal abstract class LineReader : TextReader
    {
        /// <summary>
        /// Gets underlaying base stream
        /// </summary>
        protected Stream BaseStream { get; }

        protected LineReader(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            BaseStream = stream;
        }

        #region TextReader Implementation

        public override int Peek()
        {
            var value = BaseStream.ReadByte();
            if (value != -1)
                BaseStream.Seek(-1, SeekOrigin.Current);
            return value;
        }

        public override int Read()
        {
            return BaseStream.ReadByte();
        }

        #endregion

        #region Extensions

        public string Peek(int length)
        {
            var word = BaseStream.Read(length);
            if (word.Length > 0)
                BaseStream.Seek(-1 * word.Length, SeekOrigin.Current);

            return word;
        }

        /// <summary>
        /// Sets the position within the current stream using
        /// <see cref="SeekOrigin.Current"/> as reference point
        /// </summary>
        /// <param name="offset">A byte offset relative to the current stream position</param>
        /// <returns>The new position within the current stream</returns>
        public long Seek(long offset)
        {
            return BaseStream.Seek(offset, SeekOrigin.Current);
        }

        /// <summary>
        /// Reads a byte as char from the stream and advances the position within the stream by one.
        /// </summary>
        /// <returns>The unsigned byte cast to an char</returns>
        internal char ReadAsChar()
        {
            return (char)BaseStream.ReadByte();
        }

        public int SkipPeek(params int[] toSkip)
        {
            return BaseStream.SkipPeek(toSkip);
        }

        public int SkipWhitespace()
        {
            return BaseStream.SkipWhitespace();
        }

        protected void ReadUntil(StringBuilder builder, params int[] toSkip)
        {
            int value;
            while (true)
            {
                value = BaseStream.ReadByte();
                if (value == -1)
                    break;

                if (toSkip.Contains(value))
                    break;

                builder.Append((char)value);
            }

            if (value != -1)
                BaseStream.Seek(-1, SeekOrigin.Current);
        }

        /// <summary>
        /// Read until a character from a given range is found.
        /// </summary>
        /// <param name="toSkip">Array of elements to skip</param>
        public string ReadUntil(params int[] toSkip)
        {
            var builder = new StringBuilder();
            ReadUntil(builder, toSkip);
            return builder.ToString();
        }

        private void ReadWhile(StringBuilder builder, params int[] toTake)
        {
            int value;
            while (true)
            {
                value = BaseStream.ReadByte();
                if (value == -1)
                    break;

                if (!toTake.Contains(value))
                    break;

                builder.Append((char)value);
            }

            if (value != -1)
                BaseStream.Seek(-1, SeekOrigin.Current);
        }

        /// <summary>
        /// Read while a character from a given range is found.
        /// </summary>
        /// <param name="toTake">Array of elements to skip</param>
        public string ReadWhile(params int[] toTake)
        {
            var builder = new StringBuilder();
            ReadWhile(builder, toTake);
            return builder.ToString();
        }

        #endregion
    }
}