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
using System.Windows.Forms;
using HtmlRenderer.Entities;
using HtmlRenderer.Parse;

namespace HtmlRenderer
{
    /// <summary>
    /// You can use it just as an HTML container, just place the panel on a form and feed the <see cref="Text"/> property with HTML code.
    /// </summary>
    public class HtmlPanel : ScrollableControl
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

        string _html;

        /// <summary>
        /// the raw base stylesheet data used in the control
        /// </summary>
        private string _baseRawCssData;

        /// <summary>
        /// the base stylesheet data used in the control
        /// </summary>
        private CssData _baseCssData;

        #endregion


        /// <summary>
        /// Creates a new HtmlPanel and sets a basic css for it's styling.
        /// </summary>
        public HtmlPanel()
        {
            AutoScroll = true;
            BackColor = SystemColors.Window;
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
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
        [Category("Appearance")]
        [Description("Set base stylesheet to be used by html rendered in the control.")]
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
        /// Gets or sets a value indicating whether the container enables the user to scroll to any controls placed outside of its visible boundaries. 
        /// </summary>
        [Browsable(true)]
        [Description("Sets a value indicating whether the container enables the user to scroll to any controls placed outside of its visible boundaries.")]
        public override bool AutoScroll
        {
            get{return base.AutoScroll;}
            set { base.AutoScroll = value; }
        }

        /// <summary>
        /// Gets or sets the text of this panel
        /// </summary>
        [Browsable(true)]
        [Description("Sets the html of this control.")]
        public override string Text
        {
            get { return _html; }
            set
            {
                _html = value;
                if (!IsDisposed && _html != null)
                {
                    VerticalScroll.Value = VerticalScroll.Minimum;
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
            if(_htmlContainer != null)
            {
                _htmlContainer.LinkClicked -= OnLinkClicked;
                _htmlContainer = null;
            }
            
            _htmlContainer = new HtmlContainer(Text, _bridge, _baseCssData);
            if(LinkClicked != null)
            {
                _htmlContainer.LinkClicked += OnLinkClicked;
            }
        }

        /// <summary>
        /// Perform the layout of the html in the control.
        /// </summary>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            PerformHtmlLayout();

            base.OnLayout(levent);

            // to handle if vertical scrollbar is appearing or disappearing
            if (_htmlContainer != null && Math.Abs(_htmlContainer.MaxSize.Width - ClientSize.Width) > 0.1)
            {
                PerformHtmlLayout();
                base.OnLayout(levent);
            }
        }

        /// <summary>
        /// Perform html container layout by the current panel client size.
        /// </summary>
        private void PerformHtmlLayout()
        {
            if (_htmlContainer != null)
            {
                _htmlContainer.MaxSize = new SizeF(ClientSize.Width,0);

                using (Graphics g = CreateGraphics())
                {
                    _htmlContainer.PerformLayout(g);
                }

                AutoScrollMinSize = Size.Round(_htmlContainer.ActualSize);
            }
        }

        /// <summary>
        /// Perform paint of the html in the control.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_htmlContainer != null)
            {
                _htmlContainer.ScrollOffset = AutoScrollPosition;
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                _htmlContainer.PerformPaint(e.Graphics);
            }
        }

        /// <summary>
        /// Set focus on the control for keyboard scrrollbars handling.
        /// </summary>
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Focus();
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
        /// Handle key down event for selection, copy and scrollbars handling.
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (_htmlContainer != null)
                _htmlContainer.HandleKeyDown(this, e);
            if (e.KeyCode == Keys.Up)
            {
                VerticalScroll.Value = Math.Max(VerticalScroll.Value - 70, VerticalScroll.Minimum);
                PerformLayout();                
            }
            else if (e.KeyCode == Keys.Down)
            {
                VerticalScroll.Value = Math.Min(VerticalScroll.Value + 70, VerticalScroll.Maximum);
                PerformLayout();
            }
            else if (e.KeyCode == Keys.PageDown)
            {
                VerticalScroll.Value = Math.Min(VerticalScroll.Value + 400, VerticalScroll.Maximum);
                PerformLayout();
            }
            else if (e.KeyCode == Keys.PageUp)
            {
                VerticalScroll.Value = Math.Max(VerticalScroll.Value - 400, VerticalScroll.Minimum);
                PerformLayout();
            }
            else if (e.KeyCode == Keys.End)
            {
                VerticalScroll.Value = VerticalScroll.Maximum;
                PerformLayout();
            }
            else if (e.KeyCode == Keys.Home)
            {
                VerticalScroll.Value = VerticalScroll.Minimum;
                PerformLayout();
            }
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

        /// <summary>
        /// Used to add arrow keys to the handled keys in <see cref="OnKeyDown"/>.
        /// </summary>
        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                    return true;
                case Keys.Shift | Keys.Right:
                case Keys.Shift | Keys.Left:
                case Keys.Shift | Keys.Up:
                case Keys.Shift | Keys.Down:
                    return true;
            }
            return base.IsInputKey(keyData);
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