using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.DOMElements
{
    public enum NodeChangedAction
    {
        //     A node is being inserted in the tree.
        Insert = 0,
        //
        //     A node is being removed from the tree.
        Remove = 1,
        //
        //     A node value is being changed.
        Change = 2,
    }
}
