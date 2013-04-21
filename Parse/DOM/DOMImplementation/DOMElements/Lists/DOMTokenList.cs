using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.Core.Dom.Interfaces;

namespace Parse.DOM.DOMElements
{
    /// <summary>
    /// FULL SUPPORTED Live standart on 02.2013
    /// </summary>
    public class DOMTokenList : List<string>, IDOMTokenList
    {
        public long length { get { return base.Count; } }
        public string item(int index)
        {
            return base[index];
        }
        public bool contains(string token)
        {
            CheckFormat(token);

            return base.Contains(token);
        }
        public void add(params string[] tokens)
        {
            for (int i = 0; i < tokens.Length; i++)
            {
                CheckFormat(tokens[i]);

                if (!base.Contains(tokens[i]))
                {
                    base.Add(tokens[i]);
                }
            }
        }
        public void remove(params string[] tokens)
        {
            for (int i = 0; i < tokens.Length; i++)
            {
                CheckFormat(tokens[i]);

                base.Remove(tokens[i]);
            }
        }
        public bool toggle(string token, bool force)
        {
            CheckFormat(token);

            if (force)
            {
                add(token);
                return true;
            }
            else
            {
                remove(token);
                return false;
            }
        }
        public bool toggle(string token)
        {
            CheckFormat(token);

            if (base.Remove(token))
            {
                return false;
            }
            else
            {
                base.Add(token);
                return true;
            }
        }

        public string stringifier()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < base.Count; i++)
            {
                sb.Append(base[i]);
            }
            return sb.ToString();
        }

        private void CheckFormat(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new DOMError("SyntaxError");

            if (token.Contains(' '))
                throw new DOMError("InvalidCharacterError");
        }
    };
}
