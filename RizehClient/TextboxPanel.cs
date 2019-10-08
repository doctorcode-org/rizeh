using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Parsnet
{
    public class TextboxPanel : Panel
    {
        public Color BorderColor { get; set; }
        public int BorderWidth { get; set; }

        private new BorderStyle BorderStyle { get; set; }

        public TextboxPanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var pen = new Pen(BorderColor,BorderWidth);
            using (SolidBrush brush = new SolidBrush(BackColor))
            {
                e.Graphics.FillRectangle(brush, e.ClipRectangle);
                e.Graphics.DrawRectangle(pen, 0, 0, e.ClipRectangle.Right, e.ClipRectangle.Bottom);
            }
        }
    }
}
