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
using System.Drawing;
using HtmlRenderer.Dom;
using HtmlRenderer.Entities;
using HtmlRenderer.Parse;

namespace HtmlRenderer.Utils
{
    /// <summary>
    /// Utility method for handling CSS stuff.
    /// </summary>
    internal static class CssUtils
    {
        #region Fields and Consts

        /// <summary>
        /// collection of all existing fonts to check if font exists
        /// </summary>
        private static readonly HashSet<string> _existingFonts = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// cache of all the font used not to create same font again and again
        /// </summary>
        private static readonly Dictionary<string, Dictionary<float, Dictionary<FontStyle, Font>>> _fontsCache = new Dictionary<string, Dictionary<float, Dictionary<FontStyle, Font>>>();

        /// <summary>
        /// cache of the font sizes not to call Font.GetHeight() each time it is required
        /// </summary>
        private static readonly Dictionary<Font, float> _fontHeightCache = new Dictionary<Font, float>();

        /// <summary>
        /// cache of brush color to brush instance
        /// </summary>
        private static readonly Dictionary<Color, Brush> _brushesCache = new Dictionary<Color, Brush>();

        /// <summary>
        /// cache for holding regions data by font
        /// </summary>
        private static readonly Dictionary<Font, WhitespaceData> _whitespaceCache = new Dictionary<Font, WhitespaceData>();

        /// <summary>
        /// used for the regions cache calculations
        /// </summary>
        private static readonly StringFormat _sf;

        /// <summary>
        /// Brush for selection background
        /// </summary>
        private static readonly Brush _selectionBackcolor = new SolidBrush(Color.FromArgb(0xa9,0x33, 0x99, 0xFF));

        /// <summary>
        /// default CSS parsed data singleton
        /// </summary>
        private static CssData _defaultCssData ;

        #endregion


        /// <summary>
        /// Init.
        /// </summary>
        static CssUtils()
        {
            _sf = new StringFormat();
            _sf.SetMeasurableCharacterRanges(new[] { new CharacterRange(0, 1) });
        }

        /// <summary>
        /// default CSS parsed data singleton
        /// </summary>
        public static CssData DefaultCssData
        {
            get
            {
                if(_defaultCssData == null)
                {
                    _defaultCssData = new CssData();
                    CssParser.ParseStyleSheet(_defaultCssData, CssDefaults.DefaultStyleSheet);
                }
                return _defaultCssData;
            }
        }

        /// <summary>
        /// Brush for selection background
        /// </summary>
        public static Brush SelectionBackcolor
        {
            get { return _selectionBackcolor; }
        }

        /// <summary>
        /// Check if the given font family exists by name
        /// </summary>
        /// <param name="fontFamily">the font to check</param>
        /// <returns>true - font exists by given family name, false - otherwise</returns>
        public static bool IsFontExists(string fontFamily)
        {
            if(_existingFonts.Count == 0)
            {
                foreach (var family in FontFamily.Families)
                {
                    _existingFonts.Add(family.Name);
                }
            }
            return _existingFonts.Contains(fontFamily);
        }

        /// <summary>
        /// Get cached font instance for the given font properties.<br/>
        /// Improve performance not to create same font multiple times.
        /// </summary>
        /// <returns>cached font instance</returns>
        public static Font GetCachedFont(string family, float size, FontStyle style)
        {
            Font font = null;
            if (_fontsCache.ContainsKey(family))
            {
                var a = _fontsCache[family];
                if (a.ContainsKey(size))
                {
                    var b = a[size];
                    if (b.ContainsKey(style))
                    {
                        font = b[style];
                    }
                }
                else
                {
                    _fontsCache[family][size] = new Dictionary<FontStyle, Font>();
                }
            }
            else
            {
                _fontsCache[family] = new Dictionary<float, Dictionary<FontStyle, Font>>();
                _fontsCache[family][size] = new Dictionary<FontStyle, Font>();
            }

            if (font == null)
            {
                font = new Font(family, size, style);
                _fontsCache[family][size][style] = font;
            }
            return font;
        }

        /// <summary>
        /// Get cached font height for the given font.<br/>
        /// Improve performance not to access the GetHeight property of a font as it is expensive.<br/>
        /// Should be used with <see cref="GetCachedFont"/> as the cache uses the font object itself as key.
        /// </summary>
        /// <param name="font">the font to get its height</param>
        /// <returns>the height of the font</returns>
        public static float GetFontHeight(Font font)
        {
            float height;
            if(!_fontHeightCache.TryGetValue(font,out height))
            {
                _fontHeightCache[font] = height = font.GetHeight();
            }
            return height;
        }

