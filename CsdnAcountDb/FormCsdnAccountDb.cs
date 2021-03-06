﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace CsdnAcountDb
{
    public partial class FormCsdnAccountDb : Form
    {
        EmailRobot m_emailRobot;

        public FormCsdnAccountDb()
        {
            InitializeComponent();
        }

        private void FormCsdnAccountDb_Load(object sender, EventArgs e)
        {
            Tools.SetWebBrowserFeatures(11);
            this.Text = this.Text + "_IE" + Tools.GetBrowserVersion().ToString();

            m_emailRobot = new EmailRobot(webBrowser1, timer_CheckEmail);
        }

        private void buttonCreateDB_Click(object sender, EventArgs e)
        {
            DbCsdnAccountDb accountDb = new DbCsdnAccountDb("CsdnAccount.db");

            // Open the file to read from.
            using (StreamReader sr = File.OpenText("www.csdn.net.sql"))
            {
                string s;
                int indexFlag1, indexFlag2, indexAt;
                DbCsdnAccountDb.AccountInfo info = new DbCsdnAccountDb.AccountInfo();
                while ((s = sr.ReadLine()) != null)
                {
                    indexFlag1 = s.IndexOf("#");
                    indexFlag2 = s.LastIndexOf("#");

                    info.csdnUsername = s.Substring(0, indexFlag1 - 1);
                    info.csdnPassword = s.Substring(indexFlag1 + 2, indexFlag2 - indexFlag1 - 3);
                    info.email = s.Substring(indexFlag2 + 2, s.Length - indexFlag2 - 2);

                    indexAt = info.email.IndexOf("@");
                    info.emailServer = info.email.Substring(indexAt + 1, info.email.Length - indexAt - 1);

                    accountDb.AddAccountInfo(info);
                }
            }
        }

        private void buttonCreateServerTable_Click(object sender, EventArgs e)
        {
            buttonCreateServerTable.Enabled = false;

            DbCsdnAccountDb accountDb = new DbCsdnAccountDb("CsdnAccount.db");
            accountDb.GetServerName();

            MessageBox.Show("serverTable create done");
        }

        private void buttonCheckEmail_Click(object sender, EventArgs e)
        {

        }

        private void timer_CheckEmail_Tick(object sender, EventArgs e)
        {
            m_emailRobot.timerBrain();
        }
    }
}
