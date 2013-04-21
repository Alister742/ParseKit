using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMSupport.DOMElements.Nodes.Interfaces;
using ParseKit.CORE.DOMCore.DOMElements.Nodes;

namespace Parse.DOM.DOMElements
{
    class ChildNode : NodeDom4, IChildNode
    {
        public Element previousElementSibling { get; private set; }
        public Element nextElementSibling { get; private set; }
    }

//DocumentType implements ChildNode;
//Element implements ChildNode;
//CharacterData implements ChildNode;
}
