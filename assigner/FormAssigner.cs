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
            if (objInfo == null)
            {
                MessageBox.Show("all obj is assigned");
                return;
            }

            DataManager.accountInfo accountInfo = resourceDB.GetUnAssignedAccountInfo();
            if (accountInfo == null)
            {
                MessageBox.Show("all account is assigned");
                return;
            }

            DataManager.WorkingObjectInfo workingObjInfo = new DataManager.WorkingObjectInfo();
            workingObjInfo.url = objInfo.url;
            workingObjInfo.userName = accountInfo.userName;
            workingObjInfo.password = accountInfo.password;
            workingObjInfo.lastListPageUrl = objInfo.lastListPageUrl;
            workingObjInfo.isReadyForWork = true;

            DataManager workingObjDB = new DataManager("workingObject-temp.accdb");
            if(workingObjDB.AddWorkingObjectInfo(workingObjInfo))
            {

            }
        }
    }
}
