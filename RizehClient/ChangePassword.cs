using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Parsnet
{
    public partial class ChangePassword : Form
    {
        public ChangePassword()
        {
            InitializeComponent();
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            Regex PasswordRegex = new Regex(@".{8,16}");
            errPassword.SetError(tbpNewPassword, "");
            errPassword.SetError(tbpOldPassword, "");
            errPassword.SetError(tbpConfirmPassword, "");

            if (String.IsNullOrEmpty(txtOldPassword.Text))
            {
                errPassword.SetIconAlignment(tbpOldPassword, ErrorIconAlignment.MiddleRight);
                errPassword.SetIconPadding(tbpOldPassword, 10);
                errPassword.SetError(tbpOldPassword, "رمز قدیم خود را وارد کنید");
                return;
            }
            else if (String.IsNullOrEmpty(txtNewPassword.Text))
            {
                errPassword.SetIconAlignment(tbpNewPassword, ErrorIconAlignment.MiddleRight);
                errPassword.SetIconPadding(tbpNewPassword, 10);
                errPassword.SetError(tbpNewPassword, "رمز جدید خود را وارد کنید");
                return;
            }
            else if (String.IsNullOrEmpty(txtConfirmPassword.Text))
            {
                errPassword.SetIconAlignment(tbpConfirmPassword, ErrorIconAlignment.MiddleRight);
                errPassword.SetIconPadding(tbpConfirmPassword, 10);
                errPassword.SetError(tbpConfirmPassword, "تکرار رمز جدید خود را وارد کنید");
                return;
            }
            else if (txtConfirmPassword.Text != txtNewPassword.Text)
            {
                errPassword.SetIconAlignment(tbpConfirmPassword, ErrorIconAlignment.MiddleRight);
                errPassword.SetIconPadding(tbpConfirmPassword, 10);
                errPassword.SetError(tbpConfirmPassword, "تکرار رمز جدید با رمز جدید برابر نیست");
                return;
            }
            else if (PasswordRegex.IsMatch(txtNewPassword.Text) == false)
            {
                errPassword.SetIconAlignment(tbpNewPassword, ErrorIconAlignment.MiddleRight);
                errPassword.SetIconPadding(tbpNewPassword, 10);
                errPassword.SetError(tbpNewPassword, "رمز عبور حداقل 8 و حداکثر 16 کارکتر است");
                return;
            }

            var owner = (MainForm)this.Owner;
            owner.SendMessage(Commands.ChangePassword, new { oldPassword = txtOldPassword.Text, newPassword = txtNewPassword.Text });
            this.Close();
        }

        private void ChangePassword_Load(object sender, EventArgs e)
        {

        }

        private void txtPassword_Enter(object sender, EventArgs e)
        {
            var box = (TextBox)sender;
            box.BackColor = Color.FromArgb(3, 143, 105);
            box.Parent.BackColor = Color.FromArgb(3, 143, 105);
        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {
            var box = (TextBox)sender;
            box.BackColor = Color.FromArgb(3, 166, 120);
            box.Parent.BackColor = Color.FromArgb(3, 166, 120);
        }

    }
}
