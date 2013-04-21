// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they bagin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using HtmlRenderer.Demo.Properties;
using Timer = System.Threading.Timer;

namespace HtmlRenderer.Demo
{
    public partial class DemoForm : Form
    {
        #region Fields and Consts

        /// <summary>
        /// the html samples used for performance testing
        /// </summary>
        private readonly List<string> _perfTestSamples = new List<string>();

        /// <summary>
        /// the html samples to show in the demo
        /// </summary>
        private readonly Dictionary<string, string> _samples = new Dictionary<string, string>();

        /// <summary>
        /// timer to update the rendered html when html in editor changes with delay
        /// </summary>
        private readonly Timer _updateHtmlTimer;

        /// <summary>
        /// used ignore html editor updates when updating seperatly
        /// </summary>
        private bool _updateLock;
        
        #endregion


        /// <summary>
        /// Init.
        /// </summary>
        public DemoForm()
        {
            InitializeComponent();

            //Icon = Resources.html;

            _htmlPanel.Bridge = this;
            _htmlToolTip.Bridge = this;

            //_htmlToolTip.SetToolTip(_htmlPanel, Resources.Tooltip);

            _htmlEditor.Font = new Font(FontFamily.GenericMonospace, 10);
            
            StartPosition = FormStartPosition.CenterScreen;
            var size = Screen.GetWorkingArea(Point.Empty);
            Size = new Size((int) (size.Width*0.7), (int) (size.Height*0.8));

            _updateHtmlTimer = new Timer(OnUpdateHtmlTimerTick);
        }


        #region Private methods

        /// <summary>
        /// On text change in the html editor update 
        /// </summary>
        private void OnHtmlEditorTextChanged(object sender, EventArgs e)
        {
            if (!_updateLock)
            {
                _updateHtmlTimer.Change(1000, int.MaxValue);
            }
        }

        /// <summary>
        /// Update the html renderer with text from html editor.
        /// </summary>
        private void OnUpdateHtmlTimerTick(object state)
        {
            BeginInvoke(new MethodInvoker(() =>
            {
                _updateLock = true;
                
                _htmlPanel.Text = _htmlEditor.Text;
                //SyntaxHilight.AddColoredText(_htmlEditor.Text, _htmlEditor);

                UpdateWebBrowserHtml();

                _updateLock = false;
            }));
        }

        /// <summary>
        /// Open the current html is external process - the default user browser.
        /// </summary>
        private void OnOpenExternalViewButtonClick(object sender, EventArgs e)
        {
            var html = _showGeneratedHtmlCB.Checked ? _htmlPanel.GetHtml() : _htmlEditor.Text;
            var tmpFile = Path.ChangeExtension(Path.GetTempFileName(), ".htm");
            File.WriteAllText(tmpFile, html);
            Process.Start(tmpFile);
        }

        /// <summary>
        /// Show\Hide the web browser viwer.
        /// </summary>
        private void OnToggleWebBrowserButton_Click(object sender, EventArgs e)
        {
            _webBrowser.Visible = !_webBrowser.Visible;
            _splitter.Visible = _webBrowser.Visible;
            _toggleWebBrowserButton.Text = _webBrowser.Visible ? "Hide IE View" : "Show IE View";

            if(_webBrowser.Visible)
            {
                _webBrowser.Width = _splitContainer2.Panel2.Width / 2;
                UpdateWebBrowserHtml();
            }
        }

        /// <summary>
        /// Update the html shown in the web browser
        /// </summary>
        private void UpdateWebBrowserHtml()
        {
            if (_webBrowser.Visible)
            {
                _webBrowser.DocumentText = _showGeneratedHtmlCB.Checked ? _htmlPanel.GetHtml() : GetFixedHtml();
            }
        }

