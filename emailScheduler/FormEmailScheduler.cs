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
    public partial class FormEmailScheduler : Form
    {
        BlogRobot m_blogRobot = null;

        public FormEmailScheduler()
        {
            InitializeComponent();

            Tools.SetWebBrowserFeatures(11);
            this.Text = this.Text + "_IE" + Tools.GetBrowserVersion().ToString();

            
            m_blogRobot = new BlogRobot(webBrowser1, timerRobotBrain);

            webBrowser1.Navigate("https://captainbed.vip/wp-json/my/discount_email");

            timerRobotBrain.Enabled = true;
            timerRobotBrain.Interval = 1000 * 60 * 10;
        }

        private void navigateToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void submitToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void accountToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void timerRobotBrain_Tick(object sender, EventArgs e)
        {
            //if (DateTime.Now.Hour < 8 || DateTime.Now.Hour > 22)
            //    return;
            webBrowser1.Navigate("https://captainbed.vip/wp-json/my/discount_email");
        }

        private void resetNeedFinishNumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_blogRobot.ResetNeedFinishNum();
        }
    }
}
