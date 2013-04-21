using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.DOMElements
{
    public class DomWalker
    {
        public DomWalker(Node root, long whatToShow, NodeFilter filter)
        {
            this.root = root;
            this.whatToShow = whatToShow;
            this.filter = filter;
        }

        public Node root { get; private set; }
        public long whatToShow { get; private set; }
        public NodeFilter filter { get; private set; }
        
        private bool _activeFlag = false;

        protected int Filter(Node node)
        {
            if (_activeFlag)
            {
                throw new DOMError("The object is in an invalid state.");
            }

            int nodeType = node.nodeType;

            if (!((whatToShow)whatToShow).HasFlag((whatToShow)((1 << nodeType) / 2)))
            {
                return FilterResult.FILTER_SKIP;
            }
            if (filter == null)
            {
                return FilterResult.FILTER_ACCEPT;
            }
            _activeFlag = true;

            int result;
            try
            {
                result = filter.acceptNode(node);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                _activeFlag = false;
            }

            return result;
        }
    }
}
