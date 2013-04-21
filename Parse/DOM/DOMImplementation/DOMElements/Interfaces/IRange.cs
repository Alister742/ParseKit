using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.Interfaces
{
    interface IRange
    {
         Node startContainer { get; }
         long startOffset { get; }
         Node endContainer { get; }
         long endOffset { get; }
         bool collapsed { get; }
         Node commonAncestorContainer { get; }

         void setStart(Node refNode, long offset);
         void setEnd(Node refNode, long offset);
         void setStartBefore(Node refNode);
         void setStartAfter(Node refNode);
         void setEndBefore(Node refNode);
         void setEndAfter(Node refNode);
         void collapse(bool toStart);
         void selectNode(Node refNode);
         void selectNodeContents(Node refNode);

         /*
         const short START_TO_START = 0;
         const short START_TO_END = 1;
         const short END_TO_END = 2;
         const short END_TO_START = 3;
         */
         short compareBoundaryPoints(short how, Range sourceRange);

         void deleteContents();
        //Moving a Node, with methods such as Document.adoptNode, Node.appendChild, or Range.extractContents [DOM Range], must not cause the event listeners attached to it to be removed or un-registered.
         DocumentFragment extractContents();
         DocumentFragment cloneContents();
         void insertNode(Node node);
         void surroundContents(Node newParent);

        //Copying a Node, with methods such as Node.cloneNode or Range.cloneContents [DOM Range], must not copy the event listeners attached to it. Event listeners can be attached to the newly created Node afterwards, if so desired.
         Range cloneRange();
         void detach();

         bool isPointInRange(Node node, long offset);
         short comparePoint(Node node, long offset);

         bool intersectsNode(Node node);

         string stringifier();
    }
}
