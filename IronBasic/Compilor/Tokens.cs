using System.Collections.Generic;
using System.Reflection;

namespace IronBasic.Compilor
{
    /// <summary>
    /// GW-BASIC Tokens
    /// <see cref="http://www.chebucto.ns.ca/~af380/GW-BASIC-tokens.html"/>
    /// </summary>
    public static class Token
    {
        // indirect line number references
        public const int LinePointer = '\x0D';
        public const int LineNumber = '\x0E';

        /// <summary>
        /// An octal constant
        /// </summary>
        public const int OctalConstant = '\x0B';

        /// <summary>
        /// A hexadecimal constant
        /// </summary>
        public const int HexadecimalConstant = '\x0C';

        /// <summary>
        /// A one-byte integer constant, 11 to 255.
        /// </summary>
        public const int ByteConstant = '\x0F';

        /// <summary>
        /// A two-byte integer constant.
        /// </summary>
        public const int IntegerConstant = '\x1C';

        /// <summary>
        /// A four-byte single-precision floating-point constant.
        /// </summary>
        public const int FloatConstant = '\x1D';

        /// <summary>
        /// An eight-byte double-precision floating-point constant.
        /// </summary>
        public const int DoubleConstant = '\x1F';

        // Number Constants
        public const int Constant0 = '\x11';
        public const int Constant1 = '\x12';
        public const int Constant2 = '\x13';
        public const int Constant3 = '\x14';
        public const int Constant4 = '\x15';
        public const int Constant5 = '\x16';
        public const int Constant6 = '\x17';
        public const int Constant7 = '\x18';
        public const int Constant8 = '\x19';
        public const int Constant9 = '\x1A';
        public const int Constant10 = '\x1B';

        [Keyword("END")] public const string KeywordEnd = "\x81";

        [Keyword("FOR")] public const string KeywordFor = "\x82";

        [Keyword("NEXT")] public const string KeywordNext = "\x83";

        [Keyword("DATA")] public const string KeywordData = "\x84";

        [Keyword("INPUT")] public const string KeywordInput = "\x85";

        [Keyword("DIM")] public const string KeywordDim = "\x86";

        [Keyword("READ")] public const string KeywordRead = "\x87";

        [Keyword("LET")] public const string KeywordLet = "\x88";

        [Keyword("GOTO")] public const string KeywordGoto = "\x89";

        [Keyword("RUN")] public const string KeywordRun = "\x8a";

        [Keyword("IF")] public const string KeywordIf = "\x8b";

        [Keyword("RESTORE")] public const string KeywordRestore = "\x8c";

        [Keyword("GOSUB")] public const string KeywordGoSub = "\x8d";

        [Keyword("RETURN")] public const string KeywordReturn = "\x8e";

        [Keyword("REM")] public const string KeywordRem = "\x8f";

        [Keyword("STOP")] public const string KeywordStop = "\x90";

        [Keyword("PRINT")] public const string KeywordPrint = "\x91";

        [Keyword("CLEAR")] public const string KeywordClear = "\x92";

        [Keyword("LIST")] public const string KeywordList = "\x93";

        [Keyword("NEW")] public const string KeywordNew = "\x94";

        [Keyword("ON")] public const string KeywordOn = "\x95";

        [Keyword("WAIT")] public const string KeywordWait = "\x96";

        [Keyword("DEF")] public const string KeywordDef = "\x97";

        [Keyword("POKE")] public const string KeywordPoke = "\x98";

        [Keyword("CONT")] public const string KeywordCont = "\x99";

        [Keyword("OUT")] public const string KeywordOut = "\x9c";

        [Keyword("LPRINT")] public const string KeywordLprint = "\x9d";

        [Keyword("LLIST")] public const string KeywordLlist = "\x9e";

        [Keyword("WIDTH")] public const string KeywordWidth = "\xa0";

        [Keyword("ELSE")] public const string KeywordElse = "\xa1";

