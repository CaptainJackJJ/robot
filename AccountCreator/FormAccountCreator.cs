using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace AccountCreator
{
    public partial class FormAccountCreator : Form
    {
        AccountCreatorRobot m_Robot = null;

        public FormAccountCreator()
        {
            CleanTempFiles();

            InitializeComponent();

            Tools.SetWebBrowserFeatures(11);
            this.Text = this.Text + "_IE" + Tools.GetBrowserVersion().ToString();

            timer1.Enabled = false;
            m_Robot = new AccountCreatorRobot(webBrowser1, timer1);

            CreateAccount();
        }

        /// <summary>
        /// 清除文件夹
        /// </summary>
        /// <param name="path">文件夹路径</param>
        static void FolderClear(string path)
        {
            try
            {
                System.IO.DirectoryInfo diPath = new System.IO.DirectoryInfo(path);
                foreach (System.IO.FileInfo fiCurrFile in diPath.GetFiles())
                {
                    fiCurrFile.Delete();
                    //FileDelete(fiCurrFile.FullName);

                }
                foreach (System.IO.DirectoryInfo diSubFolder in diPath.GetDirectories())
                {
                    FolderClear(diSubFolder.FullName); // Call recursively for all subfolders
                }
            }
            catch
            { }
        }
        /// <summary>
        /// 执行命令行
        /// </summary>
        /// <param name="cmd"></param>
        static void RunCmd(string cmd)
        {
            ProcessStartInfo p = new ProcessStartInfo();
            p.FileName = "cmd.exe";
            p.Arguments = "/c " + cmd;
            p.WindowStyle = ProcessWindowStyle.Hidden;  // Use a hidden window
            Process pro = Process.Start(p);
            pro.WaitForExit();
        }
        /// <summary>
        /// 删除临时文件
        /// </summary>
        public static void CleanTempFiles()
        {
            FolderClear(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache));

            FolderClear(Environment.GetFolderPath(Environment.SpecialFolder.Cookies));

            RunCmd("RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 22783");
        }

        private void FormAccountCreator_Load(object sender, EventArgs e)
        {
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
    }
}
