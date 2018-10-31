using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                System.Diagnostics.Process.Start("experiment.exe");
            }
        }
    }
}
