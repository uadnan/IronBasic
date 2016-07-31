using System.IO;
using System.Text;

namespace IronBasic.Compilor.IO
{
    public class DetokenisedLineWriter : LineWriter
    {
        public DetokenisedLineWriter(Stream stream) : base(stream)
        {
        }

        private int MoveBack(int value)
        {
            var max = BaseStream.Length > value ? value : (int)BaseStream.Length;
            BaseStream.Seek(-max, SeekOrigin.Current);
            return max;
        }

        public string ReadLast(int length = 1)
        {
            var max = MoveBack(length);
            var builder = new StringBuilder();

            for (var i = 0; i < max; i++)
            {
                builder.Append((char)BaseStream.ReadByte());
            }

            return builder.ToString();
        }

        public void Replace(string with)
        {
            BaseStream.Seek(0, SeekOrigin.Begin);
            Write(with);
        }
    }
}