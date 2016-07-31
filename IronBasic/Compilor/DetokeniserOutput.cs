using System;

namespace IronBasic.Compilor
{
    public sealed class DetokeniserOutput
    {
        public DetokeniserOutput(int line, string text)
        {
            if (line < -1)
                throw new ArgumentException("Line number must be greater then or equals -1", nameof(line));

            if (text == null)
                text = string.Empty;

            LineNumber = line;            
            Text = text;
        }

        public int LineNumber { get; }

        public string Text { get; }
    }
}