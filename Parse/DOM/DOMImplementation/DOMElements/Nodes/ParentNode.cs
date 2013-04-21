using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Lists;
using ParseKit.CORE.DOMCore.DOMElements.Nodes.Interfaces;

namespace Parse.DOM.DOMElements
{
    class ParentNode : IParentNode
    {
        public HTMLCollection children { get; private set; }
        public Element firstElementChild { get { return children.First(); } }
        public Element lastElementChild { get { return children.Last(); } }
        public long childElementCount { get { return children.Count; } }

        //NEW (SUPPORTED!)
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
