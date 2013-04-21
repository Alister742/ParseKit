using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.Interfaces
{
    interface ICharacterData
    {
         string data { get; set; }
         int length { get; }
         string substringData(int offset, int count);
         void appendData(string data);
         void insertData(int offset, string data);
         void deleteData(int offset, int count);
         void replaceData(int offset, int count, string data);

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
