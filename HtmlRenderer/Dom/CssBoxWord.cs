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

using System.Drawing;
using HtmlRenderer.Entities;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{
    /// <summary>
    /// Represents a word inside an inline box
    /// </summary>
    /// <remarks>
    /// Because of performance, words of text are the most atomic 
    /// element in the project. It should be characters, but come on,
    /// imagine the performance when drawing char by char on the device.<br/>
    /// It may change for future versions of the library.
    /// </remarks>
    internal sealed class CssBoxWord : CssRectangle
    {
        #region Fields and Consts

        /// <summary>
        /// the CSS box owner of the word
        /// </summary>
        private readonly CssBox _ownerBox;

        /// <summary>
        /// was there a whitespace before the word chars (before trim)
        /// </summary>
        private readonly bool _hasSpaceBefore;

        /// <summary>
        /// was there a whitespace after the word chars (before trim)
        /// </summary>
        private readonly bool _hasSpaceAfter;

        private readonly string _word;
        private PointF _lastMeasureOffset;
        private Image _image;

        /// <summary>
        /// the css line box that this word belongs to
        /// </summary>
        private CssLineBox _lineBox;

        /// <summary>
        /// If the word is selected this points to the selection handler for more data
        /// </summary>
        private SelectionHandler _selection;

        #endregion


        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="owner">the CSS box owner of the word</param>
        /// <param name="word">the word chars </param>
        /// <param name="hasSpaceBefore">was there a whitespace before the word chars (before trim)</param>
        /// <param name="hasSpaceAfter">was there a whitespace after the word chars (before trim)</param>
        public CssBoxWord(CssBox owner, string word, bool hasSpaceBefore, bool hasSpaceAfter)
        {
            _ownerBox = owner;
            _word = word;
            _hasSpaceBefore = hasSpaceBefore;
            _hasSpaceAfter = hasSpaceAfter;
        }

        /// <summary>
        /// Creates a new BoxWord which represents an image
        /// </summary>
        /// <param name="owner">the CSS box owner of the word</param>
        /// <param name="image"></param>
        public CssBoxWord(CssBox owner, Image image)
        {
            _ownerBox = owner;
            Image = image;
        }

        /// <summary>
        /// was there a whitespace before the word chars (before trim)
        /// </summary>
        public bool HasSpaceBefore
        {
            get { return _hasSpaceBefore; }
        }

        /// <summary>
        /// was there a whitespace after the word chars (before trim)
        /// </summary>
        public bool HasSpaceAfter
        {
            get { return _hasSpaceAfter; }
        }

        /// <summary>
        /// Gets the width of the word including white-spaces
        /// </summary>
        public float FullWidth
        {
            get { return Width; }
        }

        /// <summary>
        /// Gets the image this words represents (if one exists)
        /// </summary>
        public Image Image
        {
            get { return _image; }
            set 
            { 
                _image = value;

                if (value != null)
                {
                    CssLength w = new CssLength(OwnerBox.Width);
                    CssLength h = new CssLength(OwnerBox.Height);
                    
                    bool hasImageTagWidth = w.Number > 0 && w.Unit == CssUnit.Pixels;
                    if (hasImageTagWidth)
                    {
                        Width = w.Number;
                    }
                    else
                    {
                        Width = value.Width;
                    }

                    bool hasImageTagHeight = h.Number > 0 && h.Unit == CssUnit.Pixels;
                    if (hasImageTagHeight)
                    {
                        Height = h.Number;
                    }
                    else
                    {
                        Height = value.Height;
                    }

                    // If only the width was set in the html tag, ratio the height.
                    if (hasImageTagWidth && !hasImageTagHeight)
                    {
                        // Devide the given tag width with the actual image width, to get the ratio.
                        float ratio = Width/value.Width;
                        Height = value.Height*ratio;
                    }
                    // If only the height was set in the html tag, ratio the width.
                    else if (hasImageTagHeight && !hasImageTagWidth)
                    {
                        // Devide the given tag height with the actual image height, to get the ratio.
                        float ratio = Height / value.Height;
                        Width = value.Width * ratio;
                    }

                    Height += OwnerBox.ActualBorderBottomWidth + OwnerBox.ActualBorderTopWidth + OwnerBox.ActualPaddingTop + OwnerBox.ActualPaddingBottom;
                    
                }
            }
        }

        /// <summary>
        /// Gets if the word represents an image.
        /// </summary>
        public bool IsImage
        {
            get { return Image != null; }
        }

        /// <summary>
        /// Gets a bool indicating if this word is composed only by spaces.
        /// Spaces include tabs and line breaks
        /// </summary>
        public bool IsSpaces
        {
            get { return string.IsNullOrEmpty(Text.Trim()); }
        }

        /// <summary>
        /// Gets if the word is composed by only a line break
        /// </summary>
        public bool IsLineBreak
        {
            get { return Text == "\n"; }
        }

        /// <summary>
        /// Gets if the word is composed by only a tab
        /// </summary>
        public bool IsTab
        {
            get { return Text == "\t"; }
        }

        /// <summary>
        /// Gets the Box where this word belongs.
        /// </summary>
        public CssBox OwnerBox
        {
            get { return _ownerBox; }
        }

        /// <summary>
        /// Gets the text of the word
        /// </summary>
        public string Text
        {
            get { return _word; }
        }

        /// <summary>
        /// the css line box that this word belongs to
        /// </summary>
        public CssLineBox LineBox
        {
            get { return _lineBox; }
            set { _lineBox = value; }
        }

        /// <summary>
        /// If the word is selected this points to the selection handler for more data
        /// </summary>
        public SelectionHandler Selection
        {
            get { return _selection; }
            set { _selection = value; }
        }

        /// <summary>
        /// is the word is currently selected
        /// </summary>
        public bool Selected
        {
            get { return _selection != null; }
        }

        /// <summary>
        /// the selection start index if the word is partially selected (-1 if not selected or fully selected)
        /// </summary>
        public int SelectedStartIndex
        {
            get { return _selection != null ? _selection.GetSelectingStartIndex(this) : -1; }
        }

        /// <summary>
        /// the selection end index if the word is partially selected (-1 if not selected or fully selected)
        /// </summary>
        public int SelectedEndIndexOffset
        {
            get { return _selection != null ? _selection.GetSelectedEndIndexOffset(this) : -1; }
        }

        /// <summary>
        /// the selection start offset if the word is partially selected (-1 if not selected or fully selected)
        /// </summary>
        public float SelectedStartOffset
        {
            get { return _selection != null ? _selection.GetSelectedStartOffset(this) : -1; }
        }

        /// <summary>
        /// the selection end offset if the word is partially selected (-1 if not selected or fully selected)
        /// </summary>
        public float SelectedEndOffset
        {
            get { return _selection != null ? _selection.GetSelectedEndOffset(this) : -1; }
        }

        /// <summary>
        /// Gets or sets an offset to be considered in measurements
        /// </summary>
        internal PointF LastMeasureOffset
        {
            get { return _lastMeasureOffset; }
            set { _lastMeasureOffset = value; }
        }

        /// <summary>
        /// Represents this word for debugging purposes
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} ({1} char{2})", Text.Replace(' ', '-').Replace("\n", "\\n"), Text.Length, Text.Length != 1 ? "s" : string.Empty);
        }
    }
}
