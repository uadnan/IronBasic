using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using IronBasic.Compilor;
using IronBasic.Compilor.IO;
using IronBasic.Runtime.Exceptions;

namespace IronBasic.Runtime
{
    public enum BasicProgramMode : byte
    {
        Ascii = 0x00,
        Binary = 0xff,
        Protected = 0xfe
    }

    public class BasicProgram
    {
        private const int BasicLastLineNumber = 65536;

        private Dictionary<int, long> _lineNumberMap = new Dictionary<int, long>();
        private bool _protected;
        private int? _lastStored;

        public BasicProgram(Tokeniser tokeniser, int maxLineNumber = BasicLastLineNumber,
            bool allowProtect = false, bool allowCodePoke = false, int position = 0)
        {
            Tokeniser = tokeniser;
            MaxLineNumber = maxLineNumber;
            AllowProtect = allowProtect;
            AllowCodePoke = allowCodePoke;
            Position = position;

            Erase();
        }

        public BytecodeStream Bytecode { get; } = new BytecodeStream();

        public int MaxLineNumber { get; }

        public bool AllowProtect { get; }

        public bool AllowCodePoke { get; }

        public Tokeniser Tokeniser { get; }

        /// <summary>
        /// Gets or sets memory location of program
        /// </summary>
        public int Position { get; set; }

        public long Length => Bytecode.Length;

        public int? LastStored => _lastStored;

        public int this[long offset]
        {
            get
            {
                offset -= Position;
                var bytes = Bytecode.ToArray();
                if (offset >= bytes.Length)
                    return -1;

                return bytes[offset];
            }
            set
            {
                if (!AllowCodePoke)
                {
                    Trace.TraceWarning("Ignored POKE into program code");
                    return;
                }

                offset -= Position;
                var position = Bytecode.Position;

                // move pointer to end
                Bytecode.Seek(0, SeekOrigin.End);
                if (offset > Bytecode.Position)
                    Bytecode.Write(new string('\0', (int)(offset - Bytecode.Position)));
                else
                    Bytecode.Seek(offset, SeekOrigin.Begin);

                Bytecode.WriteByte((byte)value);
                // restore program pointer
                Bytecode.Seek(position, SeekOrigin.Begin);
                RebuildLineNumbers();
            }
        }

        /// <summary>
        /// Erase the program from memory.
        /// </summary>
        public void Erase()
        {
            Bytecode.Clear();
            Bytecode.Write("\0\0\0");

            _protected = false;
            _lastStored = null;
            _lineNumberMap = new Dictionary<int, long>
            {
                { 65536, 0 }
            };
        }

        /// <summary>
        /// Write \0\0\0 and cut the program of beyond the current position.
        /// </summary>
        public void Truncate()
        {
            Truncate(null);
        }

        /// <summary>
        /// Write bytecode and cut the program of beyond the current position.
        /// </summary>
        /// <param name="byteCode">Bytecode to write</param>
        public void Truncate(string byteCode)
        {
            Bytecode.Clear();
            Bytecode.Write(string.IsNullOrEmpty(byteCode) ? "\0\0\0" : byteCode);
        }

        /// <summary>
        /// Get line number for stream position.
        /// </summary>
        /// <param name="position">Position of stream</param>
        /// <returns>Program line number</returns>
        public int GetLineNumber(long position)
        {
            var lineNumber = -1;
            foreach (var line in _lineNumberMap)
            {
                if (line.Value <= position && line.Key > lineNumber)
                    lineNumber = line.Key;
            }

            return lineNumber;
        }

        private static BasicProgramMode FindDocumentType(Stream inputStream)
        {
            var firstByte = inputStream.ReadByte();
            BasicProgramMode programMode;
            switch (firstByte)
            {
                case (byte)BasicProgramMode.Binary:
                    programMode = BasicProgramMode.Binary;
                    break;
                case (byte)BasicProgramMode.Protected:
                    programMode = BasicProgramMode.Protected;
                    break;
                case 0xfc:
                    throw new NotSupportedException("Q-BASIC File Type is not supported");
                default:
                    inputStream.Seek(0, SeekOrigin.Begin);
                    return BasicProgramMode.Ascii;
            }

            return programMode;
        }

        /// <summary>
        /// Load program from ascii, bytecode or protected stream.
        /// </summary>
        /// <param name="stream">Program stream</param>
        /// <param name="rebuildLineNumber">Should rebuild line numbers</param>
        public void Load(Stream stream, bool rebuildLineNumber = true)
        {
            Erase();
            var programMode = FindDocumentType(stream);
            Bytecode.WriteByte((byte)programMode);
            switch (programMode)
            {
                case BasicProgramMode.Binary:
                    stream.CopyTo(Bytecode);
                    break;
                case BasicProgramMode.Protected:
                    _protected = AllowProtect;
                    ProtectedFileDecoder.Decode(stream).CopyTo(Bytecode);
                    break;
                case BasicProgramMode.Ascii:
                    Merge(stream);
                    break;
            }

            if (rebuildLineNumber && programMode != BasicProgramMode.Ascii)
                RebuildLineNumbers();
        }