        [Keyword("TRON")] public const string KeywordTron = "\xa2";

        [Keyword("TROFF")] public const string KeywordTroff = "\xa3";

        [Keyword("SWAP")] public const string KeywordSwap = "\xa4";

        [Keyword("ERASE")] public const string KeywordErase = "\xa5";

        [Keyword("EDIT")] public const string KeywordEdit = "\xa6";

        [Keyword("ERROR")] public const string KeywordError = "\xa7";

        [Keyword("RESUME")] public const string KeywordResume = "\xa8";

        [Keyword("DELETE")] public const string KeywordDelete = "\xa9";

        [Keyword("AUTO")] public const string KeywordAuto = "\xaa";

        [Keyword("RENUM")] public const string KeywordRenum = "\xab";

        [Keyword("DEFSTR")] public const string KeywordDefStr = "\xac";

        [Keyword("DEFINT")] public const string KeywordDefInt = "\xad";

        [Keyword("DEFSNG")] public const string KeywordDefSng = "\xae";

        [Keyword("DEFDBL")] public const string KeywordDefDbl = "\xaf";

        [Keyword("LINE")] public const string KeywordLine = "\xb0";

        [Keyword("WHILE")] public const string KeywordWhile = "\xb1";

        [Keyword("WEND")] public const string KeywordWend = "\xb2";

        [Keyword("CALL")] public const string KeywordCall = "\xb3";

        [Keyword("WRITE")] public const string KeywordWrite = "\xb7";

        [Keyword("OPTION")] public const string KeywordOption = "\xb8";

        [Keyword("RANDOMIZE")] public const string KeywordRandomize = "\xb9";

        [Keyword("OPEN")] public const string KeywordOpen = "\xba";

        [Keyword("CLOSE")] public const string KeywordClose = "\xbb";

        [Keyword("LOAD")] public const string KeywordLoad = "\xbc";

        [Keyword("MERGE")] public const string KeywordMerge = "\xbd";

        [Keyword("SAVE")] public const string KeywordSave = "\xbe";

        [Keyword("COLOR")] public const string KeywordColor = "\xbf";

        [Keyword("CLS")] public const string KeywordCls = "\xc0";

        [Keyword("MOTOR")] public const string KeywordMotor = "\xc1";

        [Keyword("BSAVE")] public const string KeywordBinarySave = "\xc2";

        [Keyword("BLOAD")] public const string KeywordBinaryLoad = "\xc3";

        [Keyword("SOUND")] public const string KeywordSound = "\xc4";

        [Keyword("BEEP")] public const string KeywordBeep = "\xc5";

        [Keyword("PSET")] public const string KeywordPset = "\xc6";

        [Keyword("PRESET")] public const string KeywordPreset = "\xc7";

        [Keyword("SCREEN")] public const string KeywordScreen = "\xc8";

        [Keyword("KEY")] public const string KeywordKey = "\xc9";

        [Keyword("LOCATE")] public const string KeywordLocate = "\xca";

        [Keyword("TO")] public const string KeywordTo = "\xcc";

        [Keyword("THEN")] public const string KeywordThen = "\xcd";

        [Keyword("TAB(")] public const string KeywordTab = "\xce";

        [Keyword("STEP")] public const string KeywordStep = "\xcf";

        [Keyword("USR")] public const string KeywordUsr = "\xd0";

        [Keyword("FN")] public const string KeywordFn = "\xd1";

        [Keyword("SPC(")] public const string KeywordSpc = "\xd2";

        [Keyword("NOT")] public const string KeywordNot = "\xd3";

        [Keyword("ERL")] public const string KeywordErl = "\xd4";

        [Keyword("ERR")] public const string KeywordErr = "\xd5";

        [Keyword("STRING$")] public const string KeywordString = "\xd6";

        [Keyword("USING")] public const string KeywordUsing = "\xd7";

        [Keyword("INSTR")] public const string KeywordInstr = "\xd8";

