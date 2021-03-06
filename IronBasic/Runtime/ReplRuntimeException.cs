﻿using System;

namespace IronBasic.Runtime
{
    /// <summary>
    /// REPL runtime exception raised to signal specific issue identified
    /// through <see cref="ReplExceptionCode"/>
    /// </summary>
    public class ReplRuntimeException : Exception
    {
        public ReplRuntimeException(ReplExceptionCode code) : base($"REPL Exception: {code}")
        {
            Code = code;
        }

        public ReplExceptionCode Code { get; }
    }

    /// <summary>
    /// When raised, signals <see cref="ReplEngine"/> to stop execution
    /// </summary>
    public class ExitReplException : Exception
    {
    }

    /// <summary>
    /// When raised, signals <see cref="ReplEngine"/> to reset
    /// </summary>
    public class ResetReplException : Exception
    {
    }

    /// <summary>
    /// When raised, signals <see cref="ReplEngine"/> to break executaion
    /// </summary>
    public class BreakReplException : Exception
    {
        public BreakReplException(bool stop)
        {
            Stop = stop;
        }

        public bool Stop { get; }
    }
}