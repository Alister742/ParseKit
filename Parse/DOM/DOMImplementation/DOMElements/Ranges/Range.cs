using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.Core.Dom.Interfaces;

namespace Parse.DOM.DOMElements
{
    public class Range : IRange
    {
        public Node startContainer { get; private set; }
        public long startOffset { get; private set; }
        public Node endContainer { get; private set; }
        public long endOffset { get; private set; }
        public bool collapsed { get; private set; }
        public Node commonAncestorContainer { get; private set; }

        public void setStart(Node refNode, long offset)
        {
            throw new NotImplementedException();
        }
        public void setEnd(Node refNode, long offset)
        {
            throw new NotImplementedException();
        }
        public void setStartBefore(Node refNode)
        {
            throw new NotImplementedException();
        }
        public void setStartAfter(Node refNode)
        {
            throw new NotImplementedException();
        }
        public void setEndBefore(Node refNode)
        {
            throw new NotImplementedException();
        }
        public void setEndAfter(Node refNode)
        {
            throw new NotImplementedException();
        }
        public void collapse(bool toStart)
        {
            throw new NotImplementedException();
        }
        public void selectNode(Node refNode)
        {
            throw new NotImplementedException();
        }
        public void selectNodeContents(Node refNode)
        {
            throw new NotImplementedException();
        }

        public const short START_TO_START = 0;
        public const short START_TO_END = 1;
        public const short END_TO_END = 2;
        public const short END_TO_START = 3;
        public short compareBoundaryPoints(short how, Range sourceRange)
        {
            throw new NotImplementedException();
        }

        public void deleteContents()
        {
            throw new NotImplementedException();
        }
        //Moving a Node, with methods such as Document.adoptNode, Node.appendChild, or Range.extractContents [DOM Range], must not cause the event listeners attached to it to be removed or un-registered.
        public DocumentFragment extractContents()
        {
            throw new NotImplementedException();
        }
        public DocumentFragment cloneContents()
        {
            throw new NotImplementedException();
        }
        public void insertNode(Node node)
        {
            throw new NotImplementedException();
        }
        public void surroundContents(Node newParent)
        {
            throw new NotImplementedException();
        }

        //Copying a Node, with methods such as Node.cloneNode or Range.cloneContents [DOM Range], must not copy the event listeners attached to it. Event listeners can be attached to the newly created Node afterwards, if so desired.
        public Range cloneRange()
        {
            throw new NotImplementedException();
        }
        public void detach()
        {
            throw new NotImplementedException();
        }

        public bool isPointInRange(Node node, long offset)
        {
            throw new NotImplementedException();
        }
        public short comparePoint(Node node, long offset)
        {
            throw new NotImplementedException();
        }

        public bool intersectsNode(Node node)
        {
            throw new NotImplementedException();
        }

        public string stringifier()
        {
            throw new NotImplementedException();

            StringBuilder sb = new StringBuilder();

            ///для всех ТЕКСТОВЫХ ('Text') нод из диапазона сцепить значения в строку и вернуть

            return sb.ToString();
        }
    };
}
