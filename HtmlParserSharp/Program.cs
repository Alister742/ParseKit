using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Text;

namespace HtmlParserSharp
{
	/// <summary>
	/// This is contains a sample entry point for testing and benchmarks.
	/// </summary>
	public class Program
	{
		static SimpleHtmlParser parser = new SimpleHtmlParser();

		private static IEnumerable<FileInfo> GetTestFiles()
		{
			//DirectoryInfo dir = new DirectoryInfo("SampleData");
			//return dir.GetFiles("*.html", SearchOption.AllDirectories);
			for (int i = 0; i < 10; i++)
			{
				yield return new FileInfo(Path.Combine("SampleData", "test.html"));
			}
		}

		public static void Main(string[] args)
		{

			Console.Write("Parsing ... ");
            var result = GetTestFiles().Select((file) =>
                {
                    var doc = parser.Parse(file.FullName);
                    doc.Save("test.xml");
                    XDocument.Load("test.xml");
                    return doc;
                }).ToList();

            foreach (var item in result)
            {
                XmlDocument dc = (XmlDocument)item;
                printChilds(dc.ChildNodes);
            }


            using (StreamReader sr = new StreamReader(@"C:\Documents and Settings\Администратор\Мои документы\Visual Studio 2010\Projects\MineWorker\HtmlParserSharp\SampleData\test.html", Encoding.UTF8))
            {
                var doc = parser.Parse(@"C:\Documents and Settings\Администратор\Мои документы\Visual Studio 2010\Projects\MineWorker\HtmlParserSharp\SampleData\test.html");

                printChilds(doc.ChildNodes);
            }

			Console.WriteLine("done.");
			Console.ReadKey();
		}

        static void printChilds(XmlNodeList childs)
        {
            if (childs == null || childs.Count == 0)
                return;

            foreach (XmlNode child in childs)
            {
                Console.WriteLine(child.Name);

                XmlNodeList childsOfChild = child.ChildNodes;
                int lenght = childsOfChild.Count;

                printChilds(childsOfChild);
            }
        }
	}
}
