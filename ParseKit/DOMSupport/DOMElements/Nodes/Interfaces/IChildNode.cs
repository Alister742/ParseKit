using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Nodes;

namespace ParseKit.DOMSupport.DOMElements.Nodes.Interfaces
{
    interface IChildNode
    {
         Element? previousElementSibling { get;  }
         Element? nextElementSibling { get;  }

        //NEW
         void before(Node nodes);
         void after(Node nodes);
         void replace(Node nodes);
         void before(string nodes);
         void after(string nodes);
         void replace(string nodes);
         void remove();
    };
}
