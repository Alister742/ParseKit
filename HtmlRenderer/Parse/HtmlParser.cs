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
using System.Text.RegularExpressions;
using HtmlRenderer.Dom;
using HtmlRenderer.Entities;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Parse
{
    /// <summary>
    /// 
    /// </summary>
    internal static class HtmlParser
    {
        /// <summary>
        /// Parses the document
        /// </summary>
        public static CssBox ParseDocument(string document)
        {
            document = RemoveHtmlComments(document);
            
            int lastEnd = -1;
            CssBox root = null;
            CssBox curBox = null;
            var tags = RegexParserUtils.Match(RegexParserUtils.HtmlTag, document);
            foreach (Match tagmatch in tags)
            {
                string text = tagmatch.Index > 0 ? document.Substring(lastEnd + 1, tagmatch.Index - lastEnd - 1) : String.Empty;

                var emptyText = String.IsNullOrEmpty(text.Trim());
                if (!emptyText)
                {
                    if(curBox == null)
                        root = curBox = CssBox.CreateBlock();

                    var abox = CssBox.CreateBox(curBox);
                    abox.Text = text;
                }

                var tag = ParseHtmlTag(tagmatch.Value);

                if (tag.IsClosing)
                {
                    // handle tags that have no content but whitespace
                    if(emptyText && curBox != null && curBox.Boxes.Count == 0 && !string.IsNullOrEmpty(text))
                    {
                        var abox = CssBox.CreateBox(curBox);
                        abox.Text = " ";
                    }

                    // need to find the parent tag to go one level up
                    curBox = DomUtils.FindParent(root, tag.Name, curBox);
                }
                else if (tag.IsSingle)
                {
                    // the current box is not changed
                    new CssBox(curBox, tag);
                }
                else
                {
                    // go one level down, make the new box the current box
                    curBox = new CssBox(curBox, tag);
                }

                if(root == null && curBox != null)
                {
                    root = curBox;
                    root.Display = CssConstants.Block;
                }

                lastEnd = tagmatch.Index + tagmatch.Length - 1;
            }

            if(root == null)
            {
                root = CssBox.CreateBlock();
                var abox = CssBox.CreateBox(root);
                abox.Text = document;
            }
            else if (lastEnd < document.Length)
            {
                var endText = document.Substring(lastEnd+1);
                if(!string.IsNullOrEmpty(endText.Trim()))
                {
                    var abox = CssBox.CreateBox(root);
                    abox.Text = endText;
                }
            }

            return root;
        }

        public static void TranslateAttributes(HtmlTag tag, CssBox box)
        {
            string t = tag.Name.ToUpper();

            foreach (string att in tag.Attributes.Keys)
            {
                string value = tag.Attributes[att];

                switch (att)
                {
                    case HtmlConstants.Align:
                        if (value == HtmlConstants.Left || value == HtmlConstants.Center || value == HtmlConstants.Right || value == HtmlConstants.Justify)
                            box.TextAlign = value;
                        else
                            box.VerticalAlign = value;
                        break;
                    case HtmlConstants.Background:
                        box.BackgroundImage = value;
                        break;
                    case HtmlConstants.Bgcolor:
                        box.BackgroundColor = value;
                        break;
                    case HtmlConstants.Border:
                        box.BorderLeftWidth = box.BorderTopWidth = box.BorderRightWidth = box.BorderBottomWidth = TranslateLength(value);
                        
                        if (t == HtmlConstants.Table)
                        {
                            ApplyTableBorder(box, value);
                        }
                        else
                        {
                            box.BorderTopStyle = box.BorderLeftStyle = box.BorderRightStyle = box.BorderBottomStyle = CssConstants.Solid;
                        }
                        break;
                    case HtmlConstants.Bordercolor:
                        box.BorderLeftColor = box.BorderTopColor = box.BorderRightColor = box.BorderBottomColor = value;
                        break;
                    case HtmlConstants.Cellspacing:
                        box.BorderSpacing = TranslateLength(value);
                        break;
                    case HtmlConstants.Cellpadding:
                        ApplyTablePadding(box, value);
                        break;
                    case HtmlConstants.Color:
                        box.Color = value;
                        break;
                    case HtmlConstants.Dir:
                        box.Direction = value;
                        break;
                    case HtmlConstants.Face:
                        box.FontFamily = CssParser.ParseFontFamilyProperty(value);
                        break;
                    case HtmlConstants.Height:
                        box.Height = TranslateLength(value);
                        break;
                    case HtmlConstants.Hspace:
                        box.MarginRight = box.MarginLeft = TranslateLength(value);
                        break;
                    case HtmlConstants.Nowrap:
                        box.WhiteSpace = CssConstants.Nowrap;
                        break;
                    case HtmlConstants.Size:
                        if (t == HtmlConstants.Hr)
                            box.Height = TranslateLength(value);
                        break;
                    case HtmlConstants.Valign:
                        box.VerticalAlign = value;
                        break;
                    case HtmlConstants.Vspace:
                        box.MarginTop = box.MarginBottom = TranslateLength(value);
                        break;
                    case HtmlConstants.Width:
                        box.Width = TranslateLength(value);
                        break;

                }
            }
        }

        /// <summary>
        /// Splits the text into words and saves the result
        /// </summary>
        public static void ParseToWords(CssBox owner, List<CssBoxWord> words, string text)
        {
            words.Clear();

            int startIdx = 0;
            while (startIdx < text.Length)
            {
                while (startIdx < text.Length && char.IsWhiteSpace(text[startIdx]))
                {
                    startIdx++;
                }

                var endIdx = startIdx + 1;
                while (endIdx < text.Length && !char.IsWhiteSpace(text[endIdx]))
                {
                    endIdx++;
                }

                if (startIdx < text.Length)
                {
                    var hasSpaceBefore = startIdx > 0 && char.IsWhiteSpace(text[startIdx - 1]);
                    var hasSpaceAfter = endIdx < text.Length && char.IsWhiteSpace(text[endIdx]);
                    var word = HtmlUtils.DecodeHtml(text.Substring(startIdx, endIdx - startIdx));
                    words.Add(new CssBoxWord(owner, word, hasSpaceBefore, hasSpaceAfter));
                }
                startIdx = endIdx + 1;
            }
        }


        #region Private methods
        
        /// <summary>
        /// Remove html comments from parsing the html tree.
        /// </summary>
        /// <param name="document">the html to remove comments from</param>
        /// <returns>the html without comments</returns>
        private static string RemoveHtmlComments(string document)
        {
            int startIdx = 0;
            while (startIdx > -1)
            {
                startIdx = document.IndexOf("<!--", startIdx);
                if(startIdx > -1)
                {
                    int endIdx = document.IndexOf("-->", startIdx);
                    if (endIdx > -1)
                    {
                        document = document.Remove(startIdx, endIdx - startIdx + 3);
                    }
                    else
                    {
                        startIdx += 4;
                    }
                }
            }
            return document;
        }

        /// <summary>
        /// Parse raw html tag source to <seealso cref="HtmlTag"/> object.<br/>
        /// Extract attributes found on the tag.
        /// </summary>
        /// <param name="source">the html tag to parse</param>
        private static HtmlTag ParseHtmlTag(string source)
        {
            source = source.Substring(1, source.Length - (source.Length > 2 && source[source.Length - 2] == '/' ? 3 : 2));

            int spaceIndex = source.IndexOf(" ");

            //Extract tag name
            string tagName;
            if (spaceIndex < 0)
            {
                tagName = source;
            }
            else
            {
                tagName = source.Substring(0, spaceIndex);
            }

            //Check if is end tag
            bool isClosing = false;
            if (tagName.StartsWith("/"))
            {
                isClosing = true;
                tagName = tagName.Substring(1);
            }

            tagName = tagName.ToLower();

            //Extract attributes
            var attributes = new Dictionary<string, string>();
            var atts = RegexParserUtils.Match(RegexParserUtils.HmlTagAttributes, source);
            foreach (Match att in atts)
            {
                if (!att.Value.Contains(@"="))
                {
                    if (!attributes.ContainsKey(att.Value))
                        attributes.Add(att.Value.ToLower(), string.Empty);
                }
                else
                {
                    //Extract attribute and value
                    string[] chunks = new string[2];
                    chunks[0] = att.Value.Substring(0, att.Value.IndexOf('='));
                    chunks[1] = att.Value.Substring(att.Value.IndexOf('=') + 1);

                    string attname = chunks[0].Trim();
                    string attvalue = chunks[1].Trim();

                    if (attvalue.Length > 2 && ((attvalue.StartsWith("\"") && attvalue.EndsWith("\"")) || (attvalue.StartsWith("\'") && attvalue.EndsWith("\'")) ))
                    {
                        attvalue = attvalue.Substring(1, attvalue.Length - 2);
                    }

                    if (!attributes.ContainsKey(attname))
                    {
                        attributes.Add(attname, attvalue);
                    }
                }
            }

            return new HtmlTag(tagName, attributes, isClosing);
        }

        /// <summary>
        /// Converts an HTML length into a Css length
        /// </summary>
        /// <param name="htmlLength"></param>
        /// <returns></returns>
        private static string TranslateLength(string htmlLength)
        {
            CssLength len = new CssLength(htmlLength);

            if (len.HasError)
            {
                return htmlLength + "px";
            }

            return htmlLength;
        }

        /// <summary>
        /// Cascades to the TD's the border spacified in the TABLE tag.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="border"></param>
        private static void ApplyTableBorder(CssBox table, string border)
        {
            foreach (CssBox box in table.Boxes)
            {
                foreach (CssBox cell in box.Boxes)
                {
                    cell.BorderLeftWidth = cell.BorderTopWidth = cell.BorderRightWidth = cell.BorderBottomWidth = TranslateLength(border);
                }
            }
        }

        /// <summary>
        /// Cascades to the TD's the border spacified in the TABLE tag.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="padding"></param>
        private static void ApplyTablePadding(CssBox table, string padding)
        {
            foreach (CssBox box in table.Boxes)
            {
                foreach (CssBox cell in box.Boxes)
                {
                    cell.PaddingLeft = cell.PaddingTop = cell.PaddingRight = cell.PaddingBottom = TranslateLength(padding);
                }
            }
        }

        #endregion
    }
}