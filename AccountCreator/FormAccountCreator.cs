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

            timer1.Enabled = false;
            m_Robot = new AccountCreatorRobot(webBrowser1, timer1);
        }

        private void FormAccountCreator_Load(object sender, EventArgs e)
        {
            int y = 50;
            webBrowser1.Location = new Point(0, y);
            webBrowser1.Size = new Size(this.Size.Width, this.Size.Height - y);

#if DEBUG
            MessageBox.Show("do not use debug");
            Environment.Exit(0);
#endif
        }

        private void buttonCreateAccount_Click(object sender, EventArgs e)
        {
            m_Robot.SetTaskType(AccountCreatorRobot.EnumTaskType.Create);
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            m_Robot.timerBrain();
        }

        private void buttonSetAccount_Click(object sender, EventArgs e)
        {
            m_Robot.SetTaskType(AccountCreatorRobot.EnumTaskType.Set);
            timer1.Enabled = true;

            buttonSetAccount.Enabled = false;
            buttonSetDone.Enabled = true;
        }

        private void buttonSetDone_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
                return;

            m_Robot.SetDoneUnsetAccount();

            buttonSetAccount.Enabled = true;
            buttonSetDone.Enabled = false;
        }

        private void buttonUnbind_Click(object sender, EventArgs e)
        {
            m_Robot.SetTaskType(AccountCreatorRobot.EnumTaskType.Unbind);
            timer1.Enabled = true;
        }
    }
}
