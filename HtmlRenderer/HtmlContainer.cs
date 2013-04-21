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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using HtmlRenderer.Dom;
using HtmlRenderer.Entities;
using HtmlRenderer.Parse;
using HtmlRenderer.Utils;

namespace HtmlRenderer
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class HtmlContainer
    {
        #region Fields and Consts

        /// <summary>
        /// the root css box of the parsed html
        /// </summary>
        private readonly CssBox _root;

        /// <summary>
        /// Handler for text selection in the html. 
        /// </summary>
        private readonly SelectionHandler _selectionHandler;

        /// <summary>
        /// used to resolve external references in html code (property, method calls)
        /// </summary>
        private readonly object _bridge;

        /// <summary>
        /// the parsed stylesheet data used for handling the html
        /// </summary>
        private readonly CssData _cssData;

        /// <summary>
        /// Gets or sets a value indicating if antialiasing should be avoided 
        /// for geometry like backgrounds and borders
        /// </summary>
        private bool _avoidGeometryAntialias;

        /// <summary>
        /// Gets or sets a value indicating if antialiasing should be avoided
        /// for text rendering
        /// </summary>
        private bool _avoidTextAntialias;

        /// <summary>
        /// the top-left most location of the rendered html
        /// </summary>
        private PointF _location;

        /// <summary>
        /// the max width and height of the rendered html, effects layout, actual size cannot exceed this values.<br/>
        /// Set zero for unlimited.<br/>
        /// </summary>
        private SizeF _maxSize;

        /// <summary>
        /// Gets or sets the scroll offset of the document for scroll controls
        /// </summary>
        private PointF _scrollOffset;

        /// <summary>
        /// The actual size of the rendered html (after layout)
        /// </summary>
        private SizeF _actualSize;
        
        #endregion


        /// <summary>
        /// Init with optinals document and stylesheet.
        /// </summary>
        /// <param name="htmlSource">the html to init with, init empty if not given</param>
        /// <param name="bridge">used to resolve external references in html code (property, method calls)</param>
        /// <param name="baseCssData">optional: the stylesheet to init with, init default if not given</param>
        public HtmlContainer(string htmlSource, object bridge, CssData baseCssData = null)
        {
            ArgChecker.AssertArgNotNullOrEmpty(htmlSource, "htmlSource");

            _bridge = bridge;
            _cssData = baseCssData ?? CssUtils.DefaultCssData;

            if(htmlSource != null)
            {
                _root = DomParser.GenerateCssTree(htmlSource, ref _cssData, bridge);
                if (_root != null)
                {
                    _root.HtmlContainer = this;
                    _selectionHandler = new SelectionHandler(_root);
                }
            }
        }

        /// <summary>
        /// Raised when the user clicks on a link in the html.<br/>
        /// Allows canceling the execution of the link.
        /// </summary>
        public event EventHandler<HtmlLinkClickedEventArgs> LinkClicked;

        /// <summary>
        /// the parsed stylesheet data used for handling the html
        /// </summary>
        public CssData CssData
        {
            get { return _cssData; }
        }

        /// <summary>
        /// used to resolve external references in html code (property, method calls).
        /// </summary>
        public object Bridge
        {
            get { return _bridge; }
        }

        /// <summary>
        /// Gets or sets a value indicating if antialiasing should be avoided for geometry like backgrounds and borders (default - false).
        /// </summary>
        public bool AvoidGeometryAntialias
        {
            get { return _avoidGeometryAntialias; }
            set { _avoidGeometryAntialias = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating if antialiasing should be avoided for text rendering (default - false).
        /// </summary>
        public bool AvoidTextAntialias
        {
            get { return _avoidTextAntialias; }
            set { _avoidTextAntialias = value; }
        }

        /// <summary>
        /// The scroll offset of the html
        /// </summary>
        public PointF ScrollOffset
        {
            get { return _scrollOffset; }
            set { _scrollOffset = value; }
        }

        /// <summary>
        /// the top-left most location of the rendered html
        /// </summary>
        public PointF Location
        {
            get { return _location; }
            set { _location = value; }
        }

        /// <summary>
        /// the max width and height of the rendered html, effects layout, actual size cannot exceed this values.<br/>
        /// Set zero for unlimited.
        /// </summary>
        public SizeF MaxSize
        {
            get { return _maxSize; }
            set { _maxSize = value; }
        }

        /// <summary>
        /// The actual size of the rendered html (after layout)
        /// </summary>
        public SizeF ActualSize
        {
            get { return _actualSize; }
            internal set { _actualSize = value; }
        }

        /// <summary>
        /// Get html from the current DOM tree with style if requested.
        /// </summary>
        /// <param name="styleGen">Optional: controls the way styles are generated when html is generated (default: <see cref="HtmlGenerationStyle.Inline"/>)</param>
        /// <returns>generated html</returns>
        public string GetHtml(HtmlGenerationStyle styleGen = HtmlGenerationStyle.Inline)
        {
            return DomUtils.GenerateHtml(_root);
        }

        /// <summary>
        /// Measures the bounds of box and children, recursively.
        /// </summary>
        /// <param name="g">Device context to draw</param>
        public void PerformLayout(Graphics g)
        {
            ArgChecker.AssertArgNotNull(g, "g");

            if (_root != null)
            {
                _root.Location = _location;
                _root.Size = new SizeF(_maxSize.Width > 0 ? _maxSize.Width : 9999, _maxSize.Height);
                _actualSize = new SizeF(0, 0);
                _root.MeasureBounds(g);
            }
        }

        /// <summary>
        /// Render the html using the given device.
        /// </summary>
        /// <param name="g">the device to use to render</param>
        public void PerformPaint(Graphics g)
        {
            ArgChecker.AssertArgNotNull(g, "g");

            Region prevClip = null;
            if (MaxSize.Height > 0)
            {
                prevClip = g.Clip;
                g.SetClip(new RectangleF(_location, _maxSize));
            }

            if (_root != null)
            {
                _root.Paint(g);
            }

            if (prevClip != null)
            {
                g.SetClip(prevClip, CombineMode.Replace);
            }
        }

        /// <summary>
        /// Handle mouse down to handle selection.
        /// </summary>
        /// <param name="parent">the control hosting the html to invalidate</param>
        /// <param name="e">the mouse event args</param>
        public void HandleMouseDown(Control parent, MouseEventArgs e)
        {
            ArgChecker.AssertArgNotNull(parent, "parent");
            ArgChecker.AssertArgNotNull(e, "e");

            if (_selectionHandler != null)
                _selectionHandler.HandleMouseDown(parent, OffsetByScroll(e.Location));
        }

        /// <summary>
        /// Handle mouse up to handle selection and link click.
        /// </summary>
        /// <param name="parent">the control hosting the html to invalidate</param>
        /// <param name="e">the mouse event args</param>
        public void HandleMouseUp(Control parent, MouseEventArgs e)
        {
            ArgChecker.AssertArgNotNull(parent, "parent");
            ArgChecker.AssertArgNotNull(e, "e");

            if (_selectionHandler != null)
            {
                var inSelection = _selectionHandler.HandleMouseUp(parent, e.Button);
                if (!inSelection && (e.Button & MouseButtons.Left) != 0)
                {
                    var loc = OffsetByScroll(e.Location);
                    var link = DomUtils.GetLinkBox(_root, loc);
                    if (link != null)
                    {
                        if (LinkClicked != null)
                        {
                            var args = new HtmlLinkClickedEventArgs(link.GetAttribute("href"));
                            LinkClicked(this, args);
                            if (args.Handled)
                            {
                                return;
                            }
                        }

                        CssValueParser.GoLink(link.GetAttribute("href", string.Empty), Bridge);
                    }
                }
            }
        }
        
        /// <summary>
        /// Handle mouse double click to select word under the mouse.
        /// </summary>
        /// <param name="parent">the control hosting the html to set cursor and invalidate</param>
        /// <param name="e">mouse event args</param>
        public void HandleMouseDoubleClick(Control parent, MouseEventArgs e)
        {
            ArgChecker.AssertArgNotNull(parent, "parent");
            ArgChecker.AssertArgNotNull(e, "e");

            if (_selectionHandler != null)
                _selectionHandler.SelectWord(parent, OffsetByScroll(e.Location));
        }

        /// <summary>
        /// Handle mouse move to handle hover cursor and text selection.
        /// </summary>
        /// <param name="parent">the control hosting the html to set cursor and invalidate</param>
        /// <param name="e">the mouse event args</param>
        public void HandleMouseMove(Control parent, MouseEventArgs e)
        {
            ArgChecker.AssertArgNotNull(parent, "parent");
            ArgChecker.AssertArgNotNull(e, "e");

            if (_selectionHandler != null) 
                _selectionHandler.HandleMouseMove(parent, OffsetByScroll(e.Location));
        }

        /// <summary>
        /// Handle key down event for selection and copy.
        /// </summary>
        /// <param name="parent">the control hosting the html to invalidate</param>
        /// <param name="e">the pressed key</param>
        public void HandleKeyDown(Control parent, KeyEventArgs e)
        {
            ArgChecker.AssertArgNotNull(parent, "parent");
            ArgChecker.AssertArgNotNull(e, "e");

            if (e.Control && _selectionHandler != null)
            {
                // select all
                if (e.KeyCode == Keys.A)
                {
                    _selectionHandler.SelectAll(parent);
                }

                // copy currently selected text
                if (e.KeyCode == Keys.C)
                {
                    _selectionHandler.CopySelectedHtml();
                }
            }
        }

        /// <summary>
        /// Adjust the offset of the given location by the current scroll offset.
        /// </summary>
        /// <param name="location">the location to adjust</param>
        /// <returns>the adjusted location</returns>
        private Point OffsetByScroll(Point location)
        {
            location.Offset(-(int)ScrollOffset.X, -(int)ScrollOffset.Y);
            return location;
        }
    }
}