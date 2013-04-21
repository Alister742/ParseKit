using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Text.RegularExpressions;
using System.Collections;


namespace ProxyFactory.Parser
{
    class proxyHttpParser : IProxySiteProvider
    {
        string _ipPortPattern = @"<td[^>]*>(?<ip>[\.\d]*)</td>\s*<td[^>]*>\s*<script type=""text/javascript"">[\s\S]*?(?<port>\([\w^]*\))[\s\S]*?</script>";
        string _varPattern = @"(?<varName>\w*)\s?=\s?(?<varValue>\d*);";
        string _xorVarPattern = @"(?<varName>\w*)\s?=\s?(?<xorOne>\w*)\^(?<xorTwo>\w*);";

        //string _variablesPattern = @"<script type=""text/javascript"">[\s\S]*?(?<variables>(\w*\s?=\s?[\w*\^]*;)+)[\s\S]*?</script>";


        Regex _ipPortRx;
        //Regex _varScriptRx;
        Regex _varRx;
        Regex _xorVarRx;
        Hashtable _varTable = new Hashtable();

        public proxyHttpParser()
        {
            _ipPortRx = new Regex(_ipPortPattern, RegexOptions.Compiled);
            //_varScriptRx = new Regex(_variablesPattern);
            _varRx = new Regex(_varPattern, RegexOptions.Compiled);
            _xorVarRx = new Regex(_xorVarPattern, RegexOptions.Compiled);
        }

        public List<RatedProxy> ParsePage(string data)
        {
            if (data == null) return null;

            MatchCollection variablesMatchs = _ipPortRx.Matches(data);
            JavaScriptXorEmul antijavaParser = new JavaScriptXorEmul();
            antijavaParser.AddVariables(_varRx.Matches(data), "varName", "varValue");
            antijavaParser.AddXorVariables(_xorVarRx.Matches(data), "varName");
            return antijavaParser.ParseProxyMatches(_ipPortRx.Matches(data), "ip", "port");
        }
    }
}
