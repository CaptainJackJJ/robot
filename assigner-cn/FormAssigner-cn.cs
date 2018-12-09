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
            dbAssignerCn accountDB = new dbAssignerCn("cnAccount.db");
            dbAssignerCn objectDB = new dbAssignerCn("Object-cn.db");
            dbAssignerCn workingObjDB = new dbAssignerCn("workingObject-cn-temp.db");

            while (true)
            {
                dbAssignerCn.ObjectInfo objInfo = objectDB.GetUnAssignedObjectInfo();
                if (objInfo == null)
                {
                    MessageBox.Show("all obj is assigned");
                    return;
                }

                // TODO: only get same workStation as target workStation or workStation is empty
                dbAssignerCn.AccountInfo accountInfo = accountDB.GetUnAssignedAccountInfo();
                if (accountInfo == null)
                {
                    MessageBox.Show("all account is assigned");
                    return;
                }

                dbAssignerCn.WorkingObjectInfo workingObjInfo = new dbAssignerCn.WorkingObjectInfo();
                workingObjInfo.url = objInfo.url;
                workingObjInfo.userName = accountInfo.userName;
                workingObjInfo.password = accountInfo.password;
                workingObjInfo.lastListPageUrl = objInfo.lastListPageUrl;

                if (workingObjDB.AddWorkingObjectInfo(workingObjInfo))
                {
                    //accountInfo.assignedNum++;
                    accountInfo.workStation = "any"; // TODO: set to the same station as target station
                    if (accountDB.UpdateAccountInfo(accountInfo))
                    {
                        objectDB.UpdateObjInfo(objInfo);
                    }
                }
            }
        }
    }
}
