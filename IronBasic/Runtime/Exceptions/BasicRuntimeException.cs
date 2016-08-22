using System;

namespace IronBasic.Runtime.Exceptions
{
    public class BasicRuntimeException : Exception
    {
        public BasicRuntimeException(BasicExceptionCode code) : base($"REPL Exception: {code}")
        {
            Code = code;
        }

        public BasicExceptionCode Code { get; }
    }
}