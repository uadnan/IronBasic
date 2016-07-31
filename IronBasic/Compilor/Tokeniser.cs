using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IronBasic.Compilor.IO;

namespace IronBasic.Compilor
{
    public class Tokeniser
    {
        private readonly IDictionary<string, string> _tokenKeywordMap;
        private readonly IDictionary<string, string> _keywordTokenMap;

        public Tokeniser(Grammar grammar)
        {
            _tokenKeywordMap = Tokens.GetKeywords(grammar);
            _keywordTokenMap = _tokenKeywordMap.ToDictionary(p => p.Value, p => p.Key);
        }

        /// <summary>
        /// Convert an ascii line number to tokenised start-of-line
        /// </summary>
        private void TokeniseLineNumber(DetokenisedLineReader reader, TokenisedLineWriter writer)
        {
            var lineNumber = reader.ReadLineNumber();
            writer.WriteLineNumber(lineNumber);

            if (!string.IsNullOrEmpty(lineNumber))
            {
                // ignore single whitespace after line number, if any,
                // unless line number is zero (as does GW)
                if (reader.Peek() == ' ' && lineNumber != "\0\0")
                    reader.Read();
            }
        }

        private void TokeniseJumpNumbers(DetokenisedLineReader reader, TokenisedLineWriter writer)
        {
            var word = reader.ReadLineNumber();
            if (!string.IsNullOrEmpty(word))
            {
                writer.Write(Token.LineNumber);
                writer.Write(word);
            }
            else
            {
                reader.Read();
                writer.Write('.');
            }
        }

        private void TokeniseNumber(DetokenisedLineReader reader, TokenisedLineWriter writer)
        {
            var current = reader.Peek();
            if (current == -1)
                return;

            if (current == '&') // handle hex or oct constants
            {
                reader.ReadChar();
                if (char.ToUpper((char)reader.Peek()) == 'H')
                {
                    // hex constant
                    writer.WriteHex(reader.ReadHex());
                }
                else
                    writer.WriteOctal(reader.ReadOctal());
            }
            else if (Constants.AsciiDigits.Contains(current) ||
                     current == '.' || current == '+' || current == '-')
            {
                // handle other numbers
                // note GW passes signs separately as a token
                // and only stores positive numbers in the program   
                writer.Write(reader.ReadDecimal());
            }
            else
            {
                // why is this here?
                // this looks wrong but hasn't hurt so far
                reader.BaseStream.Seek(-1, SeekOrigin.Current);
            }
        }

        private void TokeniseRemarks(DetokenisedLineReader reader, TokenisedLineWriter writer)
        {
            writer.Write(reader.ReadUntil('\r', '\0'));
        }

