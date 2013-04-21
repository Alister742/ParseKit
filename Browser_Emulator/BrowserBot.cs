using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using mshtml;

namespace Browser_Emulator
{
    class BrowserBot
    {
        //WebBrowser Browser { get; set; }
        HtmlDocument _htmlDoc;
        IHTMLDocument2 _document2 { get { return _htmlDoc.DomDocument as IHTMLDocument2; } }
        IHTMLDocument3 _document3 { get { return _htmlDoc.DomDocument as IHTMLDocument3; } }
        IHTMLDocument4 _document4 { get { return _htmlDoc.DomDocument as IHTMLDocument4; } }

        public BrowserBot(HtmlDocument doc)
        {
            //Browser = browser;
            _htmlDoc = doc;
        }

        #region Element Search

        #region Accurate
        public HtmlElementCollection GetByTag(string tag)
        {
            return _htmlDoc.GetElementsByTagName(tag);
        }
        public HtmlElement GetByNameAttr(string name)
        {
            foreach (HtmlElement elem in _htmlDoc.All)
            {
                if (elem.Name == name)
                    return elem;
            }
            return null;
        }
        public HtmlElement GetById(string id)
        {
            foreach (HtmlElement elem in _htmlDoc.All)
            {
                if (elem.Id == id)
                    return elem;
            }
            return null;
        }
        public HtmlElement GetByStyle(string style)
        {
            foreach (HtmlElement elem in _htmlDoc.All)
            {
                if (elem.Style != null && elem.Style == style)
                    return elem;
            }
            return null;
        }
        public HtmlElement GetByOuterText(string text)
        {
            foreach (HtmlElement elem in _htmlDoc.All)
            {
                if (elem.InnerText != null && elem.OuterText == text)
                    return elem;
            }
            return null;
        }
        public HtmlElement GetByInnerText(string text)
        {
            foreach (HtmlElement elem in _htmlDoc.All)
            {
                if (elem.InnerText != null && elem.InnerText == text)
                    return elem;
            }
            return null;
        }
        #endregion

        #region Contain
        public HtmlElement GetByNameContains(string name)
        {
            foreach (HtmlElement elem in _htmlDoc.All)
            {
                if (elem.Name.Contains(name))
                    return elem;
            }
            return null;
        }
        public HtmlElement GetByIdContains(string id)
        {
            foreach (HtmlElement elem in _htmlDoc.All)
            {
                if (elem.Id.Contains(id))
                    return elem;
            }
            return null;
        }
        public HtmlElement GetByStyleContains(string style)
        {
            foreach (HtmlElement elem in _htmlDoc.All)
            {
                if (elem.Style !=null && elem.Style.Contains(style))
                    return elem;
            }
            return null;
        }
        public HtmlElement GetByOuterHtmlContains(string html)
        {
            foreach (HtmlElement elem in _htmlDoc.All)
            {
                if (elem.InnerHtml !=null && elem.OuterHtml.Contains(html))
                    return elem;
            }
            return null;
        }
        public HtmlElement GetByOuterTextContains(string text)
        {
            foreach (HtmlElement elem in _htmlDoc.All)
            {
                if (elem.InnerText != null && elem.OuterText.Contains(text))
                    return elem;
            }
            return null;
        }
        public HtmlElement GetByInnerHtmlContains(string html)
        {
            foreach (HtmlElement elem in _htmlDoc.All)
            {
                if (elem.InnerHtml !=null && elem.InnerHtml.Contains(html))
                    return elem;
            }
            return null;
        }
        public HtmlElement GetByInnerTextContains(string text)
        {
            foreach (HtmlElement elem in _htmlDoc.All)
            {
                if (elem.InnerText !=null && elem.InnerText.Contains(text))
                    return elem;
            }
            return null;
        }
        #endregion

        #region Regex
        public HtmlElement GetByNameRx(Regex nameRx)
        {
            foreach (HtmlElement elem in _htmlDoc.All)
            {
                if (nameRx.IsMatch(elem.Name))
                    return elem;
            }
            return null;
        }
        public HtmlElement GetByIdRx(Regex idRx)
        {
            foreach (HtmlElement elem in _htmlDoc.All)
            {
                if (idRx.IsMatch(elem.Id))
                    return elem;
            }
            return null;
        }
        public HtmlElement GetByStyle(Regex styleRx)
        {
            foreach (HtmlElement elem in _htmlDoc.All)
            {
                if (elem.Style != null && styleRx.IsMatch(elem.Style))
                    return elem;
            }
            return null;
        }
        public HtmlElement GetByOuterHtmlRx(Regex htmlRx)
        {
            foreach (HtmlElement elem in _htmlDoc.All)
            {
                if (elem.InnerHtml != null && htmlRx.IsMatch(elem.OuterHtml))
                    return elem;
            }
            return null;
        }
        public HtmlElement GetByOuterTextRx(Regex textRx)
        {
            foreach (HtmlElement elem in _htmlDoc.All)
            {
                if (elem.InnerText != null &&  textRx.IsMatch(elem.OuterText))
                    return elem;
            }
            return null;
        }
        public HtmlElement GetByInnerHtmlRx(Regex htmlRx)
        {
            foreach (HtmlElement elem in _htmlDoc.All)
            {
                if (elem.InnerHtml !=null && htmlRx.IsMatch(elem.InnerHtml))
                    return elem;
            }
            return null;
        }
        public HtmlElement GetByInnerTextRx(Regex textRx)
        {
            foreach (HtmlElement elem in _htmlDoc.All)
            {
                if (elem.InnerText != null && textRx.IsMatch(elem.InnerText))
                    return elem;
            }
            return null;
        }
        #endregion

        #endregion

        #region Element manipulation

        public void Click(HtmlElement elem)
        {
            elem.InvokeMember("click");
        }
        public void DoubleClick(HtmlElement elem)
        {
            Click(elem);
            Click(elem);
        }
        public void SetValue(HtmlElement elem, string text)
        {
            elem.SetAttribute("value", text);
        }
        public void PressKey(HtmlElement elem, char key)
        {
            Focus(elem);
            SendKeys.SendWait(key.ToString());
        }
        public void Focus(HtmlElement elem)
        {
            elem.Focus();
        }

        public void SetComboBoxValue(HtmlElement elem, string val)
        {
            elem.SetAttribute("selectedIndex", val);
        }

        public void SetCheckRadioBox(HtmlElement elem, bool val)
        {
            elem.SetAttribute("checked", val ? "true" : "false");
        }
        #endregion

        #region Cookies
        public void SetCookies(string cookies)
        {
            if (_document2 != null)
                _document2.cookie = cookies;
        }

        public string GetCookies(string cookies)
        {
            return _document2 != null ? _document2.cookie : null;
        }

        public void ClearCookies()
        {
            if (_document2 != null)
                _document2.cookie = null;
        }
        #endregion
    }
}
