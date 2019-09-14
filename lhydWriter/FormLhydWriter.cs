using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkObjCollector
{
    public partial class FormLhydWriter : Form
    {
        lhydWriter m_Robot;

        public FormLhydWriter()
        {
            InitializeComponent();
        }

        private void FormCollector_Load(object sender, EventArgs e)
        {
            Tools.SetWebBrowserFeatures(11);
            this.Text = this.Text + "_IE" + Tools.GetBrowserVersion().ToString();

            m_Robot = new lhydWriter(webBrowser1, timer1);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            m_Robot.timerBrain();
        }
    }
}
