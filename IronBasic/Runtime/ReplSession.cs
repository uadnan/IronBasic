using System;

namespace IronBasic.Runtime
{
    /// <summary>
    /// GW-BASIC REPL Session
    /// </summary>
    public class ReplSession
    {
        public ReplSession(ReplSessionConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
        }
    }
}