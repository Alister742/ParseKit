using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace ProxyFactory.Parser
{
    class JavaScriptXorEmul
    {
        Hashtable _varTable = new Hashtable();

        public List<RatedProxy> ParseProxyMatches(MatchCollection ipPortMatches, string ipGroupName, string portGroupName)
        {
            List<RatedProxy> proxies = new List<RatedProxy>();
            foreach (Match ipPort in ipPortMatches)
            {
                string ip = ipPort.Groups[ipGroupName].Value.Replace(" ", "");
                string portExp = ipPort.Groups[portGroupName].Value.Replace(")", "").Replace("(", "").Replace(" ", "");
                string[] portDigits = SplitPortExp(portExp);
                string port = StickPortDigits(portDigits);

                if (ip.IsValidIP() && port.IsValidPort())
                    proxies.Add(new RatedProxy(ip + ":" + port));
            }
            return proxies;
        }

        private string[] SplitPortExp(string portExp)
        {
            return portExp.Contains('+') ? portExp.Split('+') : new string[] { portExp };
        }

        public string StickPortDigits(string[] portDigits)
        {
            string port = "";
            foreach (var digitExp in portDigits)
            {
                if (digitExp.Contains("^"))
                {
                    string[] xorExps = digitExp.Split('^');
                    int xorResult = -1;
                    for (int i = 0; i < xorExps.Length; i++)
                    {
                        if (xorResult == -1) xorResult = GetXorInteger(xorExps[i]);
                        else xorResult = xorResult ^ GetXorInteger(xorExps[i]);
                    }
                    port += xorResult;
                }
                else if (_varTable.Contains(digitExp)) port += (int)_varTable[digitExp];
            }
            return port;
        }

        public void AddXorVariables(MatchCollection xorVarMatches, string nameGroup)
        {
            foreach (Match xorVar in xorVarMatches)
            {
                string xorOne = xorVar.Groups["xorOne"].Value;
                string xorTwo = xorVar.Groups["xorTwo"].Value;
                int xorOneInt;
                int xorTwoInt;

                xorOneInt = GetXorInteger(xorOne);
                xorTwoInt = GetXorInteger(xorTwo);
                if (xorOneInt != -1 && xorTwoInt != -1) _varTable.Add(xorVar.Groups[nameGroup].Value, xorOneInt ^ xorTwoInt);
            }
        }

        public void AddVariables(MatchCollection varMatches, string nameGroup, string valGroup)
        {
            foreach (Match var in varMatches)
            {
                int val;
                if (Int32.TryParse(var.Groups[valGroup].Value, out val))
                {
                    string key = var.Groups[nameGroup].Value;
                    if (!_varTable.ContainsKey(key)) _varTable.Add(key, val);
                }
            }
        }

        public int GetXorInteger(string xorOne)
        {
            int xorOneInt;
            if (!Int32.TryParse(xorOne, out xorOneInt))
            {
                if (_varTable.ContainsKey(xorOne)) xorOneInt = (int)_varTable[xorOne];
                else xorOneInt = -1;
            }
            return xorOneInt;
        }
    }
}
