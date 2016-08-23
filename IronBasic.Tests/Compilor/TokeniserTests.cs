using IronBasic.Compilor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronBasic.Tests.Compilor
{
    [TestClass]
    public class TokeniserTests
    {
        private static readonly string[] FormattedLines =
        {
            "10 PRINT \"Hello Wolrd\"",
            "20 PRINT#1, ASC(STR$(1))",
            "30 ? 12/12 : ? \"123\"+123",

            // Functions
            "40 ? ABS(-1)",
            "50 ?ASC(\"a\")",
            "60?ATN(12)",
            "70 ? CDBL(12)",
            "80 ? chr$(12)",
            "90 ? cint(12.19)",
            "100 ? cos(12)",
            "110 ? CSNG(12)",
            "120 ? CSRLIN",
            "130 ? CVI(\"a\")",
            "140 ? CVS(\"a\")",
            "150 ? CVD(\"a\")",
            "160 ? Environ$(12)",
            "170 ? EOF(1)",
            "180 ? ERDEV",
            "190 ? ERDEV$",
            "200 ? ERL",
            "210 ? ERR",
            "220 ? EXP(1)",
            "230 ? EXterr(120)",
            "240 ? FIX(12.9)", 
            "250 ? FN myFunction (x,y)",
            "260 ? FRE(0)",
            "270 ? HEX$(12)",
            "280 ? INKEY$",
            "290 ? IMP(2)",
            "300 ? IMPUT$(123)",
            "310 ? INSTR(1, \"nuaman umer\", \"na\")",
            "320 ? INT(123#)",
            "330 ? IOCTL$(12)",
            "340 ? LEFT$(\"nauman umer\", 3)",
            "350 ? LOC(12)",
            "360 ? LEN(\"nuaman umer\")",
            "370 ? LOF(12)",
            "380 ? LOG(12)",
            "390 ? LPOS(12)",
            "400 ? MID$(\"nauman\", 2, 2)",
            "410 ? MKD$(12#)",
            "420 ? MKI$(12)",
            "430 ? MKS$(12!)",
            "440 ? OCT$(12)",
            "450 ? PEEK(12)",
            "460 ? PEN(8)",
            "470 ? PLAY(12)",
            "480 ? PMAP(3, 3)",
            "490 ? POINT(3)",
            "500 ? POINT(2, 3)",
            "510 ? POS(12)",
            "520 ? RIGHT$(\"nauman umer\", 5)",
            "530 ? RND(0)",
            "540 ? SCREEN(2, 3, 3)",
            "550 ? SGN(-12)",
            "560 ? SIN(12)",
            "570 ? \"nuamnan\" + SPAce$(8)+ \"umer\"",
            "580 ? SQR(4)",
            "590 ? STRICK(12)",
            "600 ? STR$(12)",
            "700 ? STRIG(3)",
            "710 ? TAN(12)",
            "720 ? TIME$",
            "730 ? TIMER",
            "740 ? USR 2 (1)",
            "750 ? VAL(\"nauman\")",
            "760 ? VARPTR(12)",
            "770 ? VARPTR$(12)",

            // Statements
            "0 AUTO",
            "10 BEEP",
            "20 BEEP on",
            "30 BLOAD 12, 2",
            "40 BSAVE 12, 2, 3",
            "30 CALL",
            "40 CALLS 12 3",
            "0 CHAIN",
            "50 CHDIR dir_spec",
            "60 CIRCLE 3 (2, 3), 50, 3, 5, 6, 0",
            "70 CLEAR 12, 23, 256, 25555",
            "80 CLOSE# 2",
            "90 CLS 3",
            "100 COLOR 2, 3, 4",
            "110 COMMON a",
            "120 COM 2 stop",
            "130 CONT",
            "140 DATA 11,25,6,,8,9",
            "150 a = DATE$",
            "160 DEF FN nam x,y,x = x+y+z",
            "180 DEFDBL 12 - 23",
            "190 DEFINT 12 - 23",
            "200 DEFSTR 12 - 23",
            "220 DEFSNG 12 - 23",
            "230 DEF SEG = 12",
            "240 DEF USR2 = 12",
            "250 DIM nauman (50)",
            "260 DRAW \"U10L10\"",
            "270 END",
            "280 IF -1 THEN ? -1 :ELSE ? 0 : STOP",
            "290 ENVIRON \"\"",
            "300 ERASE nauman",
            "400 ERROR (404)",
            "410 FIELD#2,12 AS nam",
            "420 Files fileter_spec",
            "430 for a = 1 to 10 step 2: print a:a=a-1:NEXT a",
            "440 GET#1, 12",
            "450 GET(2,3) - 2(4,5), anuamn",
            "460 GET 2, 1234",
            "470 GOSUB 100",
            "480 GOTO 100",
            "490 IF -1 GOTO 100",
            "500 INPUT \"nauman\";a$,b,c$",
            "600 INPUT#2, a$,b,C$",
            "610 IOCTL#2, \"jhdsjf\"",
            "620 KEY list",
            "630 KEY (2), on",
            "640 KEY 2, \"nauman\"",
            "650 KILL file_spec",
            "660 LCOPY 12",
            "670 LET a= 12",
            "680 LINE 2 (2, 4) - 4 (1,3)",
            "690 LINE INPUT \"nauman\", \"sds\"",
            "700 LINE INPUT # 2, \"sjdghs\"",
            "710 LOCTAE 2,3, 0, 0",
            "720 LOCK # 12, 12 to 24",
            "730 LPRINT \"nauman\" USING \"####.##\"",
            "740 LSET as = 12+2",
            "750 RSET as = 12+2",
            "760 MID$ (\"sdas\", 1, 2) = a$",
            "770 MKDIR dir",
            "780 MOTOR (23)",
            "790 NOISE 12, 23, 12",
            "800 ON S GOTO 12, 23, 32",
            "810 ON COM(2) GOSUB 12,34,35",
            "820 ON ERROR GOTO 12",
            "830 Open m, #2 , file_spec, 255",
            "840 OPTION BASE 0",
            "850 OUT 3, 24",
            "860 PAINT 2 (2,4), m, 3, 4",
            "870 PALETTE 2, 3",
            "880 PALETTE USING n, 2",
            "890 PCOPY 0, 2",
            "900 PEN on",
            "910 PLAY on",
            "920 POKE 12, 12",
            "930 PSET 2 (2, 4), 2",
            "940 PUT#2, 255",
            "950 RANDOMIZE",
            "960 READ a$,b, C$",
            "970 REM jhdjkhfkjdhfkjs : rem Dfsdfsdf",
            "980 RESET",
            "990 RESOTRE 23",
            "1000 RESUME",
            "1010 RETURN",
            "1020 RMDIR",
            "1030 SCREEN 0",
            "1040 SHELL",
            "1050 SOUND",
            "1060 STRIG(2) off",
            "1070 swap a$, b$",
            "1080 TERM",
            "1090 UNLOCK # 2, 25 TO 30",
            "1100 View 0 (2,3)-(4,5)",
            "1110 VIEW PRINT 1 TO 20",
            "1120 WAIT 2, 2, 2",
            "1130 While -1: print \"hello world\":wend",
            "1140 WIDTH 1,2",
            "1150 WINDOW 2(1,2)-(3,4)",
            "1160 WRITE# 2, 1234",

            //Operators
            "10 ? 1^2",
            "20 ? 1*0",
            "30 ? 1/2\\3",
            "40 ? 1MOD2",
            "50 ? 1+1-1",
            "60 ? (1=1)+(1<>1)+(1><1)+(1>1)+(1<1)+(1<=1)+(1=<1)+(1>=1)+(1=>1)", // = -5
            "70 ? NOT(1=1)", // = 0
            "80 ? (1=1)AND(1=1)", //= -1
            "80 ? (1=1)OR(1=1)", //= -1
            "80 ? (1=1)XOR(1=1)", //= 0
            "80 ? (1=1)EQV(1=1)", //= -1
            "80 ? (1=1)IMP(1=1)", //= -1

            //Variables
            "10 a$ = \"jkdfk\"",
            "10 a# = 12",
            "10 a% = 1",
            "10 a! = 12",

            //ARRAY
            "10 a[0,0,0]= 12"
        };

        [TestMethod]
        public void TestFormattedLines()
        {
            var tokeniser = new Tokeniser(Grammar.All);
            foreach (var line in FormattedLines)
            {
                var tokenisedLine = tokeniser.Tokenise(line);
                var detokenisedLine = tokeniser.DetokeniseLine(tokenisedLine).Text;

                var retokenizedLine = tokeniser.Tokenise(detokenisedLine);
                var redetokenisedLine = tokeniser.DetokeniseLine(retokenizedLine).Text;

                Assert.IsTrue(detokenisedLine == redetokenisedLine, $"Either tokenisation or detokenisation of '{line}' has some serious issues");
            }
        }
    }
}