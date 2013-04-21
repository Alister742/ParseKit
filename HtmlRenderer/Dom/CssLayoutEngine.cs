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
using HtmlRenderer.Entities;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{
    /// <summary>
    /// Helps on CSS Layout.
    /// </summary>
    internal static class CssLayoutEngine
    {
        /// <summary>
        /// Creates line boxes for the specified blockbox
        /// </summary>
        /// <param name="g"></param>
        /// <param name="blockBox"></param>
        public static void CreateLineBoxes(Graphics g, CssBox blockBox)
        {
            blockBox.LineBoxes.Clear();

            float maxRight = blockBox.ActualRight - blockBox.ActualPaddingRight - blockBox.ActualBorderRightWidth;

            //Get the start x and y of the blockBox
            float startx = blockBox.Location.X + blockBox.ActualPaddingLeft - 0 + blockBox.ActualBorderLeftWidth;
            float starty = blockBox.Location.Y + blockBox.ActualPaddingTop - 0 + blockBox.ActualBorderTopWidth;
            float curx = startx + blockBox.ActualTextIndent;
            float cury = starty;

            //Reminds the maximum bottom reached
            float maxBottom = starty;

            //First line box
            CssLineBox line = new CssLineBox(blockBox);

            //Flow words and boxes
            FlowBox(g, blockBox, blockBox, maxRight, 0, startx,ref line, ref curx, ref cury, ref maxBottom);

            //Gets the rectangles foreach linebox
            foreach (var linebox in blockBox.LineBoxes)
            {
                BubbleRectangles(blockBox, linebox);
                linebox.AssignRectanglesToBoxes();
                ApplyAlignment(g, linebox);
                if (blockBox.Direction == CssConstants.Rtl) 
                    ApplyRightToLeft(linebox);
            }

            blockBox.ActualBottom = maxBottom + blockBox.ActualPaddingBottom + blockBox.ActualBorderBottomWidth;
        }

        /// <summary>
        /// Applies special vertical alignment for table-cells
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cell"></param>
        public static void ApplyCellVerticalAlignment(Graphics g, CssBox cell)
        {
            if (cell.VerticalAlign == CssConstants.Top || cell.VerticalAlign == CssConstants.Baseline) return;

            float cellbot = cell.ClientBottom;
            float bottom = cell.GetMaximumBottom(cell, 0f);
            float dist = 0f;

            if (cell.VerticalAlign == CssConstants.Bottom)
            {
                dist = cellbot - bottom;
            }
            else if (cell.VerticalAlign == CssConstants.Middle)
            {
                dist = (cellbot - bottom) / 2;
            }

            foreach (CssBox b in cell.Boxes)
            {
                b.OffsetTop(dist);
            }

            //float top = cell.ClientTop;
            //float bottom = cell.ClientBottom;
            //bool middle = cell.VerticalAlign == CssConstants.Middle;

            //foreach (LineBox line in cell.LineBoxes)
            //{
            //    for (int i = 0; i < line.RelatedBoxes.Count; i++)
            //    {

            //        float diff = bottom - line.RelatedBoxes[i].Rectangles[line].Bottom;
            //        if (middle) diff /= 2f;
            //        RectangleF r = line.RelatedBoxes[i].Rectangles[line];
            //        line.RelatedBoxes[i].Rectangles[line] = new RectangleF(r.X, r.Y + diff, r.Width, r.Height);

            //    }

            //    foreach (BoxWord word in line.Words)
            //    {
            //        float gap = word.Top - top;
            //        word.Top = bottom - gap - word.Height;
            //    }
            //}
        }


        #region Private methods

        /// <summary>
        /// Recursively flows the content of the box using the inline model
        /// </summary>
        /// <param name="g">Device Info</param>
        /// <param name="blockbox">Blockbox that contains the text flow</param>
        /// <param name="box">Current box to flow its content</param>
        /// <param name="maxright">Maximum reached right</param>
        /// <param name="linespacing">Space to use between rows of text</param>
        /// <param name="startx">x starting coordinate for when breaking lines of text</param>
        /// <param name="line">Current linebox being used</param>
        /// <param name="curx">Current x coordinate that will be the left of the next word</param>
        /// <param name="cury">Current y coordinate that will be the top of the next word</param>
        /// <param name="maxbottom">Maximum bottom reached so far</param>
        private static void FlowBox(Graphics g, CssBox blockbox, CssBox box, float maxright, float linespacing, float startx, ref CssLineBox line, ref float curx, ref float cury, ref float maxbottom)
        {
            var startY = cury;
            box.FirstHostingLineBox = line;

            foreach (CssBox b in box.Boxes)
            {
                float leftspacing = b.ActualMarginLeft + b.ActualBorderLeftWidth + b.ActualPaddingLeft;
                float rightspacing = b.ActualMarginRight + b.ActualBorderRightWidth + b.ActualPaddingRight;

                b.RectanglesReset();
                b.MeasureWordsSize(g);

                curx += leftspacing;

                if (b.Words.Count > 0)
                {
                    // fix word space for first word in inline tag
                    if (!b.Words[0].IsImage && b.Words[0].HasSpaceBefore && b.Display == CssConstants.Inline)
                    {
                        var sib = DomUtils.GetPreviousContainingBlockSibling(b);
                        if (sib != null && sib.Display == CssConstants.Inline)
                            curx += b.ActualWordSpacing;
                    }

                    foreach (var word in b.Words)
                    {
                        if (maxbottom - cury < box.ActualLineHeight)
                            maxbottom += box.ActualLineHeight - (maxbottom - cury);

                        if ((b.WhiteSpace != CssConstants.Nowrap && curx + word.Width + rightspacing > maxright) || word.IsLineBreak)
                        {
                            curx = startx;
                            cury = maxbottom + linespacing;

                            line = new CssLineBox(blockbox);

                            if (word.IsImage || word.Equals(b.FirstWord))
                            {
                                curx += leftspacing;
                            }
                        }

                        // atodo: why?
                        line.ReportExistanceOf(word);

                        word.Left = curx; // -word.LastMeasureOffset.X + 1;
                        word.Top = cury; // - word.LastMeasureOffset.Y;

                        curx = word.Right + (word.IsImage ? b.ActualWordSpacing : 0); // +word.SpacesAfterWidth;

                        maxbottom = Math.Max(maxbottom, word.Bottom); //+ (word.IsImage ? topspacing + bottomspacing : 0));
                    }

                    // fix word space for last word that is right before inline tag
                    var lastWord = b.Words[b.Words.Count-1];
                    if (!lastWord.IsImage && !lastWord.HasSpaceAfter)
                    {
                        var next = DomUtils.GetNextSibling(b);
                        if (next == null || next.Display == CssConstants.Inline)
                            curx -= b.ActualWordSpacing + CssUtils.GetWordEndWhitespace(b.ActualFont);
                    }
                }
                else
                {
                    FlowBox(g, blockbox, b, maxright, linespacing, startx, ref line, ref curx, ref cury, ref maxbottom);
                }

                curx += rightspacing;
            }

            // handle height setting
            if(maxbottom - startY < box.ActualHeight)
            {
                maxbottom += box.ActualHeight - (maxbottom - startY);
            }

            // handle box that is only a whitespace
            if (box.Text == " " && !box.IsImage && box.IsInline && box.Boxes.Count == 0 && box.Words.Count == 0)
            {
                curx += box.ActualWordSpacing;
            }

            box.LastHostingLineBox = line;
        }

        /// <summary>
        /// Recursively creates the rectangles of the blockBox, by bubbling from deep to outside of the boxes 
        /// in the rectangle structure
        /// </summary>
        private static void BubbleRectangles(CssBox box, CssLineBox line)
        {
            if (box.Words.Count > 0)
            {
                float x = float.MaxValue, y = float.MaxValue, r = float.MinValue, b = float.MinValue;
                List<CssBoxWord> words = line.WordsOf(box);

                if (words.Count > 0)
                {
                    foreach (CssBoxWord word in words)
                    {
                        x = Math.Min(x, word.Left); // - word.SpacesBeforeWidth);
                        r = Math.Max(r, word.Right); // + word.SpacesAfterWidth);
                        y = Math.Min(y, word.Top);
                        b = Math.Max(b, word.Bottom);
                    }
                    line.UpdateRectangle(box, x, y, r, b);
                }
            }
            else
            {
                foreach (CssBox b in box.Boxes)
                {
                    BubbleRectangles(b, line);
                }
            }
        }

        /// <summary>
        /// Applies vertical and horizontal alignment to words in lineboxes
        /// </summary>
        /// <param name="g"></param>
        /// <param name="lineBox"></param>
        private static void ApplyAlignment(Graphics g, CssLineBox lineBox)
        {

            #region Horizontal alignment

            switch (lineBox.OwnerBox.TextAlign)
            {
                case CssConstants.Right:
                    ApplyRightAlignment(g, lineBox);
                    break;
                case CssConstants.Center:
                    ApplyCenterAlignment(g, lineBox);
                    break;
                case CssConstants.Justify:
                    ApplyJustifyAlignment(g, lineBox);
                    break;
                default:
                    ApplyLeftAlignment(g, lineBox);
                    break;
            }

            #endregion

            ApplyVerticalAlignment(g, lineBox);
        }

        /// <summary>
        /// Applies right to left direction to words
        /// </summary>
        /// <param name="line"></param>
        private static void ApplyRightToLeft(CssLineBox line)
        {
            float left = line.OwnerBox.ClientLeft;
            float right = line.OwnerBox.ClientRight;

            foreach (CssBoxWord word in line.Words)
            {
                float diff = word.Left - left;
                float wright = right - diff;
                word.Left = wright - word.Width;
            }
        }

        /// <summary>
        /// Applies vertical alignment to the linebox
        /// </summary>
        /// <param name="g"></param>
        /// <param name="lineBox"></param>
        private static void ApplyVerticalAlignment(Graphics g, CssLineBox lineBox)
        {
            float baseline = float.MinValue;
            foreach (var box in lineBox.Rectangles.Keys)
            {
                baseline = Math.Max(baseline, lineBox.Rectangles[box].Top);
            }

            var boxes = new List<CssBox>(lineBox.Rectangles.Keys);
            foreach (CssBox box in boxes)
            {
                //Important notes on http://www.w3.org/TR/CSS21/tables.html#height-layout
                switch (box.VerticalAlign)
                {
                    case CssConstants.Sub:
                        lineBox.SetBaseLine(g, box, baseline + lineBox.Rectangles[box].Height*.2f);
                        break;
                    case CssConstants.Super:
                        lineBox.SetBaseLine(g, box, baseline - lineBox.Rectangles[box].Height*.2f);
                        break;
                    case CssConstants.TextTop:

                        break;
                    case CssConstants.TextBottom:

                        break;
                    case CssConstants.Top:

                        break;
                    case CssConstants.Bottom:

                        break;
                    case CssConstants.Middle:

                        break;
                    default:
                        //case: baseline
                        lineBox.SetBaseLine(g, box, baseline);
                        break;
                }
            }
        }

        /// <summary>
        /// Applies centered alignment to the text on the linebox
        /// </summary>
        /// <param name="g"></param>
        /// <param name="lineBox"></param>
        private static void ApplyJustifyAlignment(Graphics g, CssLineBox lineBox)
        {
            if (lineBox.Equals(lineBox.OwnerBox.LineBoxes[lineBox.OwnerBox.LineBoxes.Count - 1])) return;

            float indent = lineBox.Equals(lineBox.OwnerBox.LineBoxes[0]) ? lineBox.OwnerBox.ActualTextIndent : 0f;
            float textSum = 0f;
            float words = 0f;
            float availWidth = lineBox.OwnerBox.ClientRectangle.Width - indent;

            #region Gather text sum

            foreach (CssBoxWord w in lineBox.Words)
            {
                textSum += w.Width;
                words += 1f;
            }

            #endregion

            if (words <= 0f) return; //Avoid Zero division
            float spacing = (availWidth - textSum)/words; //Spacing that will be used
            float curx = lineBox.OwnerBox.ClientLeft + indent;

            foreach (CssBoxWord word in lineBox.Words)
            {
                word.Left = curx;
                curx = word.Right + spacing;

                if (word == lineBox.Words[lineBox.Words.Count - 1])
                {
                    word.Left = lineBox.OwnerBox.ClientRight - word.Width;
                }

                //TODO: Background rectangles are being deactivated when justifying text.
            }



        }

        /// <summary>
        /// Applies centered alignment to the text on the linebox
        /// </summary>
        /// <param name="g"></param>
        /// <param name="line"></param>
        private static void ApplyCenterAlignment(Graphics g, CssLineBox line)
        {
            if (line.Words.Count == 0) return;

            CssBoxWord lastWord = line.Words[line.Words.Count - 1];
            float right = line.OwnerBox.ActualRight - line.OwnerBox.ActualPaddingRight - line.OwnerBox.ActualBorderRightWidth;
            float diff = right - lastWord.Right - lastWord.LastMeasureOffset.X - lastWord.OwnerBox.ActualBorderRightWidth - lastWord.OwnerBox.ActualPaddingRight;
            diff /= 2;

            if (diff <= 0) return;

            foreach (CssBoxWord word in line.Words)
            {
                word.Left += diff;
            }

            foreach (CssBox b in line.Rectangles.Keys)
            {
                RectangleF r = b.Rectangles[line];
                b.Rectangles[line] = new RectangleF(r.X + diff, r.Y, r.Width, r.Height);
            }
        }

        /// <summary>
        /// Applies right alignment to the text on the linebox
        /// </summary>
        /// <param name="g"></param>
        /// <param name="line"></param>
        private static void ApplyRightAlignment(Graphics g, CssLineBox line)
        {
            if (line.Words.Count == 0) return;


            CssBoxWord lastWord = line.Words[line.Words.Count - 1];
            float right = line.OwnerBox.ActualRight - line.OwnerBox.ActualPaddingRight - line.OwnerBox.ActualBorderRightWidth;
            float diff = right - lastWord.Right - lastWord.LastMeasureOffset.X - lastWord.OwnerBox.ActualBorderRightWidth - lastWord.OwnerBox.ActualPaddingRight;


            if (diff <= 0) return;

            //if (line.OwnerBox.Direction == CssConstants.Rtl)
            //{

            //}

            foreach (CssBoxWord word in line.Words)
            {
                word.Left += diff;
            }

            foreach (CssBox b in line.Rectangles.Keys)
            {
                RectangleF r = b.Rectangles[line];
                b.Rectangles[line] = new RectangleF(r.X + diff, r.Y, r.Width, r.Height);
            }
        }

        /// <summary>
        /// Simplest alignment, just arrange words.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="line"></param>
        private static void ApplyLeftAlignment(Graphics g, CssLineBox line)
        {
            //No alignment needed.

            //foreach (LineBoxRectangle r in line.Rectangles)
            //{
            //    float curx = r.Left + (r.Index == 0 ? r.OwnerBox.ActualPaddingLeft + r.OwnerBox.ActualBorderLeftWidth / 2 : 0);

            //    if (r.SpaceBefore) curx += r.OwnerBox.ActualWordSpacing;

            //    foreach (BoxWord word in r.Words)
            //    {
            //        word.Left = curx;
            //        word.Top = r.Top;// +r.OwnerBox.ActualPaddingTop + r.OwnerBox.ActualBorderTopWidth / 2;

            //        curx = word.Right + r.OwnerBox.ActualWordSpacing;
            //    }
            //}
        }

        #endregion
    }
}