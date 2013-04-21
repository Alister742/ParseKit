using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.Core.Dom.Interfaces;

namespace Parse.DOM.DOMElements
{
    public class CharacterData : NodeDom4, ICharacterData
    {
        private string _data;

        public CharacterData(string data, Document doc)
            : base(doc)
        {
            _data = data;
        }

        public int Totallength
        {
            get
            {
                int total = 0;
                for (int i = 0; i < childNodes.length; i++)
                {
                    if (childNodes[i] is CharacterData)
                    {
                        total += (childNodes[i] as CharacterData).length;
                    }
                }
                return total;
            }
        }

        #region Interface ICharacterData
        public string data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }
        public int length { get { return data.Length; } }

        public string substringData(int offset, int count)
        {
            if (offset > length)
            {
                throw new Exception();
            }

            if (offset + count > length)
            {
                count = length - offset;
            }

            return _data.Substring(offset, count);
        }
        public void appendData(string data)
        {
            _data += data;
        }
        public void insertData(int offset, string data)
        {
            if (offset > length)
            {
                throw new Exception();
            }

            _data.Insert(offset, data);
        }
        public void deleteData(int offset, int count)
        {
            if (offset > length)
            {
                throw new Exception();
            }

            if (offset + count > length)
            {
                count = length - offset;
            }
            _data.Remove(offset, count);
        }
        public void replaceData(int offset, int count, string data)
        {
            if (offset > length)
            {
                throw new Exception();
            }

            if (offset + count > length)
            {
                count = length - offset;
            }
            
            _data.Remove(offset, count);
            _data.Insert(offset, data);
        }
        #endregion
    };
}
