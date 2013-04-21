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
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        bool logShown;
        bool renderShown;
        bool proxyShown;
        bool patternShown;
        bool parseShown;

      
        private void AddCompletedItem(DevourTarget target, RatedProxy asociatedProxy, List<KeyValuePair<string, string>> data = null)
        {
            if (target == null)
                return;

            TreeNode item = new TreeNode(string.Format("[{0}] Downloaded page from {1}, attempts {2}, lifes {3}", DateTime.Now.ToLongTimeString(), target.Uri.Host, target.Attempts, target.Lifes));
            TreeNode proxy = new TreeNode("Proxy: localhost");
            TreeNode readerClass = new TreeNode(string.Format("Reader: {0}", target.Reader.GetType().Name));
            TreeNode uri = new TreeNode(string.Format("Uri: {0}", target.Uri.OriginalString));
            TreeNode dataNode = new TreeNode("Data");
            TreeNode uriParams = new TreeNode("Params");

            if (asociatedProxy != null)
            {
                string proxyAddr = string.Format("Proxy: {0}:{1}", asociatedProxy.Address.Host, asociatedProxy.Address.Port);
                proxy.Nodes.Add(string.Format("aLatency: {0}"), asociatedProxy.AvgLatency.ToString());
                proxy.Nodes.Add(string.Format("aSpeed: {0}"), asociatedProxy.AvgSpeed.ToString());
                proxy.Nodes.Add(string.Format("SitesRate: {0}"), asociatedProxy.SitesRate.ToString());
                proxy.Nodes.Add(string.Format("DownloadsRate: {0}"), asociatedProxy.MultidownloadRate.ToString());
                proxy.Nodes.Add(string.Format("CheckTimes: {0}"), asociatedProxy.CheckTimes.ToString());
                proxy.Nodes.Add(string.Format("RBL_ban: {0}"), asociatedProxy.RBLBanRate.ToString());
                proxy.Nodes.Add(string.Format("SERate: {0}"), asociatedProxy.SEQuality.ToString());
                proxy.ForeColor = Color.Orchid;
            }
            else
                proxy.ForeColor = Color.PaleVioletRed;

            if (data != null)
            {
                dataNode.Name = string.Format("Data ({0})", data.Count);
                for (int i = 0; i < data.Count; i++)
                {
                    dataNode.Nodes.Add(string.Format("{0}: {1}", data[i].Key, data[i].Value)); 
                }
                proxy.ForeColor = Color.Orchid;
            }
            else
                dataNode.ForeColor = Color.PaleVioletRed;

            Dictionary<string,string> prms = target.Uri.GetParams();
            if (prms != null)
            {
                foreach (var prm in prms)
                {
                    uriParams.Nodes.Add(string.Format("name: {0}, val {1} \r\n", prm.Key, prm.Value));
                }

                uriParams.Text = string.Format("Params ({0})", prms.Count);
            }

            uri.Nodes.Add(uriParams);
            uri.Nodes.Add(string.Format("UserInfo: {0}", target.Uri.UserInfo));
            uri.Nodes.Add(string.Format("Authority: {0}", target.Uri.Authority));
            item.Nodes.Add(dataNode);
            item.Nodes.Add(readerClass);
            item.Nodes.Add(proxy);
            item.Nodes.Add(uri);
            item.ForeColor = Color.LightGreen;

            //treeView1.Nodes.Add(item);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!logShown)
            {
                Log form = new Log();
                form.Show();
                logShown = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!renderShown)
            {
                HtmlRender form = new HtmlRender();
                form.Show();
                renderShown = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!proxyShown)
            {
                Proxy form = new Proxy();
                form.Show();
                proxyShown = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!patternShown)
            {
                Pattern form = new Pattern();
                form.Show();
                patternShown = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (!parseShown)
            {
                Parse form = new Parse();
                form.Show();
                parseShown = true;
            }
        }
    }
}
