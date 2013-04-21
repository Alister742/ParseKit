using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Lists;

namespace ParseKit.DOMElements._Classes.Nodes
{
    class ParentNode
    {
        public HTMLCollection children { get; private set; }
        public Element? firstElementChild { get; private set; }
        public Element? lastElementChild { get; private set; }
        public long childElementCount { get; private set; }

        //NEW
        public void prepend(Node nodes)
        {
            if (children.length <= 0)
                return;

            if (children[0] == null)
                return;

            children[0].prepend(nodes);
        }
        public void append(Node nodes)
        {
            if (children.length <= 0)
                return;

            if (children[0] == null)
                return;

            children[0].append(nodes);
        }
        public void prepend(string nodes)
        {
            if (children.length <= 0)
                return;

            if (children[0] == null)
                return;

            children[0].prepend(nodes);
        }
        public void append(string nodes)
        {
            if (children.length <= 0)
                return;

            if (children[0] == null)
                return;

            children[0].append(nodes);
        }
    };

    //Document implements ParentNode;
    //DocumentFragment implements ParentNode;
    //Element implements ParentNode;
}
