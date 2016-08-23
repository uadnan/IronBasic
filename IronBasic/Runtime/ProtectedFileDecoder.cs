using System.IO;

namespace IronBasic.Runtime
{
    public static class ProtectedFileDecoder
    {
        private static readonly int[] Key1 =
        {
            0xA9, 0x84, 0x8D, 0xCD, 0x75, 0x83, 0x43, 0x63, 0x24, 0x83, 0x19, 0xF7, 0x9A
        };

        private static readonly int[] Key2 =
        {
            0x1E, 0x1D, 0xC4, 0x77, 0x26, 0x97, 0xE0, 0x74, 0x59, 0x88, 0x7C
        };

        public static Stream Decode(Stream inputStream)
        {
            var outputStream = new MemoryStream();
            var index = 0;

            while (inputStream.CanRead)
            {
                var currentValue = inputStream.ReadByte();
                if (currentValue == -1)
                    break;

                currentValue -= 11 - index % 11;
                currentValue ^= Key1[index % 13];

                currentValue ^= Key2[index % 11];
                currentValue += 13 - index % 13;

                outputStream.WriteByte((byte) (currentValue % 256));
                index = (index + 1) % (13 * 11);
            }

            outputStream.Seek(0, SeekOrigin.Begin);
            return outputStream;
        }

        public static Stream Encode(Stream inputStream)
        {
            var outputStream = new MemoryStream();
            var index = 0;
            var next = inputStream.ReadByte();

            while (next != -1)
            {
                next -= 13 - index % 13;
                next ^= Key1[index % 13];
                next ^= Key2[index % 11];
                next += 11 - (index % 11);

                outputStream.WriteByte((byte)(next % 256));
                index = (index + 1) % (13 * 11);
                next = inputStream.ReadByte();
            }
            
            outputStream.Seek(0, SeekOrigin.Begin);
            return outputStream;
        }
    }
}