using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Parsnet
{
    public partial class HtmlPanel : UserControl
    {
        private string _Content;

        public String Content
        {
            get
            {
                return _Content;
            }
            set
            {
                _Content = value;
                WB.DocumentText = value;
            }
        }

        public HtmlPanel()
        {
            InitializeComponent();
            WB.DocumentText = _Content;
        }

        private void WB_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void HtmlPanel_Load(object sender, EventArgs e)
        {
            WB.DocumentText = _Content;
        }
    }
}
