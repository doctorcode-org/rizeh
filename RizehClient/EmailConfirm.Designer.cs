namespace Parsnet
{
    partial class EmailConfirm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textboxPanel1 = new Parsnet.TextboxPanel();
            this.txtConfirmCode = new System.Windows.Forms.TextBox();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.errCode = new System.Windows.Forms.ErrorProvider(this.components);
            this.panel1.SuspendLayout();
            this.textboxPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errCode)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.textboxPanel1);
            this.panel1.Controls.Add(this.btnConfirm);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.ForeColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.panel1.Size = new System.Drawing.Size(347, 159);
            this.panel1.TabIndex = 0;
            // 
            // textboxPanel1
            // 
            this.textboxPanel1.BackColor = System.Drawing.Color.Green;
            this.textboxPanel1.BorderColor = System.Drawing.Color.Green;
            this.textboxPanel1.BorderWidth = 2;
            this.textboxPanel1.Controls.Add(this.txtConfirmCode);
            this.textboxPanel1.Location = new System.Drawing.Point(61, 68);
            this.textboxPanel1.Name = "textboxPanel1";
            this.textboxPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.textboxPanel1.Size = new System.Drawing.Size(234, 27);
            this.textboxPanel1.TabIndex = 5;
            // 
            // txtConfirmCode
            // 
            this.txtConfirmCode.BackColor = System.Drawing.Color.Green;
            this.txtConfirmCode.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtConfirmCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtConfirmCode.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.txtConfirmCode.ForeColor = System.Drawing.Color.White;
            this.txtConfirmCode.Location = new System.Drawing.Point(5, 5);
            this.txtConfirmCode.MaxLength = 6;
            this.txtConfirmCode.Name = "txtConfirmCode";
            this.txtConfirmCode.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtConfirmCode.Size = new System.Drawing.Size(224, 17);
            this.txtConfirmCode.TabIndex = 7;
            this.txtConfirmCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnConfirm
            // 
            this.btnConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(166)))), ((int)(((byte)(120)))));
            this.btnConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirm.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnConfirm.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(143)))), ((int)(((byte)(105)))));
            this.btnConfirm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirm.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(178)));
            this.btnConfirm.ForeColor = System.Drawing.Color.White;
            this.btnConfirm.Location = new System.Drawing.Point(118, 102);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(100, 35);
            this.btnConfirm.TabIndex = 4;
            this.btnConfirm.Text = "تایید";
            this.btnConfirm.UseVisualStyleBackColor = false;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(323, 56);
            this.label2.TabIndex = 3;
            this.label2.Text = "نام نویسی شما با موفقیت انجام و کد تایید به آدرس ایمیل شما ارسال شده است ، آنرا و" +
    "ارد نمایید.";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // errCode
            // 
            this.errCode.ContainerControl = this;
            // 
            // EmailConfirm
            // 
            this.AcceptButton = this.btnConfirm;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(166)))), ((int)(((byte)(120)))));
            this.ClientSize = new System.Drawing.Size(347, 159);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EmailConfirm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "تایید آدرس ایمیل";
            this.panel1.ResumeLayout(false);
            this.textboxPanel1.ResumeLayout(false);
            this.textboxPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errCode)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.ErrorProvider errCode;
        private TextboxPanel textboxPanel1;
        private System.Windows.Forms.TextBox txtConfirmCode;

    }
}