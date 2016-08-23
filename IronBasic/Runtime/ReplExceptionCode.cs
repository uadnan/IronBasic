using System;
using System.Reflection;

namespace IronBasic.Runtime
{
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class ExceptionMessageAttribute : Attribute
    {
        public ExceptionMessageAttribute(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    /// <summary>
    /// REPL Exception Codes
    /// </summary>
    public enum ReplExceptionCode
    {
        [ExceptionMessage("NEXT without FOR")]
        NextWithoutFor = 1,
        [ExceptionMessage("Syntax error")]
        SyntaxError = 2,
        [ExceptionMessage("RETURN without GOSUB")]
        ReturnWithoutGosub = 3,
        [ExceptionMessage("Out of DATA")]
        OutOfData = 4,
        [ExceptionMessage("Illegal function call")]
        IllegalFunctionCall = 5,
        [ExceptionMessage("Overflow")]
        Overflow = 6,
        [ExceptionMessage("Out of memory")]
        OutOfMemory = 7,
        [ExceptionMessage("Undefined line number")]
        UndefinedLineNumber = 8,
        [ExceptionMessage("Subscript out of range")]
        SubscriptOutOfRange = 9,
        [ExceptionMessage("Duplicate Definition")]
        DuplicateDefination = 10,
        [ExceptionMessage("Division by zero")]
        DivisionByZero = 11,
        [ExceptionMessage("Illegal direct")]
        IllegalDirect = 12,
        [ExceptionMessage("Type mismatch")]
        TypeMismatch = 13,
        [ExceptionMessage("Out of string space")]
        OutOfStringSpace = 14,
        [ExceptionMessage("String too long")]
        StringTooLong = 15,
        [ExceptionMessage("String formula too complex")]
        StringFormulaTooComplex = 16,
        [ExceptionMessage("Can't continue")]
        CannotContinue = 17,
        [ExceptionMessage("Undefined user function")]
        UndefinedUserFunction = 18,
        [ExceptionMessage("No RESUME")]
        NoResume = 19,
        [ExceptionMessage("RESUME without error")]
        ResumeWithoutError = 20,

        [ExceptionMessage("Missing operand")]
        MissingOperand = 22,
        [ExceptionMessage("Line buffer overflow")]
        LineBufferOverflow = 23,
        [ExceptionMessage("Device Timeout")]
        DeviceTimeout = 24,
        [ExceptionMessage("Device Fault")]
        DeviceFault = 25,
        [ExceptionMessage("FOR without NEXT")]
        ForWithoutNext = 26,
        [ExceptionMessage("Out of paper")]
        OutOfPaper = 27,

        [ExceptionMessage("WHILE without WEND")]
        WhileWithoutWend = 29,
        [ExceptionMessage("WEND without WHIL")]
        WendWithoutWhile = 30,

        [ExceptionMessage("FIELD overflow")]
        FieldOverflow = 50,
        [ExceptionMessage("Internal error")]
        InternalError = 51,
        [ExceptionMessage("Bad file number")]
        BadFileNumber = 52,
        [ExceptionMessage("File not found")]
        FileNotFound = 53,
        [ExceptionMessage("Bad file mode")]
        BadFileMode = 54,
        [ExceptionMessage("File already open")]
        FileAlreadyOpen = 55,

        [ExceptionMessage("Device I/O error")]
        DeviceIoError = 57,
        [ExceptionMessage("File already exists")]
        FileAlreadyExists = 58,

        [ExceptionMessage("Disk full")]
        DiskFull = 61,
        [ExceptionMessage("Input past end")]
        InputPastLine = 62,
        [ExceptionMessage("Bad record number")]
        BadRecordNumber = 63,
        [ExceptionMessage("Bad file name")]
        BadFileName = 64,

        [ExceptionMessage("Direct statement in file")]
        DirectStatementInFile = 66,
        [ExceptionMessage("Too many files")]
        TooManyFiles = 67,
        [ExceptionMessage("Device Unavailable")]
        DeviceUnavailable = 68,
        [ExceptionMessage("Communication buffer overflow")]
        CommunicationBufferOverflow = 69,
        [ExceptionMessage("Permission Denied")]
        PermissionDenied = 70,
        [ExceptionMessage("Disk not Ready")]
        DiskNotReady = 71,
        [ExceptionMessage("Disk media error")]
        DiskMediaError = 72,
        [ExceptionMessage("Advanced Feature")]
        AdvancedFeature = 73,
        [ExceptionMessage("Rename across disks")]
        RenameAcrossDisks = 74,
        [ExceptionMessage("Path/File access error")]
        PathFileAccessError = 75,
        [ExceptionMessage("Path not found")]
        PathNotFound = 76,
        [ExceptionMessage("Deadlock")]
        Deadlock = 77
    }

    public static class ReplExceptionCodeExtensions
    {
        private static readonly Type ReplExceptionCodeType = typeof(ReplExceptionCode);

        /// <summary>
        /// Gets the message associated with <see cref="ReplExceptionCode"/>
        /// </summary>
        /// <param name="code">Whose message to set</param>
        /// <returns>Associated message if there is any otherwise null</returns>
        public static string GetMessage(this ReplExceptionCode code)
        {
            var members = ReplExceptionCodeType.GetMember(code.ToString());
            if (members.Length == 0)
                return null;

            var exceptionMessageAttribute = members[0].GetCustomAttribute<ExceptionMessageAttribute>();
            return exceptionMessageAttribute?.Message;
        }
    }
}