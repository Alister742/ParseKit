using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.Core.Dom.Interfaces;
using System.Collections;

namespace Parse.DOM.DOMElements
{
    public class HTMLCollection : List<Element>, IHTMLCollection
    {
        public HTMLCollection() : base() { }

        public HTMLCollection(IEnumerable<Element> collection) : base(collection) { }

        public HTMLCollection(IEnumerable collection) : base(ConcertElements(collection)) { }

        private static IEnumerable<Element> ConcertElements(IEnumerable collection)
        {
            foreach (var item in collection)
            {
                Element e = item as Element;

                if (e == null)
                    throw new ArgumentException("Cant create collection of elements, Some of objects wasnt elements");

                yield return e;
            }
        }

        //WTF
        private void ReplaceElementByName(string name, Element value)
        {
            if ((value.getAttribute("name") != name && value.id != name))
                return;

            for (int i = 0; i < base.Count; i++)
            {

                if (base[i].getAttribute("name") == name)
                {
                    base[i] = value;
                }
                else if (base[i].id == name)
                {
                    base[i] = value;
                }
            }
        }

        #region IHTMLCollection
        public long length { get { return base.Count; } }

        public Element this[int i]
        {
            get
            {
                return item(i);
            }
            set
            {
                if (i >= 0 && i < this.Count)
                {
                    base[i] = value;
                }
            }
        }

        public Element this[string name]
        {
            get
            {
                return namedItem(name) as Element;
            }
            set //WTF is it possibly
            {
                if (namedItem(name) == null)
                {
                    base.Add(value);
                }
                else
                    ReplaceElementByName(name, value);
            }
        }

        public Element item(int index)
        {
            if (index >= this.Count || index < 0)
                return null;

            return base[index];
        }

        public object namedItem(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            foreach (var item in this)
            {
                if (item.getAttribute("name") == name || item.id == name)
                {
                    return item;
                }
            }
            return null;
        }
        #endregion
    }

    //class HTMLLinkedCollection : LinkedList<Element>
    //{
    //    public long length { get { return base.Count; } }

    //    public Element this[int i]
    //    {
    //        get
    //        {
    //            return item(i);
    //        }
    //    }

    //    public Element this[string name]
    //    {
    //        get
    //        {
    //            return namedItem(name) as Element;
    //        }
    //    }

    //    public Element item(int index)
    //    {
    //        if (index > this.Count)
    //            return null;

    //        int i = 0;
    //        foreach (var item in this)
    //        {
    //            if (i == index)
    //            {
    //                return item;
    //            }
    //            i++;
    //        }

    //        return null;
    //    }

    //    public object namedItem(string name)
    //    {
    //        if (string.IsNullOrEmpty(name))
    //            return null;

    //        foreach (var item in this)
    //        {
    //            if (HTMLTagContainer.IsElementWithName(item.tagName) && item.getAttribute("name") == name)
    //            {
    //                return item;
    //            }
    //            else if (item.id == name)
    //            {
    //                return item;
    //            }
    //        }
    //        return null;
    //    }
    //};
}
