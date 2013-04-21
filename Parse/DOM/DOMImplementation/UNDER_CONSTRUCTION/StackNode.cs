
using System;
using System.Diagnostics;

namespace Parse.DOM
{
    public sealed class StackNode<T>
    {
        readonly int flags;

        internal readonly string name;

        internal readonly string popName;

        internal readonly string ns;

        internal readonly T node;

        // Only used on the list of formatting elements
        internal HtmlAttributes attributes;

        private int refcount = 1;

        public int Flags
        {
            get
            {
                return flags;
            }
        }

        public DispatchGroup Group
        {
            get
            {
                return (DispatchGroup)(flags & ElementName.GROUP_MASK);
            }
        }

        public bool IsScoping
        {
            get
            {
                return (flags & ElementName.SCOPING) != 0;
            }
        }

        public bool IsSpecial
        {
            get
            {
                return (flags & ElementName.SPECIAL) != 0;
            }
        }

        public bool IsFosterParenting
        {
            get
            {
                return (flags & ElementName.FOSTER_PARENTING) != 0;
            }
        }

        public bool IsHtmlIntegrationPoint
        {
            get
            {
                return (flags & ElementName.HTML_INTEGRATION_POINT) != 0;
            }
        }

        /// <summary>
        /// Constructor for copying. This doesn't take another <code>StackNode</code>
        /// because in C++ the caller is reponsible for reobtaining the local names
        /// from another interner.
        /// </summary>
        internal StackNode(int flags, String ns, String name, T node, String popName, HtmlAttributes attributes)
        {
            this.flags = flags;
            this.name = name;
            this.popName = popName;
            this.ns = ns;
            this.node = node;
            this.attributes = attributes;
            this.refcount = 1;
        }

        /// <summary>
        /// Short hand for well-known HTML elements.
        /// </summary>
        internal StackNode(ElementName elementName, T node)
        {
            this.flags = elementName.Flags;
            this.name = elementName.name;
            this.popName = elementName.name;
            this.ns = "http://www.w3.org/1999/xhtml";
            this.node = node;
            this.attributes = null;
            this.refcount = 1;
        }

        /// <summary>
        /// Constructor for HTML formatting elements.
        /// </summary>
        internal StackNode(ElementName elementName, T node, HtmlAttributes attributes)
        {
            this.flags = elementName.Flags;
            this.name = elementName.name;
            this.popName = elementName.name;
            this.ns = "http://www.w3.org/1999/xhtml";
            this.node = node;
            this.attributes = attributes;
            this.refcount = 1;
        }

        /// <summary>
        /// The common-case HTML constructor.
        /// </summary>
        internal StackNode(ElementName elementName, T node, string popName)
        {
            this.flags = elementName.Flags;
            this.name = elementName.name;
            this.popName = popName;
            this.ns = "http://www.w3.org/1999/xhtml";
            this.node = node;
            this.attributes = null;
            this.refcount = 1;
        }

        /// <summary>
        /// Constructor for SVG elements. Note that the order of the arguments is
        /// what distinguishes this from the HTML constructor. This is ugly, but
        /// AFAICT the least disruptive way to make this work with Java's generics
        /// and without unnecessary branches. :-(
        /// </summary>
        internal StackNode(ElementName elementName, string popName, T node)
        {
            this.flags = PrepareSvgFlags(elementName.Flags);
            this.name = elementName.name;
            this.popName = popName;
            this.ns = "http://www.w3.org/2000/svg";
            this.node = node;
            this.attributes = null;
            this.refcount = 1;
        }

        /// <summary>
        /// Constructor for MathML.
        /// </summary>
        internal StackNode(ElementName elementName, T node, string popName, bool markAsIntegrationPoint)
        {
            this.flags = PrepareMathFlags(elementName.Flags, markAsIntegrationPoint);
            this.name = elementName.name;
            this.popName = popName;
            this.ns = "http://www.w3.org/1998/Math/MathML";
            this.node = node;
            this.attributes = null;
            this.refcount = 1;
        }

        private static int PrepareSvgFlags(int flags)
        {
            flags &= ~(ElementName.FOSTER_PARENTING | ElementName.SCOPING
                    | ElementName.SPECIAL | ElementName.OPTIONAL_END_TAG);
            if ((flags & ElementName.SCOPING_AS_SVG) != 0)
            {
                flags |= (ElementName.SCOPING | ElementName.SPECIAL | ElementName.HTML_INTEGRATION_POINT);
            }
            return flags;
        }

        private static int PrepareMathFlags(int flags, bool markAsIntegrationPoint)
        {
            flags &= ~(ElementName.FOSTER_PARENTING | ElementName.SCOPING | ElementName.SPECIAL | ElementName.OPTIONAL_END_TAG);
            if ((flags & ElementName.SCOPING_AS_MATHML) != 0)
            {
                flags |= (ElementName.SCOPING | ElementName.SPECIAL);
            }
            if (markAsIntegrationPoint)
            {
                flags |= ElementName.HTML_INTEGRATION_POINT;
            }
            return flags;
        }

        public void DropAttributes()
        {
            attributes = null;
        }

        // TODO: probably we won't need these
        public void Retain()
        {
            refcount++;
        }

        public void Release()
        {
            refcount--;
        }
    }
}
