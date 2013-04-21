// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they bagin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Collections.Generic;

namespace HtmlRenderer.Utils
{
    internal static class HtmlUtils
    {
        #region Fields and Consts

        /// <summary>
        /// List of html tags that don't have content
        /// </summary>
        private static readonly List<string> _list = new List<string>(
            new[]
                {
                    "area", "base", "basefont", "br", "col",
                    "frame", "hr", "img", "input", "isindex",
                    "link", "meta", "param"
                }
            );

        /// <summary>
        /// the html encode\decode pairs
        /// </summary>
        private static readonly KeyValuePair<string,string>[] _encodeDecode = new[]
                                                           {
                                                               new KeyValuePair<string, string>("&lt;", "<"), 
                                                               new KeyValuePair<string, string>("&gt;", ">"),
                                                               new KeyValuePair<string, string>("&lsquo;", "'"),
                                                               new KeyValuePair<string, string>("&rdquo;", "\""),
                                                               new KeyValuePair<string, string>("&amp;", "&"),
                                                               new KeyValuePair<string, string>("&hellip;", "..."),
                                                           };

        /// <summary>
        /// the html decode only pairs
        /// </summary>
        private static readonly KeyValuePair<string, string>[] _decodeOnly = new[]
                                                           {
                                                               new KeyValuePair<string, string>("&nbsp;", " "),
                                                               new KeyValuePair<string, string>("&hellip;", "..."),
                                                           };

        #endregion

        /// <summary>
        /// Is the given html tag is single tag or can have content.
        /// </summary>
        /// <param name="tagName">the tag to check (must be lower case)</param>
        /// <returns>true - is single tag, false - otherwise</returns>
        public static bool IsSingleTag(string tagName)
        {
            return tagName.StartsWith("!") || (_list).Contains(tagName);
        }

        /// <summary>
        /// Decode html encoded string to regular string.<br/>
        /// Handles &lt;, &gt;, "&amp;.
        /// </summary>
        /// <param name="str">the string to decode</param>
        /// <returns>decoded string</returns>
        public static string DecodeHtml(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                foreach (var encPair in _encodeDecode)
                {
                    str = str.Replace(encPair.Key, encPair.Value);
                }
                foreach (var encPair in _decodeOnly)
                {
                    str = str.Replace(encPair.Key, encPair.Value);
                }

                var idx = str.IndexOf("&#");
                while (idx > -1)
                {
                    var endIdx = idx + 2;
                    long num = 0;
                    while (char.IsDigit(str[endIdx]))
                        num = num*10 + str[endIdx++] - '0';
                    endIdx += str[endIdx] == ';' ? 1 : 0;

                    str = str.Remove(idx, endIdx - idx);
                    str = str.Insert(idx, Convert.ToChar(num).ToString());

                    idx = str.IndexOf("&#", idx);
                }
            }
            return str;
        }

        /// <summary>
        /// Encode regular string into html encoded string.<br/>
        /// Handles &lt;, &gt;, "&amp;.
        /// </summary>
        /// <param name="str">the string to encode</param>
        /// <returns>encoded string</returns>
        public static string EncodeHtml(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                for (int i = _encodeDecode.Length-1; i >= 0; i--)
                {
                    str = str.Replace(_encodeDecode[i].Value, _encodeDecode[i].Key);
                }
            }
            return str;
        }
    }
}
