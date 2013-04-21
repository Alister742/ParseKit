using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace System
{
    public static class Extensions
    {
        #region Uri
        public static Dictionary<string, string> GetParams(this Uri uri)
        {
            if (uri == null || string.IsNullOrEmpty(uri.Query))
                return null;

            Dictionary<string, string> paramsList = new Dictionary<string, string>();

            string paramsSubstr = uri.Query.Remove(0, 1);

            string[] prms = paramsSubstr.Split('&');

            for (int i = 0; i < prms.Length; i++)
            {
                string[] nameValue = prms[i].Split('=');
                if (nameValue.Length > 0)
                {
                    string name = nameValue[0];
                    string value = nameValue.Length > 1 ? nameValue[1] : string.Empty;

                    if (paramsList.ContainsKey(name))
                    {
                        throw new Exception("Uri can't contains duplicate params");
                    }

                    paramsList.Add(name, value);
                }
            }

            return paramsList;
        }
        public static string GetParam(this Uri uri, string name)
        {
            string uriStr = uri.OriginalString;

            int paramIdx = uriStr.IndexOf(name);
            if (paramIdx == -1)
                return null;

            int equalIdx = uriStr.IndexOf('=', paramIdx);
            if (equalIdx == -1)
                return null;

            int ampIdx = uriStr.IndexOf('&', equalIdx);

            int startParamIdx = equalIdx + 1;
            int endParamIdx = ampIdx == -1 ? uriStr.Length - equalIdx - 2 : ampIdx - equalIdx - 1;

            return uriStr.Substring(startParamIdx, endParamIdx);
        }
        #endregion

        #region Char
        static StringBuilder _content = new StringBuilder();
        public static string ToCharString(this char[] c)
        {
            _content.Clear();
            for (int i = 0; i < c.Length; i++)
            {
                _content.Append(c[i]);
            }
            return _content.ToString();
        }
        #endregion

        #region String
        static Regex _ipRx = new Regex(@"\A\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\Z", RegexOptions.Compiled);
        static Regex _emailRx = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,6}$", RegexOptions.Compiled);
        static Regex _emptiesRx = new Regex(@"\s", RegexOptions.Compiled);

        public static bool NoncaseEqual(this string s, string compareString)
        {
            return s.Equals(compareString, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsValidPort(this string s)
        {
            if (s.Length > 5)
                return false;

            int port;
            if (!Int32.TryParse(s, out port))
                return false;

            if (port > 65535 || port < 1)
                return false;

            return true;
        }

        public static bool IsValidIP(this string s)
        {
            s = s.TrimEnd(' ');
            return _ipRx.IsMatch(s);
        }

        public static string DeleteEmpties(this string s)
        {
            return _emptiesRx.Replace(s, string.Empty);
        }

        public static bool IsValidEmailAddress(this string s)
        {
            return _emailRx.IsMatch(s);
        }

        public static bool IsValidUrl(this string url)
        {
            string strRegex = "^(https?://)"
                + "?(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?"    //user@
                + @"(([0-9]{1,3}\.){3}[0-9]{1,3}"                               // IP- 199.194.52.184
                + "|"                                                           // allows either IP or domain
                + @"([0-9a-z_!~*'()-]+\.)*"                                     // tertiary domain(s)- www.
                + @"([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]"                         // second level domain
                + @"(\.[a-z]{2,6})?)"                                           // first level domain- .com or .museum is optional
                + "(:[0-9]{1,5})?"                                              // port number- :80
                + "((/?)|"                                                      // a slash isn't required if there is no file name
                + "(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$";
            return new Regex(strRegex).IsMatch(url);
        }
        #endregion
    }

    public static class ToStringExtensions
    {
        // Lookup table.
        static string[] _cache =
    {
	"0",
	"1",
	"2",
	"3",
	"4",
	"5",
	"6",
	"7",
	"8",
	"9",
	"10",
	"11",
	"12",
	"13",
	"14",
	"15",
	"16",
	"17",
	"18",
	"19",
	"20",
	"21",
	"22",
	"23",
	"24",
	"25",
	"26",
	"27",
	"28",
	"29",
	"30",
	"31",
	"32",
	"33",
	"34",
	"35",
	"36",
	"37",
	"38",
	"39",
	"40",
	"41",
	"42",
	"43",
	"44",
	"45",
	"46",
	"47",
	"48",
	"49",
	"50",
	"51",
	"52",
	"53",
	"54",
	"55",
	"56",
	"57",
	"58",
	"59",
	"60",
	"61",
	"62",
	"63",
	"64",
	"65",
	"66",
	"67",
	"68",
	"69",
	"70",
	"71",
	"72",
	"73",
	"74",
	"75",
	"76",
	"77",
	"78",
	"79",
	"80",
	"81",
	"82",
	"83",
	"84",
	"85",
	"86",
	"87",
	"88",
	"89",
	"90",
	"91",
	"92",
	"93",
	"94",
	"95",
	"96",
	"97",
	"98",
	"99",
	"100",
	"101",
	"102",
	"103",
	"104",
	"105",
	"106",
	"107",
	"108",
	"109",
	"110",
	"111",
	"112",
	"113",
	"114",
	"115",
	"116",
	"117",
	"118",
	"119",
	"120",
	"121",
	"122",
	"123",
	"124",
	"125",
	"126",
	"127",
	"128",
	"129",
	"130",
	"131",
	"132",
	"133",
	"134",
	"135",
	"136",
	"137",
	"138",
	"139",
	"140",
	"141",
	"142",
	"143",
	"144",
	"145",
	"146",
	"147",
	"148",
	"149",
	"150",
	"151",
	"152",
	"153",
	"154",
	"155",
	"156",
	"157",
	"158",
	"159",
	"160",
	"161",
	"162",
	"163",
	"164",
	"165",
	"166",
	"167",
	"168",
	"169",
	"170",
	"171",
	"172",
	"173",
	"174",
	"175",
	"176",
	"177",
	"178",
	"179",
	"180",
	"181",
	"182",
	"183",
	"184",
	"185",
	"186",
	"187",
	"188",
	"189",
	"190",
	"191",
	"192",
	"193",
	"194",
	"195",
	"196",
	"197",
	"198",
	"199"
    };

        // Lookup table last index.
        const int _top = 199;

        public static string ToStringLookup(this int value)
        {
            if (value >= 0 && value <= _top)
            {
                return _cache[value];
            }

            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
