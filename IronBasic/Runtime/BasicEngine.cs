using System;

namespace IronBasic.Runtime
{
    public enum PointerPosition : byte
    {
        DirectLine,
        Program
    }

    public class BasicEngine
    {
        public BasicEngine(Session session, BasicEngineSettings settings)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            Session = session;
            Settings = settings;

            settings.Seal();
        }

        public Session Session { get; }

        public BasicEngineSettings Settings { get; }
    }
}