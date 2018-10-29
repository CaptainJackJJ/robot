using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace assigner
{
    public partial class FormAssigner : Form
    {
        public FormAssigner()
        {
            InitializeComponent();            
        }

        private void FormAssigner_Load(object sender, EventArgs e)
        {
            DataManager resourceDB = new DataManager("Resource.accdb");
            DataManager.ObjectInfo objInfo = resourceDB.GetUnAssignedObjectInfo();

        }
    }
}