        /// <summary>
        /// Merge program from ascii or utf8 (if utf8_files is True) stream.
        /// </summary>
        /// <param name="stream">ASCII program stream to be merged</param>
        private void Merge(Stream stream)
        {
            while (true)
            {
                var line = stream.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;

                var tokenizedLine = Tokeniser.Tokenise(line);
                if (tokenizedLine.Length > 0 && tokenizedLine[0] == '\0')
                {
                    // line starts with a number, add to program memory; store_line seeks to 1 first
                    StoreLine(tokenizedLine.AsStream());
                }
                else
                {
                    // we have read the :
                    var next = stream.SkipPeek(Constants.AsciiWhitepsace);
                    if (next != -1 && next != '\0')
                        throw new BasicRuntimeException(BasicExceptionCode.DirectStatementInFile);
                }

            }
        }

        /// <summary>
        /// Update line number dictionary after deleting lines.
        /// </summary>
        private void UpdateLineMap(CodePosition position, int length)
        {
            // subtract length of line we replaced
            length -= (int)(position.AfterPosition - position.StartPosition);
            var address = Position + 1 + position.AfterPosition;
            Bytecode.Seek(position.AfterPosition + length + 1, SeekOrigin.Begin); // pass \x00
            while (true)
            {
                var nextAddressStr = Bytecode.Read(2);
                if (nextAddressStr.Length < 2 || nextAddressStr == "\0\0")
                    break;

                var nextAddress = nextAddressStr.ToUnsignedInteger();
                Bytecode.Seek(-2, SeekOrigin.Current);
                Bytecode.Write((nextAddress + length).ToBasicUnsignedInteger());
                Bytecode.Seek(nextAddress - address - 2, SeekOrigin.Current);

                address = nextAddress;
            }

            // update line number dict
            foreach (var line in position.Deleteable)
                _lineNumberMap.Remove(line);

            foreach (var line in position.Beyond)
                _lineNumberMap[line] += length;
        }

        /// <summary>
        /// Find code positions for line range.
        /// </summary>
        /// <param name="fromLine"></param>
        /// <param name="toLine"></param>
        private CodePosition FindCodePosition(int fromLine, int toLine)
        {
            var deleteable = _lineNumberMap.Select(x => x.Key).Where(x => x >= fromLine && x <= toLine).ToArray();
            var beyond = _lineNumberMap.Select(x => x.Key).Where(x => x > toLine).ToArray();

            var afterPosition = _lineNumberMap[beyond.Min()];
            long startPosition;
            if (!_lineNumberMap.TryGetValue(deleteable.Min(), out startPosition))
                startPosition = afterPosition;

            return new CodePosition(startPosition, afterPosition, deleteable, beyond);
        }

        /// <summary>
        /// Store the given tokenized line buffer.
        /// </summary>
        private void StoreLine(Stream stream)
        {
            if (_protected)
                throw new BasicRuntimeException(BasicExceptionCode.IllegalFunctionCall);

            stream.Seek(1, SeekOrigin.Begin);
            var scanLine = stream.ReadLineNumber();

            // check if stream is an empty line after the line number
            var nextNonWhitespace = stream.SkipRead(Constants.AsciiWhitepsace);
            var empty = nextNonWhitespace == -1 || nextNonWhitespace == '\0';
            var codePosition = FindCodePosition(scanLine, scanLine);
            if (empty && codePosition.Deleteable.Length == 0)
                throw new BasicRuntimeException(BasicExceptionCode.UndefinedLineNumber);

            // read the remainder of the program into a buffer to be pasted back after the write
            Bytecode.Seek(codePosition.AfterPosition, SeekOrigin.Begin);
            var rest = Bytecode.ReadToEnd();
            Bytecode.Seek(codePosition.StartPosition, SeekOrigin.Begin);
            var length = 0;

            // write the line buffer to the program buffer
            if (!empty)
            {
                // set offsets
                length = stream.ReadToEnd().Length;
                stream.Seek(0, SeekOrigin.Begin); // pass \x00\xC0\xDE

                Bytecode.WriteByte(0);
                Bytecode.Write(((int)(Position + 1 + codePosition.StartPosition + length)).ToBasicUnsignedInteger());
                Bytecode.WriteByte((byte)stream.ReadByte());
            }

            // write back the remainder of the program
            Truncate(rest);

            // update all next offsets by shifting them by the length of the added line
            UpdateLineMap(codePosition, length);

            if (!empty)
                _lineNumberMap[scanLine] = codePosition.StartPosition;

            _lastStored = scanLine;
        }

