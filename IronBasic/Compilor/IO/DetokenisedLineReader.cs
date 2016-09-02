using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IronBasic.Types;
using IronBasic.Utils;

namespace IronBasic.Compilor.IO
{
    /// <summary>
    /// Reads detokenized program line
    /// </summary>
    internal class DetokenisedLineReader : LineReader
    {
        private readonly IDictionary<string, string> _keywordTokenMap;

        private static readonly string[] AllowedLongerWords = {
            "FN",
            "SPC(",
            "TAB(",
            "USR"
        };

        public DetokenisedLineReader(string value, IDictionary<string, string> keywordTokenMap)
            : this(value.AsStream(), keywordTokenMap)
        {
        }

        public DetokenisedLineReader(Stream stream, IDictionary<string, string> keywordTokenMap)
            : base(stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            _keywordTokenMap = keywordTokenMap;
        }

        /// <summary>
        /// Read an unsigned int (line number)
        /// </summary>
        public string ReadLineNumber()
        {
            var stringBuilder = new StringBuilder();
            while (true)
            {
                var current = BaseStream.ReadByte();
                if (Constants.DecimalDigits.Contains(current) ||
                    Constants.Whitepsace.Contains(current))
                    stringBuilder.Append((char)current);
                else
                {
                    if (current != -1)
                        BaseStream.Seek(-1, SeekOrigin.Current);

                    break;
                }
            }

            // don't claim trailing w/s
            var trimmedWhitespaces = stringBuilder.TrimEnd(Constants.Whitepsace);
            if (trimmedWhitespaces > 0)
                BaseStream.Seek(-1 * trimmedWhitespaces, SeekOrigin.Current);

            for (var i = 0; i < stringBuilder.Length; i++)
            {
                if (!Constants.Whitepsace.Contains(stringBuilder[i])) continue;

                stringBuilder.Remove(i, 1);
                i--;
            }

            var word = stringBuilder.ToString();
            if (word.Length <= 0) return null;

            if (int.Parse(word) >= 65530)
            {
                // note: anything >= 65530 is illegal in GW-BASIC
                // in loading an ASCII file, GWBASIC would interpret these as
                // '6553 1' etcetera, generating a syntax error on load.
                // keep 6553 as line number and push back the last number:

                BaseStream.Seek(4 - word.Length, SeekOrigin.Current);
                word = word.Substring(0, 4);
            }

            var intValue = int.Parse(word);
            return intValue.ToBasicUnsignedInteger();
        }

        public string ReadStringLiteral()
        {
            var current = Peek();
            if (current != '"')
                return null;

            var builder = new StringBuilder();
            builder.Append(ReadAsChar());
            ReadUntil(builder, '\r', '\0', '"');

            if (Peek() == '"')
                builder.Append(ReadAsChar());

            return builder.ToString();
        }

        public string ReadHex()
        {
            var word = ReadWhile(Constants.HexDigits);
            var value = word.Length == 0 ? 0 : Convert.ToInt32(word, 16);
            return value.ToBasicUnsignedInteger();
        }

        public string ReadOctal()
        {
            // O is optional, could also be &777 instead of &O777
            if (char.ToUpper((char)Peek()) == 'O')
                BaseStream.ReadByte();

            var builder = new StringBuilder();
            while (true)
            {
                var current = Peek();
                if (current == -1)
                    break;

                if (Constants.Whitepsace.Contains(current))
                {
                    // oct literals may be interrupted by whitespace
                    BaseStream.ReadByte();
                    continue;
                }

                if (!Constants.OctalDigits.Contains(current))
                    break;

                builder.Append((char)BaseStream.ReadByte());
            }

            var value = builder.Length == 0 ? 0 : Convert.ToInt32(builder.ToString(), 8);
            return value.ToBasicUnsignedInteger();
        }

        public string ReadRemarks()
        {
            return ReadUntil('\r', '\0');
        }

        public string ReadData()
        {
            var builder = new StringBuilder();

            while (true)
            {
                builder.Append(ReadUntil('\r', '\0', ':', '"'));
                if (SkipPeek() == '"') // string literal in DATA
                    builder.Append(ReadStringLiteral());
                else
                    break;
            }

            return builder.ToString();
        }

