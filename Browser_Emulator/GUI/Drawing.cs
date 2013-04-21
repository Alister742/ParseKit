using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Browser_Emulator
{
    public class Drawing
    {
        IPainter _painter;

        public Drawing(IPainter painter)
        {
            _painter = painter;
        }

        public void DrawFocusedArea(Point start, Size size, FrameStyle style)
        {
            DrawFocusLines(start, size);
            DrawRectangle(start, size, style);
        }

        public void DrawFocusedArea(Rectangle rect, FrameStyle style)
        {
            DrawFocusedArea(rect.Location, rect.Size, style);
        }

        void DrawFocusLines(Point start, Size size)
        {
            Point sceneStart = new Point(0, 0);
            Size scene = _painter.GetSceneArea;

            // l -- Line, S/E -- point position, Up/Left/Right/Bottom - line orientation,  X/Y -- coordinate
            int l_S_Up_X = sceneStart.X;
            int l_S_UP_Y = start.Y;
            int l_E_Up_X = sceneStart.X + scene.Width;
            int l_E_Up_Y = start.Y;

            int l_S_L_X = start.X;
            int l_S_L_Y = sceneStart.Y;
            int l_E_L_X = start.X;
            int l_E_L_Y = sceneStart.Y + scene.Height;

            int l_S_R_X = start.X + size.Width;
            int l_S_R_Y = sceneStart.Y;
            int l_E_R_X = start.X + size.Width;
            int l_E_R_Y = sceneStart.Y + scene.Height;

            int l_S_B_X = sceneStart.X;
            int l_S_B_Y = start.Y + size.Height;
            int l_E_B_X = sceneStart.X + scene.Width;
            int l_E_B_Y = start.Y + size.Height;

            _painter.DrawLine(new Point(l_S_Up_X, l_S_UP_Y), new Point(l_E_Up_X, l_E_Up_Y), Color.Yellow, 1);
            _painter.DrawLine(new Point(l_S_L_X, l_S_L_Y), new Point(l_E_L_X, l_E_L_Y), Color.Yellow, 1);
            _painter.DrawLine(new Point(l_S_R_X, l_S_R_Y), new Point(l_E_R_X, l_E_R_Y), Color.Yellow, 1);
            _painter.DrawLine(new Point(l_S_B_X, l_S_B_Y), new Point(l_E_B_X, l_E_B_Y), Color.Yellow, 1);
        }

        public void DrawRectangle(Point start, Size size, FrameStyle style)
        {
            _painter.DrawRectangle(start, size, Color.Yellow, style);
        }

        public void DrawRectangle(Rectangle rect, FrameStyle style)
        {
            DrawRectangle(rect.Location, rect.Size, style);
        }

        public void DrawFilledArea(Point start, Size size)
        {
            _painter.DrawFilledRectangle(start, size, Color.Yellow);
        }

        public void DrawFilledArea(Rectangle rect)
        {
            DrawFilledArea(rect.Location, rect.Size);
        }

        public void Clean()
        {
            _painter.Clean();
        }
    }
}
