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
        }


        private void FormAccountCreator_Load(object sender, EventArgs e)
        {
            Tools.SetWebBrowserFeatures(11);
            this.Text = this.Text + "_IE" + Tools.GetBrowserVersion().ToString();

            bool limited = false;
            int createNum = AccountCreatorRobot.GetCreateNum(ref limited);
            label_createNum.Text = createNum.ToString();
            if(limited)
            {
                MessageBox.Show("已经创建5个了，要等3个小时后才能创建新的。要看清楚，这样会扣工资的");
                Environment.Exit(0);
            }
            if (createNum > 10)
            {
                MessageBox.Show("已经创建10个了。要看清楚，这样会扣工资的");
                Environment.Exit(0);
            }

            timer1.Enabled = false;
            m_Robot = new AccountCreatorRobot(webBrowser1, timer1);
            timer1.Enabled = true;

            int y = 50;
            webBrowser1.Location = new Point(0, y);
            webBrowser1.Size = new Size(this.Size.Width, this.Size.Height - y);
        }

        private void CreateAccount()
        {
            m_Robot.SetTaskType(AccountCreatorRobot.EnumTaskType.Create);
            timer1.Enabled = true;
        }

        private void buttonCreateAccount_Click(object sender, EventArgs e)
        {
            CreateAccount();
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

        private void buttonBeFan_Click(object sender, EventArgs e)
        {
            m_Robot.SetTaskType(AccountCreatorRobot.EnumTaskType.BeFan);
            timer1.Enabled = true;
        }

        private void button_unbind_Click(object sender, EventArgs e)
        {
            m_Robot.SetTaskType(AccountCreatorRobot.EnumTaskType.UnBind);
        }
    }
}
