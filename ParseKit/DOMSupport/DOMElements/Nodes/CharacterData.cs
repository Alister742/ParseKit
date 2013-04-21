using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMSupport.DOMElements.Nodes.Interfaces;

namespace ParseKit.DOMElements._Classes.Nodes
{
    class CharacterData : Node, ICharacterData
    {
        private string _data;

        public CharacterData(Document doc, string data) : base(doc)
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
        public int length
        {
            get
            {
                return _data.Length;
            }
        }

        public string substringData(long offset, long count);
        public void appendData(string data)
        {
            _data += data;
        }
        public void insertData(int offset, string data)
        {
            _data.Insert(offset, data);
        }
        public void deleteData(int offset, int count)
        {
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
        
        // NEW
        public void before(Node nodes);
        public void after(Node nodes);
        public void replace(Node nodes);
        public void before(string nodes);
        public void after(string nodes);
        public void replace(string nodes);
        public void remove();
        #endregion

    };
}
