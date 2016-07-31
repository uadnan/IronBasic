using System.Linq;

namespace IronBasic.Compilor
{
    internal static class Constants
    {
        public static T[] ConcatArrays<T>(params T[][] list)
        {
            var result = new T[list.Sum(a => a.Length)];
            var offset = 0;

            foreach (var t in list)
            {
                t.CopyTo(result, offset);
                offset += t.Length;
            }

            return result;
        }

        /// <summary>
        /// keywords than can followed by one or more line numbers
        /// </summary>
        public static readonly string[] LineNumberPrefixTokens =
        {
            Token.KeywordGoto,
            Token.KeywordThen,
            Token.KeywordElse,
            Token.KeywordGoSub,
            Token.KeywordList,
            Token.KeywordRenum,
            Token.KeywordEdit,
            Token.KeywordLlist,
            Token.KeywordDelete,
            Token.KeywordRun,
            Token.KeywordResume,
            Token.KeywordAuto,
            Token.KeywordErl,
            Token.KeywordRestore,
            Token.KeywordReturn
        };

        public static readonly int[] AsciiWhitepsace =
        {
            ' ',
            '\t',
            '\n'
        };

        public static readonly int[] AsciiDigits =
        {
            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9'
        };

        public static readonly int[] AsciiHexNumbers =
        {
            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9',
            'A', 'a',
            'B', 'b',
            'C', 'c',
            'D', 'd',
            'E', 'e',
            'F', 'f'
        };

        public static readonly int[] AsciiOctalNumbers =
        {
            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7'
        };

        public static readonly int[] AsciiOperators =
        {
            '+',
            '-',
            '=',
            '/',
            '\\',
            '^',
            '*',
            '<',
            '>'
        };

        public static readonly int[] AsciiLetters = 
        {
            'A', 'a',
            'B', 'b',
            'C', 'c',
            'D', 'd',
            'E', 'e',
            'F', 'f',
            'G', 'g',
            'H', 'h',
            'I', 'i',
            'J', 'j',
            'K', 'k',
            'L', 'l',
            'M', 'm',
            'N', 'n',
            'O', 'o',
            'P', 'p',
            'Q', 'q',
            'R', 'r',
            'S', 's',
            'T', 't',
            'U', 'u',
            'V', 'v',
            'W', 'w',
            'X', 'x',
            'Y', 'y',
            'Z', 'z'
        };

        public static readonly int[] NameChars = ConcatArrays(AsciiLetters, AsciiDigits, new int[] { '.' });
    }
}