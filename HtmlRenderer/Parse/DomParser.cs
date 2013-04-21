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
using HtmlRenderer.Dom;
using HtmlRenderer.Entities;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Parse
{
    /// <summary>
    /// Handle css DOM tree generation from raw html and stylesheet.
    /// </summary>
    internal static class DomParser
    {
        /// <summary>
        /// Generate css tree by parsing the given html and applying the given css style data on it.
        /// </summary>
        /// <param name="html">the html to parse</param>
        /// <param name="cssData">the css data to use</param>
        /// <param name="bridge">used to resolve external style references in html code</param>
        /// <returns>the root of the generated tree</returns>
        public static CssBox GenerateCssTree(string html, ref CssData cssData, object bridge)
        {
            var root = HtmlParser.ParseDocument(html);
            if (root != null)
            {
                bool cssDataChanged = false;
                CascadeStyles(root, bridge, ref cssData, ref cssDataChanged);

                CorrectLineBreaksBlocks(root);

                CorrectBlockInsideInline(root);

                CorrectInlineBoxesParent(root);
            }
            return root;
        }


        #region Private methods

        /// <summary>
        /// Applies style to all boxes in the tree.<br/>
        /// If the html tag has style defined for each apply that style to the css box of the tag.<br/>
        /// If the html tag has "class" attribute and the class name has style defined apply that style on the tag css box.<br/>
        /// If the html tag has "style" attribute parse it and apply the parsed style on the tag css box.<br/>
        /// If the html tag is "style" tag parse it content and add to the css data for all future tags parsing.<br/>
        /// If the html tag is "link" that point to style data parse it content and add to the css data for all future tags parsing.<br/>
        /// </summary>
        /// <param name="box"></param>
        /// <param name="bridge"> </param>
        /// <param name="cssData"> </param>
        /// <param name="cssDataChanged">check if the css data has been modified by the handled html not to change the base css data</param>
        private static void CascadeStyles(CssBox box, object bridge, ref CssData cssData, ref bool cssDataChanged)
        {
            box.InheritStyle();

            if (box.HtmlTag != null)
            {
                // try assign style using the html element tag
                AssignCssBlocks(box, cssData, box.HtmlTag.Name);

                // try assign style using the "class" attribute of the html element
                if (box.HtmlTag.HasAttribute("class"))
                {
                    AssignCssBlocks(box, cssData, "." + box.HtmlTag.Attributes["class"]);
                    AssignCssBlocks(box, cssData, box.HtmlTag.Name + "." + box.HtmlTag.Attributes["class"]);
                }

                // try assign style using the "id" attribute of the html element
                if (box.HtmlTag.HasAttribute("id"))
                {
                    AssignCssBlocks(box, cssData, "#" + box.HtmlTag.Attributes["id"]);
                }

                HtmlParser.TranslateAttributes(box.HtmlTag, box);

                // Check for the style="" attribute
                if (box.HtmlTag.HasAttribute("style"))
                {
                    var block = CssParser.ParseCssBlockImp(box.HtmlTag.Name, box.HtmlTag.Attributes["style"]);
                    AssignCssBlock(box, block);
                }

                // Check for the <style> tag
                if (box.HtmlTag.Name.Equals("style", StringComparison.CurrentCultureIgnoreCase) && box.Boxes.Count == 1)
                {
                    CloneCssData(ref cssData, ref cssDataChanged);
                    CssParser.ParseStyleSheet(cssData, box.Boxes[0].Text);
                }

                // Check for the <link rel=stylesheet> tag
                if (box.HtmlTag.Name.Equals("link", StringComparison.CurrentCultureIgnoreCase) &&
                    box.GetAttribute("rel", string.Empty).Equals("stylesheet", StringComparison.CurrentCultureIgnoreCase))
                {
                    CloneCssData(ref cssData, ref cssDataChanged);
                    var styleSheet = CssValueParser.GetStyleSheet(box.GetAttribute("href", string.Empty), bridge);
                    CssParser.ParseStyleSheet(cssData, styleSheet);
                }
            }

            foreach (var childBox in box.Boxes)
            {
                CascadeStyles(childBox, bridge, ref cssData, ref cssDataChanged);
            }
        }

        /// <summary>
        /// Asigns the given css style blocks to the given css box checking if matching.
        /// </summary>
        /// <param name="box">the css box to assign css to</param>
        /// <param name="cssData">the css data to use to get the matching css blocks</param>
        /// <param name="className">the class selector to search for css blocks</param>
        private static void AssignCssBlocks(CssBox box, CssData cssData, string className)
        {
            var blocks = cssData.GetCssBlock(className);
            foreach (var block in blocks)
            {
                if (IsBlockAssignableToBox(box, block))
                {
                    AssignCssBlock(box, block);
                }
            }
        }

        /// <summary>
        /// Check if the given css block is assignable to the given css box.<br/>
        /// the block is assignable if it has no hierarchial selectors or if the hierarchy matches.<br/>
        /// </summary>
        /// <param name="box">the box to check assign to</param>
        /// <param name="block">the block to check assign of</param>
        /// <returns>true - the block is assignable to the box, false - otherwise</returns>
        private static bool IsBlockAssignableToBox(CssBox box, CssBlock block)
        {
            if (block.Selectors != null)
            {
                foreach (var selector in block.Selectors)
                {
                    bool matched = false;
                    while (!matched)
                    {
                        box = box.ParentBox;
                        while (box != null && box.HtmlTag == null)
                            box = box.ParentBox;

                        if (box == null)
                            return false;

                        if (box.HtmlTag.Name == selector.Class)
                            matched = true;

                        if (!matched && box.HtmlTag.HasAttribute("class"))
                        {
                            var className = box.HtmlTag.Attributes["class"];
                            if (selector.Class == "." + className || selector.Class == box.HtmlTag.Name + "." + className)
                                matched = true;
                        }

                        if (!matched && box.HtmlTag.HasAttribute("id"))
                        {
                            var id = box.HtmlTag.Attributes["id"];
                            if (selector.Class == "#" + id)
                                matched = true;
                        }

                        if (!matched && selector.DirectParent)
                            return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Asigns the given css style block properties to the given css box.
        /// </summary>
        /// <param name="box">the css box to assign css to</param>
        /// <param name="block">the css block to assign</param>
        private static void AssignCssBlock(CssBox box, CssBlock block)
        {
            foreach (var prop in block.Properties)
            {
                var value = prop.Value;
                if (prop.Value == CssConstants.Inherit && box.ParentBox != null)
                {
                    value = CssUtils.GetPropertyValue(box.ParentBox, prop.Key);
                }
                CssUtils.SetPropertyValue(box, prop.Key, value);
            }
        }

        /// <summary>
        /// Clone css data if it has not already been cloned.<br/>
        /// Used to preserve the base css data used when changed by style inside html.
        /// </summary>
        private static void CloneCssData(ref CssData cssData, ref bool cssDataChanged)
        {
            if (!cssDataChanged)
            {
                cssDataChanged = true;
                cssData = cssData.Clone();
            }
        }

        /// <summary>
        /// Correct the DOM tree recursively by replacing  "br" html boxes with anonymous blocks that respect br spec.<br/>
        /// If the "br" tag is after inline box then the anon block will have zero height only acting as newline,
        /// but if it is after block box then it will have min-height of the font size so it will create empty line.
        /// </summary>
        /// <param name="box">the current box to correct its sub-tree</param>
        private static void CorrectLineBreaksBlocks(CssBox box)
        {
            int lastBr = -1;
            CssBox brBox;
            do
            {
                brBox = null;
                CssBox prevBox = null;
                for (int i = 0; i < box.Boxes.Count && brBox == null; i++)
                {
                    if (i > lastBr && box.Boxes[i].HtmlTag != null && box.Boxes[i].HtmlTag.Name == "br")
                    {
                        brBox = box.Boxes[i];
                        lastBr = i;
                    }
                    else
                    {
                        prevBox = box.Boxes[i];
                    }
                }

                if (brBox != null)
                {
                    var anonBlock = CssBox.CreateBlock(box, new HtmlTag("br"), brBox);
                    if (prevBox == null || prevBox.Display != CssConstants.Inline)
                        anonBlock.Height = ".9em"; // atodo: check the height to min-height when it is supported
                    brBox.ParentBox = null;
                }

            } while (brBox != null);
            

            foreach (var childBox in box.Boxes)
            {
                CorrectLineBreaksBlocks(childBox);
            }
        }

        /// <summary>
        /// Correct DOM tree if there is block boxes that are inside inline blocks.<br/>
        /// Need to rearange the tree so block box will be only the child of other block box.
        /// </summary>
        /// <param name="box">the current box to correct its sub-tree</param>
        private static void CorrectBlockInsideInline(CssBox box)
        {
            if (DomUtils.ContainsInlinesOnly(box) && !ContainsInlinesOnlyDeep(box))
            {
                CorrectBlockInsideInlineImp(box);                    
            }
            
            if (!DomUtils.ContainsInlinesOnly(box))
            {
                foreach (var childBox in box.Boxes)
                {
                    CorrectBlockInsideInline(childBox);
                }
            }
        }

        /// <summary>
        /// Rearrange the DOM of the box to have block box with boxes before the inner block box and after.
        /// </summary>
        /// <param name="box">the box that has the problem</param>
        private static void CorrectBlockInsideInlineImp(CssBox box)
        {
            if (box.Boxes.Count > 1)
            {
                var leftBlock = CssBox.CreateBlock(box);

                while (ContainsInlinesOnlyDeep(box.Boxes[0]))
                    box.Boxes[0].ParentBox = leftBlock;
                leftBlock.SetBeforeBox(box.Boxes[0]);

                var splitBox = box.Boxes[1];
                splitBox.ParentBox = null;

                CorrectBlockSplitBadBox(box, splitBox, leftBlock);

                if (box.Boxes.Count > 2)
                {
                    var rightBox = CssBox.CreateBox(box, null, box.Boxes[2]);
                    while (box.Boxes.Count > 3)
                        box.Boxes[3].ParentBox = rightBox;
                }
                box.Display = CssConstants.Block;

            }
            else
            {
                box.Boxes[0].Display = CssConstants.Block;
            }
        }

        /// <summary>
        /// Split bad box that has inline and block boxes into two parts, the left - before the block box
        /// and right - after the block box.
        /// </summary>
        /// <param name="parentBox">the parent box that has the problem</param>
        /// <param name="badBox">the box to split into different boxes</param>
        /// <param name="leftBlock">the left block box that is created for the split</param>
        private static void CorrectBlockSplitBadBox(CssBox parentBox, CssBox badBox, CssBox leftBlock)
        {
            var leftbox = CssBox.CreateBox(leftBlock, badBox.HtmlTag);
            leftbox.InheritStyle(badBox, true);
            
            while (badBox.Boxes[0].IsInline && ContainsInlinesOnlyDeep(badBox.Boxes[0]))
                badBox.Boxes[0].ParentBox = leftbox;

            var splitBox = badBox.Boxes[0];
            if (!ContainsInlinesOnlyDeep(splitBox))
            {
                CorrectBlockSplitBadBox(parentBox, splitBox, leftBlock);
                splitBox.ParentBox = null;
            }
            else
            {
                splitBox.ParentBox = parentBox;
            }

            if (badBox.Boxes.Count > 0)
            {
                CssBox rightBox = null;
                if (splitBox.ParentBox != null || parentBox.Boxes.Count < 2)
                {
                    rightBox = CssBox.CreateBox(parentBox, badBox.HtmlTag);
                    rightBox.InheritStyle(badBox, true);

                    if (parentBox.Boxes.Count > 2)
                        rightBox.SetBeforeBox(parentBox.Boxes[1]);

                    splitBox.SetBeforeBox(rightBox);
                }
                else if (parentBox.Boxes.Count > 2)
                {
                    rightBox = parentBox.Boxes[2];
                }

                while (badBox.Boxes.Count > 0)
                    badBox.Boxes[0].ParentBox = rightBox;
            }
            else if(splitBox.ParentBox != null && parentBox.Boxes.Count > 1)
            {
                splitBox.SetBeforeBox(parentBox.Boxes[1]);
            }
        }

        /// <summary>
        /// Makes block boxes be among only block boxes and all inline boxes have block parent box.<br/>
        /// Inline boxes should live in a pool of Inline boxes only so they will define a single block.<br/>
        /// At the end of this process a block box will have only block siblings and inline box will have
        /// only inline siblings.
        /// </summary>
        /// <param name="box">the current box to correct its sub-tree</param>
        private static void CorrectInlineBoxesParent(CssBox box)
        {
            if (ContainsVariantBoxes(box))
            {
                for (int i = 0; i < box.Boxes.Count; i++)
                {
                    if (box.Boxes[i].IsInline)
                    {
                        var newbox = CssBox.CreateBlock(box, null, box.Boxes[i++]);
                        while (i < box.Boxes.Count && box.Boxes[i].IsInline)
                        {
                            box.Boxes[i].ParentBox = newbox;
                        }
                    }
                }
            }

            if (!DomUtils.ContainsInlinesOnly(box))
            {
                foreach (var childBox in box.Boxes)
                {
                    CorrectInlineBoxesParent(childBox);
                }
            }
        }

        /// <summary>
        /// Check if the given box contains only inline child boxes in all subtree.
        /// </summary>
        /// <param name="box">the box to check</param>
        /// <returns>true - only inline child boxes, false - otherwise</returns>
        private static bool ContainsInlinesOnlyDeep(CssBox box)
        {
            foreach (var childBox in box.Boxes)
            {
                if (!childBox.IsInline || !ContainsInlinesOnlyDeep(childBox))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if the given box contains inline and block child boxes.
        /// </summary>
        /// <param name="box">the box to check</param>
        /// <returns>true - has variant child boxes, false - otherwise</returns>
        private static bool ContainsVariantBoxes(CssBox box)
        {
            bool hasBlock = false;
            bool hasInline = false;
            for (int i = 0; i < box.Boxes.Count && (!hasBlock || !hasInline); i++)
            {
                var isBlock = box.Boxes[i].Display != CssConstants.Inline;
                hasBlock = hasBlock || isBlock;
                hasInline = hasInline || !isBlock;
            }

            return hasBlock && hasInline;
        }

        #endregion
    }
}