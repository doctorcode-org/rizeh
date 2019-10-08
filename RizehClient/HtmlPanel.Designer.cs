namespace Parsnet
{
    partial class HtmlPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.WB = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // WB
            // 
            this.WB.AllowNavigation = false;
            this.WB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WB.IsWebBrowserContextMenuEnabled = false;
            this.WB.Location = new System.Drawing.Point(0, 0);
            this.WB.MinimumSize = new System.Drawing.Size(20, 20);
            this.WB.Name = "WB";
            this.WB.ScrollBarsEnabled = false;
            this.WB.Size = new System.Drawing.Size(354, 208);
            this.WB.TabIndex = 0;
            this.WB.WebBrowserShortcutsEnabled = false;
            this.WB.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.WB_DocumentCompleted);
            // 
            // HtmlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.WB);
            this.Name = "HtmlPanel";
            this.Size = new System.Drawing.Size(354, 208);
            this.Load += new System.EventHandler(this.HtmlPanel_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser WB;
    }
}
