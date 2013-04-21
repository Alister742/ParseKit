using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM
{
    public static class KnownAttrs
    {
        static KnownAttrs()
        {
            InitGlobalAttributes();
        }

        static HashSet<string> _globalAttributes;

        /// <summary>
        /// The following attributes are common to and may be specified on all HTML elements
        /// </summary>
        public static bool IsGlobalAttribute(string name)
        {
            return _globalAttributes.Contains(name);
        }

        #region Initialization
        private static void InitGlobalAttributes()
        {
            _globalAttributes = new HashSet<string>();

            string[] attr = new string[] 
            {"accesskey", "class", "contenteditable", "contextmenu", "dir","draggable","dropzone","draggable","hidden","inert","id","itemid",
                "itemref", "itemprop", "itemscope", "itemtype", "spellcheck", "lang", "style", "tabindex", "title", "translate", "base"};
            AddHashRange(_globalAttributes, attr);
        }
        static void AddHashRange(HashSet<string> hashset, string[] range)
        {
            for (int i = 0; i < range.Length; i++)
            {
                hashset.Add(range[i]);
            }
        }
        #endregion
    }

    public static class KnownTags
    {
        static KnownTags()
        {
            InitFormatting();
            InitSpecialTags();
            InitPTagCloses();
            InitDecode();
            InitAutoEnd();
            InitVoids();
            InitIgnor();
            InitResetable();
        }

        public static class HtmlAliases
        {
            public static string GetOriginal(string alias)
            {
                if (_aliases == null)
                    Create();

                foreach (var item in _aliases)
                {
                    if (item.Key.Contains(alias))
                        return item.Value;
                }
                return null;
            }

            static List<KeyValuePair<HashSet<string>, string>> _aliases;

            static void Create()
            {
                _aliases = new List<KeyValuePair<HashSet<string>, string>>();

                AddAlias("actuate", new string[] { "xlink:actuate" });
                AddAlias("arcrole", new string[] { "xlink:arcrole" });
                AddAlias("href", new string[] { "xlink:href" });
                AddAlias("role", new string[] { "xlink:role" });
                AddAlias("show", new string[] { "xlink:show" });
                AddAlias("type", new string[] { "xlink:type" });
                AddAlias("base", new string[] { "xml:base" });
                AddAlias("lang", new string[] { "xml:lang" });
                AddAlias("space", new string[] { "xml:space" });
                AddAlias("xmlns", new string[] { "xmlns" });
                AddAlias("href", new string[] { "xlink:href" });
                AddAlias("xlink", new string[] { "xmlns:xlink" });
            }

            static void AddAlias(string original, string[] values)
            {
                HashSet<string> aliases = new HashSet<string>(values);
                _aliases.Add(new KeyValuePair<HashSet<string>, string>(aliases, original));
            }
        }

        static List<KeyValuePair<string, HashSet<string>>> _namespaces;
        static Dictionary<string, string> _autoEndTags;
        static Dictionary<string, string> _named;
        static HashSet<string> _voidTags;
        static HashSet<string> _ignorTags;
        static HashSet<string> _pTagCloses;
        static HashSet<string> _specialParsingTags;
        static HashSet<string> _formatting;
        static HashSet<string> _resetable;
        static HashSet<string> _tableScope;
        static HashSet<string> _impliedEndTags;
        static HashSet<string> _elementsWithName;

        /// <summary>
        /// Denotes elements that can be affected when a form element is reset.
        /// </summary>
        public static bool IsResetable(string name)
        {
            return _resetable.Contains(name);
        }
        /// <summary>
        /// Return the string namespace of tag
        /// </summary>
        public static string GetNamespace(string name)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// The following HTML elements are those that end up in the list of active formatting elements
        /// </summary>
        public static bool IsFormatting(string name)
        {
            return _formatting.Contains(name);
        }

        /// <summary>
        /// Tags who autoclose if next tag is one of the following
        /// </summary>
        public static bool IsAutoEndTag(string currentTag, string prevoiusTag)
        {
            return _autoEndTags.ContainsKey(currentTag) && _autoEndTags[currentTag].Contains(prevoiusTag);
        }
        /// <summary>
        /// Tags who have no content -- must be closed emmediatly and his _content will be put as TEXT
        /// </summary>
        public static bool IsVoidTag(string name)
        {
            return _voidTags.Contains(name);
        }
        /// <summary>
        /// An hand managed tags like <html> or </html> [perhaps no will be use in current tree constructing model]
        /// </summary>
        public static bool IsIgnorTags(string name)
        {
            return _ignorTags.Contains(name);
        }
        /// <summary>
        /// Named symbols. For example '&' symbol named as &amp;
        /// </summary>
        public static string GetNamedChar(string name)
        {
            string encoded;
            return _named.TryGetValue(name, out encoded) ? encoded : null;
        }
        /// <summary>
        /// Elements who close <p> tag if is before
        /// </summary>
        public static bool IsPTagClosers(string name)
        {
            return _pTagCloses.Contains(name);
        }
        /// <summary>
        /// A tags who have special rules for parsing and DOM inserting
        /// </summary>
        public static bool IsSpecial(string name)
        {
            return _specialParsingTags.Contains(name);
        }
        /// <summary>
        /// Html Table element including tags
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsTableScopeTag(string name)
        {
            return _tableScope.Contains(name);
        }
        public static bool IsImpliedEndTag(string name)
        {
            return _impliedEndTags.Contains(name);
        }
        public static bool IsElementWithName(string tag)
        {
            return _elementsWithName.Contains(tag);
        }

        #region Initialization
        private static void InitElementsWithNames()
        {
            _elementsWithName = new HashSet<string>();
            string[] elements = new string[] { "a", "applet", "area", "embed", "form", "frame", "frameset", "iframe", "img", "object" };
            AddHashRange(_elementsWithName, elements);
        }
        private static void InitImpliedEndTags()
        {
            _impliedEndTags = new HashSet<string>();
            string[] attr = new string[] { "dd", "dt", "li", "option", "optgroup", "p", "rp", "rt" };
            AddHashRange(_impliedEndTags, attr);
        }
        private static void InitTableScope()
        {
            _tableScope = new HashSet<string>();
            string[] attr = new string[] { "caption", "table", "tbody", "tfoot", "thead", "tr", "td", "th" };
            AddHashRange(_tableScope, attr);
        }
        private static void InitResetable()
        {
            _resetable = new HashSet<string>();
            string[] attr = new string[] { "input", "keygen", "output", "select", "textarea" };
            AddHashRange(_resetable, attr);
        }
        private static void InitFormatting()
        {
            _formatting = new HashSet<string>();
            string[] attr = new string[] { "a", "b", "big", "code", "em", "font", "i", "nobr", "s", "small", "strike", "strong", "tt", "applet", "button", "table", "marquee", "object" };
            AddHashRange(_formatting, attr);
        }
        private static void InitSpecialTags()
        {
            string[] attr = new string[] { "address", "applet", "area", "article", "aside", "base", "basefont", "bgsound", "blockquote", "body", 
                "br", "button", "caption", "center", "col", "colgroup", "menuitem", "dd", "details", "dir", "div", "dl", "dt", "embed", "fieldset", 
                "figcaption", "figure", "footer", "form", "frame", "frameset", "h1", "h2", "h3", "h4", "h5", "h6", "head", "header", "hgroup", "hr", 
                "html", "iframe", "img", "input", "isindex", "li", "link", "listing", "marquee", "menu", "meta", "nav", "noembed", "noframes", 
                "noscript", "object", "ol", "p", "param", "plaintext", "pre", "script", "section", "select", "source", "style", "summary", "table", 
                "tbody", "td", "textarea", "tfoot", "th", "thead", "title", "tr", "track", "ul", "wbr", "xmp", "mi", "mo", "mn", "ms", "mtext", 
                "annotation-xml", "foreignObject", "desc", "title"};
            AddHashRange(_specialParsingTags, attr);
        }
        private static void InitPTagCloses()
        {
            _pTagCloses = new HashSet<string>();
            //"address", "article", "aside", "blockquote", "center", "details", "dialog", "dir", "div", "dl", "fieldset", "figcaption", "figure", "footer", "header", "hgroup", "menu", "nav", "ol", "p", "section", "summary", "ul")
            //"h1", "h2", "h3", "h4", "h5", "h6")
        }
        private static void InitDecode()
        {
            _named = new Dictionary<string, string>();
            _named.Add("&lt;", "<");
            _named.Add("&gt;", ">");
            _named.Add("&lsquo;", "'");
            _named.Add("&rdquo;", "\"");
            _named.Add("&amp;", "&");
            _named.Add("&hellip;", "...");
            _named.Add("&nbsp;", " ");
            _named.Add("&hellip;", "...");
            _named.Add("&#33", "!");
            _named.Add("&quot;", "\"");
            _named.Add("&amp;", "&");
            _named.Add("&frasl;", "/");
            _named.Add("&ndash;", "–");
            _named.Add("&mdash;", "—");
            _named.Add("&nbsp;", "	 ");
            _named.Add("&iexcl;", "¡");
            _named.Add("&cent;", "¢");
            _named.Add("&pound;", "£");
            _named.Add("&curren;", "¤");
            _named.Add("&yen;", "¥");
            _named.Add("&brvbar;", "¦");
            _named.Add("&brkbar;", "¦");
            _named.Add("&sect;", "§");
            _named.Add("&uml;", "¨");
            _named.Add("&die;", "¨");
            _named.Add("&copy;", "©");
            _named.Add("&ordf;", "ª");
            _named.Add("&laquo;", "«");
            _named.Add("&not;", "¬");
            _named.Add("&reg;", "®");
            _named.Add("&macr;", "¯");
            _named.Add("&hibar;", "¯");
            _named.Add("&deg;", "°");
            _named.Add("&plusmn;", "±");
            _named.Add("&sup2;", "²");
            _named.Add("&sup3;", "³");
            _named.Add("&acute;", "´");
            _named.Add("&micro;", "µ");
            _named.Add("&para;", "¶");
            _named.Add("&middot;", "·");
            _named.Add("&cedil;", "¸");
            _named.Add("&sup1;", "¹");
            _named.Add("&ordm;", "º");
            _named.Add("&raquo;", "»");
            _named.Add("&frac14;", "¼");
            _named.Add("&frac12;", "½");
            _named.Add("&frac34;", "¾");
            _named.Add("&iquest;", "¿");
            _named.Add("&Agrave;", "À");
            _named.Add("&Aacute;", "Á");
            _named.Add("&Acirc;", "Â");
            _named.Add("&Atilde;", "Ã");
            _named.Add("&Auml;", "Ä");
            _named.Add("&Aring;", "Å");
            _named.Add("&AElig;", "Æ");
            _named.Add("&Ccedil;", "Ç");
            _named.Add("&Egrave;", "È");
            _named.Add("&Eacute;", "É");
            _named.Add("&Ecirc;", "Ê");
            _named.Add("&Euml;", "Ë");
            _named.Add("&Igrave;", "Ì");
            _named.Add("&Iacute;", "Í");
            _named.Add("&Icirc;", "Î");
            _named.Add("&Iuml;", "Ï");
            _named.Add("&Ntilde;", "Ñ");
            _named.Add("&Ograve;", "Ò");
            _named.Add("&Oacute;", "Ó");
            _named.Add("&Ocirc;", "Ô");
            _named.Add("&Otilde;", "Õ");
            _named.Add("&Ouml;", "Ö");
            _named.Add("&times;", "×");
            _named.Add("&Oslash;", "Ø");
            _named.Add("&Ugrave;", "Ù");
            _named.Add("&Uacute;", "Ú");
            _named.Add("&Ucirc;", "Û");
            _named.Add("&Uuml;", "Ü");
            _named.Add("&Yacute;", "Ý");
            _named.Add("&THORN;", "Þ");
            _named.Add("&szlig;", "ß");
            _named.Add("&agrave;", "à");
            _named.Add("&aacute;", "á");
            _named.Add("&acirc;", "â");
            _named.Add("&atilde;", "ã");
            _named.Add("&auml;", "ä");
            _named.Add("&aring;", "å");
            _named.Add("&aelig;", "æ");
            _named.Add("&ccedil;", "ç");
            _named.Add("&egrave;", "è");
            _named.Add("&eacute;", "é");
            _named.Add("&ecirc;", "ê");
            _named.Add("&euml;", "ë");
            _named.Add("&igrave;", "ì");
            _named.Add("&iacute;", "í");
            _named.Add("&icirc;", "î");
            _named.Add("&iuml;", "ï");
            _named.Add("&eth;", "ð");
            _named.Add("&ntilde;", "ñ");
            _named.Add("&ograve;", "ò");
            _named.Add("&oacute;", "ó");
            _named.Add("&ocirc;", "ô");
            _named.Add("&otilde;", "õ");
            _named.Add("&ouml;", "ö");
            _named.Add("&divide;", "÷");
            _named.Add("&oslash;", "ø");
            _named.Add("&ugrave;", "ù");
            _named.Add("&uacute;", "ú");
            _named.Add("&ucirc;", "û");
            _named.Add(" &uuml;", "ü");
            _named.Add("&yacute;", "ý");
            _named.Add("&thorn;", "þ");
            _named.Add("&yuml;", "ÿ");
        }
        private static void InitAutoEnd()
        {
            //if next node == _autoEndTags.value and last node == _autoEndTags.key then close the _autoEndTags.key tag
            _autoEndTags = new Dictionary<string, string>();
            _autoEndTags.Add("li", "li");
            _autoEndTags.Add("dt", "dt dd");
            _autoEndTags.Add("dd", "dd dt");
            _autoEndTags.Add("p", "address,article,aside,blockquote,center,details,dialog,dir,div,dl,fieldset,figcaption,figure,footer,header,hgroup,menu,nav,ol,p,section,summary,ul,h1,h2,h3,h4,h5,h6");
            _autoEndTags.Add("rt", "rt rp");
            _autoEndTags.Add("rp", "rt rp");
            _autoEndTags.Add("optgroup", "optgroup");
            _autoEndTags.Add("option", "option optgroup");
            _autoEndTags.Add("thead", "tbody tfoot");
            _autoEndTags.Add("tbody", "tbody tfoot");
            _autoEndTags.Add("tfoot", "tbody");
            _autoEndTags.Add("tr", "tr");
            _autoEndTags.Add("td", "td th");
            _autoEndTags.Add("th", "td th");
        }
        private static void InitVoids()
        {
            _voidTags = new HashSet<string>();

            string[] voids = new string[] { "basefont", "bgsound", "area", "base", "br", "col", "command", "embed", "hr", "img", "input", "keygen", "link", "meta", "param", "source", "track", "wbr" };
            AddHashRange(_voidTags, voids);
        }
        private static void InitIgnor()
        {
            _ignorTags = new HashSet<string>();
            string[] ignor = new string[] { "<html>", "</html>", "<head>", "</head>", "<body>", "</body>" };
            AddHashRange(_ignorTags, ignor);
        }
        static void AddHashRange(HashSet<string> hashset, string[] range)
        {
            for (int i = 0; i < range.Length; i++)
            {
                hashset.Add(range[i]);
            }
        }
        #endregion
    }
}
