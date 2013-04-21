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
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using HtmlRenderer.Dom;
using HtmlRenderer.Entities;

namespace HtmlRenderer.Parse
{
    /// <summary>
    /// Parse CSS properties values like numbers, urls, etc.
    /// </summary>
    internal static class CssValueParser
    {
        /// <summary>
        /// Evals a number and returns it. If number is a percentage, it will be multiplied by <see cref="hundredPercent"/>
        /// </summary>
        /// <param name="number">Number to be parsed</param>
        /// <param name="hundredPercent">Number that represents the 100% if parsed number is a percentage</param>
        /// <returns>Parsed number. Zero if error while parsing.</returns>
        public static float ParseNumber(string number, float hundredPercent)
        {
            if (string.IsNullOrEmpty(number))
            {
                return 0f;
            }

            string toParse = number;
            bool isPercent = number.EndsWith("%");
            float result;

            if (isPercent) toParse = number.Substring(0, number.Length - 1);

            if (!float.TryParse(toParse, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out result))
            {
                return 0f;
            }

            if (isPercent)
            {
                result = (result / 100f) * hundredPercent;
            }

            return result;
        }

        /// <summary>
        /// Parses a length. Lengths are followed by an unit identifier (e.g. 10px, 3.1em)
        /// </summary>
        /// <param name="length">Specified length</param>
        /// <param name="hundredPercent">Equivalent to 100 percent when length is percentage</param>
        /// <param name="fontAdjust">if the length is in pixels and the length is font related it needs to use 72/96 factor</param>
        /// <param name="box"></param>
        /// <returns>the parsed length value with adjustments</returns>
        public static float ParseLength(string length, float hundredPercent, CssBoxProperties box, bool fontAdjust = false)
        {
            return ParseLength(length, hundredPercent, box.GetEmHeight(), fontAdjust, false);
        }

        /// <summary>
        /// Parses a length. Lengths are followed by an unit identifier (e.g. 10px, 3.1em)
        /// </summary>
        /// <param name="length">Specified length</param>
        /// <param name="hundredPercent">Equivalent to 100 percent when length is percentage</param>
        /// <param name="emFactor"></param>
        /// <param name="fontAdjust">if the length is in pixels and the length is font related it needs to use 72/96 factor</param>
        /// <param name="returnPoints">Allows the return float to be in points. If false, result will be pixels</param>
        /// <returns>the parsed length value with adjustments</returns>
        public static float ParseLength(string length, float hundredPercent, float emFactor, bool fontAdjust, bool returnPoints)
        {
            //Return zero if no length specified, zero specified
            if (string.IsNullOrEmpty(length) || length == "0") return 0f;

            //If percentage, use ParseNumber
            if (length.EndsWith("%")) return ParseNumber(length, hundredPercent);

            //If no units, return zero
            if (length.Length < 3) return 0f;

            //Get units of the length
            string unit = length.Substring(length.Length - 2, 2);

            //Factor will depend on the unit
            float factor;

            //Number of the length
            string number = length.Substring(0, length.Length - 2);

            //TODO: Units behave different in paper and in screen!
            switch (unit)
            {
                case CssConstants.Em:
                    factor = emFactor;
                    break;
                case CssConstants.Ex:
                    factor = emFactor/2;
                    break;
                case CssConstants.Px:
                    factor = fontAdjust ? 72f / 96f : 1f; //atodo: check support for hi dpi
                    break;
                case CssConstants.Mm:
                    factor = 3f; //3 pixels per millimeter
                    break;
                case CssConstants.Cm:
                    factor = 37f; //37 pixels per centimeter
                    break;
                case CssConstants.In:
                    factor = 96f; //96 pixels per inch
                    break;
                case CssConstants.Pt:
                    factor = 96f / 72f; // 1 point = 1/72 of inch

                    if (returnPoints)
                    {
                        return ParseNumber(number, hundredPercent);
                    }

                    break;
                case CssConstants.Pc:
                    factor = 96f / 72f * 12f; // 1 pica = 12 points
                    break;
                default:
                    factor = 0f;
                    break;
            }

            

            return factor * ParseNumber(number, hundredPercent);
        }