        private void Tokenise(DetokenisedLineReader reader, TokenisedLineWriter writer)
        {

            var value = reader.SkipPeek(Constants.AsciiWhitepsace);
            if (value == -1)
                return; // EOF

            // read the line number
            TokeniseLineNumber(reader, writer);

            var allowJump = false;
            var allowNumber = true;
            var accpetClosingBracket = false;

            while (true)
            {
                var current = reader.Peek();

                // anything after NUL is ignored till EOL
                if (current == '\0')
                {
                    reader.Read();
                    reader.ReadUntil('\r');
                    break;
                }

                if (current == '\r' || current == -1) // EOF or 
                    break;

                if (Constants.AsciiWhitepsace.Contains(current)) // whitespace
                {
                    reader.Read();
                    writer.Write((char)current);
                }
                else if (current == '"') // string literals
                {
                    writer.Write(reader.ReadStringLiteral());
                }

                // handle jump numbers
                else if (allowJump && allowNumber && (Constants.AsciiDigits.Contains(current) || current == '.'))
                {
                    TokeniseJumpNumbers(reader, writer);
                }
                // numbers following var names with no operator or token in between
                // should not be parsed, eg OPTION BASE 1
                // note we don't include leading signs, encoded as unary operators
                // number starting with . or & are always parsed

                else if (current == '&' || current == '.' ||
                         (allowNumber && !allowJump && Constants.AsciiDigits.Contains(current)))
                {
                    TokeniseNumber(reader, writer);
                }

                // operator keywords ('+', '-', '=', '/', '\\', '^', '*', '<', '>')
                else if (Constants.AsciiOperators.Contains(current))
                {
                    reader.Read();

                    // operators don't affect line number mode - can do line number
                    // arithmetic and RENUM will do the strangest things
                    // this allows for 'LIST 100-200' etc.
                    writer.Write(_keywordTokenMap[current.ToString()]);
                    allowNumber = true;
                }

                // special case ' -> :REM'
                else if (current == '\'')
                {
                    reader.Read();
                    writer.Write(':');
                    writer.Write(Token.KeywordRem);
                    writer.Write(Token.KeywordORem);

                    TokeniseRemarks(reader, writer);
                }

                // special case ? -> PRINT
                else if (current == '?')
                {
                    reader.Read();
                    writer.Write(Token.KeywordPrint);

                    allowNumber = true;
                }

                // keywords & variable names
                else if (Constants.AsciiLetters.Contains(current))
                {
                    var word = reader.ReadWord();
                    writer.Write(word);

                    // handle non-parsing modes
                    switch (word)
                    {
                        case "REM":
                        case "'":
                            writer.Write(reader.ReadRemarks());
                            break;
                        case "DATA":
                            writer.Write(reader.ReadData());
                            break;
                        default:
                            allowJump = Constants.LineNumberPrefixTokens.Contains(word);
                            // numbers can follow tokenised keywords
                            // (which does not include the word 'AS')

                            allowNumber = _keywordTokenMap.ContainsKey(word);
                            if (word == "SPC(" || word == "TAB(")
                                accpetClosingBracket = true;
                            break;
                    }
                }
                else
                {
                    reader.Read();
                    if (current == ',' || current == '#' || current == ';')
                        allowNumber = true; // can separate numbers as well as jumpnums
                    else if (current == '(' || current == '[')
                    {
                        allowJump = false;
                        allowNumber = true;
                    }
                    else if (current == ')' && accpetClosingBracket)
                    {
                        accpetClosingBracket = false;
                        allowJump = false;
                        allowNumber = true;
                    }
                    else
                    {
                        allowJump = false;
                        allowNumber = false;
                    }

                    // replace all other nonprinting chars by spaces;
                    if (current >= 32 && current <= 127)
                        writer.Write((char)current);
                    else
                        writer.Write(' ');
                }
            }
        }

        /// <summary>
        /// Converts ASCII Program line to tokenised form
        /// </summary>
        /// <param name="line">ASCII Program line to tokenise</param>
        /// <returns>Tokenised program line</returns>
        public string Tokenise(string line)
        {
            using (var inputStream = new DetokenisedLineReader(line, _keywordTokenMap))
            using (var outputStream = new MemoryStream())
            {
                using (var outputWriter = new TokenisedLineWriter(outputStream))
                {
                    Tokenise(inputStream, outputWriter);
                }

                return Encoding.UTF8.GetString(outputStream.ToArray());
            }
        }

        private bool ShouldAddWhiteSpace(int nextValue)
        {
            if (nextValue <= 0)
                return false;

            var nextStr = nextValue.ToCharString();
            if (Tokens.OperatorTokens.Contains(nextStr))
                return false;

            if (nextStr == Token.KeywordORem)
                return false;

            switch (nextValue)
            {
                case '"':
                case ',':
                case ' ':
                case ':':
                case '(':
                case ')':
                case '$':
                case '%':
                case '!':
                case '#':
                case '_':
                case '@':
                case '~':
                case '|':
                case '`':
                    return false;

            }

            return true;
        }


