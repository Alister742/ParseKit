using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;


namespace ProxyFactory.Parser
{
    class SpysRuParser : IProxySiteProvider
    {
        string _ipPortPattern = @"<font[^>]*>(?<ip>[^<]*)<script type=""text/javascript"">(?<port>.*?)</script>";
        string _varPattern = @"(?<varName>\w*)=(?<varValue>\d*);";
        string _xorVarPattern = @"(?<varName>\w*)=(?<xorOne>\w*)\^(?<xorTwo>\w*);";
        string _variablesPattern = @"<script type=""text/javascript"">\s*(?<variables>(\w*=[\w|^]*;)+)\s*</script>";
        
        Regex _ipPortRx;
        Regex _varScriptRx;
        Regex _varRx;
        Regex _xorVarRx;

        public SpysRuParser()
        {
            _ipPortRx = new Regex(_ipPortPattern, RegexOptions.Compiled);
            _varScriptRx = new Regex(_variablesPattern);
            _varRx = new Regex(_varPattern);
            _xorVarRx = new Regex(_xorVarPattern);
        }

        public List<RatedProxy> ParsePage(string data)
        {
            if (data == null) return null;

            string varScriptStr = _varScriptRx.Match(data).Groups["variables"].Value;
            JavaScriptXorEmul antijavaParser = new JavaScriptXorEmul();
            antijavaParser.AddVariables(_varRx.Matches(varScriptStr), "varName", "varValue");
            antijavaParser.AddXorVariables(_xorVarRx.Matches(varScriptStr), "varName");

            return antijavaParser.ParseProxyMatches(_ipPortRx.Matches(data), "ip", "port");
        }
    }
}
