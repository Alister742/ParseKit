using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMSupport.DOMElements.Nodes.Interfaces;

namespace ParseKit.DOMElements._Classes.Nodes
{
    class ChildNode : IChildNode
    {
      public  Element? previousElementSibling{get; private set;}
      public   Element? nextElementSibling{get; private set;}

        //NEW
      public void before(Node nodes);
      public void after(Node nodes);
      public void replace(Node nodes);
      public void before(string nodes);
      public void after(string nodes);
      public void replace(string nodes);
      public void remove();
};

//DocumentType implements ChildNode;
//Element implements ChildNode;
//CharacterData implements ChildNode;
}
