using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ParseKit.CORE
{
    /// <summary>
    /// Not Thread safe for more faster work
    /// </summary>
    class FileWorker
    {
        public static Encoding Encoding = Encoding.UTF8;

        public static void DeleteFile(string fullPath)
        {
            try
            {
                File.Delete(fullPath);
            }
            catch (Exception)
            { }
        }

        public static void Save(string fullPath, List<string> lines, bool append = true)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(fullPath, append, Encoding))
                {
                    foreach (var line in lines)
                    {
                        sw.WriteLine(line);
                    }
                }
            }
            catch (Exception)
            { }
        }

        public static void Save(string fullPath, string line, bool append = true)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(fullPath, append, Encoding))
                {
                    sw.WriteLine(line);
                }
            }
            catch (Exception)
            { }
        }

        public static List<string> Load(string fullPath)
        {
            List<string> lines = new List<string>();
            try
            {
                using (StreamReader sr = new StreamReader(fullPath, Encoding))
                {
                    while (!sr.EndOfStream)
                    {
                        lines.Add(sr.ReadLine());
                    }
                }
                return lines;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
