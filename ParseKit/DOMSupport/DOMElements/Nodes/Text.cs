using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParseKit.DOMElements._Classes.Nodes
{
    class Text : CharacterData
    {
        public Text(Document doc, string data)
            : base(doc, data)
        { 
        }

        StringBuilder _concatBuilder = new StringBuilder();

        public Text splitText(int offset)
        {
            if (offset > base.length)
	            throw new Exception();

            int count = base.length - offset;

            string subData = base.substringData(offset, count);

            Text txt = new Text(ownerDocument, subData);

            if (this.parentNode != null)
                txt.parentNode = this.parentNode;
            
            this.parentElement.insertBefore(

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
