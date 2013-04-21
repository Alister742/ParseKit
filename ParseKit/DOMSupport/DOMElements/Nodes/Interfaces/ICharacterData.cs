using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Nodes;

namespace ParseKit.DOMSupport.DOMElements.Nodes.Interfaces
{
    interface ICharacterData
    {
         string data { get; set; }
         long length { get { return data.Length; } }
         string substringData(long offset, long count);
         void appendData(string data);
         void insertData(long offset, string data);
         void deleteData(long offset, long count);
         void replaceData(long offset, long count, string data);

        // NEW
        void before(Node nodes);
        void after(Node nodes);
        void replace(Node nodes);
        void before(string nodes);
        void after(string nodes);
        void replace(string nodes);
        void remove();
    }
}