        [Keyword("'")] public const string KeywordORem = "\xd9";

        [Keyword("VARPTR")] public const string KeywordVarptr = "\xda";

        [Keyword("CSRLIN")] public const string KeywordCsrlin = "\xdb";

        [Keyword("POINT")] public const string KeywordPoint = "\xdc";

        [Keyword("OFF")] public const string KeywordOff = "\xdd";

        [Keyword("INKEY$")] public const string KeywordInkey = "\xde";

        [Keyword(">")] public const string OperatorGreaterThen = "\xe6";

        [Keyword("=")] public const string OperatorEqual = "\xe7";

        [Keyword("<")] public const string OperatorLessThen = "\xe8";

        [Keyword("+")] public const string OperatorPlus = "\xe9";

        [Keyword("-")] public const string OperatorMinus = "\xea";

        [Keyword("*")] public const string OperatorTimes = "\xeb";

        [Keyword("/")] public const string OperatorDivision = "\xec";

        [Keyword("^")] public const string OperatorCaret = "\xed";

        [Keyword("AND")] public const string OperatorAnd = "\xee";

        [Keyword("OR")] public const string OperatorOr = "\xef";

        [Keyword("XOR")] public const string OperatorXOr = "\xf0";

        [Keyword("EQV")] public const string OperatorEquivalent = "\xf1";

        [Keyword("IMP")] public const string OperatorImp = "\xf2";

        [Keyword("MOD")] public const string OperatorMod = "\xf3";

        [Keyword("\\")] public const string OperatorIntegerDivision = "\xf4";

        [Keyword("CVI")] public const string KeywordCvi = "\xfd\x81";

        [Keyword("CVS")] public const string KeywordCvs = "\xfd\x82";

        [Keyword("CVD")] public const string KeywordCvd = "\xfd\x83";

        [Keyword("MKI$")] public const string KeywordMki = "\xfd\x84";

        [Keyword("MKS$")] public const string KeywordMks = "\xfd\x85";

        [Keyword("MKD$")] public const string KeywordMkd = "\xfd\x86";

        [Keyword("EXTERR")] public const string KeywordExterr = "\xfd\x8b";

        [Keyword("FILES")] public const string KeywordFiles = "\xfe\x81";

        [Keyword("FIELD")] public const string KeywordField = "\xfe\x82";

        [Keyword("SYSTEM")] public const string KeywordSystem = "\xfe\x83";

        [Keyword("NAME")] public const string KeywordName = "\xfe\x84";

        [Keyword("LSET")] public const string KeywordLset = "\xfe\x85";

        [Keyword("RSET")] public const string KeywordRset = "\xfe\x86";

        [Keyword("KILL")] public const string KeywordKill = "\xfe\x87";

        [Keyword("PUT")] public const string KeywordPut = "\xfe\x88";

        [Keyword("GET")] public const string KeywordGet = "\xfe\x89";

        [Keyword("RESET")] public const string KeywordReset = "\xfe\x8a";

        [Keyword("COMMON")] public const string KeywordCommon = "\xfe\x8b";

        [Keyword("CHAIN")] public const string KeywordChain = "\xfe\x8c";

        [Keyword("DATE$")] public const string KeywordDate = "\xfe\x8d";

        [Keyword("TIME$")] public const string KeywordTime = "\xfe\x8e";

        [Keyword("PAINT")] public const string KeywordPaint = "\xfe\x8f";

        [Keyword("COM")] public const string KeywordCom = "\xfe\x90";

        [Keyword("CIRCLE")] public const string KeywordCircle = "\xfe\x91";

        [Keyword("DRAW")] public const string KeywordDraw = "\xfe\x92";

        [Keyword("PLAY")] public const string KeywordPlay = "\xfe\x93";

        [Keyword("TIMER")] public const string KeywordTimer = "\xfe\x94";

        [Keyword("ERDEV")] public const string KeywordErdev = "\xfe\x95";

