using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.Interfaces
{
    interface IDOMTokenList
    {
        long length { get; }
        string item(int index);
        bool contains(string token);
        void add(params string[] tokens);
        void remove(params string[] tokens);
        bool toggle(string token, bool force);
        bool toggle(string token);

        string stringifier();
    }
}
