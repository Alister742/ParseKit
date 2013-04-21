using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace Browser_Emulator
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AreaStyle style = AreaStyle.LoadStyles(Path.Combine(Application.StartupPath, "styles")) ?? new AreaStyle();
            Application.Run(new Area(style));
        }
    }
}