        /// <summary>
        /// Preparse to build line number dictionary.
        /// </summary>
        private void RebuildLineNumbers()
        {
            _lineNumberMap.Clear();
            var offsets = new List<long>();

            Bytecode.Seek(0, SeekOrigin.Begin);
            var scanPosition = 0L;

            while (true)
            {
                Bytecode.ReadByte(); // pass \x00
                var scanline = Bytecode.ReadLineNumber();
                if (scanline == -1)
                {
                    // if parse_line_number returns -1, it leaves the stream pointer here: 00 _00_ 00 1A
                    break;
                }

                _lineNumberMap[scanline] = scanPosition;
                Bytecode.SkipUntilLineEnd();
                scanPosition = Bytecode.Position;
                offsets.Add(scanPosition);
            }

            _lineNumberMap[BasicLastLineNumber] = scanPosition;
            // rebuild offsets
            Bytecode.Seek(0, SeekOrigin.Begin);
            var lastPosition = 0L;

            foreach (var postion in offsets)
            {
                Bytecode.ReadByte();
                Bytecode.Write(((int)(Position + 1 + postion)).ToBasicUnsignedInteger());
                Bytecode.Read((int)(postion - lastPosition - 3));
                lastPosition = postion;
            }

            // ensure program is properly sealed - last offset must be 00 00. keep, but ignore, anything after.
            Bytecode.Write("\0\0\0");
        }

        /// <summary>
        /// Save the program to stream stream in defined mode.
        /// </summary>
        /// <param name="stream">Stream to which to save</param>
        /// <param name="mode">Mode in which to save program</param>
        public void SaveTo(Stream stream, BasicProgramMode mode)
        {
            if (mode != BasicProgramMode.Protected && _protected)
                throw new BasicRuntimeException(BasicExceptionCode.IllegalFunctionCall);

            var current = Bytecode.Position;

            // skip first \x00 in bytecode
            Bytecode.Seek(1, SeekOrigin.Begin);
            switch (mode)
            {
                case BasicProgramMode.Binary:
                    Bytecode.CopyTo(stream);
                    break;
                case BasicProgramMode.Protected:
                    ProtectedFileDecoder.Encode(Bytecode).CopyTo(stream);
                    break;
                case BasicProgramMode.Ascii:
                    while (true)
                    {
                        var output = Tokeniser.DetokeniseLine(Bytecode);
                        if (output.LineNumber == -1 || output.LineNumber > MaxLineNumber)
                            break;

                        stream.Write(output.Text);
                    }
                    break;
            }

            Bytecode.Seek(current, SeekOrigin.Begin);
        }

        /// <summary>
        /// Deletes the stored program
        /// </summary>
        public void Delete()
        {
            Delete(_lineNumberMap.Select(x => x.Key).Min(), BasicLastLineNumber);
        }

        /// <summary>
        /// Deletes the stored program starting from specified lineNumber
        /// </summary>
        /// <param name="startLine">Line from where to start</param>
        public void Delete(int startLine)
        {
            Delete(startLine, BasicLastLineNumber);
        }

        /// <summary>
        /// Deletes the stored program within specified range
        /// </summary>
        /// <param name="startLine">Starting line</param>
        /// <param name="lastLine">Ending line</param>
        public void Delete(int startLine, int lastLine)
        {
            var codePosition = FindCodePosition(startLine, lastLine);
            if (codePosition.Deleteable.Length == 0) // no lines selected
                throw new BasicRuntimeException(BasicExceptionCode.IllegalFunctionCall);

            Bytecode.Seek(codePosition.AfterPosition, SeekOrigin.Begin);
            var rest = Bytecode.ReadToEnd();
            Bytecode.Seek(codePosition.StartPosition, SeekOrigin.Begin);
            Truncate(rest);

            UpdateLineMap(codePosition, 0);
        }

        public string[] GetLines(int startLine)
        {
            return GetLines(startLine, MaxLineNumber);
        }

        public string[] GetLines(int startLine, int endLine)
        {
            if (_protected)
                throw new BasicRuntimeException(BasicExceptionCode.IllegalFunctionCall);

            // 65529 is max insertable line number for GW-BASIC 3.23.
            // however, 65530-65535 are executed if present in tokenised form.
            // in GW-BASIC, 65530 appears in LIST, 65531 and above are hidden

            // sort by positions, not line numbers!
            var linesByPostion = _lineNumberMap.Where(x => x.Key >= startLine && x.Key <= endLine)
                .Select(x => x.Value).OrderBy(x => x);

            var lines = new List<string>();
            var current = Bytecode.Position;

            foreach (var position in linesByPostion)
            {
                Bytecode.Seek(position + 1, SeekOrigin.Begin);
                lines.Add(Tokeniser.DetokeniseLine(Bytecode).Text);
            }

            Bytecode.Seek(current, SeekOrigin.Begin);
            return lines.ToArray();
        }

        /// <summary>
        /// Check if the given line stream starts with a line number.
        /// </summary>
        /// <param name="stream">Line stream</param>
        /// <param name="isEmpty">True if line is empty i.e. contains only line number</param>
        /// <returns>Line number</returns>
        public int VerifyLineStartsWithNumber(Stream stream, out bool isEmpty)
        {
            stream.Seek(1, SeekOrigin.Begin);
            var scanLine = stream.ReadLineNumber();
            var next = stream.SkipRead(Constants.AsciiWhitepsace);

            // check if linebuf is an empty line after the line number
            isEmpty = next == -1 || next == '\0';
            if (Constants.AsciiDigits.Contains(next))
                throw new BasicRuntimeException(BasicExceptionCode.SyntaxError);

            return scanLine;
        }

        // TODO: Add EDIT & RENUM command support
    }
}