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
using System.Drawing.Drawing2D;
using HtmlRenderer.Entities;
using HtmlRenderer.Parse;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{
    /// <summary>
    /// Represents a CSS Box of text or replaced elements.
    /// </summary>
    /// <remarks>
    /// The Box can contains other boxes, that's the way that the CSS Tree
    /// is composed.
    /// 
    /// To know more about boxes visit CSS spec:
    /// http://www.w3.org/TR/CSS21/box.html
    /// </remarks>
    internal class CssBox : CssBoxProperties
    {
        #region Fields and Consts

        /// <summary>
        /// the parent css box of this css box in the hierarchy
        /// </summary>
        private CssBox _parentBox;
        
        /// <summary>
        /// the root container for the hierarchy
        /// </summary>
        private HtmlContainer _htmlContainer;

        /// <summary>
        /// the html tag that is associated with this css box, null if anonymous box
        /// </summary>
        private readonly HtmlTag _htmltag;

        private readonly List<CssBoxWord> _boxWords = new List<CssBoxWord>();
        private readonly List<CssBox> _boxes = new List<CssBox>();
        private readonly List<CssLineBox> _lineBoxes = new List<CssLineBox>();
        private readonly List<CssLineBox> _parentLineBoxes = new List<CssLineBox>();
        private readonly Dictionary<CssLineBox, RectangleF> _rectangles = new Dictionary<CssLineBox, RectangleF>();
        
        /// <summary>
        /// Do not use or alter this flag
        /// </summary>
        /// <remarks>
        /// Flag that indicates that CssTable algorithm already made fixes on it.
        /// </remarks>
        internal bool _tableFixed;

        private bool _wordsSizeMeasured;
        private CssBox _listItemBox;
        private CssLineBox _firstHostingLineBox;
        private CssLineBox _lastHostingLineBox;

        #endregion

        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="parentBox">optional: the parent of this css box in html</param>
        /// <param name="tag">optional: the html tag associated with this css box</param>
        internal CssBox(CssBox parentBox = null, HtmlTag tag = null)
        {
            if(parentBox != null)
            {
                _parentBox = parentBox;
                _parentBox.Boxes.Add(this);
            }
            _htmltag = tag;
        }


        /// <summary>
        /// Gets the HtmlContainer of the Box.
        /// WARNING: May be null.
        /// </summary>
        public HtmlContainer HtmlContainer
        {
            get { return _htmlContainer ?? (_parentBox != null ? _parentBox.HtmlContainer : null); }
            set { _htmlContainer = value; }
        }

        /// <summary>
        /// Gets or sets the parent box of this box
        /// </summary>
        public CssBox ParentBox
        {
            get { return _parentBox; }
            set
            {
                //Remove from last parent
                if (_parentBox != null && _parentBox.Boxes.Contains(this))
                {
                    _parentBox.Boxes.Remove(this);
                }

                _parentBox = value;

                //Add to new parent
                if (value != null && !value.Boxes.Contains(this))
                {
                    _parentBox.Boxes.Add(this);
                    _htmlContainer = value.HtmlContainer;
                }
            }
        }

        /// <summary>
        /// Gets the • box
        /// </summary>
        public CssBox ListItemBox
        {
            get { return _listItemBox; }
        }

        /// <summary>
        /// Gets the childrenn boxes of this box
        /// </summary>
        public List<CssBox> Boxes
        {
            get { return _boxes; }
        }

        /// <summary>
        /// is the box "Display" is "Inline", is this is an inline box and not block.
        /// </summary>
        public bool IsInline
        {
            get { return Display == CssConstants.Inline; }
        }

        /// <summary>
        /// Gets the containing block-box of this box. (The nearest parent box with display=block)
        /// </summary>
        public CssBox ContainingBlock
        {
            get
            {
                if (ParentBox == null)
                {
                    return this; //This is the initial containing block.
                }

                CssBox box = ParentBox;
                while ( box.Display != CssConstants.Block &&
                        box.Display != CssConstants.Table &&
                        box.Display != CssConstants.TableCell &&
                        box.ParentBox != null)
                {
                    box = box.ParentBox;
                }

                //Comment this following line to treat always superior box as block
                if (box == null) 
                    throw new Exception("There's no containing block on the chain");

                return box;
            }
        }

        /// <summary>
        /// Gets the HTMLTag that hosts this box
        /// </summary>
        public HtmlTag HtmlTag
        {
            get { return _htmltag; }
        }

        /// <summary>
        /// Gets if this box represents an image
        /// </summary>
        public bool IsImage
        {
            get { return Words.Count == 1 && Words[0].IsImage; } 
        }

        /// <summary>
        /// Tells if the box is empty or contains just blank spaces
        /// </summary>
        public bool IsSpaceOrEmpty
        {
            get 
            {
                if ((Words.Count != 0 || Boxes.Count != 0) && (Words.Count != 1 || !Words[0].IsSpaces))
                {
                    foreach (CssBoxWord word in Words)
                    {
                        if (!word.IsSpaces)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the inner text of the box
        /// </summary>
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                HtmlParser.ParseToWords(this, Words, value);
            }
        }

        /// <summary>
        /// Gets the line-boxes of this box (if block box)
        /// </summary>
        internal List<CssLineBox> LineBoxes
        {
            get { return _lineBoxes; }
        }

        /// <summary>
        /// Gets the linebox(es) that contains words of this box (if inline)
        /// </summary>
        internal List<CssLineBox> ParentLineBoxes
        {
            get { return _parentLineBoxes; }
        }

        /// <summary>
        /// Gets the rectangles where this box should be painted
        /// </summary>
        internal Dictionary<CssLineBox, RectangleF> Rectangles
        {
            get { return _rectangles; }
        }

        /// <summary>
        /// Gets the BoxWords of text in the box
        /// </summary>
        internal List<CssBoxWord> Words
        {
            get { return _boxWords; }
        }

        /// <summary>
        /// Gets the first word of the box
        /// </summary>
        internal CssBoxWord FirstWord
        {
            get { return Words[0]; }
        }

        /// <summary>
        /// Gets or sets the first linebox where content of this box appear
        /// </summary>
        internal CssLineBox FirstHostingLineBox
        {
            get { return _firstHostingLineBox; }
            set { _firstHostingLineBox = value; }
        }

        /// <summary>
        /// Gets or sets the last linebox where content of this box appear
        /// </summary>
        internal CssLineBox LastHostingLineBox
        {
            get { return _lastHostingLineBox; }
            set { _lastHostingLineBox = value; }
        }

        /// <summary>
        /// Create new css block box.
        /// </summary>
        /// <returns>the new block box</returns>
        public static CssBox CreateBlock()
        {
            var box = new CssBox();
            box.Display = CssConstants.Block;
            return box;
        }

        /// <summary>
        /// Create new css block box for the given parent with the given optional html tag and insert it either
        /// at the end or before the given optional box.<br/>
        /// If no html tag is given the box will be anonymous.<br/>
        /// If no before box is given the new box will be added at the end of parent boxes collection.<br/>
        /// If before box doesn't exists in parent box exception is thrown.<br/>
        /// </summary>
        /// <remarks>
        /// To learn more about anonymous block boxes visit CSS spec:
        /// http://www.w3.org/TR/CSS21/visuren.html#anonymous-block-level
        /// </remarks>
        /// <param name="parent">the box to add the new block box to it as child</param>
        /// <param name="tag">optional: the html tag to define the box</param>
        /// <param name="before">optional: to insert as specific location in parent box</param>
        /// <returns>the new block box</returns>
        public static CssBox CreateBlock(CssBox parent, HtmlTag tag = null, CssBox before = null)
        {
            ArgChecker.AssertArgNotNull(parent, "parent");

            var newBox = CreateBox(parent, tag, before);
            newBox.Display = CssConstants.Block;
            return newBox;
        }

        /// <summary>
        /// Create new css box for the given parent with the given optional html tag and insert it either
        /// at the end or before the given optional box.<br/>
        /// If no html tag is given the box will be anonymous.<br/>
        /// If no before box is given the new box will be added at the end of parent boxes collection.<br/>
        /// If before box doesn't exists in parent box exception is thrown.<br/>
        /// </summary>
        /// <remarks>
        /// To learn more about anonymous inline boxes visit: http://www.w3.org/TR/CSS21/visuren.html#anonymous
        /// </remarks>
        /// <param name="parent">the box to add the new box to it as child</param>
        /// <param name="tag">optional: the html tag to define the box</param>
        /// <param name="before">optional: to insert as specific location in parent box</param>
        /// <returns>the new box</returns>
        public static CssBox CreateBox(CssBox parent, HtmlTag tag = null, CssBox before = null)
        {
            ArgChecker.AssertArgNotNull(parent, "parent");

            var newBox =  new CssBox(parent, tag);
            newBox.InheritStyle();
            if (before != null)
            {
                newBox.SetBeforeBox(before);
            }
            return newBox;
        }

        /// <summary>
        /// Measures the bounds of box and children, recursively.<br/>
        /// Performs layout of the DOM structure creating lines by set bounds restrictions.
        /// </summary>
        /// <param name="g">Device context to use</param>
        public void MeasureBounds(Graphics g)
        {
            MeasureBoundsImp(g);
        }

        /// <summary>
        /// Paints the fragment
        /// </summary>
        /// <param name="g"></param>
        public void Paint(Graphics g)
        {
            PaintImp(g);
        }

        /// <summary>
        /// Set this box in 
        /// </summary>
        /// <param name="before"></param>
        public void SetBeforeBox(CssBox before)
        {
            int index = _parentBox.Boxes.IndexOf(before);
            if (index < 0)
                throw new Exception("before box doesn't exist on parent");

            _parentBox.Boxes.Remove(this);
            _parentBox.Boxes.Insert(index, this);
        }


        #region Private Methods

        /// <summary>
        /// Measures the bounds of box and children, recursively.<br/>
        /// Performs layout of the DOM structure creating lines by set bounds restrictions.
        /// </summary>
        /// <param name="g">Device context to use</param>
        private void MeasureBoundsImp(Graphics g)
        {
            if (Display == CssConstants.None)
                return;

            RectanglesReset();

            MeasureWordsSize(g);

            if (Display == CssConstants.Block ||
                Display == CssConstants.ListItem ||
                Display == CssConstants.Table ||
                Display == CssConstants.InlineTable ||
                Display == CssConstants.TableCell ||
                Display == CssConstants.None)
            {
                if (Display != CssConstants.TableCell)
                {
                    var prevSibling = DomUtils.GetPreviousSibling(this);
                    float left = ContainingBlock.Location.X + ContainingBlock.ActualPaddingLeft + ActualMarginLeft + ContainingBlock.ActualBorderLeftWidth;
                    float top = (prevSibling == null && ParentBox != null ? ParentBox.ClientTop : ParentBox == null ? Location.Y : 0) + MarginTopCollapse(prevSibling) + (prevSibling != null ? prevSibling.ActualBottom + prevSibling.ActualBorderBottomWidth : 0);
                    Location = new PointF(left, top);
                    ActualBottom = top;
                }

                // Because their width and height are set by CssTable
                if (Display != CssConstants.TableCell && Display != CssConstants.Table)
                {
                    //width at 100% (or auto)
                    float minwidth = GetMinimumWidth();
                    float width = ContainingBlock.Size.Width
                                  - ContainingBlock.ActualPaddingLeft - ContainingBlock.ActualPaddingRight
                                  - ContainingBlock.ActualBorderLeftWidth - ContainingBlock.ActualBorderRightWidth
                                  - ActualMarginLeft - ActualMarginRight - ActualBorderLeftWidth - ActualBorderRightWidth;

                    //Check width if not auto
                    if (Width != CssConstants.Auto && !string.IsNullOrEmpty(Width))
                    {
                        width = CssValueParser.ParseLength(Width, width, this);
                    }

                    if (width < minwidth || width >= 9999) 
                        width = minwidth;

                    Size = new SizeF(width, Size.Height);
                }

                //If we're talking about a table here..
                if (Display == CssConstants.Table || Display == CssConstants.InlineTable)
                {
                    new CssTable(this, g);
                }
                else
                {
                    //If there's just inlines, create LineBoxes
                    if (DomUtils.ContainsInlinesOnly(this))
                    {
                        ActualBottom = Location.Y;
                        CssLayoutEngine.CreateLineBoxes(g, this); //This will automatically set the bottom of this block
                    }
                    else if (_boxes.Count > 0)
                    {
                        foreach (var childBox in Boxes)
                        {
                            childBox.MeasureBoundsImp(g);
                        }
                        ActualBottom = MarginBottomCollapse();
                    }
                }
            }

            if (HtmlContainer != null)
            {
                HtmlContainer.ActualSize = new SizeF(Math.Max(HtmlContainer.ActualSize.Width, Size.Width < 9999 ? Size.Width : 0), Math.Max(HtmlContainer.ActualSize.Height, Size.Height));
            }
        }

        /// <summary>
        /// Assigns words its width and height
        /// </summary>
        /// <param name="g"></param>
        internal void MeasureWordsSize(Graphics g)
        {
            // Check if measure white space if not yet done to measure once
            if (!_wordsSizeMeasured)
            {
                MeasureWordSpacing(g);
                
                if (HtmlTag != null && HtmlTag.Name == "img")
                {
                    var image = CssValueParser.GetImage(GetAttribute("src"), HtmlContainer.Bridge);
                    var word = new CssBoxWord(this, image);
                    Words.Clear();
                    Words.Add(word);
                }
                else if(Words.Count > 0)
                {
                    foreach (var boxWord in Words)
                    {
                        var sf = new StringFormat();
                        sf.SetMeasurableCharacterRanges(new[] { new CharacterRange(0, boxWord.Text.Length) });

                        var regions = g.MeasureCharacterRanges(boxWord.Text, ActualFont, new RectangleF(0, 0, float.MaxValue, float.MaxValue), sf);

                        SizeF s = regions[0].GetBounds(g).Size;
                        PointF p = regions[0].GetBounds(g).Location;

                        boxWord.LastMeasureOffset = new PointF(p.X, p.Y);
                        boxWord.Width = s.Width + ActualWordSpacing;
                        boxWord.Height = s.Height;
                    }
                }
                _wordsSizeMeasured = true;
            }
        }

        /// <summary>
        /// Get the parent of this css properties instance.
        /// </summary>
        /// <returns></returns>
        protected sealed override CssBoxProperties GetParent()
        {
            return _parentBox;
        }

        /// <summary>
        /// Gets the index of the box to be used on a (ordered) list
        /// </summary>
        /// <returns></returns>
        private int GetIndexForList()
        {
            int index = 0;

            foreach (CssBox b in ParentBox.Boxes)
            {
                if (b.Display == CssConstants.ListItem) 
                    index++;
                if (b.Equals(this)) 
                    return index;
            }

            return index;
        }

        /// <summary>
        /// Creates the <see cref="ListItemBox"/>
        /// </summary>
        /// <param name="g"></param>
        private void CreateListItemBox(Graphics g)
        {
            if (Display == CssConstants.ListItem)
            {
                if (_listItemBox == null)
                {
                    _listItemBox = new CssBox();
                    _listItemBox.InheritStyle(this, false);
                    _listItemBox.Display = CssConstants.Inline;
                    _listItemBox._htmlContainer = HtmlContainer;

                    if (ParentBox != null && ListStyleType == CssConstants.Decimal)
                    {
                        _listItemBox.Text = GetIndexForList() + ".";
                    }
                    else
                    {
                        _listItemBox.Text = "•";
                    }
                    
                    _listItemBox.MeasureBoundsImp(g);
                    _listItemBox.Size = new SizeF(_listItemBox.Words[0].Width, _listItemBox.Words[0].Height); 
                }
                _listItemBox.Words[0].Left = Location.X - _listItemBox.Size.Width - 5;
                _listItemBox.Words[0].Top = Location.Y + ActualPaddingTop;// +FontAscent;
            }
        }

        /// <summary>
        /// Searches for the first word occourence inside the box, on the specified linebox
        /// </summary>
        /// <param name="b"></param>
        /// <param name="line"> </param>
        /// <returns></returns>
        internal CssBoxWord FirstWordOccourence(CssBox b, CssLineBox line)
        {
            if (b.Words.Count == 0 && b.Boxes.Count == 0)
            {
                return null;
            }

            if (b.Words.Count > 0)
            {   
                foreach (CssBoxWord word in b.Words)
                {
                    if (line.Words.Contains(word))
                    {
                        return word;
                    }
                }
                return null;
            }
            else
            {
                foreach (CssBox bb in b.Boxes)
                {
                    CssBoxWord w = FirstWordOccourence(bb, line);

                    if (w != null)
                    {
                        return w;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the specified Attribute, returns string.Empty if no attribute specified
        /// </summary>
        /// <param name="attribute">Attribute to retrieve</param>
        /// <returns>Attribute value or string.Empty if no attribute specified</returns>
        internal string GetAttribute(string attribute)
        {
            return GetAttribute(attribute, string.Empty);
        }

        /// <summary>
        /// Gets the value of the specified attribute of the source HTML tag.
        /// </summary>
        /// <param name="attribute">Attribute to retrieve</param>
        /// <param name="defaultValue">Value to return if attribute is not specified</param>
        /// <returns>Attribute value or defaultValue if no attribute specified</returns>
        internal string GetAttribute(string attribute, string defaultValue)
        {
            if (HtmlTag == null)
            {
                return defaultValue;
            }

            if (!HtmlTag.HasAttribute(attribute))
            {
                return defaultValue;
            }

            return HtmlTag.Attributes[attribute];
        }

        /// <summary>
        /// Gets the minimum width that the box can be.
        /// The box can be as thin as the longest word plus padding.
        /// The check is deep thru box tree.
        /// </summary>
        /// <returns></returns>
        internal float GetMinimumWidth()
        {
            float maxw = 0f;
            float padding = 0f;
            CssBoxWord word = null;

            GetMinimumWidth_LongestWord(this, ref maxw, ref word);

            if (word != null)
            {
                GetMinimumWidth_BubblePadding(word.OwnerBox, this, ref padding);
            }

            return maxw + padding;
        }

        /// <summary>
        /// Bubbles up the padding from the starting box
        /// </summary>
        /// <param name="box"></param>
        /// <param name="endbox"> </param>
        /// <param name="sum"> </param>
        /// <returns></returns>
        private void GetMinimumWidth_BubblePadding(CssBox box, CssBox endbox, ref float sum)
        {
            //float padding = box.ActualMarginLeft + box.ActualBorderLeftWidth + box.ActualPaddingLeft +
            //    box.ActualMarginRight + box.ActualBorderRightWidth + box.ActualPaddingRight;

            float padding =  box.ActualBorderLeftWidth + box.ActualPaddingLeft +
                 box.ActualBorderRightWidth + box.ActualPaddingRight;

            sum += padding;

            if (!box.Equals(endbox))
            {
                GetMinimumWidth_BubblePadding(box.ParentBox, endbox, ref sum);
            }
        }

        /// <summary>
        /// Gets the longest word (in width) inside the box, deeply.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="maxw"> </param>
        /// <param name="word"> </param>
        /// <returns></returns>
        private void GetMinimumWidth_LongestWord(CssBox b, ref float maxw, ref CssBoxWord word)
        {

            if (b.Words.Count > 0)
            {
                foreach (CssBoxWord w in b.Words)
                {
                    if (w.FullWidth > maxw)
                    {
                        maxw = w.FullWidth;
                        word = w;
                    }
                }
            }
            else
            {
                foreach(CssBox bb in b.Boxes)
                    GetMinimumWidth_LongestWord(bb, ref maxw,ref word);
            }

        }

        /// <summary>
        /// Gets the maximum bottom of the boxes inside the startBox
        /// </summary>
        /// <param name="startBox"></param>
        /// <param name="currentMaxBottom"></param>
        /// <returns></returns>
        internal float GetMaximumBottom(CssBox startBox, float currentMaxBottom)
        {
            foreach (CssLineBox line in startBox.Rectangles.Keys)
            {
                currentMaxBottom = Math.Max(currentMaxBottom, startBox.Rectangles[line].Bottom);
            }

            foreach (CssBox b in startBox.Boxes)
            {
                currentMaxBottom = Math.Max(currentMaxBottom, b.ActualBottom);
                currentMaxBottom = Math.Max(currentMaxBottom, GetMaximumBottom(b, currentMaxBottom));
            }

            return currentMaxBottom;
        }

        /// <summary>
        /// Get the width of the box at full width (No line breaks)
        /// </summary>
        /// <returns></returns>
        internal float GetFullWidth(Graphics g)
        {
            float sum = 0f;
            float paddingsum = 0f;
            GetFullWidth_WordsWith(this, ref sum, ref paddingsum);

            return paddingsum + sum;
        }

        /// <summary>
        /// Gets the longest word (in width) inside the box, deeply.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="sum"> </param>
        /// <param name="paddingsum"> </param>
        /// <returns></returns>
        private void GetFullWidth_WordsWith(CssBox b, ref float sum, ref float paddingsum)
        {
            float? oldSum = null;
            if (b.Display != CssConstants.Inline)
            {
                oldSum = sum;
                sum = 0;
            }

            paddingsum += b.ActualBorderLeftWidth + b.ActualBorderRightWidth + b.ActualPaddingRight + b.ActualPaddingLeft;

            if (b.Words.Count > 0)
            {
                foreach (CssBoxWord word in b.Words)
                    sum += word.FullWidth;
            }
            else
            {
                foreach (CssBox bb in b.Boxes)
                {
                    GetFullWidth_WordsWith(bb, ref sum, ref paddingsum);
                }
            }
            
            if (oldSum.HasValue)
            {
                sum = Math.Max(sum, oldSum.Value);
            }
        }

        /// <summary>
        /// Gets if this box has only inline siblings (including itself)
        /// </summary>
        /// <returns></returns>
        internal bool HasJustInlineSiblings()
        {
            if (ParentBox == null)
            {
                return false;
            }

            return DomUtils.ContainsInlinesOnly(ParentBox);
        }

        /// <summary>
        /// Gets the rectangles where inline box will be drawn. See Remarks for more info.
        /// </summary>
        /// <returns>Rectangles where content should be placed</returns>
        /// <remarks>
        /// Inline boxes can be splitted across different LineBoxes, that's why this method
        /// Delivers a rectangle for each LineBox related to this box, if inline.
        /// </remarks>

         /// <summary>
        /// Inherits inheritable values from parent.
        /// </summary>
        internal new void InheritStyle(CssBox box = null, bool everything = false)
        {
            base.InheritStyle(box ?? ParentBox, everything);
        }

        /// <summary>
        /// Gets the result of collapsing the vertical margins of the two boxes
        /// </summary>
        /// <param name="prevSibling">the previous box under the same parent</param>
        /// <returns>Resulting top margin</returns>
        private float MarginTopCollapse(CssBoxProperties prevSibling)
        {
            float value;
            if (prevSibling != null )
            {
                value = Math.Max(prevSibling.ActualMarginBottom, ActualMarginTop);
                CollapsedMarginTop = value;
            }
            else if (_parentBox != null && ActualPaddingTop < 0.1 && ActualPaddingBottom < 0.1 && _parentBox.ActualPaddingTop < 0.1 && _parentBox.ActualPaddingBottom < 0.1)
            {
                value = Math.Max(0, ActualMarginTop - Math.Max(_parentBox.ActualMarginTop, _parentBox.CollapsedMarginTop));
            }
            else
            {
                value = ActualMarginTop;
            }
  
            // fix for hr tag
            if (value < 0.1 && HtmlTag != null && HtmlTag.Name == "hr")
            {
                value = GetEmHeight() * 1.1f;
            }
            
            return value;
        }

        /// <summary>
        /// Gets the result of collapsing the vertical margins of the two boxes
        /// </summary>
        /// <returns>Resulting bottom margin</returns>
        private float MarginBottomCollapse()
        {
            float margin = 0;
            if (_parentBox == null || (ParentBox.Boxes.IndexOf(this) == ParentBox.Boxes.Count - 1 && _parentBox.ActualMarginBottom < 0.1))
            {
                var lastChildBottomMargin = _boxes[_boxes.Count - 1].ActualMarginBottom;
                margin = Height == "auto" ? Math.Max(ActualMarginBottom, lastChildBottomMargin) : lastChildBottomMargin;
            }
            return Math.Max(ActualBottom, _boxes[_boxes.Count - 1].ActualBottom + margin + ActualPaddingBottom);
        }

        /// <summary>
        /// Deeply offsets the top of the box and its contents
        /// </summary>
        /// <param name="amount"></param>
        internal void OffsetTop(float amount)
        {
            List<CssLineBox> lines = new List<CssLineBox>();
            foreach (CssLineBox line in Rectangles.Keys)
                lines.Add(line);

            foreach (CssLineBox line in lines)
            {
                RectangleF r = Rectangles[line];
                Rectangles[line] = new RectangleF(r.X, r.Y + amount, r.Width, r.Height);
            }

            foreach (CssBoxWord word in Words)
            {
                word.Top += amount;
            }
            
            foreach (CssBox b in Boxes)
            {
                b.OffsetTop(amount);
            }
            //TODO: Aquí me quede: no se mueve bien todo (probar con las tablas rojas)
            Location = new PointF(Location.X, Location.Y + amount);
        }

        /// <summary>
        /// Paints the fragment
        /// </summary>
        /// <param name="g"></param>
        private void PaintImp(Graphics g)
        {
            if (Display != CssConstants.None && (Display != CssConstants.TableCell || EmptyCells != CssConstants.Hide || !IsSpaceOrEmpty))
            {
                var areas = Rectangles.Count == 0 ? new List<RectangleF>(new[] { Bounds }) : new List<RectangleF>(Rectangles.Values);

                RectangleF[] rects = areas.ToArray();
                PointF offset = HtmlContainer != null ? HtmlContainer.ScrollOffset : PointF.Empty;

                for (int i = 0; i < rects.Length; i++)
                {
                    var actualRect = rects[i];
                    actualRect.Offset(offset);

                    PaintBackground(g, actualRect, i == rects.Length - 1);
                    PaintBorder(g, actualRect, i == 0, i == rects.Length - 1);
                }

                if (IsImage)
                {
                    var word = Words[0];
                    RectangleF r = word.Bounds;
                    r.Offset(offset);
                    r.Height -= ActualBorderTopWidth + ActualBorderBottomWidth + ActualPaddingTop + ActualPaddingBottom;
                    r.Y += ActualBorderTopWidth + ActualPaddingTop;

                    //HACK: round rectangle only when necessary
                    g.DrawImage(word.Image, Rectangle.Round(r));

                    if (word.Selected)
                    {
                        g.FillRectangle(CssUtils.SelectionBackcolor, word.Left - word.LastMeasureOffset.X + offset.X, word.Top + offset.Y, word.Width, DomUtils.GetCssLineBoxByWord(word).LineHeight);
                    }
                }
                else if (Words.Count > 0)
                {
                    Font font = ActualFont;
                    var brush = CssUtils.GetSolidBrush(CssValueParser.GetActualColor(Color));
                    foreach (var word in Words)
                    {
                        if(word.Selected)
                        {
                            // handle paint selected word background and with partial word selection
                            var left = word.SelectedStartOffset > -1 ? word.SelectedStartOffset : 0;
                            var width = word.SelectedEndOffset > -1 ? word.SelectedEndOffset + word.LastMeasureOffset.X : word.Width;


                            //REFACTOR THIS STUFF
                            Brush b = CssUtils.SelectionBackcolor;
                            CssLineBox box = DomUtils.GetCssLineBoxByWord(word);
                            float h = box != null ? box.LineHeight : 0;
                            float w =  width - left;
                            float x = word.Left - word.LastMeasureOffset.X + offset.X + left;
                            float y = word.Top + offset.Y;


                            g.FillRectangle(b, x, y, w, h);
                        }

                        g.DrawString(word.Text, font, brush, word.Left - word.LastMeasureOffset.X + offset.X, word.Top + offset.Y);
                    }
                }

                for (int i = 0; i < rects.Length; i++)
                {
                    var actualRect = rects[i];
                    actualRect.Offset(offset);
                    PaintDecoration(g, actualRect, i == 0, i == rects.Length - 1);
                }

                foreach (CssBox b in Boxes)
                {
                    b.Paint(g);
                }

                CreateListItemBox(g);

                if (ListItemBox != null)
                {
                    ListItemBox.Paint(g);
                }
            }
        }

        /// <summary>
        /// Paints the border of the box
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rectangle"> </param>
        /// <param name="isFirst"> </param>
        /// <param name="isLast"> </param>
        private void PaintBorder(Graphics g, RectangleF rectangle, bool isFirst, bool isLast)
        {
            if (rectangle.Width > 0 && rectangle.Height > 0)
            {
                SmoothingMode smooth = g.SmoothingMode;

                if (HtmlContainer != null && !HtmlContainer.AvoidGeometryAntialias && IsRounded)
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                }

                //Top border
                if (!(string.IsNullOrEmpty(BorderTopStyle) || BorderTopStyle == CssConstants.None))
                {
                    var b = CssUtils.GetSolidBrush(BorderTopStyle == CssConstants.Inset ? CssDrawingUtils.Darken(ActualBorderTopColor) : ActualBorderTopColor);
                    g.FillPath(b, CssDrawingUtils.GetBorderPath(CssDrawingUtils.Border.Top, this, rectangle, isFirst, isLast));
                }


                if (isLast)
                {
                    //Right Border
                    if (!(string.IsNullOrEmpty(BorderRightStyle) || BorderRightStyle == CssConstants.None))
                    {
                        var b = CssUtils.GetSolidBrush(BorderRightStyle == CssConstants.Outset ? CssDrawingUtils.Darken(ActualBorderRightColor) : ActualBorderRightColor);
                        g.FillPath(b, CssDrawingUtils.GetBorderPath(CssDrawingUtils.Border.Right, this, rectangle, isFirst, true));
                    }
                }

                //Bottom border
                if (!(string.IsNullOrEmpty(BorderBottomStyle) || BorderBottomStyle == CssConstants.None))
                {
                    var b = CssUtils.GetSolidBrush(BorderBottomStyle == CssConstants.Outset ? CssDrawingUtils.Darken(ActualBorderBottomColor) : ActualBorderBottomColor);
                    g.FillPath(b, CssDrawingUtils.GetBorderPath(CssDrawingUtils.Border.Bottom, this, rectangle, isFirst, isLast));
                }

                if (isFirst)
                {
                    //Left Border
                    if (!(string.IsNullOrEmpty(BorderLeftStyle) || BorderLeftStyle == CssConstants.None))
                    {
                        var b = CssUtils.GetSolidBrush(BorderLeftStyle == CssConstants.Inset ? CssDrawingUtils.Darken(ActualBorderLeftColor) : ActualBorderLeftColor);
                        g.FillPath(b, CssDrawingUtils.GetBorderPath(CssDrawingUtils.Border.Left, this, rectangle, true, isLast));
                    }
                }

                g.SmoothingMode = smooth;
            }
        }

        /// <summary>
        /// Paints the background of the box
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rectangle"> </param>
        /// <param name="b"> </param>
        /// <param name="isLast">is the </param>
        private void PaintBackground(Graphics g, RectangleF rectangle, bool isLast)
        {
            //HACK: Background rectangles are being deactivated when justifying text.
            if (ContainingBlock.TextAlign != CssConstants.Justify && rectangle.Width > 0 && rectangle.Height > 0)
            {
                Brush brush = null;
                bool dispose = false;
                SmoothingMode smooth = g.SmoothingMode;

                if (BackgroundGradient != CssConstants.None)
                {
                    brush = new LinearGradientBrush(rectangle, ActualBackgroundColor, ActualBackgroundGradient, ActualBackgroundGradientAngle);
                    dispose = true;
                }
                else if (ActualBackgroundColor != System.Drawing.Color.Empty && ActualBackgroundColor != System.Drawing.Color.Transparent)
                {
                    brush = CssUtils.GetSolidBrush(ActualBackgroundColor);
                }

                if(brush != null)
                {
                    // atodo: handle it correctly (tables background)
//                    if (isLast)
//                        rectangle.Width -= ActualWordSpacing + CssUtils.GetWordEndWhitespace(ActualFont);
                        
                    GraphicsPath roundrect = null;
                    if (IsRounded)
                    {
                        roundrect = CssDrawingUtils.GetRoundRect(rectangle, ActualCornerNW, ActualCornerNE, ActualCornerSE, ActualCornerSW);
                    }

                    if (HtmlContainer != null && !HtmlContainer.AvoidGeometryAntialias && IsRounded)
                    {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                    }

                    if (roundrect != null)
                    {
                        g.FillPath(brush, roundrect);
                    }
                    else
                    {
                        g.FillRectangle(brush, rectangle);
                    }

                    g.SmoothingMode = smooth;

                    if (roundrect != null) roundrect.Dispose();
                    if(dispose) brush.Dispose();
                }
            }
        }

        /// <summary>
        /// Paints the text decoration
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rectangle"> </param>
        /// <param name="isFirst"> </param>
        /// <param name="isLast"> </param>
        private void PaintDecoration(Graphics g, RectangleF rectangle, bool isFirst, bool isLast)
        {
            if (string.IsNullOrEmpty(TextDecoration) || TextDecoration == CssConstants.None || IsImage) return;

            float desc = CssUtils.GetDescent(ActualFont);
            float asc = CssUtils.GetAscent(ActualFont);
            float y = 0f;

            if (TextDecoration == CssConstants.Underline)
            {
                y = rectangle.Bottom - desc + 0.6f;
            }
            else if (TextDecoration == CssConstants.LineThrough)
            {
                y = rectangle.Bottom - desc - asc / 2;
            }
            else if (TextDecoration == CssConstants.Overline)
            {
                y = rectangle.Bottom - desc - asc - 2;
            }

            y -= ActualPaddingBottom - ActualBorderBottomWidth;

            float x1 = rectangle.X;
            float x2 = rectangle.Right;

            if (isFirst) x1 += ActualPaddingLeft + ActualBorderLeftWidth;
            if (isLast)
            {
                x2 -= ActualPaddingRight + ActualBorderRightWidth;
                x2 -= ActualWordSpacing + 0.8f*CssUtils.GetWordEndWhitespace(ActualFont);
            }

            g.DrawLine(new Pen(ActualColor), x1, y, x2, y);
        }

        /// <summary>
        /// Offsets the rectangle of the specified linebox by the specified gap,
        /// and goes deep for rectangles of children in that linebox.
        /// </summary>
        /// <param name="lineBox"></param>
        /// <param name="gap"></param>
        internal void OffsetRectangle(CssLineBox lineBox, float gap)
        {
            if (Rectangles.ContainsKey(lineBox))
            {
                RectangleF r = Rectangles[lineBox];
                Rectangles[lineBox] = new RectangleF(r.X, r.Y + gap, r.Width, r.Height);
            }

            //foreach (Box b in Boxes)
            //{
            //    b.OffsetRectangle(lineBox, gap);
            //}
        }

        /// <summary>
        /// Resets the <see cref="Rectangles"/> array
        /// </summary>
        internal void RectanglesReset()
        {
            _rectangles.Clear();
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string tag = GetType().Name;
            if (HtmlTag != null)
            {
                tag = string.Format("<{0}>", HtmlTag.Name);
            }
            else
            {
                tag = "anon";
            }

            if (Display == CssConstants.Block)
            {
                return string.Format("{0}{1} Block {2}, Children:{3}", ParentBox == null ? "Root: " : string.Empty, tag, FontSize, Boxes.Count);
            }
            else if (Display == CssConstants.None)
            {
                return string.Format("{0}{1} None", ParentBox == null ? "Root: " : string.Empty, tag);
            }
            else
            {
                return string.Format("{0}{1} {2}: {3}", ParentBox == null ? "Root: " : string.Empty, tag, Display, Text);
            }
        }

        #endregion
    }
}