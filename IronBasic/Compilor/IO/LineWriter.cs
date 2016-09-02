using System.IO;

namespace IronBasic.Compilor.IO
{
    /// <summary>
    /// Base class to write program line
    /// </summary>
    internal class LineWriter : StreamWriter
    {
        public LineWriter(Stream stream) : base(stream)
        {
        }
    }
}