        [Keyword("IOCTL")] public const string KeywordIoctl = "\xfe\x96";

        [Keyword("CHDIR")] public const string KeywordChdir = "\xfe\x97";

        [Keyword("MKDIR")] public const string KeywordMkdir = "\xfe\x98";

        [Keyword("RMDIR")] public const string KeywordRmdir = "\xfe\x99";

        [Keyword("SHELL")] public const string KeywordShell = "\xfe\x9a";

        [Keyword("ENVIRON")] public const string KeywordEnviron = "\xfe\x9b";

        [Keyword("VIEW")] public const string KeywordView = "\xfe\x9c";

        [Keyword("WINDOW")] public const string KeywordWindow = "\xfe\x9d";

        [Keyword("PMAP")] public const string KeywordPmap = "\xfe\x9e";

        [Keyword("PALETTE")] public const string KeywordPalette = "\xfe\x9f";

        [Keyword("LCOPY")] public const string KeywordLcopy = "\xfe\xa0";

        [Keyword("CALLS")] public const string KeywordCalls = "\xfe\xa1";

        [Keyword("PCOPY")] public const string KeywordPcopy = "\xfe\xa5";

        [Keyword("LOCK")] public const string KeywordLock = "\xfe\xa7";

        [Keyword("UNLOCK")] public const string KeywordUnlock = "\xfe\xa8";

        [Keyword("LEFT$")] public const string KeywordLeft = "\xff\x81";

        [Keyword("RIGHT$")] public const string KeywordRight = "\xff\x82";

        [Keyword("MID$")] public const string KeywordMid = "\xff\x83";

        [Keyword("SGN")] public const string KeywordSgn = "\xff\x84";

        [Keyword("INT")] public const string KeywordInt = "\xff\x85";

        [Keyword("ABS")] public const string KeywordAbs = "\xff\x86";

        [Keyword("SQR")] public const string KeywordSqr = "\xff\x87";

        [Keyword("RND")] public const string KeywordRnd = "\xff\x88";

        [Keyword("SIN")] public const string KeywordSin = "\xff\x89";

        [Keyword("LOG")] public const string KeywordLog = "\xff\x8a";

        [Keyword("EXP")] public const string KeywordExp = "\xff\x8b";

        [Keyword("COS")] public const string KeywordCos = "\xff\x8c";

        [Keyword("TAN")] public const string KeywordTan = "\xff\x8d";

        [Keyword("ATN")] public const string KeywordAtn = "\xff\x8e";

        [Keyword("FRE")] public const string KeywordFre = "\xff\x8f";

        [Keyword("INP")] public const string KeywordInp = "\xff\x90";

        [Keyword("POS")] public const string KeywordPos = "\xff\x91";

        [Keyword("LEN")] public const string KeywordLen = "\xff\x92";

        [Keyword("STR$")] public const string KeywordStr = "\xff\x93";

        [Keyword("VAL")] public const string KeywordVal = "\xff\x94";

        [Keyword("ASC")] public const string KeywordAsc = "\xff\x95";

        [Keyword("CHR$")] public const string KeywordChr = "\xff\x96";

        [Keyword("PEEK")] public const string KeywordPeek = "\xff\x97";

        [Keyword("SPACE$")] public const string KeywordSpace = "\xff\x98";

        [Keyword("OCT$")] public const string KeywordOct = "\xff\x99";

        [Keyword("HEX$")] public const string KeywordHex = "\xff\x9a";

        [Keyword("LPOS")] public const string KeywordLpos = "\xff\x9b";

        [Keyword("CINT")] public const string KeywordCint = "\xff\x9c";

        [Keyword("CSNG")] public const string KeywordCsng = "\xff\x9d";

        [Keyword("CDBL")] public const string KeywordCdbl = "\xff\x9e";

        [Keyword("FIX")] public const string KeywordFix = "\xff\x9f";