        /// <summary>
        /// Get cached solid brush instance for the given color.
        /// </summary>
        /// <param name="color">the color to get brush for</param>
        /// <returns>brush instance</returns>
        public static Brush GetSolidBrush(Color color)
        {
            if (color == Color.White)
            {
                return Brushes.White;
            }
            else if (color == Color.Black)
            {
                return Brushes.Black;
            }
            else
            {
                Brush brush;
                if (!_brushesCache.TryGetValue(color, out brush))
                {
                    brush = new SolidBrush(color);
                    _brushesCache[color] = brush;
                }
                return brush;
            }
        }

        /// <summary>
        /// Gets the white space width of the specified box
        /// </summary>
        /// <param name="g"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public static float WhiteSpace(Graphics g, CssBoxProperties box)
        {
            float w = MeasureWhitespace(g, box.ActualFont);
            if (!(String.IsNullOrEmpty(box.WordSpacing) || box.WordSpacing == CssConstants.Normal))
            {
                w += CssValueParser.ParseLength(box.WordSpacing, 0, box, true);
            }
            return w;
        }

        /// <summary>
        /// Gets the ascent of the font
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        /// <remarks>
        /// Font metrics from http://msdn.microsoft.com/en-us/library/xwf9s90b(VS.71).aspx
        /// </remarks>
        public static float GetAscent(Font font)
        {
            return font.Size * font.FontFamily.GetCellAscent(font.Style) / font.FontFamily.GetEmHeight(font.Style);
        }

        /// <summary>
        /// Gets the descent of the font
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        /// <remarks>
        /// Font metrics from http://msdn.microsoft.com/en-us/library/xwf9s90b(VS.71).aspx
        /// </remarks>
        public static float GetDescent(Font font)
        {
            return font.Size * font.FontFamily.GetCellDescent(font.Style) / font.FontFamily.GetEmHeight(font.Style);
        }

        /// <summary>
        /// Gets the line spacing of the font
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        /// <remarks>
        /// Font metrics from http://msdn.microsoft.com/en-us/library/xwf9s90b(VS.71).aspx
        /// </remarks>
        public static float GetLineSpacing(Font font)
        {
            return font.Size * font.FontFamily.GetLineSpacing(font.Style) / font.FontFamily.GetEmHeight(font.Style);
        }
        
        /// <summary>
        /// Get the margin added to the end of the word in the given font.
        /// </summary>
        /// <param name="font">the font to measure with</param>
        /// <returns>thw margin width</returns>
        public static float GetWordEndWhitespace(Font font)
        {
            WhitespaceData width;
            _whitespaceCache.TryGetValue(font, out width);
            return width.WordEndWhitespace;
        }

        /// <summary>
        /// Check if the given css box is display block (not inline)
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public static bool IsBlockDisplay(CssBox box)
        {
            return box.Display != CssConstants.Inline;
        }

