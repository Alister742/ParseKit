using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Browser_Emulator
{
    public interface IPainter
    {
        void DrawLine(Point start, Point end, Color color, int depth);
        void DrawRectangle(Point start, Size size, Color color, FrameStyle style);
        //void DrawRectangle(Rectangle rect, Color color, FrameStyle style);
        void DrawFilledRectangle(Point start, Size size, Color color);
        //void DrawFilledRectangle(Rectangle rect, Color color);
        void Clean();
        Point GetStartLocation { get; }
        Size GetSceneArea { get; }
    }
}
