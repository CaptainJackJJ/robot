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
        AccountCreatorRobot m_Robot = null;

        public FormAccountCreator()
        {
            InitializeComponent();

            Tools.SetWebBrowserFeatures(11);
            this.Text = this.Text + "_IE" + Tools.GetBrowserVersion().ToString();

            timer1.Enabled = true;
            m_Robot = new AccountCreatorRobot(webBrowser1, timer1);
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            m_Robot.timerBrain();
        }
    }
}
