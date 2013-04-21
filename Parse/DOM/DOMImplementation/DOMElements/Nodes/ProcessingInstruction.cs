using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.DOMElements
{
    public class ProcessingInstruction : CharacterData
    {
        public ProcessingInstruction(string target, Document doc, string data = null)
            : base(data, doc)
        {
            this.target = target;
        }

        public string target { get; private set; }
    };
}
