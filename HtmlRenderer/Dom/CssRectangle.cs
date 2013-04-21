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

namespace HtmlRenderer.Dom
{
    /// <summary>
    /// Encapsulate rectangle properties to be used by derived classes.
    /// </summary>
    internal abstract class CssRectangle
    {
        #region Fields and Consts
        
        /// <summary>
        /// Left of the rectangle
        /// </summary>
        private float _left;

        /// <summary>
        /// Top of the rectangle
        /// </summary>
        private float _top;

        /// <summary>
        /// Width of the rectangle
        /// </summary>
        private float _width;

        /// <summary>
        /// Height of the rectangle
        /// </summary>
        private float _height;        

        #endregion


        /// <summary>
        /// Left of the rectangle
        /// </summary>
        public float Left
        {
            get { return _left; }
            set { _left = value; }
        }

        /// <summary>
        /// Top of the rectangle
        /// </summary>
        public float Top
        {
            get { return _top; }
            set { _top = value; }
        }

        /// <summary>
        /// Width of the rectangle
        /// </summary>
        public float Width
        {
            get { return _width; }
            set { _width = value; }
        }

        /// <summary>
        /// Height of the rectangle
        /// </summary>
        public float Height
        {
            get { return _height; }
            set { _height = value; }
        }

        /// <summary>
        /// Gets or sets the right of the rectangle. When setting, it only affects the Width of the rectangle.
        /// </summary>
        public float Right
        {
            get { return Bounds.Right; }
            set { Width = value - Left; }
        }

        /// <summary>
        /// Gets or sets the bottom of the rectangle. When setting, it only affects the Height of the rectangle.
        /// </summary>
        public float Bottom
        {
            get { return Bounds.Bottom; }
            set { Height = value - Top; }
        }

        /// <summary>
        /// Gets or sets the bounds of the rectangle
        /// </summary>
        public RectangleF Bounds
        {
            get { return new RectangleF(Left, Top, Width, Height); }
            set
            {
                Left = value.Left; 
                Top = value.Top; 
                Width = value.Width; 
                Height = value.Height;
            }
        }

        /// <summary>
        /// Gets or sets the location of the rectangle
        /// </summary>
        public PointF Location
        {
            get { return new PointF(Left, Top); }
            set
            {
                Left = value.X; 
                Top = value.Y;
            }
        }

        /// <summary>
        /// Gets or sets the size of the rectangle
        /// </summary>
        public SizeF Size
        {
            get { return new SizeF(Width, Height); }
            set
            {
                Width = value.Width; 
                Height = value.Height;
            }
        }
    }
}
