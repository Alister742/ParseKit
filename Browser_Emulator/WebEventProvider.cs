using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using mshtml;

namespace Browser_Emulator
{
    class WebEventProvider
    {
        //Area _area;
        public WebEventProvider()
        {
            //_area = area;
        }

        #region Update events
        public void UpdateBrowserEvents(WebBrowser browser)
        {
            IHTMLDocument2 document = browser.Document.DomDocument as IHTMLDocument2;
            UpdateDocumentEvents(document);
            UpdateElementsEvents(document.all);
        }

        public void UpdateDocumentEvents(IHTMLDocument2 document)
        {
            if (document != null)
            {
                //_document.MouseMove += new HtmlElementEventHandler(doc_MouseMove);
                //_document.Click += new HtmlElementEventHandler(doc_Click);
            }
        }

        public void UpdateElementsEvents(IHTMLElementCollection elements)
        {
            foreach (IHTMLElement3 element in elements)
            {
                //element.onmouseenter += new HTMLElementEvents_onmouseenterEventHandler(dd);
                //element.onmouseup += new HTMLElementEvents_onmouseupEventHandler(dd);
                //element.MouseEnter += new HtmlElementEventHandler(element_MouseEnter);
            }
        }

        #endregion

        #region Events 
        void doc_MouseMove(object sender, HtmlElementEventArgs e)
        {
            //UpdateFocusDraw();
        }
        void doc_Click(object sender, HtmlElementEventArgs e)
        {
            //HtmlElement elem = (sender as HtmlDocument).GetElementFromPoint(e.ClientMousePosition);
            //if (elem!=null && elem!=_lastElement)
            //    UpdateLast(elem);
        }

        void element_MouseEnter(object sender, HtmlElementEventArgs e)
        {
                //HtmlElement elem = e.FromElement;
                //if (elem != null && _lastElement != elem)
                //{
                //    UpdateLastSelected(elem);
                //    UpdateFocusDraw();
                //}
        }
        void el_MouseDown(object sender, HtmlElementEventArgs e)
        {
                //HtmlElement elFrom = e.FromElement;
                //if (elFrom != null && _lastElement != elFrom)
                //{
                //    //string fromMsg = e.FromElement.TagName + " " + e.FromElement.Name;
                //    //if (fromMsg != string.Empty)
                //    //    _area.textBox2.Text += string.Format("el_MouseOver From={0}, To=\r\n", fromMsg);

                //    //string elFrom = e.FromElement == null ? string.Empty : e.FromElement.TagName + " " + e.FromElement.Name;
                //    //string elTo = e.ToElement == null ? string.Empty : e.ToElement.TagName + " " + e.ToElement.Name;

                //    //HtmlElement parent = elFrom.Parent;
                //    //if (parent != null)
                //    //    _area.Drawing.DrawRectangle(parent.ClientRectangle, FrameStyle.Dashed);

                //    //_area.Drawing.DrawFocusedArea(elFrom.ClientRectangle);

                //    _area.propertyGrid1.SelectedObject = elFrom;
                //    _lastElement = elFrom;
                //}
        }
        #endregion
    }
}
