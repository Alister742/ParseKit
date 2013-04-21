using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using ParseKit.CORE;


namespace Browser_Emulator
{
    //public struct Location
    //{
    //    int X;
    //    int Y;

    //    public Location(int x, int y)
    //    {
    //        X = x;
    //        Y = y;
    //    }
    //}

    //public struct Size
    //{
    //    int Width;
    //    int Height;

    //    public Size(int width, int height)
    //    {
    //        Width = width;
    //        Height = height;
    //    }
    //}

    public class AreaStyle
    {
        public bool Maximized { get; set; }
        public Size Size { get; set; }
        public Point Location { get; set; }
        public int BotPanelWidth { get; set; }
        public int PropertyPanelWidth { get; set; }
        public int HtmlWindowHeight { get; set; }
        public int BrowserHeight { get; set; }

        public AreaStyle()
        {
            LoadDefault();
        }

        //public AreaStyle(bool maximized, Size size, Point location, int botPanelWidth, int propertyPanelWidth, int tagTreeHeight, int browserHeight)
        //{
        //            public bool Maximized { get; set; }
        //public Size Size { get; set; }
        //public Point Location { get; set; }
        //public int BotPanelWidth { get; set; }
        //public int PropertyPanelWidth { get; set; }
        //public int TagTreeHeight { get; set; }
        //public int BrowserHeight { get; set; }
        //    LoadDefault();
        //}

        private void LoadDefault()
        {
            Maximized = true;
            Size = new Size(1083, 618);
            Location = new Point(100, 100);
            BotPanelWidth = 100;
            BrowserHeight = 650;
            PropertyPanelWidth = 200;
            HtmlWindowHeight = 50;
        }

        public static AreaStyle LoadStyles(string fullPath)
        {
            List<string> lines = FileWorker.Load(fullPath);

            if (lines != null)
            {
                try
                {
                    AreaStyle styles = new AreaStyle();
                    styles.Maximized = GetValueFromfileStr(lines[0]) == "1" ? true : false;

                    string[] sizeP = GetValueFromfileStr(lines[1]).Split(',');
                    styles.Size = new Size(Int32.Parse(sizeP[0]), Int32.Parse(sizeP[1]));

                    string[] locationP = GetValueFromfileStr(lines[2]).Split(',');
                    styles.Location = new Point(Int32.Parse(locationP[0]), Int32.Parse(locationP[1]));

                    styles.BotPanelWidth = Int32.Parse(GetValueFromfileStr(lines[3]));
                    styles.PropertyPanelWidth = Int32.Parse(GetValueFromfileStr(lines[4]));
                    styles.HtmlWindowHeight = Int32.Parse(GetValueFromfileStr(lines[5]));
                    styles.BrowserHeight = Int32.Parse(GetValueFromfileStr(lines[6]));
                    
                    return styles;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        private static string GetValueFromfileStr(string str)
        {
            int startPos = str.IndexOf('=');
            int endPos =  str.IndexOf(';');
            if (endPos > startPos && startPos != -1 && endPos != -1)
            {
                return str.Substring(startPos + 1, endPos - startPos - 1).Trim();
            }
            else
                return null;
        }

        public static void SaveStyles(string fullPath, AreaStyle styles)
        {
            List<string> lines = new List<string>();

            lines.Add(string.Format("Maximized={0};", styles.Maximized ? 1 : 0));
            lines.Add(string.Format("Size={0},{1};", styles.Size.Width, styles.Size.Height));
            lines.Add(string.Format("Location={0},{1};", styles.Location.X, styles.Location.Y));
            lines.Add(string.Format("BotPanelWidth={0};", styles.BotPanelWidth));
            lines.Add(string.Format("PropertyPanelWidth={0};", styles.PropertyPanelWidth));
            lines.Add(string.Format("TagTreeHeight={0};", styles.HtmlWindowHeight));
            lines.Add(string.Format("BrowserHeight={0};", styles.BrowserHeight));

            FileWorker.Save(fullPath, lines, false); 
        }
    }
}
