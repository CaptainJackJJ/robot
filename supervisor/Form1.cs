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


namespace supervisor
{
    public partial class FormSupervisor : Form
    {
        public FormSupervisor()
        {
            InitializeComponent();
        }

        private void timerSupervisor_Tick(object sender, EventArgs e)
        {            
            if (System.Diagnostics.Process.GetProcessesByName("experiment").ToList().Count <= 0)
            {
                CleanTempFiles();

                System.Diagnostics.Process.Start("experiment.exe");
            }
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
            Process.Start(p);
        }
        /// <summary>
        /// 删除临时文件
        /// </summary>
        public static void CleanTempFiles()
        {
            FolderClear(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache));
            RunCmd("RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 8");
        }
    }
}