        /// <summary>
        /// Parses a color value in CSS style; e.g. #ff0000, red, rgb(255,0,0), rgb(100%, 0, 0)
        /// </summary>
        /// <param name="colorValue">Specified color value; e.g. #ff0000, red, rgb(255,0,0), rgb(100%, 0, 0)</param>
        /// <returns>System.Drawing.Color value</returns>
        public static Color GetActualColor(string colorValue)
        {
            int r;
            int g;
            int b;
            Color onError = Color.Empty;

            if (string.IsNullOrEmpty(colorValue)) return onError;

            colorValue = colorValue.ToLower().Trim();

            if (colorValue.StartsWith("#"))
            {
                string hex = colorValue.Substring(1);

                if (hex.Length == 6)
                {
                    r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                    g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                    b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                }
                else if (hex.Length == 3)
                {
                    r = int.Parse(new String(hex.Substring(0, 1)[0], 2), System.Globalization.NumberStyles.HexNumber);
                    g = int.Parse(new String(hex.Substring(1, 1)[0], 2), System.Globalization.NumberStyles.HexNumber);
                    b = int.Parse(new String(hex.Substring(2, 1)[0], 2), System.Globalization.NumberStyles.HexNumber);
                }
                else
                {
                    return onError;
                } 
            }
            else if (colorValue.StartsWith("rgb(") && colorValue.EndsWith(")"))
            {
                string rgb = colorValue.Substring(4, colorValue.Length - 5);
                string[] chunks = rgb.Split(',');

                if (chunks.Length == 3)
                {
                    unchecked
                    {
                        r = Convert.ToInt32(ParseNumber(chunks[0].Trim(), 255f));
                        g = Convert.ToInt32(ParseNumber(chunks[1].Trim(), 255f));
                        b = Convert.ToInt32(ParseNumber(chunks[2].Trim(), 255f)); 
                    }
                }
                else
                {
                    return onError;
                }
            }
            else
            {
                string hex = string.Empty;

                switch (colorValue)
                {
                    case CssConstants.Maroon:
                        hex = "#800000"; break;
                    case CssConstants.Red:
                        hex = "#ff0000"; break;
                    case CssConstants.Orange:
                        hex = "#ffA500"; break;
                    case CssConstants.Olive:
                        hex = "#808000"; break;
                    case CssConstants.Purple:
                        hex = "#800080"; break;
                    case CssConstants.Fuchsia:
                        hex = "#ff00ff"; break;
                    case CssConstants.White:
                        hex = "#ffffff"; break;
                    case CssConstants.Lime:
                        hex = "#00ff00"; break;
                    case CssConstants.Green:
                        hex = "#008000"; break;
                    case CssConstants.Navy:
                        hex = "#000080"; break;
                    case CssConstants.Blue:
                        hex = "#0000ff"; break;
                    case CssConstants.Aqua:
                        hex = "#00ffff"; break;
                    case CssConstants.Teal:
                        hex = "#008080"; break;
                    case CssConstants.Black:
                        hex = "#000000"; break;
                    case CssConstants.Silver:
                        hex = "#c0c0c0"; break;
                    case CssConstants.Gray:
                        hex = "#808080"; break;
                    case CssConstants.Yellow:
                        hex = "#FFFF00"; break;
                }

                if (string.IsNullOrEmpty(hex))
                {
                    return onError;
                }
                else
                {
                    Color c = GetActualColor(hex);
                    r = c.R;
                    g = c.G;
                    b = c.B;
                }
            }

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Parses a border value in CSS style; e.g. 1px, 1, thin, thick, medium
        /// </summary>
        /// <param name="borderValue"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float GetActualBorderWidth(string borderValue, CssBoxProperties b)
        {
            if (string.IsNullOrEmpty(borderValue))
            {
                return GetActualBorderWidth(CssConstants.Medium, b);
            }

            switch (borderValue)
            {
                case CssConstants.Thin:
                    return 1f;
                case CssConstants.Medium:
                    return 2f;
                case CssConstants.Thick:
                    return 4f;
                default:
                    return Math.Abs(ParseLength(borderValue, 1, b));
            }
        }

         /// <summary>
        /// Split the value by spaces; e.g. Useful in values like 'padding:5 4 3 inherit'
        /// </summary>
        /// <param name="value">Value to be splitted</param>
        /// <returns>Splitted and trimmed values</returns>
        public static string[] SplitValues(string value)
        {
            return SplitValues(value, ' ');
        }

        /// <summary>
        /// Split the value by the specified separator; e.g. Useful in values like 'padding:5 4 3 inherit'
        /// </summary>
        /// <param name="value">Value to be splitted</param>
        /// <param name="separator"> </param>
        /// <returns>Splitted and trimmed values</returns>
        public static string[] SplitValues(string value, char separator)
        {
            //TODO: CRITICAL! Don't split values on parenthesis (like rgb(0, 0, 0)) or quotes ("strings")

            if (!string.IsNullOrEmpty(value))
            {
                string[] values = value.Split(separator);
                List<string> result = new List<string>();

                foreach (string t in values)
                {
                    string val = t.Trim();

                    if (!string.IsNullOrEmpty(val))
                    {
                        result.Add(val);
                    }
                }

                return result.ToArray();
            }

            return new string[] { };
        }

        /// <summary>
        /// Gets the image of the specified path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bridge">used to resolve external references in html code (property, method calls)</param>
        /// <returns>the image object (can be error image if failed)</returns>
        public static Image GetImage(string path, object bridge)
        {
            object source = DetectSource(path,bridge);
            Uri uri = source as Uri;

            FileInfo finfo = source as FileInfo;
            PropertyInfo prop = source as PropertyInfo;
            MethodInfo method = source as MethodInfo;

            try
            {
                if (finfo != null)
                {
                    return !finfo.Exists ? null : Image.FromFile(finfo.FullName);
                }
                else if (prop != null)
                {
                    return !prop.PropertyType.IsSubclassOf(typeof (Image)) && prop.PropertyType != typeof (Image) ? null : prop.GetValue(null, null) as Image;
                }
                else if (method != null)
                {
                    return !method.ReturnType.IsSubclassOf(typeof (Image)) ? null : method.Invoke(null, null) as Image;
                }
                else if (source is string)
                {
                    string[] s = ((string)source).Substring(((string)source).IndexOf(':') + 1).Split(new[] { ',' }, 2);
                    if (s.Length == 2)
                    {
                        var mediaTypeParametersAndBase64 = s[0].Split(new[] { ';' }).Select(part => part.Trim().ToLower()).ToList();
                        if (mediaTypeParametersAndBase64.Count(part => part.StartsWith("image/")) != 0)
                        {
                            byte[] imageData = mediaTypeParametersAndBase64.Count(part => part == "base64") != 0 ? Convert.FromBase64String(s[1].Trim()) : new System.Text.UTF8Encoding().GetBytes(Uri.UnescapeDataString(s[1].Trim()));
                            return Image.FromStream(new MemoryStream(imageData));
                        }
                    }

                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return new Bitmap(20, 20); //aTODO: Return error image
            }
        }

        /// <summary>
        /// Gets the content of the stylesheet specified in the path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bridge">used to resolve external references in html code (property, method calls)</param>
        /// <returns>the stylesheet string</returns>
        public static string GetStyleSheet(string path, object bridge)
        {
            object source = DetectSource(path, bridge);

            FileInfo finfo = source as FileInfo;
            PropertyInfo prop = source as PropertyInfo;
            MethodInfo method = source as MethodInfo;

            try
            {
                if (finfo != null)
                {
                    if (!finfo.Exists) return null;

                    StreamReader sr = new StreamReader(finfo.FullName);
                    string result = sr.ReadToEnd();
                    sr.Dispose();

                    return result;
                }
                else if (prop != null)
                {
                    return prop.PropertyType != typeof (string) ? null : prop.GetValue(bridge, null) as string;
                }
                else if (method != null)
                {
                    return method.ReturnType != typeof (string) ? null : method.Invoke(bridge, null) as string;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Executes the desired action when the user clicks a link
        /// </summary>
        /// <param name="href"></param>
        /// <param name="bridge">used to resolve external references in html code (property, method calls)</param>
        public static void GoLink(string href, object bridge)
        {
            object source = DetectSource(href,bridge);

            FileInfo finfo = source as FileInfo;
            MethodInfo method = source as MethodInfo;
            Uri uri = source as Uri;

            if (finfo != null || uri != null)
            {
                var nfo = new ProcessStartInfo(href);
                nfo.UseShellExecute = true;
                Process.Start(nfo);
            }
            else if (method != null)
            {
                method.Invoke(null, null);
            }
            else
            {
                //Nothing to do.
            }
        }


        #region Private methods

        /// <summary>
        /// Returns the object specific to the path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bridge">used to resolve external references in html code (property, method calls)</param>
        /// <returns>One of the following possible objects: FileInfo, MethodInfo, PropertyInfo</returns>
        private static object DetectSource(string path, object bridge)
        {
            if (path.StartsWith("method:", StringComparison.CurrentCultureIgnoreCase))
            {
                if(bridge != null)
                {
                    var method = bridge.GetType().GetMethod(path.Substring(7), BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
                    return method.GetParameters().Length > 0 ? null : method;
                }
                else
                {
                    return null;
                }
            }
            else if (path.StartsWith("property:", StringComparison.CurrentCultureIgnoreCase))
            {
                return bridge != null ? bridge.GetType().GetProperty(path.Substring(9), BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public) : null;
            }
            else if (path.StartsWith("data:image", StringComparison.CurrentCultureIgnoreCase))
            {
                return path; 
            }
            else if (Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute))
            {
                //return new Uri(path);
                return new Uri("http://google.com/");
            }
            else
            {
                return new FileInfo(path);
            }
        }

        #endregion
    }
}