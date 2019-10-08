using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Parsnet
{
    class JustifyLabel:Label
    {
        protected override void OnPrint(PaintEventArgs e)
        {
            base.OnPrint(e);
        }
    }
}
