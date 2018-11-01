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
            DataManagerSqlLiteAssigner accountDB = new DataManagerSqlLiteAssigner("Account.db");
            DataManagerSqlLiteAssigner objectDB = new DataManagerSqlLiteAssigner("Object.db");
            DataManagerSqlLiteAssigner workingObjDB = new DataManagerSqlLiteAssigner("workingObject-temp.db");

            while (true)
            {
                DataManagerSqlLiteAssigner.ObjectInfo objInfo = objectDB.GetUnAssignedObjectInfo();
                if (objInfo == null)
                {
                    MessageBox.Show("all obj is assigned");
                    return;
                }

                // TODO: only get same workStation as target workStation or workStation is empty
                DataManagerSqlLiteAssigner.AccountInfo accountInfo = accountDB.GetUnAssignedAccountInfo();
                if (accountInfo == null)
                {
                    MessageBox.Show("all account is assigned");
                    return;
                }

                DataManagerSqlLiteAssigner.WorkingObjectInfo workingObjInfo = new DataManagerSqlLiteAssigner.WorkingObjectInfo();
                workingObjInfo.url = objInfo.url;
                workingObjInfo.userName = accountInfo.userName;
                workingObjInfo.password = accountInfo.password;
                workingObjInfo.lastListPageUrl = objInfo.lastListPageUrl;
                workingObjInfo.isReadyForWork = true; // TODO: set true if this account has no workingObj, else false.

                if (workingObjDB.AddWorkingObjectInfo(workingObjInfo))
                {
                    accountInfo.assignedNum++;
                    accountInfo.workStation = "1"; // TODO: set to the same station as target station
                    if (accountDB.UpdateAccountInfo(accountInfo))
                    {
                        objInfo.assignedAccount = accountInfo.userName;
                        objectDB.UpdateObjInfo(objInfo);
                    }
                }
            }
        }
    }
}
