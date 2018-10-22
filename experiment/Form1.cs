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
    public partial class Form1 : Form
    {
        MyBrowser m_browser = null;

        public Form1()
        {
            InitializeComponent();

            Tools.SetWebBrowserFeatures(11);
            this.Text = this.Text + "_IE" + Tools.GetBrowserVersion().ToString();

            m_browser = new MyBrowser(webBrowser1,timerAfterDocCompleted);                    
        }



        private void navigateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://blog.csdn.net/jiangjunshow/article/details/77711593");
        }

        private void submitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_browser.ClickLogin();
        }

        private void accountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_browser.ClickAccountLogin();
        }

        private void timerAfterDocCompleted_Tick(object sender, EventArgs e)
        {            
            m_browser.timerAfterDocCompleted();
        }
    }
}