        public string ReadDecimal()
        {
            var hasExpotent = false;
            var hasDecimal = false;
            var kill = false;

            var builder = new StringBuilder();
            while (true)
            {
                var current = Read();
                if (current == -1)
                    break;

                var currentChar = char.ToUpper((char)current);
                if ("\x1c\x1d\x1f".Contains(currentChar))
                {
                    // ASCII separator chars invariably lead to zero result
                    kill = true;
                }
                else if (currentChar == '.' && !hasDecimal && !hasExpotent)
                {
                    hasDecimal = true;
                    builder.Append(currentChar);
                }
                else if ((currentChar == 'E' || currentChar == 'D') && !hasExpotent)
                {
                    // there's a special exception for number followed by EL or EQ
                    // presumably meant to protect ELSE and maybe EQV ?        
                    if (currentChar == 'E' && "LQ".Contains(char.ToUpper((char)Peek())))
                    {
                        BaseStream.Seek(-1, SeekOrigin.Current);
                        break;
                    }

                    hasExpotent = true;
                    builder.Append(currentChar);
                }
                else if ((currentChar == '-' || currentChar == '+') &&
                         (builder.Length > 0 && "ED".Contains(builder[builder.Length - 1])))
                {
                    // must be first token or in exponent
                    builder.Append(currentChar);
                }
                else if (Constants.DecimalDigits.Contains(currentChar))
                {
                    builder.Append(currentChar);
                }
                else if (Constants.Whitepsace.Contains(currentChar))
                {
                    // we'll remove this later but need to keep it for now
                    // so we can reposition the stream on removing trailing whitespace
                    builder.Append(currentChar);
                }
                else if ((currentChar == '!' || currentChar == '#') && !hasExpotent)
                {
                    builder.Append(currentChar);
                    break;
                }
                else if (currentChar == '%') // swallow a %, but break parsing
                    break;
                else
                {
                    BaseStream.Seek(-1, SeekOrigin.Current);
                    break;
                }
            }

            var word = kill ? "0" : builder.ToString();

            // don't claim trailing whitespace
            while (word.Length > 0 && Constants.Whitepsace.Contains(word[word.Length - 1]))
            {
                word = word.Substring(0, word.Length - 1);
                BaseStream.Seek(-1, SeekOrigin.Current);
            }

            // remove all internal whitespace
            builder.Clear();
            foreach (var c in word.Where(c => !Constants.Whitepsace.Contains(c)))
            {
                builder.Append(c);
            }

            word = builder.ToString();
            var intValue = word.TryParseInt32();
            builder.Clear();

            if (word.Length == 1 && Constants.DecimalDigits.Contains(word[0]))
            {
                builder.Append((char)(0x11 + intValue));
            }
            else if (!(hasExpotent || hasDecimal || "!#".Contains(word[word.Length - 1])) &&
                     intValue <= 0x7fff && intValue >= -0x8000)
            {
                if (intValue <= 0xff && intValue >= 0) // one-byte constant
                {
                    builder.Append((char)Token.ByteConstant);
                    builder.Append((char)intValue);
                }
                else // two-byte constant
                {
                    builder.Append((char)Token.IntegerConstant);

                    var number = intValue.ToBasicSignedInteger();
                    builder.Append(number);
                }
            }
            else
            {
                var mbf = MbfFloat.Parse(word).ToBytes();
                builder.Append((char)(mbf.Length == 4 ? Token.FloatConstant : Token.DoubleConstant));
                builder.Append(new string(mbf.Select(f => (char)f).ToArray()));
            }

            return builder.ToString();
        }

        public string ReadWord()
        {
            var builder = new StringBuilder();
            while (true)
            {
                var current = Read();
                if (current == -1)
                    return builder.ToString();

                builder.Append(char.ToUpper((char)current));

                // special cases 'GO     TO' -> 'GOTO', 'GO SUB' -> 'GOSUB'
                string word;
                if (builder.ToString() == "GO")
                {
                    var position = BaseStream.Position;
                    // GO SUB allows 1 space
                    if (Peek(4).ToUpper() == " SUB")
                    {
                        builder.Clear();
                        builder.Append("GOSUB");

                        BaseStream.Seek(4, SeekOrigin.Current);
                    }
                    else
                    {
                        // GOTO allows any number of spaces
                        SkipWhitespace();
                        if (Peek(2).ToUpper() == "TO")
                        {
                            builder.Clear();
                            builder.Append("GOTO");

                            BaseStream.Seek(2, SeekOrigin.Current);
                        }
                        else
                            BaseStream.Seek(position, SeekOrigin.Begin);
                    }

                    word = builder.ToString();
                    if (word == "GOTO" || word == "GOSUB")
                    {
                        var next = Peek();
                        if (Constants.NameChars.Contains(next))
                        {
                            BaseStream.Seek(position, SeekOrigin.Begin);
                            builder.Clear();
                            builder.Append("GO");
                        }
                    }
                }

                word = builder.ToString();
                if (_keywordTokenMap.ContainsKey(word))
                {
                    // ignore if part of a longer name, except FN, SPC(, TAB(, USR
                    if (!AllowedLongerWords.Contains(word))
                    {
                        var next = Peek();
                        if (next != -1 && Constants.NameChars.Contains(next))
                            continue;
                    }

                    var token = _keywordTokenMap[word];
                    switch (word)
                    {
                        case "ELSE": // handle special case ELSE -> :ELSE
                            return ":" + token;
                        case "WHILE": // handle special case WHILE -> WHILE+
                            return token + Token.OperatorPlus;
                    }

                    return token;
                }

                // allowed names: letter + (letters, numbers, .)
                if (!Constants.NameChars.Contains(current))
                {
                    word = builder.ToString();
                    BaseStream.Seek(-1, SeekOrigin.Current);
                    return word.Substring(0, word.Length - 1);
                }
            }
        }
    }
}