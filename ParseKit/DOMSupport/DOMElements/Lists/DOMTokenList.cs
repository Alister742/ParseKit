using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParseKit.DOMElements._Classes.Lists
{
    class DOMTokenList
    {
        public long length { get; private set; }
        public string? item(long index);
        public bool contains(string token);
        public void add(string tokens);
        public void remove(string tokens);
        public bool toggle(string token, bool force = false);
        public stringifier()
        {

        }
    };
}
