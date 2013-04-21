using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Nodes;

namespace ParseKit.DOMElements._Classes.Lists
{
    class NodeList : List<Node>
    {
        internal void RemoveNulls()
        {
            int i = 0;
            while (i < base.Count)
            {
                if (base[i] == null)
                    RemoveAt(i);
                else
                    i++;
            }
        }

        #region Interface NodeList

        public Node? item(int index)
        {
            return base[index];
        }
        public long length { get { return base.Count; } }

        #endregion
    };
}