        /// <summary>
        /// Get CSS box property value by the CSS name.<br/>
        /// Used as a mapping between CSS property and the class property.
        /// </summary>
        /// <param name="cssBox">the CSS box to get it's property value</param>
        /// <param name="propName">the name of the CSS property</param>
        /// <returns>the value of the property, null if no such property exists</returns>
        public static string GetPropertyValue(CssBox cssBox, string propName)
        {
            switch (propName)
            {
                case "border-bottom-width":     return cssBox.BorderBottomWidth;
                case "border-left-width":       return cssBox.BorderLeftWidth;
                case "border-right-width":      return cssBox.BorderRightWidth;
                case "border-top-width":        return cssBox.BorderTopWidth;
                case "border-bottom-style":     return cssBox.BorderBottomStyle;
                case "border-left-style":       return cssBox.BorderLeftStyle;
                case "border-right-style":      return cssBox.BorderRightStyle;
                case "border-top-style":        return cssBox.BorderTopStyle;
                case "border-bottom-color":     return cssBox.BorderBottomColor;
                case "border-left-color":       return cssBox.BorderLeftColor;
                case "border-right-color":      return cssBox.BorderRightColor;
                case "border-top-color":        return cssBox.BorderTopColor;
                case "border-spacing":          return cssBox.BorderSpacing;
                case "border-collapse":         return cssBox.BorderCollapse;
                case "corner-radius":           return cssBox.CornerRadius;
                case "corner-nw-radius":        return cssBox.CornerNWRadius;
                case "corner-ne-radius":        return cssBox.CornerNERadius;
                case "corner-se-radius":        return cssBox.CornerSERadius;
                case "corner-sw-radius":        return cssBox.CornerSWRadius;
                case "margin-bottom":           return cssBox.MarginBottom;
                case "margin-left":             return cssBox.MarginLeft;
                case "margin-right":            return cssBox.MarginRight;
                case "margin-top":              return cssBox.MarginTop;
                case "padding-bottom":          return cssBox.PaddingBottom;
                case "padding-left":            return cssBox.PaddingLeft;
                case "padding-right":           return cssBox.PaddingRight;
                case "padding-top":             return cssBox.PaddingTop;
                case "left":                    return cssBox.Left;
                case "top":                     return cssBox.Top;
                case "width":                   return cssBox.Width;
                case "height":                  return cssBox.Height;
                case "background-color":        return cssBox.BackgroundColor;
                case "background-image":        return cssBox.BackgroundImage;
                case "background-repeat":       return cssBox.BackgroundRepeat;
                case "background-gradient":     return cssBox.BackgroundGradient;
                case "background-gradient-angle":return cssBox.BackgroundGradientAngle;
                case "color":                    return cssBox.Color;
                case "display":                  return cssBox.Display;
                case "direction":                return cssBox.Direction;
                case "empty-cells":              return cssBox.EmptyCells;
                case "float":                    return cssBox.Float;
                case "position":                 return cssBox.Position;
                case "line-height":              return cssBox.LineHeight;
                case "vertical-align":           return cssBox.VerticalAlign;
                case "text-indent":              return cssBox.TextIndent;
                case "text-align":               return cssBox.TextAlign;
                case "text-decoration":          return cssBox.TextDecoration;
                case "white-space":              return cssBox.WhiteSpace;
                case "word-spacing":             return cssBox.WordSpacing;
                case "font-family":              return cssBox.FontFamily;                
                case "font-size":                return cssBox.FontSize;
                case "font-style":               return cssBox.FontStyle;
                case "font-variant":             return cssBox.FontVariant;
                case "font-weight":              return cssBox.FontWeight;
                case "list-style":               return cssBox.ListStyle;
                case "list-style-position":      return cssBox.ListStylePosition;
                case "list-style-image":         return cssBox.ListStyleImage;
                case "list-style-type":          return cssBox.ListStyleType;
            }
            return null;
        }

