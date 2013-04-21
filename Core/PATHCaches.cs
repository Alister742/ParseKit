using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Threading;

namespace Core
{
    public static class PATH
    {
        static string _dataFilePath = @"c:\!Работа\!SE Feeds\!Resources\PATH";
        static string _PATHdata;

        static string _proxyPath;
        static string _resourceBasePath;
        static string _sitesPath;
        static string _themesPath;
        static string _logPath;
        static string _hidemeDotRuHashes;
        static string _unknownHidemeDotRuHashes;
        static string _tempFolder;
        static string _rblFilePath;
        static string _sitePatterns;
        static string _sePatterns;
        static string _anonymousPatterns;
        static string _tagClassPatterns;
        static string _tagClassEqualTable;

        #region Path
        public static string AnonPatterns
        {
            get
            {
                if (_anonymousPatterns == null)
                    _anonymousPatterns = ReadTag("anonymousPatterns");
                return _anonymousPatterns;
            }
        }
        public static string SEPatterns
        {
            get
            {
                if (_sePatterns == null)
                    _sePatterns = ReadTag("sePatterns");
                return _sePatterns;
            }
        }
        public static string SitePatterns
        {
            get
            {
                if (_sitePatterns == null)
                    _sitePatterns = ReadTag("sitesPatterns");
                return _sitePatterns;
            }
        }
        public static string RBLSites
        {
            get
            {
                if (_rblFilePath == null)
                    _rblFilePath = ReadTag("rbl");
                return _rblFilePath;
            }
        }
        public static string Proxy
        {
            get 
            {
                if (_proxyPath == null)
                    _proxyPath = ReadTag("proxy");
                return _proxyPath;
            }
        }
        public static string ResourceBase
        {
            get
            {
                if (_resourceBasePath == null)
                    _resourceBasePath = ReadTag("resourceBasePath");
                return _resourceBasePath;
            }
        }
        public static string Sites
        {
            get
            {
                if (_sitesPath == null)
                    _sitesPath = ReadTag("sitesfolder");
                return _sitesPath;
            }
        }
        public static string Themes
        {
            get
            {
                if (_themesPath == null)
                    _themesPath = ReadTag("themes");
                return _themesPath;
            }
        }
        public static string Log
        {
            get
            {
                if (_logPath == null)
                {
                    try
                    {
                        _logPath = ReadTag("logfile");
                    }
                    catch (Exception ex)
                    {
                        _logPath = Environment.CurrentDirectory + "\\Log";
                        Console.WriteLine("Can't get Log path. " + ex.Message + " In method: " + ex.TargetSite);
                    }
                }
                return _logPath;
            }
        }
        public static string HidemeDotRuHashes
        {
            get
            {
                if (_hidemeDotRuHashes == null)
                    _hidemeDotRuHashes = ReadTag("hidemeDotRuPortHashes");
                return _hidemeDotRuHashes;
            }
        }
        public static string UnknownHidemeHashes
        {
            get
            {
                if (_unknownHidemeDotRuHashes == null)
                    _unknownHidemeDotRuHashes = ReadTag("unknHidemeDotRuPortHashes");
                return _unknownHidemeDotRuHashes;
            }
        }
        public static string Temp
        {
            get
            {
                if (_tempFolder == null)
                    _tempFolder = ReadTag("tempfolder");
                return _tempFolder;
            }
        }
        public static string TagClassPatterns
        {
            get
            {
                if (_tagClassPatterns == null)
                    _tagClassPatterns = ReadTag("tagClassPatterns");
                return _tagClassPatterns;
            }
        }
        #endregion

        public static string ReadTag(string tagName)
        {
            string data;
            while (true)
            {
                data = GetPATHdata();
                if (string.IsNullOrEmpty(data))
                    Thread.Sleep(15000);    
                else break;
            }

            string tagContent = string.Empty;

            using (StringReader sr = new StringReader(data))
            {
                using (XmlReader reader = XmlReader.Create(new StringReader(data)))
                {
                    reader.ReadToFollowing(tagName);
                    tagContent = reader.ReadElementContentAsString();
                }
            }
            if (string.IsNullOrEmpty(tagContent))
                throw new Exception(tagName + " tag: content is null or empty, please check 'DATA' file.");
            return tagContent;
        }

        private static string GetPATHdata()
        {
            if (_PATHdata == null)
            {
                StreamReader dataReader = null;
                try
                {
                    //dataReader = new StreamReader(_dataFilePath, Encoding.Default);
                    //_PATHdata = dataReader.ReadToEnd();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                    Console.WriteLine("PATH File read error, without resources and setting work can't be continued. Do 15 sec pause, before next try...");
                    return null;
                }
                finally
                {
                    if (dataReader != null) dataReader.Dispose();
                }
            }
            return _PATHdata;
        }
    }
}
