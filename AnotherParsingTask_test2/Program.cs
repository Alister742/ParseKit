using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace AnotherParsingTask_test2
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Process proc = Process.GetProcessesByName("EXCEL")[0];
                proc.Kill();
            }
            catch (Exception)
            {
            }

            new Updater().Update(args[0], args[1]);
            Console.Read();
        }
    }
}
