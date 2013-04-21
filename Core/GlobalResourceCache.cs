using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;




namespace Core
{
    static class GlobalResourceCache
    {
        public static object sync = new object();

        static List<string[]> _rblPatterns;
        static List<string[]> _sePatterns;
        static SePatterns _yaCheck;
        static SePatterns _GoogCheck;
        static AnonymousRegexes _anonChecks;

        public static List<RatedProxy> Proxies { get; set; }

        public static AnonymousRegexes AnonymCheck
        {
            get
            {
                if (_anonChecks == null)
                {
                    List<string[]> anonSets = LoadValidFileData(PATH.AnonPatterns, 3, "|");

                    if (anonSets != null && anonSets.Count > 0)
                    {
                        string[] anonSet = anonSets[0];
                        _anonChecks = new AnonymousRegexes(anonSet[0], anonSet[1], anonSet[2]);
                    }
                }
                return _anonChecks;
            }
        }

        public static SePatterns YaPagePattern 
        {
            get
            {
                if (_yaCheck == null)
                {
                    LoadSePatterns();
                }
                return _yaCheck;
            }
        }

        public static SePatterns GooglePagePattern
        {
            get
            {
                if (_GoogCheck == null)
                {
                    LoadSePatterns();
                }
                return _GoogCheck;
            }
        }

        public static void LoadSePatterns()
        {
            _sePatterns = LoadValidFileData(PATH.SEPatterns, 3, "[^?]");
            foreach (var set in _sePatterns)
            {
                if (set[0] == "Ya")
                    _yaCheck = new SePatterns(set[1], set[2]);
                else if (set[0] == "Goo")
                    _GoogCheck = new SePatterns(set[1], set[2]);
            }
        }

        public static List<string[]> RBLList
        {
            get
            {
                if (_rblPatterns == null)
                {
                    _rblPatterns = LoadValidFileData(PATH.RBLSites, 3, "|");
                }
                return _rblPatterns;
            }
        }

        static List<string[]> LoadValidFileData(string path, int column, string separator)
        {
            string[] splitPatterns = { separator };
            try
            {
                List<string[]> setList = new List<string[]>();
                lock (sync)
                {
                    List<string> lines = FileWorker.Load(path);

                    foreach (var line in lines)
                    {
                        string[] set = line.Split(splitPatterns, StringSplitOptions.RemoveEmptyEntries);

                        if (set.Length == column)
                            setList.Add(set);
                    }
                }
                return setList;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void WipeCache()
        {
            Proxies = null;
            _rblPatterns = null;
            _sePatterns = null;
            _yaCheck = null;
            _GoogCheck = null;
            _anonChecks = null;

            GC.Collect();
        }
    }
}