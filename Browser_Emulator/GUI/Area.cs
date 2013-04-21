using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using mshtml;
using System.Threading;
using System.Reflection;
using Browser_Emulator.Interfaces;
using ParseKit;
using ParseKit.CapchaRecognize;
using System.Text.RegularExpressions;
using System.Diagnostics;
using ParseKit.CORE;

namespace Browser_Emulator
{
    public partial class Area : Form, IMessageFilter, IWebBrowserEvents
    {
        static Random _rand = new Random();
        Drawing _drawing;
        WebEventProvider _webEventProvider;
        bool _inspectMode { get { return checkBox2.Checked; }}
        AreaStyle _style;
        HtmlElement _lastElement;
        IHTMLDocument2 _document { get { return webBrowser1.Document.DomDocument as IHTMLDocument2; } }

        string _lastUser;
        string _lastPwd;
        string _lastEmail;

        public Area(AreaStyle style)
        {
            InitializeComponent();
            _style = style;
            _drawing = new Drawing(new GDIPainter(webBrowser1));
            _webEventProvider = new WebEventProvider();
            Application.AddMessageFilter(this);

            Antigate.OnError += new Antigate.ErrorDel(Antigate_OnError);
            Antigate.OnRecognized += new Antigate.RecognizedDel(Antigate_OnRecognized);
        }

        void ClickSubmit(object o)
        {
            try
            {
                HtmlDocument doc = webBrowser1.Document;
                BrowserBot bsb = new BrowserBot(doc);
                HtmlElement el = bsb.GetByNameAttr("captcha_response_field");
                bsb.SetValue(el, o as string);

                el = bsb.GetById("submit");
                bsb.Click(el);
            }
            catch (Exception e)
            {
                
                
            }

        }

        void Antigate_OnRecognized(string text)
        {
            try
            {
                webBrowser1.Invoke(new Action<object>(ClickSubmit), text);
            }
            catch (Exception e)
            {
            }

        }

        void Antigate_OnError(ErrordState state)
        {
            MessageBox.Show("Capcha error : " + state.ToString());
        }

        private void ApplyStyles(AreaStyle style)
        {
            this.WindowState = style.Maximized ? FormWindowState.Maximized : FormWindowState.Normal;
            this.Size = style.Size;
            this.Location = style.Location;
            this.BotPanel.Width = style.BotPanelWidth;
            this.PropertyPanel.Width = style.PropertyPanelWidth;
            this.splitContainer1.SplitterDistance = style.BrowserHeight > splitContainer1.Panel1MinSize ? style.BrowserHeight : splitContainer1.Panel1MinSize;
            this.splitContainer2.SplitterDistance = style.HtmlWindowHeight > splitContainer2.Panel1MinSize ? style.HtmlWindowHeight : splitContainer2.Panel1MinSize;
        }

