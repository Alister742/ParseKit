using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.DOMElements
{
    public class NodeChangedEventArgs : EventArgs
    {
        public NodeChangedEventArgs(Node node, Node oldParent, Node newParent, string oldValue, string newValue, NodeChangedAction action)
        {
            this.Node = node;
            this.OldParent = oldParent;
            this.NewParent = newParent;
            this.Action = action;
            this.OldValue = oldValue;
            this.NewValue = newValue; 
        }

        public NodeChangedAction Action { get; private set; }

        public Node NewParent { get; private set; }

        public string NewValue { get; private set; }

        public Node Node { get; private set; }

        public Node OldParent { get; private set; }

        public string OldValue { get; private set; }
    }
}
