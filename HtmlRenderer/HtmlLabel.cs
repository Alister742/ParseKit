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
using System.ComponentModel;
using System.Drawing.Text;
using System.Windows.Forms;
using HtmlRenderer.Entities;
using HtmlRenderer.Parse;

namespace HtmlRenderer
{
    /// <summary>
    /// Provides HTML rendering on the text of the label
    /// </summary>
    public class HtmlLabel : Control
    {
        #region Fields and Consts

        /// <summary>
        /// 
        /// </summary>
        private HtmlContainer _htmlContainer;

        /// <summary>
        /// used to resolve external references in html code (property, method calls)
        /// </summary>
        private object _bridge;

        /// <summary>
        /// the raw base stylesheet data used in the control
        /// </summary>
        private string _baseRawCssData;

        /// <summary>
        /// the base stylesheet data used in the panel
        /// </summary>
        private CssData _baseCssData;

        /// <summary>
        /// is to handle auto size of the control height only
        /// </summary>
        private bool _autoSizeHight;

        #endregion

        
        /// <summary>
        /// Creates a new HTML Label
        /// </summary>
        public HtmlLabel()
        {
            AutoSize = true;
            BackColor = SystemColors.Window;
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(System.Windows.Forms.ControlStyles.Opaque, false);
        }

        /// <summary>
        /// Raised when the user clicks on a link in the html.<br/>
        /// Allows canceling the execution of the link.
        /// </summary>
        public event EventHandler<HtmlLinkClickedEventArgs> LinkClicked;

        /// <summary>
        /// used to resolve external references in html code (property, method calls)
        /// </summary>
        [Browsable(false)]
        public object Bridge
        {
            get { return _bridge; }
            set { _bridge = value; }
        }

        /// <summary>
        /// Set base stylesheet to be used by html rendered in the panel.
        /// </summary>
        [Browsable(true)]
        [Description("Set base stylesheet to be used by html rendered in the control.")]
        [Category("Appearance")]
        public string BaseStylesheet
        {
            get { return _baseRawCssData; }
            set
            {
                _baseRawCssData = value;
                _baseCssData = CssParser.ParseStyleSheet(value, true);
            }
        }