        #region Inspect Keyhook
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x0101;
        const int WM_LBUTTONDOWN = 0x0201;
        const int MK_CONTROL = 0x11;
        const int WM_RBUTTONDOWN = 0x0204;
        const int WM_MOUSEMOVE = 0x0200;
        const int MK_LBUTTON = 0x0001;
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_KEYDOWN)
            {
                if ((int)m.WParam == (int)Keys.Q && GetKeyState((int)Keys.LControlKey) > 1)
	            {
                    checkBox2.Checked = !checkBox2.Checked;
                    label1.Text = "INSPECT MODE " + (checkBox2.Checked ? "ON" : "OFF");
                    return true;
	            }
            }
            else if (m.Msg == WM_LBUTTONDOWN && (int)m.WParam == MK_LBUTTON)
            {
                if (_inspectMode)
                {
                    Point p = Cursor.Position;
                    if (IsWebWindowCoordinate(p.X, p.Y))
                    {
                        HtmlElement el = SelectElement(m.LParam, typeof(HtmlElement)) as HtmlElement;
                        if (el != null)
                            UpdatePropertyWindow(el);

                        return true;
                    }
                }
            }
            else if (m.Msg == WM_MOUSEMOVE)
            {
                if (_inspectMode)
                {
                    Point p = Cursor.Position;
                    if (IsWebWindowCoordinate(p.X, p.Y))
                    {
                        HtmlElement element = SelectElement(m.LParam, typeof(HtmlElement)) as HtmlElement;

                        if (element != null && element != _lastElement)
                        {
                            UpdateLastSelected(element);
                            FocusDraw(element.DomElement as IHTMLElement);
                        }
                        return true;
                    }
                }
            }
            else if (m.Msg == WM_KEYUP)
            {
            }
            return false;
        }

        [System.Runtime.InteropServices.DllImport("user32", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern ushort GetKeyState(int nVirtKey);
        #endregion

        #region Methods

        public void RaiseBeforeNavigate(string url, int flags, string targetFrameName, ref object postData, string headers, ref bool cancel)
        {
            string strPostData = "";
            if (postData != null)
            {
                strPostData = System.Text.Encoding.UTF8.GetString((byte[])postData);
                textBox2.Text = strPostData;
            }
        }

        object SelectElement(IntPtr point, Type type)
        {
            int x = (int)point & 0xFFFF;
            int y = (int)point >> 16;

            if (type == typeof(IHTMLElement))
                return ElementFromPoint(x, y);
            else if (type == typeof(HtmlElement))
                return ElementFromPoint(new Point(x, y));

            return null;
        }

        Control GetControlByHandle(IntPtr handle, Control parent)
        {
            if (parent.Handle == handle)
		         return parent;

            if (parent.HasChildren)
            {
                foreach (Control control in parent.Controls)
	            {
                    Control temp = GetControlByHandle(handle, control);
                    if (temp !=null)
                        return temp;
	            }
            }
            return null;
        }

        bool IsWebWindowCoordinate(int x, int y)
        {
            Point location = webBrowser1.PointToScreen(new Point(0, 0));
            bool xValid = x >= location.X && x <= location.X + webBrowser1.Width;
            bool yValid = y >= location.Y && y <= location.Y + webBrowser1.Height;
            return xValid && yValid;
        }

        AreaStyle GetCurAreaStyle()
        {
            AreaStyle style = new AreaStyle();
            style.Maximized = this.WindowState == FormWindowState.Maximized ? true : false;
            style.Size = this.Size;
            style.Location = this.Location;
            style.BotPanelWidth = this.BotPanel.Width;
            style.PropertyPanelWidth = this.PropertyPanel.Width;
            style.BrowserHeight = this.splitContainer1.SplitterDistance;
            style.HtmlWindowHeight = splitContainer2.Height - this.splitContainer2.SplitterDistance;
            return style;
        }

        void UpdatePropertyWindow(object obj)
        {
            propertyGrid1.SelectedObject = obj;
        }

        void UpdateLastSelected(HtmlElement element)
        {
            _lastElement = element;
        }

        void FocusDraw(IHTMLElement element)
        {
            if (element != null)
            {
                _drawing.Clean();


                if (element.parentElement != null)
                {
                    //Rectangle parent = GetDrawingRectangle(element.parentElement);
                    //_drawing.DrawRectangle(parent, FrameStyle.Dashed);
                }

                Rectangle area = GetDrawingRectangle(element);
                _drawing.DrawFocusedArea(area, FrameStyle.Dashed);
             
            }
        }

        Rectangle GetDrawingRectangle(IHTMLElement element)
        {
            int x = 0;
            int y = 0;
            GetTotalOffset(ref x, ref y, element);
            return new Rectangle(x, y, element.offsetWidth, element.offsetHeight);
        }

        void GetTotalOffset(ref int x, ref int y, IHTMLElement element)
        {
            x += element.offsetLeft;
            y += element.offsetTop;

            IHTMLElement parent = element.offsetParent;
            if (parent!=null)
                GetTotalOffset(ref x, ref y, parent);

            return;
        }

        public IHTMLElement ElementFromPoint(int x, int y)
        {
            return _document == null ? null : _document.elementFromPoint(x, y);
        }

        public HtmlElement ElementFromPoint(Point p)
        {
            HtmlDocument doc = webBrowser1.Document;
            return doc == null ? null : doc.GetElementFromPoint(p);
        }

        #endregion

        #region FormEvents
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox elements = sender as ListBox;
            if (elements.SelectedIndex > 0 && elements.SelectedIndex < webBrowser1.Document.All.Count)
            {
                HtmlElement el = webBrowser1.Document.All[elements.SelectedIndex];
                if (el != _lastElement)
                {
                    UpdateLastSelected(el);
                    propertyGrid1.SelectedObject = el;
                }
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    webBrowser1.Navigate(textBox1.Text);
                }
                e.Handled = true;
            }
        }

        private void Area_Load(object sender, EventArgs e)
        {
            IPainter p = new GDIPainter(this);
            propertyGrid1.SelectedObject = p;
            ApplyStyles(_style);
        }

        private void Area_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.RemoveMessageFilter(this);

            AreaStyle.SaveStyles(Path.Combine(Application.StartupPath, "styles"), GetCurAreaStyle());
        }

        private void textBox2_RegionChanged(object sender, EventArgs e)
        {
            MessageBox.Show("asdasdas");
        }

        public string GetString(int lenght = 10)
        {
            Regex rx = new Regex(@"\d");
            string s = string.Empty;

            while (s.Length < lenght)
	        {
		        s += rx.Replace(Path.GetRandomFileName(), "").Replace(".", "");
	        }

            if (s.Length > 15)
            {
                return s.Substring(0, 15);
            }

            return s;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BrowserBot bb = new BrowserBot(webBrowser1.Document);
            string cookie = webBrowser1.Document.Cookie;

            HtmlElement el;
            el = bb.GetById("firstName");
            bb.SetValue(el, GetString());

            el = bb.GetById("lastName");
            bb.SetValue(el, GetString());

            _lastEmail = GetString() + "@asdasd.ru";
            el = bb.GetById("email");
            bb.SetValue(el, _lastEmail);

            el = bb.GetById("city");
            bb.SetValue(el, GetString());

            _lastUser = GetString();
            el = bb.GetById("username");
            bb.SetValue(el, _lastUser);

            _lastPwd = GetString() + "7";
            el = bb.GetById("password");
            bb.SetValue(el, _lastPwd);

            el = bb.GetById("passwordConfirm");
            bb.SetValue(el, _lastPwd);

            HtmlElementCollection elc = webBrowser1.Document.GetElementsByTagName("img");
            foreach (HtmlElement item in elc)
            {
                string classAttr = new Regex(@"class=([^\s]*)").Match(item.OuterHtml).Groups[1].Value;
                if (classAttr == "oCaptchaImage")
                {
                    Uri uri = new Uri(item.GetAttribute("src"));
                    DownloaderObj obj = new DownloaderObj(uri, null);
                    Downloader.DownloadSync(obj);
                    byte[] b = obj.Data;

                    Antigate.PostImage(b, ImgType.Jpg);
                }
            }



            #region GoogleAccounts
            /*
             el = bb.FindById("GmailAddress");
            bb.SetValue(el, Path.GetRandomFileName());

            string password = Path.GetRandomFileName();
            el = bb.FindById("Passwd");
            bb.SetValue(el, password);
            el = bb.FindById("PasswdAgain");
            bb.SetValue(el, password);

            el = bb.FindById("BirthDay");
            int day = _rand.Next(1, 28);
            bb.SetValue(el, day.ToString());

            el = bb.FindById("BirthMonth");
            int month = _rand.Next(1, 13);
            bb.SetComboBoxValue(el, month.ToString());

            el = bb.FindById("BirthYear");
            int year = 1970 + _rand.Next(0, 30);
            bb.SetValue(el, year.ToString());

            el = bb.FindById("Gender");
            int gender = _rand.Next(0, 3);
            string genderVal = string.Empty;
            if (gender == 0)
                genderVal = "FEMALE";
            else if (gender == 1)
		        genderVal = "MALE";
	        else if (gender == 2)
		        genderVal = "OTHER";
            bb.SetValue(el, genderVal);

            el = bb.FindById("CountryCode");
            Regex country = new Regex("<OPTION value=(?<country>[^>]*)>");
            MatchCollection contries = country.Matches(el.OuterHtml);
            int i = _rand.Next(0, contries.Count);
            bb.SetValue(el, contries[i].Groups["country"].Value);

            el = bb.FindById("HomepageSet");
            el.OuterHtml = el.OuterHtml.Replace("CHECKED", "UNCHECKED");

            //el = bb.FindById("recaptcha_response_field");
            //string capcha = "";
            //bb.SetValue(el, capcha);

            el = bb.FindById("TermsOfService");
            bb.SetCheckRadioBox(el, true);

            el = bb.FindById("submitbutton");
            bb.Click(el);
             */
            #endregion

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser browser = sender as WebBrowser;
            _webEventProvider.UpdateBrowserEvents(browser);
            label1.Text = "Navigated " + browser.Url;

            //listBox1.Items.Clear();
            //foreach (HtmlElement el in browser.Document.All)
            //{
            //    listBox1.Items.Add(string.Format("Tag: {0}; Name: {1}; Id: {2}", el.TagName, el.Name, el.Id)); 
            //}
        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            WebBrowser browser = sender as WebBrowser;
            label1.Text = "Navigating " + browser.Url;
            BrowserControl.TabPages[0].Text = browser.Url.Host;
            textBox1.Text = browser.Url.OriginalString;

            if (webBrowser1.Document.Url.OriginalString.Contains("please-verify"))
            {
                StreamWriter sw = new StreamWriter(@"c:\odesk", true, Encoding.UTF8);
                sw.WriteLine(string.Format("{0}|{1}|{2}", _lastUser, _lastPwd, _lastEmail));
                sw.Close();
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                webBrowser1.Visible = false;
            }
            else if (!checkBox3.Checked)
            {
                webBrowser1.Visible = true;
            }
        }
        #endregion

        private void button4_Click(object sender, EventArgs e)
        {

            webBrowser1.Navigate("http://accounts.google.com");
            //string s = @"continue=https%3A%2F%2Faccounts.google.com%2FManageAccount&timeStmp=1355947256889&secTok=.AG5fkS-d-ewIJv2qo-aCVrcRe4mqaO59Eg%3D%3D&dsh=-8837438456277183378&ktl=AsrBF*qZCggyYjS+Zx8WuC+xTVhhxl5Iv*vDuveORjHf+WBFgiPd6XESPTph9v_X0yMW0NiWkrOdl32YkWhhx4thgQxLucMig8Dc5Wnb+.e0P5Gw9tIigXepxh_ewQxLu2KcQHbrz*C_q98Rm-.xTxMsxnEE5B.UC6GjTeK0+ush5&ktf=FirstName+LastName+GmailAddress+Passwd+PasswdAgain+RecoveryPhoneNumber+RecoveryEmailAddress+&_utf8=%E2%98%83&bgresponse=%21A0JJtbxzKuElskRNZCqWTxnwTQIAAAL-UgAAARkqBYd3AbshZOrqm4k2GRHr3VZir6Dy3hjTVcLV5hmaCnBeAaNKtVpvHAuPwPsjjB65OLBx6a0kOsiz1hQ0UBbe_Nj4UVbxDyXMGgaQK9edeONerl0YrNmWXZ8VnbxaoOANK9RXEACrMgcmRZegVUY57Pdbskgv6ztLUx27cVGq0EFJLRqwTQPJXa5b0ujg1So3QW_fX_XRygvDk-3UbFezYkYSuijgn357BzyJ9eFqtnRI_c5P3UXR2lWl22iYdPJuSQcMt1vkywxIHjCLFG8P53hhFI_kBCGqvor1Nw7dle5LmPAKudD1nw40i1kYGTnvyRErSF2-WBTee8CUJPikl-jXrYtUuP56clLioyWx6PzCv5-qRdg9IceYXrK82daWrXg3SM1doYw04paZI9yFetrXdrvfubnfWTjYHgsssgeJloDmOgDnAuwgG0OxUXTp_2Y2TFvP9O8KpSd5YCcXG3uVHvJ9LgN8-hC7kZRn07k3JEAghigkVhNIPc6HLSXm9L5JhlVTV7F3aLaqitloUCLFBeeXoE-IfMibyu4DhMUKe4c1FFrlhjbZX43-BWt93ZdKab2ruRosXyazm76rUKhTNxEeQKGXo2j13HlEVazmXqrjqjMapfbMifIqESlzculrVeDceF7adIxMmqArHziN1yJc9pnv9aKKVOlNgW09LYGtICx94zU6rNcSB7As0VZRqr1GAEKhmTq0DXkxbCdFWm5_TJZUBTjOBnnEdNNM6BSw0jVNvh3DuI4dgE-ef-On1BCi0ViGTLdMfDls_f1et5z3_CN4VnKGWeEtWG4s28c4jjDtQADaWTXXNZIGxSvZ43AoAxlFakE0CyUDROBx3q-G7lFlP3AmtVfhCA-n386zL4AUUNVgtWH4ITUdGGsHyJJTikRoh5GPfnXXXh7xE6dk-QEgBHT6I_Fn7PYfJbJHAw3Yry4Ok-1TSAGSOrJiq0dv3lTLVx8wVopHhGrjDPmboJVx8tK0p7KvNokKrO6L-8Dslu_bgMuO0I5MusWdZfV_UBHkGF8DuCNhrvDi42sqOkW1glGblwf3OvJCo_xvBwUW9JsQtAXCFBd2rDGDcB-WPqqEVTM1kMHfpAtWdjDLBweaqhYEOpF9bjRrWWip3QTejpvCfaa5Dwue8uar0qCVtkGsFXd6hKgrVQ1KcCRfYIm_iq_GQO7e0-QjH6Y0ZNoeUHIAgfvIdD7a2ExGXpgAQGG9Ma9qMWbw-VcBDIGI2uldVMT0dcNYdA2zcFESwcsT6Tmjffd1W3_w9oO_77w3XrYftpf-BTew8s1y_mctorvwzvcM5KvjvIVGsokSnSUAlRKuyZxqJp5Ug_Z2Nz9lD7TnxKFYBaN8B0iFYkjPsAkdkucjAeMppwFGXe-jXjtMLWtGtJkymcTippLJiuYCSFNhESVwoWa_r9TbCqt53tTn-izBnNqt0RQff42WUuRLrQpmImOaibIvVZFkh6bBaPjQ8iRJtvLtEWLzQZ-1LJLoClilZ34oCs97qNSZ-lHzGRWf207OdZrKsq-ZEuRCMtDfFhQEEMLus9_BSxBW5H3IIrOXhETIbLo_kmj7I0A1Ri42yfJmdaH9UfgnMs_v6VJmgChdnr6LOhM_CC0UIn34I_ES4l8R_eSHJpN4LwcKobsJEUX0_9E2CjrPTBWdP8Sw_TaZjJ66ZnuEabpvlYz5HcoPUmnK49eGds1efjAwFBzM9E3UXoVJV-0IrSWhWv1DMHQ_0WTgq3dG1s6J51TOKzbxHxSfsO23kzGwI4hnZvGB_YzdFU4JQE3c8R4pSU15902ZSgvngn72sRUBVaz8Bd8ouBpk_O8X8yUQVmrDtVGj6pTG7xgZMNj--WpwAAW2Oxej_HeLmG8iZKaM4AKv_A&FirstName=asdasda&LastName=asdadadasd&GmailAddress=asdasdad337&Passwd=rundll32.exe+modemui.dll+InvokeControlPanel&PasswdAgain=rundll32.exe+modemui.dll+InvokeControlPanel&BirthDay=11&BirthMonth=04&BirthYear=1990&Gender=FEMALE&RecoveryPhoneCountry=RU&RecoveryPhoneNumber=&RecoveryEmailAddress=&HomepageSetSuccess=&signuptoken=03AHJ_VuuLluhaqoaiTz2h5AhZ66oS7B0hVZB-uEgoEYCdWBUkdasrihr5ro-Y5pDbFZjotnlEoJpEeerMQ226ySR-Vvju2cRV0lHJvlpg9gaathtMGvR0INsFDt0ClNFsld_YRpNMYjlg50TQ0fIn9KPk0cbWFTRC34rBag8SUxU-I0Dw1u02ayod9b0-lYA4lG76Z670e30g&signuptoken_audio=03AHJ_Vuvcay5hovv6DfzOTHSfcCr90UIuWKU3WvhdWCsq8H5LHE8ylUBiQ3frjxIxPXe7Xu9I4sNxmZaqto40yFMeX9uiHlhezyCQ3HCbFT3vQ5NQABk-zmEUkRN0dDib0bu-idtvMyogldloRXyn6ASzOcuIscl0Qc4gvMT8kfjQzPlNvBnhLLGG42bA_mYCDff5v4HgzWMaQvUGnMzNwx2sqGeVkoJoww&signupcaptchaStats=4-GAHQ1wzdVqaJbMC_GIt3T_Lo24fSX-3Kg1yBkdTy0%3AlV_q7mTt-wvNo4nyBd2iBA&recaptchaKeyVersion=0&recaptcha_challenge_field=03AHJ_VutBNfFYui0e6fU4RWX_MgYrQxzzmrUzP4gy_TJGXLlWkY7OUShYYb539FRezovZOeXxZr53AgKQFSo6ZA2oCqUtZ8NnoP_KOkzGHmCs6DSA1yrXZNO72QuARzn8Ui_MHokR2oD4aCUwwEBeE4chkeeYnr6T2WvwnzHTvYjvFJ_OBViqXC4&recaptcha_response_field=ycgions&CountryCode=RU&TermsOfService=yes&Personalization=yes";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Process.Start("RunDll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 4351");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!webBrowser1.Document.Url.OriginalString.Contains("www.odesk.com"))
            {
                webBrowser1.GoHome();
            }
            else
            {
                webBrowser1.Navigate("https://www.odesk.com/signup/contractor");
            }
        }
    }
}
