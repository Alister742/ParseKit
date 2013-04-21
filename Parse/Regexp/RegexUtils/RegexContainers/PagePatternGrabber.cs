using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using ParseKit;

namespace Parse.Regexp
{
    class PagePatternGrabber
    {
        static object sync = new object();

        static Regex classRx = new Regex(@"<(?<tag>\w+)([^>]*)(?=class=)class=[""|'](?<class>[^""|^']*)[""|']", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static List<string> GrabClassStructure(string page)
        {
            HashSet<string> pageTags = new HashSet<string>();
            MatchCollection tagsClasses = classRx.Matches(page);

            foreach (Match tagClass in tagsClasses)
            {
                string tagClassStr = tagClass.Groups["tag"].Value + "_" + tagClass.Groups["class"].Value.DeleteEmpties();
                pageTags.Add(tagClassStr);
            }
            return pageTags.ToList();
        }

        #region Patterns File Managing
        public void SavePagePattern(List<string> patterns, string uri, string path)
        {
            StringBuilder sb = new StringBuilder(uri);
            foreach (var pattern in patterns)
		        sb.Append('|' + pattern);
            try 
	        {	        
		        lock (sync)
                {
                    using (StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8))
                    {
                        sw.WriteLine(sb);
                    } 
                }
	        }
	        catch (Exception)
	        {
		        return;
	        }
        }

        public PatternsContainer LoadPagePattern(string uristr, string path)
        {
            List<string> patterns = new List<string>();
            try
            {
                lock (sync)
                {
                    using (StreamReader sw = new StreamReader(path, Encoding.UTF8))
                    {
                        while (!sw.EndOfStream)
                        {
                            string[] uriAndPatt = sw.ReadLine().Split();
                            if (uriAndPatt.Length < 2)
                                continue;

                            string line = sw.ReadLine();

                            if (uriAndPatt[0] == uristr)
                            {
                                for (int i = 1; i < uriAndPatt.Length; i++)
                                {
                                    patterns.Add(uriAndPatt[i]);
                                }
                            }
                        }
                    }
                }
                Uri uri = UriHandler.CreateUri(uristr);
                if (uri == null) 
                    return null;

                return new PatternsContainer(uri,  patterns);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<PatternsContainer> LoadPatterns(string path)
        {
            List<PatternsContainer> paterns = new List<PatternsContainer>();
            try
            {
                lock (sync)
                {
                    using (StreamReader sw = new StreamReader(path, Encoding.UTF8))
                    {
                        while (!sw.EndOfStream)
                        {
                            string[] uriAndPatt = sw.ReadLine().Split('|');

                            if (uriAndPatt.Length < 2)
                                continue;

                            Uri uri = UriHandler.CreateUri(uriAndPatt[0]);
                            if (uri == null)
                                continue;

                            List<string> pattList = new List<string>();
                            for (int i = 1; i < uriAndPatt.Length; i++)
                            {
                                pattList.Add(uriAndPatt[i]);
                            }
                            paterns.Add(new PatternsContainer(uri, pattList));
                        }
                    }
                }
                return paterns;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
    }
}
