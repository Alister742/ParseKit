using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.DOMElements
{
    public class Comment : CharacterData
    {
        public Comment(string text, Document doc)
            : base(text, doc)
        {
        }
        
    };
}
