using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParseKit.DOMElements._Classes.Nodes
{

    class MutationObserver
    {
        public MutationObserver(Action<sequence<MutationRecord>, MutationObserver> callback)
        {

        }
        public void observe(Node target, MutationObserverInit options);
        public void disconnect();
        public sequence<MutationRecord> takeRecords();
    };

    delegate void MutationCallbac(sequence<MutationRecord> mutations, MutationObserver observer);// += () (sequence<MutationRecord> mutations, MutationObserver observer);

    class MutationObserverInit
    {
        public bool childList = false;
        public bool attributes = false;
        public bool characterData = false;
        public bool subtree = false;
        public bool attributeOldValue = false;
        public bool characterDataOldValue = false;
        public sequence<string> attributeFilter;
    };
}
