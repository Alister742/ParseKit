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

using System.Collections.Generic;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Entities
{
    internal sealed class HtmlTag
    {
        #region Fields and Consts

        /// <summary>
        /// the name of the html tag
        /// </summary>
        private readonly string _name;
        
        /// <summary>
        /// collection of attributes and thier value the html tag has
        /// </summary>
        private readonly Dictionary<string, string> _attributes;
        
        /// <summary>
        /// is the html tag is closing tag
        /// </summary>
        private readonly bool _isClosing;

        #endregion


        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="name">the name of the html tag</param>
        /// <param name="attributes">collection of attributes and thier value the html tag has</param>
        /// <param name="isClosing">is the html tag is closing tag</param>
        public HtmlTag(string name, Dictionary<string, string> attributes = null, bool isClosing = false)
        {
            ArgChecker.AssertArgNotNullOrEmpty(name, "name");

            _name = name;
            _attributes = attributes ?? new Dictionary<string, string>();
            _isClosing = isClosing;
        }

        /// <summary>
        /// Gets the name of this tag
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets if the tag is actually a closing tag
        /// </summary>
        public bool IsClosing
        {
            get { return _isClosing; }
        }

        /// <summary>
        /// Gets if the tag is single placed; in other words it doesn't need a closing tag; 
        /// e.g. &lt;br&gt;
        /// </summary>
        public bool IsSingle
        {
            get { return HtmlUtils.IsSingleTag(Name); }
        }

        /// <summary>
        /// Gets collection of attributes and thier value the html tag has
        /// </summary>
        public Dictionary<string, string> Attributes
        {
            get { return _attributes; }
        }

        /// <summary>
        /// Gets a boolean indicating if the attribute list has the specified attribute
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public bool HasAttribute(string attribute)
        {
            return Attributes.ContainsKey(attribute);
        }

        /// <summary>
        /// Get attribute value for given attribute name or null if not exists.
        /// </summary>
        /// <param name="attribute">attribute name to get by</param>
        /// <returns>attribute value or null if not found</returns>
        public string TryGetAttribute(string attribute)
        {
            return _attributes.ContainsKey(attribute) ? _attributes[attribute] : null;
        }

        public override string ToString()
        {
            return string.Format("<{1}{0}>", Name, IsClosing ? "/" : string.Empty);
        }
    }
}