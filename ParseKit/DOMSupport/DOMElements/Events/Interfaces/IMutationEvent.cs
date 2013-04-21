using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Events;
using ParseKit.DOMElements._Classes.Nodes;

namespace ParseKit.DOMElements
{
    static class AttrChangeType
    {
        /// <summary>
        /// The Attr was modified in place.
        /// </summary>
        public static short MODIFICATION = 1;
        /// <summary>
        /// The Attr was just added.
        /// </summary>
        public static short ADDITION = 2;
        /// <summary>
        /// The Attr was just removed.
        /// </summary>
        public static short REMOVAL = 3;
    }
    // Introduced in DOM Level 2:
    interface IMutationEvent /* : IEvent */
    {

        // attrChangeType
        const short MODIFICATION = 1;
        const short ADDITION = 2;
        const short REMOVAL = 3;

        Node relatedNode { get; }
        string prevValue { get; }
        string newValue { get; }
        string attrName { get; }
        short attrChange { get; }
        void initMutationEvent(string typeArg,
                                              bool canBubbleArg,
                                              bool cancelableArg,
                                              Node relatedNodeArg,
                                              string prevValueArg,
                                              string newValueArg,
                                              string attrNameArg,
                                               short attrChangeArg);
    };

}
