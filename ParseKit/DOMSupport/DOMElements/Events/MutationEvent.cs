using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Events;
using ParseKit.DOMElements._Classes.Nodes;

namespace ParseKit.DOMElements
{
    // Introduced in DOM Level 2:
    class MutationEvent : Event, IMutationEvent
    {
        #region Члены IMutationEvent

        public Node relatedNode { get; private set; }

        public string prevValue { get; private set; }

        public string newValue { get; private set; }

        public string attrName { get; private set; }

        public short attrChange { get; private set; }

        public void initMutationEvent(string typeArg, bool canBubbleArg, bool cancelableArg, Node relatedNodeArg, string prevValueArg, string newValueArg, string attrNameArg, short attrChangeArg)
        {
            base.initEvent(typeArg, canBubbleArg, cancelableArg);

            relatedNode = relatedNodeArg;
            prevValue = prevValueArg;
            newValue = newValueArg;
            attrName = attrNameArg;
            attrChange = attrChangeArg;
        }

        #endregion
    };

}
