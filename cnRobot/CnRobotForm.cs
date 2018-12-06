using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Win32;
using System.Net.Security;

namespace experiment
{
    public partial class CnRobotForm : Form
    {
        CnBlogRobot m_blogRobot = null;

        public CnRobotForm()
        {
            InitializeComponent();

            Tools.SetWebBrowserFeatures(11);
            this.Text = this.Text + "_IE" + Tools.GetBrowserVersion().ToString();

            m_blogRobot = new CnBlogRobot(webBrowser1, timerRobotBrain);
        }

        private void navigateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://blog.csdn.net/jiangjunshow/article/details/77711593");
        }

        private void submitToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void accountToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void timerRobotBrain_Tick(object sender, EventArgs e)
        {
            m_blogRobot.timerBrain();
        }
    }
}
