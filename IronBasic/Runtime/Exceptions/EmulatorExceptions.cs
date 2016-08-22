using System;

namespace IronBasic.Runtime.Exceptions
{
    /// <summary>
    /// When raised, signals GW-BASIC emulator to stop execution
    /// </summary>
    public class BasicExitException : Exception
    {
    }

    /// <summary>
    /// When raised, signals GW-BASIC emulator to reset
    /// </summary>
    public class BasicResetException : Exception
    {
    }

    /// <summary>
    /// When raised, signals GW-BASIC emulator to break executaion
    /// </summary>
    public class BasicBreakException : Exception
    {
        public BasicBreakException(bool stop)
        {
            Stop = stop;
        }

        public bool Stop { get; }
    }
}