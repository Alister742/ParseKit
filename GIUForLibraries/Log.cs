using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace GIUForLibraries
{
    public partial class Log : Form
    {
        Color logFontColor = Color.Silver;

        public Log()
        {
            InitializeComponent();
        }

        private void Log_Load(object sender, EventArgs e)
        {
            GlobalLog.OnChannelMessage += new GlobalLog.SpecifyMessageDel(GlobalLog_OnChannelMessage);
            GlobalLog.OnErr += new GlobalLog.MessageDel(GlobalLog_OnErr);
            GlobalLog.OnMessage += new GlobalLog.MessageDel(GlobalLog_OnMessage);
            GlobalLog.OnException += new GlobalLog.ExceptionDel(GlobalLog_OnException);
        }

        void GlobalLog_OnException(Exception e)
        {
            string channel = LogChannel.CLR_ERR.ToString();
            TreeNode treeNode = ExtractNode(e);

            foreach (TabPage page in tabControl1.TabPages)
            {
                if (page.Name == channel + "TabPage")
                {
                    foreach (Control control in page.Controls)
                    {
                        if (control.Name == channel + "TreeView")
                        {
                            TreeView tView = control as TreeView;
                            AddTreeViewItem(tView, treeNode);
                            return;
                        }
                    }
                    TreeView treeView = CreateTreeViewOnPage(page, channel);
                    AddTreeViewItem(treeView, treeNode);
                    return;
                }
            }
            TabPage tabPage = CreateTabPage(this.tabControl1, channel);
            TreeView treView = CreateTreeViewOnPage(tabPage, channel);
            AddTreeViewItem(treView, treeNode);
        }

        void GlobalLog_OnMessage(string message, LogChannel channel)
        {
            WriteOnPage(message, channel.ToString());
        }

        private void WriteOnPage(string message, string channel)
        {
            foreach (TabPage page in tabControl1.TabPages)
            {
                if (page.Name == channel + "TabPage")
                {
                    foreach (Control control in page.Controls)
                    {
                        if (control.Name == channel + "TextBox")
                        {
                            TextBox tBox = control as TextBox;
                            WriteLineTextBox(tBox, message);
                            return;
                        }
                    }
                    TextBox textBox = CreateTextBoxOnPage(page, channel);
                    WriteLineTextBox(textBox, message);
                    return;
                }
            }
            TabPage tabPage = CreateTabPage(this.tabControl1, channel);
            TextBox txtBox = CreateTextBoxOnPage(tabPage, channel);
            WriteLineTextBox(txtBox, message);
        }

        TreeView CreateTreeViewOnPage(TabPage tabPage, string channel)
        {
            TreeView treeView = new TreeView();
            treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            treeView.Location = new System.Drawing.Point(0, 0);
            treeView.Name = channel + "TreeView";
            tabPage.Controls.Add(treeView);

            return treeView;
        }

        TextBox CreateTextBoxOnPage(TabPage tabPage, string channel)
        {
            TextBox textBox = new TextBox();
            textBox.BackColor = Color.Black;
            textBox.ReadOnly = true;
            textBox.ScrollBars = ScrollBars.Both;
            textBox.Enabled = true;
            textBox.Dock = DockStyle.Fill;
            textBox.Location = new Point(3, 3);
            textBox.Multiline = true;
            textBox.Name = channel + "TextBox";
            textBox.Font = new Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            textBox.ForeColor = logFontColor;
            InvokeOnOwnThread(tabPage, new Action(() => { tabPage.Controls.Add(textBox); }));
            textBox.Parent.Click += new EventHandler(OnParentTabClick);

            return textBox;
        }

        void OnParentTabClick(object sender, EventArgs e)
        {
            TabPage tabPage = sender as TabPage;
            if (tabPage == null)
                return;

            TextBox childTextBox = (sender as TabPage).Controls.OfType<TextBox>().FirstOrDefault();

            if (childTextBox ==null)
            return;

            childTextBox.SelectionStart = childTextBox.Text.Length;
            childTextBox.ScrollToCaret();


        }

        TabPage CreateTabPage(TabControl parent, string channel)
        {
            TabPage tabPage = new TabPage("#" + channel.ToString());
            tabPage.Name = channel.ToString() + "TabPage";
            tabPage.Location = new System.Drawing.Point(4, 22);
            tabPage.Padding = new System.Windows.Forms.Padding(3);
            tabPage.Size = new System.Drawing.Size(728, 329);
            tabPage.TabIndex = parent.TabCount > 0 ? parent.TabCount - 1 : 0;
            tabPage.UseVisualStyleBackColor = true;
            InvokeOnOwnThread(parent, new Action(() => { parent.Controls.Add(tabPage); }));

            return tabPage;
        }

        void InvokeOnOwnThread(Control control, Action act)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(act);
            }
            else
            {
                act();
            }
        }

        void WriteLineTextBox(TextBox textBox, string text)
        {
            string logText = string.Format("[{0}] {1}\r\n", DateTime.Now.ToShortTimeString(), text);

            Action writeTextBoxAction = new Action(() => { textBox.AppendText(logText); });
            InvokeOnOwnThread(textBox, writeTextBoxAction);
        }

        void GlobalLog_OnErr(string message, LogChannel channel)
        {
            WriteOnPage(message, channel.ToString());
        }

        void GlobalLog_OnChannelMessage(string message, string channel)
        {
            WriteOnPage(message, channel);
        }

        TreeNode ExtractNode(Exception e)
        {
            if (e == null)
                return null;

            TreeNode desc = new TreeNode(string.Format("[{1}], {0}", e.Message, DateTime.Now.ToLongTimeString()));
            TreeNode stackTrace = new TreeNode("Stack trace");

            string[] trace = e.StackTrace.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < trace.Length; i++)
            {
                int count = stackTrace.Nodes.Count;
                if (count == -1)
                {
                    stackTrace.Nodes.Add(trace[i]);
                }
                else
                {
                    stackTrace.Nodes[count - 1].Nodes.Add(trace[i]);
                }
            }

            desc.Nodes.Add(string.Format("From: {0}", e.TargetSite));
            desc.Nodes.Add(stackTrace);
            desc.ForeColor = Color.IndianRed;

            return desc;
        }

        void AddTreeViewItem(TreeView treeView, TreeNode node)
        {
            Action act = new Action(() => { treeView.Nodes.Add(node); });
            InvokeOnOwnThread(treeView, act);
        }

        private void Log_FormClosing(object sender, FormClosingEventArgs e)
        {
            GlobalLog.OnChannelMessage -= new GlobalLog.SpecifyMessageDel(GlobalLog_OnChannelMessage);
            GlobalLog.OnErr -= new GlobalLog.MessageDel(GlobalLog_OnErr);
            GlobalLog.OnMessage -= new GlobalLog.MessageDel(GlobalLog_OnMessage);
            GlobalLog.OnException -= new GlobalLog.ExceptionDel(GlobalLog_OnException);
        }
    }
}
