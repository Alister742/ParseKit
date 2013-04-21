using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParseKit.DOMElements._Classes.Nodes
{
    class ProcessingInstruction : CharacterData
    {
        public string target { get; private set; }
    };
}