        /// <summary>
        /// Fix the raw html by replacing bridge object properties calls with path to file with the data returned from the property.
        /// </summary>
        /// <returns>fixed html</returns>
        private string GetFixedHtml()
        {
            var html = _htmlEditor.Text;

            while(true)
            {
                var match = Regex.Match(html, @"\""property:(\w.*?)\""", RegexOptions.IgnoreCase);
                if(!match.Success)
                    break;

                var prop = _htmlPanel.Bridge.GetType().GetProperty(match.Groups[1].Value, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                if(prop != null)
                {
                    var data = prop.GetValue(_htmlPanel.Bridge, new object[0]);
                    if(data != null)
                    {
                        var tmpFile = Path.GetTempFileName();
                        if (data is string)
                        {
                            File.WriteAllText(tmpFile, (string) data);
                        }
                        else if (data is Image)
                        {
                            ((Image) data).Save(tmpFile, ImageFormat.Jpeg);
                        }
                        html = html.Replace(match.Value, "\"" + tmpFile + "\"");
                    }
                }
            }

            return html;
        }

        /// <summary>
        /// On change if to show generated html or regular update the web browser to show the new choice.
        /// </summary>
        private void OnShowGeneratedHtmlCheckedChanged(object sender, EventArgs e)
        {
            UpdateWebBrowserHtml();
        }

        /// <summary>
        /// Execute performance test by setting all sample htmls in a loop.
        /// </summary>
        private void OnRunTestButtonClick(object sender, EventArgs e)
        {
            _updateLock = true;
            _runTestButton.Text = "Running..";
            _runTestButton.Enabled = false;
            Application.DoEvents();

            var sw = Stopwatch.StartNew();

            const int iterations = 12;
            for (int i = 0; i < iterations; i++)
            {
                foreach (var html in _perfTestSamples)
                {
                    _htmlPanel.Text = html;
                    Application.DoEvents(); // so paint will be called
                }
            }
            
            sw.Stop();

            var msg = string.Format("Total: {0} mSec\r\nIterationAvg: {1:N2} msec\r\nSingleAvg: {2:N2} msec",
                                    sw.ElapsedMilliseconds, sw.ElapsedMilliseconds / (double)iterations, sw.ElapsedMilliseconds / (double)iterations / _perfTestSamples.Count);
            Clipboard.SetDataObject(msg);
            MessageBox.Show(msg, "Test run results");

            _updateLock = false;
            _runTestButton.Text = "Run Tests";
            _runTestButton.Enabled = true;
        }

        /// <summary>
        /// Gets the stylesheet for sample documents
        /// </summary>
        internal string StyleSheet
        {
            get { return @"h1, h2, h3 { color: navy; font-weight:normal; }
                    h1 { margin-bottom: .47em }
                    h2 { margin: .3em }
                    h3 { margin-bottom: .4em }
                    ul { margin-top: .5em }
                    ul li {margin: .25em}
                    body { font:10pt Tahoma }
		            pre  { border:solid 1px gray; background-color:#eee; padding:1em }
                    .gray    { color:gray; }
                    .example { background-color:#efefef; corner-radius:5px; padding:0.5em; }
                    .whitehole { background-color:white; corner-radius:5px; padding:10px; }
                    .caption { font-size: 1.1em }
                    .comment { color: green; margin-bottom: 5px; margin-left: 3px; }
                    .comment2 { color: green; margin-left: 5px; }"; }
        }

        /// <summary>
        /// Gets a html image
        /// </summary>
        internal static Image HtmlIcon
        {
            get { return Properties.Resources.html32; }
        }

        /// <summary>
        /// Gets a star image
        /// </summary>
        internal static Image StarIcon
        {
            get { return Properties.Resources.favorites32; }
        }

        /// <summary>
        /// Gets the font icon
        /// </summary>
        internal static Image FontIcon
        {
            get { return Properties.Resources.font32; }
        }

        /// <summary>
        /// Gets the comment icon
        /// </summary>
        internal static Image CommentIcon
        {
            get { return Properties.Resources.comment16; }
        }

        /// <summary>
        /// Gets the image icon
        /// </summary>
        internal static Image ImageIcon
        {
            get { return Properties.Resources.image32; }
        }

        /// <summary>
        /// Gets the method icon
        /// </summary>
        internal static Image MethodIcon
        {
            get { return Properties.Resources.method16; }
        }

        /// <summary>
        /// Gets the property icon
        /// </summary>
        internal static Image PropertyIcon
        {
            get { return Properties.Resources.property16; }
        }

        /// <summary>
        /// Gets the property icon
        /// </summary>
        internal static Image EventIcon
        {
            get { return Properties.Resources.Event16; }
        }

        /// <summary>
        /// Says hello with a message box
        /// </summary>
        internal static void SayHello()
        {
            MessageBox.Show("Hello you!");
        }

        internal static void ShowSampleForm()
        {
            //using (SampleForm f = new SampleForm())
            //{
            //    f.ShowDialog();
            //}
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            HtmlDocument doc;
            

            StreamReader sr = new StreamReader(File.Open(@"c:\test\Яндекс.Почта.html", FileMode.Open), Encoding.UTF8);
            string html = sr.ReadToEnd();
            sr.Close();

            _updateLock = true;

            //SyntaxHilight.AddColoredText(html, _htmlEditor);

            Application.UseWaitCursor = true;

            _htmlPanel.Text = html;

            Application.UseWaitCursor = false;
            _updateLock = false;

            //UpdateWebBrowserHtml();
        }
    }
}