        /// <summary>
        /// Detokenise tokens until end of line.
        /// </summary>
        private void DetokeniseCompoundStatement(TokenisedLineReader reader, DetokenisedLineWriter writer)
        {
            var stringLiteral = false;
            var comment = false;

            while (true)
            {
                var current = reader.Read();
                if (current == -1 || current == '\0')
                    // \x00 ends lines and comments when listed,
                    // if not inside a number constant
                    // stream ended or end of line                
                    break;

                if (current == '"')
                {
                    // start of literal string, passed verbatim
                    // until a closing quote or EOL comes by
                    // however number codes are *printed* as the corresponding numbers,
                    // even inside comments & literals
                    writer.Write((char)current);
                    stringLiteral = !stringLiteral;
                }
                else if (Tokens.NumberTypeTokens.Contains(current))
                {
                    reader.BaseStream.Seek(-1, SeekOrigin.Current);
                    writer.Write(reader.ReadNumber());
                }
                else if (Tokens.LineNumberTokens.Contains(current))
                {
                    // 0D: line pointer (unsigned int) - this token should not be here;
                    // interpret as line number and carry on
                    // 0E: line number (unsigned int)
                    // return str(vartypes.integer_to_int_unsigned(vartypes.bytes_to_integer(s)))
                    writer.Write(reader.ReadUnsignedInteger());
                }
                else if (comment || stringLiteral || (current >= 0x20 && current <= 0x7E))
                {
                    writer.Write((char)current);
                }
                else if (current == 0x0A)
                {
                    writer.Write("\x0A\x0D");
                }
                else if (current <= 0x09)
                {
                    writer.Write((char)current);
                }
                else
                {
                    reader.BaseStream.Seek(-1, SeekOrigin.Current);
                    if (writer.BaseStream.Length > 0)
                    {
                        // letter or number followed by token is separated by a space
                        writer.BaseStream.Seek(-1, SeekOrigin.Current);
                        var lastByte = writer.BaseStream.ReadByte();
                        if (Constants.AsciiDigits.Contains(lastByte) || !Constants.AsciiOperators.Contains(current))
                            writer.Write(' ');
                    }

                    var keyword = reader.ReadKeywords(out comment);

                    // check for special cases
                    //   [:REM']   ->  [']
                    if (writer.ReadLast(4) == ":REM")
                        writer.Replace("'");

                    //   [WHILE+]  ->  [WHILE]
                    else if (writer.ReadLast(5) == "WHILE+")
                        writer.Replace("WHILE");

                    //   [:ELSE]  ->  [ELSE]
                    else if (writer.ReadLast(4) == "ELSE")
                    {
                        // note that anything before ELSE gets cut off,
                        // e.g. if we have 1ELSE instead of :ELSE it also becomes ELSE
                        var lastSix = writer.ReadLast(6);
                        if (lastSix[1] == ':' && Constants.AsciiDigits.Contains(lastSix[0]))
                            writer.Replace(": ELSE");
                        else
                            writer.Replace(lastSix[1] + "ELSE");
                    }

                    // token followed by token or number is separated by a space,
                    // except operator tokens and SPC(, TAB(, FN, USR
                    var next = reader.Peek();
                    var currentStr = current.ToCharString();

                    if (!comment && ShouldAddWhiteSpace(next) &&
                        !(Tokens.OperatorTokens.Contains(currentStr) ||
                        Tokens.WithBracketTokens.Contains(currentStr) ||
                        currentStr == Token.KeywordUsr ||
                        currentStr == Token.KeywordFn
                        ))
                    {
                        writer.Write(' ');
                    }

                    writer.Write(keyword);
                }
            }
        }

        private int Detokenise(TokenisedLineReader reader, DetokenisedLineWriter writer)
        {
            var currentLine = reader.ReadLineNumber();
            if (currentLine < 0)
                return -1;

            // ignore up to one space after line number 0
            if (currentLine == 0 && reader.Peek() == ' ')
                reader.Read();

            writer.Write(currentLine.ToString());

            // write one extra whitespace character after line number
            // unless first char is TAB
            if (reader.Peek() != '\t')
                writer.Write(' ');

            DetokeniseCompoundStatement(reader, writer);
            return currentLine;
        }

        /// <summary>
        /// Convert a tokenised program line to ascii text
        /// </summary>
        /// <param name="line">Tokenised Line Stream</param>
        /// <returns>Detokenised ASCII line</returns>
        public DetokeniserOutput Detokenise(string line)
        {
            using (var inputStream = new TokenisedLineReader(line, _tokenKeywordMap))
            using (var outputStream = new MemoryStream())
            {
                if (inputStream.Peek() == '\0')
                    inputStream.Read();

                int lineNumber;
                using (var outputWriter = new DetokenisedLineWriter(outputStream))
                {
                    lineNumber = Detokenise(inputStream, outputWriter);
                }

                return new DetokeniserOutput(lineNumber, Encoding.UTF8.GetString(outputStream.ToArray()));
            }
        }
    }
}