using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.Core.Dom.Interfaces;

namespace Parse.DOM.DOMElements
{
    public class Text : CharacterData, IText
    {
        public Text(string data, Document doc)
            : base(data, doc)
        { 
        }

        StringBuilder _concatBuilder = new StringBuilder();

        public Text splitText(int offset)
        {
            if (offset > base.length)
	            throw new Exception();

            int count = base.length - offset;

            string subData = base.substringData(offset, count);

            Text txt = new Text(subData, this.ownerDocument);

            if (parentNode != null)
                txt.parentNode = this.parentNode;

            parentElement.insertBefore(txt, this);

            return txt;
        }
        public string wholeText
        {
            get
            {
                _concatBuilder.Clear();

                Node concat = this;

                while (true)
                {
                    if (concat.previousSibling is Text)
                    {
                        concat = concat.previousSibling;
                    }
                    else
                        break;
                }

                while (true)
                {
                    _concatBuilder.Append(concat.textContent);

                    if (concat.nextSibling is Text)
                    {
                        concat = concat.nextSibling;
                    }
                    else
                        break;
                }

                return _concatBuilder.ToString();
            }
        }
    };
}
