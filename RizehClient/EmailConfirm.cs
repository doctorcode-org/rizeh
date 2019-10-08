using System;
using System.Windows.Forms;

namespace Parsnet
{
    public partial class EmailConfirm : Form
    {
        public EmailConfirm()
        {
            InitializeComponent();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtConfirmCode.Text))
            {
                errCode.SetIconAlignment(txtConfirmCode, ErrorIconAlignment.MiddleLeft);
                errCode.SetIconPadding(txtConfirmCode, 10);
                errCode.SetError(txtConfirmCode, "کد تایید را وارد کنید");
            }
            else
            {
                var owner = (MainForm)this.Owner;
                owner.SendMessage(Commands.Confirm, new { email = owner.txtEmail.Text, code = txtConfirmCode.Text });
                this.Close();
            }
        }
    }
}