        /// <summary>
        /// Automatically sets the size of the label by content size
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Automatically sets the size of the label by content size.")]
        public override bool AutoSize
        {
            get { return base.AutoSize; }
            set
            {
                base.AutoSize = value;
                if (value)
                {
                    _autoSizeHight = false;
                    PerformLayout();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Automatically sets the height of the label by content height (width is not effected).
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Layout")]
        [Description("Automatically sets the height of the label by content height (width is not effected)")]
        public bool AutoSizeHeightOnly
        {
            get { return _autoSizeHight; }
            set
            {
                _autoSizeHight = value;
                if (value)
                {
                    AutoSize = false;
                    PerformLayout();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the max size the control get be set by <see cref="AutoSize"/> or <see cref="AutoSizeHeightOnly"/>.
        /// </summary>
        /// <returns>An ordered pair of type <see cref="T:System.Drawing.Size"/> representing the width and height of a rectangle.</returns>
        [Description("If AutoSize or AutoSizeHeightOnly is set this will restrict the max size of the control (0 is not restricted)")]
        public override Size MaximumSize
        {
            get { return base.MaximumSize; }
            set
            {
                base.MaximumSize = value;
                if (_htmlContainer != null)
                {
                    _htmlContainer.MaxSize = value;
                    PerformLayout();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the min size the control get be set by <see cref="AutoSize"/> or <see cref="AutoSizeHeightOnly"/>.
        /// </summary>
        /// <returns>An ordered pair of type <see cref="T:System.Drawing.Size"/> representing the width and height of a rectangle.</returns>
        [Description("If AutoSize or AutoSizeHeightOnly is set this will restrict the min size of the control (0 is not restricted)")]
        public override Size MinimumSize
        {
            get { return base.MinimumSize; }
            set { base.MinimumSize = value; }
        }

        /// <summary>
        /// Gets or sets the html of this control.
        /// </summary>
        [Description("Sets the html of this control.")]
        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                if (!IsDisposed)
                {
                    CreateHtmlContainer();
                    PerformLayout();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Get html from the current DOM tree with inline style.
        /// </summary>
        /// <returns>generated html</returns>
        public string GetHtml()
        {
            return _htmlContainer != null ? _htmlContainer.GetHtml() : null;
        }


        #region Private methods

        /// <summary>
        /// Creates new Html Container to handle the html in the control.
        /// </summary>
        private void CreateHtmlContainer()
        {
            if (_htmlContainer != null)
            {
                _htmlContainer.LinkClicked -= OnLinkClicked;
                _htmlContainer = null;
            }

            _htmlContainer = new HtmlContainer(Text, Bridge, _baseCssData);
            _htmlContainer.MaxSize = MaximumSize;
            if (LinkClicked != null)
            {
                _htmlContainer.LinkClicked += OnLinkClicked;
            }
        }

        /// <summary>
        /// Perform the layout of the html in the control.
        /// </summary>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (_htmlContainer != null)
            {
                if (AutoSize)
                    _htmlContainer.MaxSize = MaximumSize;
                else if (AutoSizeHeightOnly)
                    _htmlContainer.MaxSize = new SizeF(Width, 0);
                else
                    _htmlContainer.MaxSize = Size;

                using (Graphics g = CreateGraphics())
                {
                    _htmlContainer.PerformLayout(g);
                }

                if(AutoSize || _autoSizeHight)
                {
                    if (AutoSize)
                    {
                        Size = Size.Round(_htmlContainer.ActualSize);

                        if (MaximumSize.Width > 0 && MaximumSize.Width < Width)
                            Width = MaximumSize.Width;

                        if (MinimumSize.Width > 0 && MinimumSize.Width > Width)
                            Width = MinimumSize.Width;
                    }
                    else if (_autoSizeHight)
                    {
                        Height = (int)_htmlContainer.ActualSize.Height;
                    }

                    if (MaximumSize.Height > 0 && MaximumSize.Height < Height)
                        Height = MaximumSize.Height;

                    if (MinimumSize.Height > 0 && MinimumSize.Height > Height)
                        Height = MinimumSize.Height;
                }
            }

            base.OnLayout(levent);
        }

        /// <summary>
        /// Perform paint of the html in the control.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_htmlContainer != null)
            {
                e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                _htmlContainer.PerformPaint(e.Graphics);
            }
        }

        /// <summary>
        /// Handle mouse move to handle hover cursor and text selection. 
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_htmlContainer != null)
                _htmlContainer.HandleMouseMove(this, e);
        }

        /// <summary>
        /// Handle mouse down to handle selection. 
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (_htmlContainer != null)
                _htmlContainer.HandleMouseDown(this, e);
        }

        /// <summary>
        /// Handle mouse up to handle selection and link click. 
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (_htmlContainer != null)
                _htmlContainer.HandleMouseUp(this, e);
        }

        /// <summary>
        /// Handle mouse double click to select word under the mouse. 
        /// </summary>
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (_htmlContainer != null)
                _htmlContainer.HandleMouseDoubleClick(this, e);
        }

        /// <summary>
        /// Propogate the LinkClicked event from root container.
        /// </summary>
        protected void OnLinkClicked(object sender, HtmlLinkClickedEventArgs e)
        {
            if (LinkClicked != null)
            {
                LinkClicked(this, e);
            }
        }


        #region Hide not relevant properties from designer

        /// <summary>
        /// Not applicable.
        /// </summary>
        [Browsable(false)]
        public override Font Font
        {
            get { return base.Font; }
            set { base.Font = value; }
        }

        /// <summary>
        /// Not applicable.
        /// </summary>
        [Browsable(false)]
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set { base.ForeColor = value; }
        }

        /// <summary>
        /// Not applicable.
        /// </summary>
        [Browsable(false)]
        public override bool AllowDrop
        {
            get { return base.AllowDrop; }
            set { base.AllowDrop = value; }
        }

        /// <summary>
        /// Not applicable.
        /// </summary>
        [Browsable(false)]
        public override RightToLeft RightToLeft
        {
            get { return base.RightToLeft; }
            set { base.RightToLeft = value; }
        }

        /// <summary>
        /// Not applicable.
        /// </summary>
        [Browsable(false)]
        public override Cursor Cursor
        {
            get { return base.Cursor; }
            set { base.Cursor = value; }
        }

        /// <summary>
        /// Not applicable.
        /// </summary>
        [Browsable(false)]
        public new bool UseWaitCursor
        {
            get { return base.UseWaitCursor; }
            set { base.UseWaitCursor = value; }
        }

        #endregion


        #endregion
    }
}
