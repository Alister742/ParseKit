using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace Browser_Emulator
{
    enum GraphicObjType
    {
        Line,
        Rectangle,
        FilledRectangle,
    }

    class GDIPainter : IPainter
    {
        Control SceneControl;
        AutoResetEvent _waiter;
        List<object[]> _drawedObjects = new List<object[]>();

        public Point GetStartLocation
        {
            get { return new Point(0, 0); }
        }

        public Size GetSceneArea
        {
            get { return new Size(SceneControl.Width, SceneControl.Height); }
        }

        public GDIPainter(Control sceneControl)
        {
            SceneControl = sceneControl;
            _waiter = new AutoResetEvent(false);
        }

        private void CheckLocation(ref Point p)
        {
            int minX = SceneControl.Location.X + 1;
            int minY = SceneControl.Location.Y + 1;
            int maxX = minX + SceneControl.Width - 2;
            int maxY = minY + SceneControl.Height - 2;

            if (p.X < minX)
                p.X = minX;
            else if (p.X > maxX)
                p.X = maxX;

            if (p.Y < minY)
                p.Y = minY;
            else if (p.Y > maxY)
                p.Y = maxY;
        }

        private void CheckSize(ref Point start, ref Size size)
        {
            int minX = SceneControl.Location.X + 1;
            int minY = SceneControl.Location.Y + 1;
            int maxX = minX + SceneControl.Width - 2;
            int maxY = minY + SceneControl.Height - 2;

            if (start.X + size.Width < minX)
                size.Width = start.X - minX;
            else if (start.X + size.Width > maxX)
                size.Width = maxX - start.X;

            if (start.Y + size.Height < minY)
                size.Height = start.Y - minY;
            else if (start.Y + size.Height > maxY)
                size.Height = maxY - start.Y;
        }

        private void ValidateCoordinates(ref Point start, ref Size size)
        {
            CheckLocation(ref start);
            CheckSize(ref start, ref size);
        }

        public void DrawLine(Point start, Point end, Color color, int weight)
        {
            CheckLocation(ref start);
            CheckLocation(ref end);

            start = SceneControl.PointToScreen(start);
            end = SceneControl.PointToScreen(end);

            _drawedObjects.Add(new object[] { start, end });
            DrawLine(start, end, color, weight, null);
        }

        private void DrawLine(Point start, Point end, Color color, int weight, string temp = null)
        {
            start.Y -= weight / 2;
            //start.X += weight / 2;
            end.Y -= weight / 2;
            //end.X += -weight / 2;

            for (int i = 0; i < weight; i++)
            {
                ControlPaint.DrawReversibleLine(start, end, color);
                start.Y++;
                end.Y++;
            }
        }

        public void DrawRectangle(Point start, Size size, Color color, FrameStyle style)
        { 
            ValidateCoordinates(ref start, ref size);
            start = SceneControl.PointToScreen(start);

            _drawedObjects.Add(new object[] { start, size, style });
            DrawRectangle(start, size, color, style, null);
        }

        public void DrawRectangle(Point start, Size size, Color color, FrameStyle style, string temp = null)
        {
            ControlPaint.DrawReversibleFrame(new Rectangle(start, size), color, style);
        }

        public void DrawFilledRectangle(Point start, Size size, Color color)
        {
            ValidateCoordinates(ref start, ref size);

            start = SceneControl.PointToScreen(start);

            ControlPaint.FillReversibleRectangle(new Rectangle(start, size), color);
        }

        public void Clean()
        {
            foreach (var item in _drawedObjects)
            {
                Point start = (Point)item[0];
                if (item[1] is Size)
                {
                    DrawRectangle((Point)item[0], (Size)item[1], Color.Yellow, (FrameStyle)item[2], null);
                }
                else
                    DrawLine((Point)item[0], (Point)item[1], Color.Yellow, 1, null);
            }
            _drawedObjects.Clear();
            //SceneControl.Invalidate(true);
        }
    }
}
