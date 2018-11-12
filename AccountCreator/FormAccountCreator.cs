using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AccountCreator
{
    public partial class FormAccountCreator : Form
    {
        public FormAccountCreator()
        {
            InitializeComponent();
        }

        private void FormAccountCreator_Load(object sender, EventArgs e)
        {
            int y = 50;
            webBrowser1.Location = new Point(0, y);
            webBrowser1.Size = new Size(this.Size.Width, this.Size.Height - y);
        }

        private void buttonCreateAccount_Click(object sender, EventArgs e)
        {
            if(textBoxPhone.Text.Trim() == "")
            {
                MessageBox.Show("请填写正确的手机号");
                return;
            }


            textBoxPhone.Text = "";
        }
    }
}
