using System;

namespace IronBasic.Compilor
{
    /// <summary>
    /// Marks a field as GW-BASIC Keyword
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    internal class KeywordAttribute : Attribute
    {
        /// <summary>
        /// Create a new instance of <see cref="KeywordAttribute"/>
        /// </summary>
        /// <param name="keyword">String representation of keyword</param>
        public KeywordAttribute(string keyword)
        {
            Keyword = keyword;
        }

        /// <summary>
        /// Gets string representation of underlying keyword
        /// </summary>
        public string Keyword { get; }

        /// <summary>
        /// Gets of set grammer in keyword exists. Default is <see cref="Grammar.All"/>
        /// </summary>
        public Grammar Grammar { get; set; } = Grammar.All;
    }
}