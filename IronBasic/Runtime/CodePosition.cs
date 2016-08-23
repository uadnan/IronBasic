namespace IronBasic.Runtime
{
    internal sealed class CodePosition
    {
        public CodePosition(long startPosition, long afterPosition, int[] deleteable, int[] beyond)
        {
            StartPosition = startPosition;
            AfterPosition = afterPosition;
            Deleteable = deleteable;
            Beyond = beyond;
        }

        /// <summary>
        /// Lowest postion within range
        /// </summary>
        public long StartPosition { get; }

        /// <summary>
        /// Lowest position strictly above range
        /// </summary>
        public long AfterPosition { get; }

        /// <summary>
        /// Lines number within range
        /// </summary>
        public int[] Deleteable { get; }

        /// <summary>
        /// Line numbers beyond range
        /// </summary>
        public int[] Beyond { get; }
    }
}