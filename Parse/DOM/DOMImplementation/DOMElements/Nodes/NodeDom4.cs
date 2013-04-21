using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.DOMElements
{
    public class NodeDom4 : Node
    {
        public NodeDom4(Document doc)
            : base(doc)
        {
            //if (this.parentNode == null)
            //{

            //}
        }

        // NEW
        public void before(Node nodes)
        {
            if (this.parentNode == null)
                return;
            if (nodes.childNodes.Contains(this))
                throw new DOMException(ExceptionCodes.HIERARCHY_REQUEST_ERR);

            //Run the mutation method macro.

            parentNode.insertBefore(nodes, this);
        }
        public void after(Node nodes)
        {
            if (this.parentNode == null)
                return;
            if (nodes.childNodes.Contains(this))
                throw new DOMException(ExceptionCodes.HIERARCHY_REQUEST_ERR);
            //Run the mutation method macro.

            parentNode.insertBefore(nodes, this.nextSibling);
        }
        public void replace(Node nodes)
        {
            if (this.parentNode == null)
                return;
            if (nodes.childNodes.Contains(this))
                throw new DOMException(ExceptionCodes.HIERARCHY_REQUEST_ERR);
            //Run the mutation method macro.

            parentNode.replaceChild(nodes, this);
        }
        public void before(string nodes)
        {
            throw new NotImplementedException();
        }
        public void after(string nodes)
        {
            throw new NotImplementedException();
        }
        public void replace(string nodes)
        {
            throw new NotImplementedException();
        }
        public void remove()
        {
            if (this.parentNode == null)
                return;

            parentNode.removeChild(this);
        }

        public void prepend(Node nodes)
        {
            if (nodes.childNodes.Contains(this))
                throw new DOMException(ExceptionCodes.HIERARCHY_REQUEST_ERR);

            //Run the mutation method macro.
            //Pre-insert node into the context object before the context object's first child.
            if (this.childNodes.length == 0)
	        {
                append(nodes);
	        }
            else
            {
                insertBefore(nodes, this.childNodes[0]);
            }
        }
        public void append(Node nodes)
        {
            if (nodes.childNodes.Contains(this))
                throw new DOMException(ExceptionCodes.HIERARCHY_REQUEST_ERR);

            insertBefore(nodes, null);
        }
        public void prepend(string nodes)
        {
            throw new NotImplementedException();
        }
        public void append(string nodes)
        {
            throw new NotImplementedException();
        }
    }
}
