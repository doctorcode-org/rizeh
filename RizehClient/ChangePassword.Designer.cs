namespace Parsnet
{
    partial class ChangePassword
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
            this.ChangePasswordPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnChangePassword = new System.Windows.Forms.Button();
            this.errPassword = new System.Windows.Forms.ErrorProvider(this.components);
            this.tbpConfirmPassword = new Parsnet.TextboxPanel();
            this.txtConfirmPassword = new System.Windows.Forms.TextBox();
            this.tbpNewPassword = new Parsnet.TextboxPanel();
            this.txtNewPassword = new System.Windows.Forms.TextBox();
            this.tbpOldPassword = new Parsnet.TextboxPanel();
            this.txtOldPassword = new System.Windows.Forms.TextBox();
            this.ChangePasswordPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errPassword)).BeginInit();
            this.tbpConfirmPassword.SuspendLayout();
            this.tbpNewPassword.SuspendLayout();
            this.tbpOldPassword.SuspendLayout();
            this.SuspendLayout();
            // 
            // ChangePasswordPanel
            // 
            this.ChangePasswordPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(166)))), ((int)(((byte)(120)))));
            this.ChangePasswordPanel.Controls.Add(this.btnChangePassword);
            this.ChangePasswordPanel.Controls.Add(this.tbpConfirmPassword);
            this.ChangePasswordPanel.Controls.Add(this.tbpNewPassword);
            this.ChangePasswordPanel.Controls.Add(this.label3);
            this.ChangePasswordPanel.Controls.Add(this.label2);
            this.ChangePasswordPanel.Controls.Add(this.label1);
            this.ChangePasswordPanel.Controls.Add(this.tbpOldPassword);
            this.ChangePasswordPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChangePasswordPanel.Location = new System.Drawing.Point(0, 0);
            this.ChangePasswordPanel.Name = "ChangePasswordPanel";
            this.ChangePasswordPanel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ChangePasswordPanel.Size = new System.Drawing.Size(374, 215);
            this.ChangePasswordPanel.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(249, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "رمز قدیم:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(249, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "رمز جدید:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(249, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "تکرار رمز جدید:";
            // 
            // btnChangePassword
            // 
            this.btnChangePassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(166)))), ((int)(((byte)(120)))));
            this.btnChangePassword.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnChangePassword.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnChangePassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChangePassword.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(178)));
            this.btnChangePassword.ForeColor = System.Drawing.Color.White;
            this.btnChangePassword.Location = new System.Drawing.Point(101, 151);
            this.btnChangePassword.Name = "btnChangePassword";
            this.btnChangePassword.Size = new System.Drawing.Size(110, 40);
            this.btnChangePassword.TabIndex = 5;
            this.btnChangePassword.Text = "تغییر رمز";
            this.btnChangePassword.UseVisualStyleBackColor = false;
            this.btnChangePassword.Click += new System.EventHandler(this.btnChangePassword_Click);
            // 
            // errPassword
            // 
            this.errPassword.ContainerControl = this;
            this.errPassword.RightToLeft = true;
            // 
            // tbpConfirmPassword
            // 
            this.tbpConfirmPassword.BorderColor = System.Drawing.Color.White;
            this.tbpConfirmPassword.BorderWidth = 2;
            this.tbpConfirmPassword.Controls.Add(this.txtConfirmPassword);
            this.tbpConfirmPassword.Location = new System.Drawing.Point(63, 106);
            this.tbpConfirmPassword.Name = "tbpConfirmPassword";
            this.tbpConfirmPassword.Padding = new System.Windows.Forms.Padding(10);
            this.tbpConfirmPassword.Size = new System.Drawing.Size(180, 36);
            this.tbpConfirmPassword.TabIndex = 4;
            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(166)))), ((int)(((byte)(120)))));
            this.txtConfirmPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtConfirmPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtConfirmPassword.ForeColor = System.Drawing.Color.White;
            this.txtConfirmPassword.Location = new System.Drawing.Point(10, 10);
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.PasswordChar = '*';
            this.txtConfirmPassword.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtConfirmPassword.Size = new System.Drawing.Size(160, 16);
            this.txtConfirmPassword.TabIndex = 0;
            this.txtConfirmPassword.Enter += new System.EventHandler(this.txtPassword_Enter);
            this.txtConfirmPassword.Leave += new System.EventHandler(this.txtPassword_Leave);
            // 
            // tbpNewPassword
            // 
            this.tbpNewPassword.BorderColor = System.Drawing.Color.White;
            this.tbpNewPassword.BorderWidth = 2;
            this.tbpNewPassword.Controls.Add(this.txtNewPassword);
            this.tbpNewPassword.Location = new System.Drawing.Point(63, 64);
            this.tbpNewPassword.Name = "tbpNewPassword";
            this.tbpNewPassword.Padding = new System.Windows.Forms.Padding(10);
            this.tbpNewPassword.Size = new System.Drawing.Size(180, 36);
            this.tbpNewPassword.TabIndex = 1;
            // 
            // txtNewPassword
            // 
            this.txtNewPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(166)))), ((int)(((byte)(120)))));
            this.txtNewPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtNewPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNewPassword.ForeColor = System.Drawing.Color.White;
            this.txtNewPassword.Location = new System.Drawing.Point(10, 10);
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.PasswordChar = '*';
            this.txtNewPassword.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtNewPassword.Size = new System.Drawing.Size(160, 16);
            this.txtNewPassword.TabIndex = 0;
            this.txtNewPassword.Enter += new System.EventHandler(this.txtPassword_Enter);
            this.txtNewPassword.Leave += new System.EventHandler(this.txtPassword_Leave);
            // 
            // tbpOldPassword
            // 
            this.tbpOldPassword.BorderColor = System.Drawing.Color.White;
            this.tbpOldPassword.BorderWidth = 2;
            this.tbpOldPassword.Controls.Add(this.txtOldPassword);
            this.tbpOldPassword.Location = new System.Drawing.Point(63, 22);
            this.tbpOldPassword.Name = "tbpOldPassword";
            this.tbpOldPassword.Padding = new System.Windows.Forms.Padding(10);
            this.tbpOldPassword.Size = new System.Drawing.Size(180, 36);
            this.tbpOldPassword.TabIndex = 0;
            // 
            // txtOldPassword
            // 
            this.txtOldPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(166)))), ((int)(((byte)(120)))));
            this.txtOldPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtOldPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOldPassword.ForeColor = System.Drawing.Color.White;
            this.txtOldPassword.Location = new System.Drawing.Point(10, 10);
            this.txtOldPassword.Name = "txtOldPassword";
            this.txtOldPassword.PasswordChar = '*';
            this.txtOldPassword.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtOldPassword.Size = new System.Drawing.Size(160, 16);
            this.txtOldPassword.TabIndex = 0;
            this.txtOldPassword.Enter += new System.EventHandler(this.txtPassword_Enter);
            this.txtOldPassword.Leave += new System.EventHandler(this.txtPassword_Leave);
            // 
            // ChangePassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 215);
            this.Controls.Add(this.ChangePasswordPanel);
            this.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(178)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangePassword";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "تغییر رمز عبور";
            this.Load += new System.EventHandler(this.ChangePassword_Load);
            this.ChangePasswordPanel.ResumeLayout(false);
            this.ChangePasswordPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errPassword)).EndInit();
            this.tbpConfirmPassword.ResumeLayout(false);
            this.tbpConfirmPassword.PerformLayout();
            this.tbpNewPassword.ResumeLayout(false);
            this.tbpNewPassword.PerformLayout();
            this.tbpOldPassword.ResumeLayout(false);
            this.tbpOldPassword.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel ChangePasswordPanel;
        private TextboxPanel tbpOldPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtOldPassword;
        private TextboxPanel tbpConfirmPassword;
        private System.Windows.Forms.TextBox txtConfirmPassword;
        private TextboxPanel tbpNewPassword;
        private System.Windows.Forms.TextBox txtNewPassword;
        private System.Windows.Forms.Button btnChangePassword;
        private System.Windows.Forms.ErrorProvider errPassword;
    }
}