using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net;
using System.Collections;
using System.Windows.Forms;
using System.Threading;
using System.Security.Permissions;
using HtmlRenderer.Parse;
using HtmlRenderer.Demo;

namespace HtmlRenderer
{
    class Program
    {
        //static object sync = new object();
        [STAThread]
        static void Main(string[] args)
        {
            //StreamReader sr = new StreamReader(File.Open(@"C:\Documents and Settings\Администратор\Мои документы\Visual Studio 2010\Projects\MineWorker\HtmlRenderer\YouTube.html", FileMode.Open), Encoding.UTF8);
            //string s = sr.ReadToEnd();
            //HtmlParser.ParseDocument(s);



            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DemoForm());
            //Application.Run(new EmptyArea());
            
            //Console.WriteLine("lol");
            //Console.Read();
        }
    }
}
