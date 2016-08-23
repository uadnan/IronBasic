using System;

namespace IronBasic.Runtime
{
    public enum PointerPosition : byte
    {
        DirectLine,
        Program
    }

    public class ReplEngine
    {
        public ReplEngine(ReplSession session)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            Session = session;
        }

        public ReplSession Session { get; }
    }
}