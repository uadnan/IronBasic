using System.IO;

namespace IronBasic.Compilor.IO
{
    public class LineWriter : StreamWriter
    {
        public LineWriter(Stream stream) : base(stream)
        {
        }
    }
}