        [Keyword("PEN")] public const string KeywordPen = "\xff\xa0";

        [Keyword("STICK")] public const string KeywordStick = "\xff\xa1";

        [Keyword("STRIG")] public const string KeywordStrig = "\xff\xa2";

        [Keyword("EOF")] public const string KeywordEof = "\xff\xa3";

        [Keyword("LOC")] public const string KeywordLoc = "\xff\xa4";

        [Keyword("LOF")] public const string KeywordLof = "\xff\xa5";

        [Keyword("NOISE", Grammar = Grammar.Pcjr | Grammar.Tandy)] public const string KeywordNoise = "\xfe\xa4";

        [Keyword("TERM", Grammar = Grammar.Pcjr | Grammar.Tandy)] public const string KeywordTerm = "\xfe\xa6";
    }

    public static class Tokens
    {
        public static readonly int[] DigitConstantTokens =
        {
            Token.Constant0,
            Token.Constant1,
            Token.Constant2,
            Token.Constant3,
            Token.Constant4,
            Token.Constant5,
            Token.Constant6,
            Token.Constant7,
            Token.Constant8,
            Token.Constant9
        };

        public static readonly int[] NumberTypeTokens =
        {
            Token.OctalConstant,
            Token.HexadecimalConstant,
            Token.ByteConstant,
            Token.IntegerConstant,
            Token.FloatConstant,
            Token.DoubleConstant,
            Token.Constant0,
            Token.Constant1,
            Token.Constant2,
            Token.Constant3,
            Token.Constant4,
            Token.Constant5,
            Token.Constant6,
            Token.Constant7,
            Token.Constant8,
            Token.Constant9,
            Token.Constant10
        };

        public static readonly int[] LineNumberTokens =
        {
            Token.LineNumber,
            Token.LinePointer
        };

        public static readonly string[] OperatorTokens =
        {
            Token.OperatorGreaterThen,
            Token.OperatorEqual,
            Token.OperatorLessThen,
            Token.OperatorPlus,
            Token.OperatorMinus,
            Token.OperatorTimes,
            Token.OperatorDivision,
            Token.OperatorCaret,
            Token.OperatorIntegerDivision
        };

        public static readonly string[] WithBracketTokens =
        {
            Token.KeywordSpc,
            Token.KeywordTab
        };

        /// <summary>
        /// Two-byte keyword token lead bytes
        /// </summary>
        public static readonly string[] TwoByteTokens =
        {
            "\xff",
            "\xfe",
            "\xfd"
        };

        /// <summary>
        /// Tokens followed by one or more bytes to be skipped
        /// </summary>
        public static readonly IDictionary<char, int> PlusByteTokens = new Dictionary<char, int>
        {
            { (char)Token.ByteConstant, 1 },
            { '\xff', 1 },
            { '\xfe', 1 },
            { '\xfd', 1 },
            { (char)Token.OctalConstant, 2 },
            { (char)Token.HexadecimalConstant, 2 },
            { (char)Token.LinePointer, 2 },
            { (char)Token.LineNumber, 2 },
            { (char)Token.IntegerConstant, 2 },
            { (char)Token.FloatConstant, 4 },
            { (char)Token.DoubleConstant, 8 },
            { '\0', 4 }
        };

        public static IDictionary<string, string> GetKeywordTokens(Grammar grammar)
        {
            var tokenTypeInfo = typeof(Token);
            var tokenFileds = tokenTypeInfo.GetFields(BindingFlags.Public | BindingFlags.Static);

            var keywordsDictionary = new Dictionary<string, string>();
            foreach (var field in tokenFileds)
            {
                var keywordAttrib = field.GetCustomAttribute<KeywordAttribute>();
                if (keywordAttrib == null) continue;

                if (keywordAttrib.Grammar.HasFlag(grammar))
                    keywordsDictionary.Add((string)field.GetValue(null), keywordAttrib.Keyword);
            }

            return keywordsDictionary;
        }
    }
}