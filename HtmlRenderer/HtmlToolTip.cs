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
using System.Windows.Forms;
using System.Globalization;

namespace HtmlRenderer
{
    /// <summary>
    /// Provides HTML rendering on the tooltips
    /// </summary>
    public class HtmlToolTip : ToolTip
    {
        #region Fields and Consts

        /// <summary>
        /// 
        /// </summary>
        private HtmlContainer _container;

        /// <summary>
        /// used to resolve external references in html code (property, method calls)
        /// </summary>
        private object _bridge;

        #endregion

        /// <summary>
        /// Init.
        /// </summary>
        public HtmlToolTip()
        {
            OwnerDraw = true;

            Popup += OnToolTipPopup;
            Draw += OnToolTipDraw;
        }

        /// <summary>
        /// used to resolve external references in html code (property, method calls)
        /// </summary>
        public object Bridge
        {
            get { return _bridge; }
            set { _bridge = value; }
        }

        private void OnToolTipPopup(object sender, PopupEventArgs e)
        {
            string text = GetToolTip(e.AssociatedControl);
            string font = string.Format(NumberFormatInfo.InvariantInfo, "font: {0}pt {1}", e.AssociatedControl.Font.Size, e.AssociatedControl.Font.FontFamily.Name);
            
            //Create fragment container
            var documentSource = "<div><table class=htmltooltipbackground cellspacing=5 cellpadding=0 style=\"" + font + "\"><tr><td style=border:0px>" + text + "</td></tr></table></div>";
            _container = new HtmlContainer(documentSource, Bridge);
            _container.AvoidGeometryAntialias = true;
            
            //Measure bounds of the container
            using (Graphics g = e.AssociatedControl.CreateGraphics())
            {
                _container.PerformLayout(g);
            }

            //Set the size of the tooltip
            e.ToolTipSize = new Size((int) Math.Round(_container.ActualSize.Width, MidpointRounding.AwayFromZero), (int) Math.Round(_container.ActualSize.Height, MidpointRounding.AwayFromZero));
        }

        private void OnToolTipDraw(object sender, DrawToolTipEventArgs e)
        {
            e.Graphics.Clear(Color.White);

            if (_container != null)
            {
                //Draw HTML!
                _container.PerformPaint(e.Graphics);
            }
        }

        private void InitializeComponent()
        {

        }
    }
}
