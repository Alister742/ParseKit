using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Nodes;

namespace ParseKit.DOMElements._Classes.Ranges
{
    class Range
    {
        public Node startContainer { get; private set; }
        public long startOffset { get; private set; }
        public Node endContainer { get; private set; }
        public long endOffset { get; private set; }
        public bool collapsed { get; private set; }
        public Node commonAncestorContainer { get; private set; }

        public void setStart(Node refNode, long offset);
        public void setEnd(Node refNode, long offset);
        public void setStartBefore(Node refNode);
        public void setStartAfter(Node refNode);
        public void setEndBefore(Node refNode);
        public void setEndAfter(Node refNode);
        public void collapse(bool toStart);
        public void selectNode(Node refNode);
        public void selectNodeContents(Node refNode);

        public const short START_TO_START = 0;
        public const short START_TO_END = 1;
        public const short END_TO_END = 2;
        public const short END_TO_START = 3;
        public short compareBoundaryPoints(short how, Range sourceRange);

        public void deleteContents();
        //Moving a Node, with methods such as Document.adoptNode, Node.appendChild, or Range.extractContents [DOM Range], must not cause the event listeners attached to it to be removed or un-registered.
        public DocumentFragment extractContents();
        public DocumentFragment cloneContents();
        public void insertNode(Node node);
        public void surroundContents(Node newParent);

        //Copying a Node, with methods such as Node.cloneNode or Range.cloneContents [DOM Range], must not copy the event listeners attached to it. Event listeners can be attached to the newly created Node afterwards, if so desired.
        public Range cloneRange();
        public void detach();

        public bool isPointInRange(Node node, long offset);
        public short comparePoint(Node node, long offset);

        public bool intersectsNode(Node node);

        public stringifier()
        {

        }
    };
}
