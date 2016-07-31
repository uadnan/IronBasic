using System;

namespace IronBasic.Compilor
{
    [AttributeUsage(AttributeTargets.Field)]
    internal class KeywordAttribute : Attribute
    {
        public KeywordAttribute(string keyword)
        {
            Keyword = keyword;
        }

        public string Keyword { get; private set; }

        public Grammar Grammar { get; set; } = Grammar.All;
    }
}