        /// <summary>
        /// Set CSS box property value by the CSS name.<br/>
        /// Used as a mapping between CSS property and the class property.
        /// </summary>
        /// <param name="cssBox">the CSS box to set it's property value</param>
        /// <param name="propName">the name of the CSS property</param>
        /// <param name="value">the value to set</param>
        public static void SetPropertyValue(CssBox cssBox, string propName, string value)
        {
            switch (propName)
            {
                case "border-bottom-width":
                    cssBox.BorderBottomWidth = value;
                    break;
                case "border-left-width":
                    cssBox.BorderLeftWidth = value;
                    break;
                case "border-right-width":
                    cssBox.BorderRightWidth = value;
                    break;
                case "border-top-width":
                    cssBox.BorderTopWidth = value;
                    break;
                case "border-bottom-style":
                    cssBox.BorderBottomStyle = value;
                    break;
                case "border-left-style":
                    cssBox.BorderLeftStyle = value;
                    break;
                case "border-right-style":
                    cssBox.BorderRightStyle = value;
                    break;
                case "border-top-style":
                    cssBox.BorderTopStyle = value;
                    break;
                case "border-bottom-color":
                    cssBox.BorderBottomColor = value;
                    break;
                case "border-left-color":
                    cssBox.BorderLeftColor = value;
                    break;
                case "border-right-color":
                    cssBox.BorderRightColor = value;
                    break;
                case "border-top-color":
                    cssBox.BorderTopColor = value;
                    break;
                case "border-spacing":
                    cssBox.BorderSpacing = value;
                    break;
                case "border-collapse":
                    cssBox.BorderCollapse = value;
                    break;
                case "corner-radius":
                    cssBox.CornerRadius = value;
                    break;
                case "corner-nw-radius":
                    cssBox.CornerNWRadius = value;
                    break;
                case "corner-ne-radius":
                    cssBox.CornerNERadius = value;
                    break;
                case "corner-se-radius":
                    cssBox.CornerSERadius = value;
                    break;
                case "corner-sw-radius":
                    cssBox.CornerSWRadius = value;
                    break;
                case "margin-bottom":
                    cssBox.MarginBottom = value;
                    break;
                case "margin-left":
                    cssBox.MarginLeft = value;
                    break;
                case "margin-right":
                    cssBox.MarginRight = value;
                    break;
                case "margin-top":
                    cssBox.MarginTop = value;
                    break;
                case "padding-bottom":
                    cssBox.PaddingBottom = value;
                    break;
                case "padding-left":
                    cssBox.PaddingLeft = value;
                    break;
                case "padding-right":
                    cssBox.PaddingRight = value;
                    break;
                case "padding-top":
                    cssBox.PaddingTop = value;
                    break;
                case "left":
                    cssBox.Left = value;
                    break;
                case "top":
                    cssBox.Top = value;
                    break;
                case "width":
                    cssBox.Width = value;
                    break;
                case "height":
                    cssBox.Height = value;
                    break;
                case "background-color":
                    cssBox.BackgroundColor = value;
                    break;
                case "background-image":
                    cssBox.BackgroundImage = value;
                    break;
                case "background-repeat":
                    cssBox.BackgroundRepeat = value;
                    break;
                case "background-gradient":
                    cssBox.BackgroundGradient = value;
                    break;
                case "background-gradient-angle":
                    cssBox.BackgroundGradientAngle = value;
                    break;
                case "color":
                    cssBox.Color = value;
                    break;
                case "display":
                    cssBox.Display = value;
                    break;
                case "direction":
                    cssBox.Direction = value;
                    break;
                case "empty-cells":
                    cssBox.EmptyCells = value;
                    break;
                case "float":
                    cssBox.Float = value;
                    break;
                case "position":
                    cssBox.Position = value;
                    break;
                case "line-height":
                    cssBox.LineHeight = value;
                    break;
                case "vertical-align":
                    cssBox.VerticalAlign = value;
                    break;
                case "text-indent":
                    cssBox.TextIndent = value;
                    break;
                case "text-align":
                    cssBox.TextAlign = value;
                    break;
                case "text-decoration":
                    cssBox.TextDecoration = value;
                    break;
                case "white-space":
                    cssBox.WhiteSpace = value;
                    break;
                case "word-spacing":
                    cssBox.WordSpacing = value;
                    break;
                case "font-family":
                    cssBox.FontFamily = value;
                    break;
                case "font-size":
                    cssBox.FontSize = value;
                    break;
                case "font-style":
                    cssBox.FontStyle = value;
                    break;
                case "font-variant":
                    cssBox.FontVariant = value;
                    break;
                case "font-weight":
                    cssBox.FontWeight = value;
                    break;
                case "list-style":
                    cssBox.ListStyle = value;
                    break;
                case "list-style-position":
                    cssBox.ListStylePosition = value;
                    break;
                case "list-style-image":
                    cssBox.ListStyleImage = value;
                    break;
                case "list-style-type":
                    cssBox.ListStyleType = value;
                    break;
            }
        }


        #region Private methods

        /// <summary>
        /// Measure regions for specific font empty space size.
        /// </summary>
        /// <param name="g">the graphics instance to use if calculation required</param>
        /// <param name="font">the font to calculate for</param>
        /// <returns>the calculated regions</returns>
        private static float MeasureWhitespace(Graphics g, Font font)
        {
            WhitespaceData width;
            if (!_whitespaceCache.TryGetValue(font, out width))
            {
                var regions1 = g.MeasureCharacterRanges("..", font, new RectangleF(0, 0, Single.MaxValue, Single.MaxValue), _sf)[0].GetBounds(g);
                var regions2 = g.MeasureCharacterRanges(".", font, new RectangleF(0, 0, Single.MaxValue, Single.MaxValue), _sf)[0].GetBounds(g);

                var regions = g.MeasureCharacterRanges(" .", font, new RectangleF(0, 0, Single.MaxValue, Single.MaxValue), _sf)[0].GetBounds(g);

                var endWordMargin = regions2.Width - regions1.Width;
                width = new WhitespaceData((regions.Width - endWordMargin)*0.8f, endWordMargin);

                _whitespaceCache[font] = width;
            }
            return width.Whitespace;
        }

        #endregion


        #region Private struct

        /// <summary>
        /// Used to store the calculated whitespace margins
        /// </summary>
        private struct WhitespaceData
        {
            public WhitespaceData(float whitespace, float wordEndWhitespace) : this()
            {
                Whitespace = whitespace;
                WordEndWhitespace = wordEndWhitespace;
            }

            /// <summary>
            /// the margin of a single whitespace
            /// </summary>
            public float Whitespace { get; private set; }
            
            /// <summary>
            /// the margin added to the end of a word
            /// </summary>
            public float WordEndWhitespace { get; private set; }
        }

        #endregion
    }
}
