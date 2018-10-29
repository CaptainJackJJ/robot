using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.OleDb;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

namespace assigner
{
    class DataManager
    {
        public class ObjectInfo
        {
            public long id;
            public string url;
            public string lastListPageUrl;
            public string assignedAccount;
        }

        public class WorkingObjectInfo
        {
            public long id;
            public string url;
            public string userName;
            public string password;
            public string lastListPageUrl;
            public string lastFinishedArticleUrlInList;
            public short needFinishNum;
            public string lastWorkingDay;
            public bool isObjectFinished;
            public bool isReadyForWork;
        }

        private string connStr = @"Provider= Microsoft.ACE.OLEDB.12.0;Data Source = ";

        public DataManager(string dbName)
        {
            connStr += dbName;
        }

        // 执行增加、删除、修改指令
        public int ExecuteNonQuery(string sql/*, params OleDbParameter[] param*/)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connStr))
                {
                    using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                    {
                        //if (param != null)
                        //{
                        //    cmd.Parameters.AddRange(param);
                        //}

                        conn.Open();

                        return (cmd.ExecuteNonQuery());
                    }
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return -1;
        }
 

        // 执行查询指令，获取返回的datareader
        public OleDbDataReader ExecuteReader(string sql/*, params OleDbParameter[] param*/)
        {
            try
            {
                OleDbConnection conn = new OleDbConnection(connStr);
                OleDbCommand cmd = conn.CreateCommand();

                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;

                //if (param != null)
                //{
                //    cmd.Parameters.AddRange(param);
                //}

                conn.Open();

                return (cmd.ExecuteReader(CommandBehavior.CloseConnection));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return null;
        }

        public ObjectInfo GetUnAssignedObjectInfo()
        {
            string today = DateTime.Today.ToString(new CultureInfo("zh-CHS")).Substring(0, 10);
            string sql = "SELECT TOP 1 * FROM [object] WHERE assignedAccount IS NULL";

            OleDbDataReader data = ExecuteReader(sql);

            ObjectInfo info = new ObjectInfo();
            data.Read();
            if (!data.HasRows)
                return null;

            info.id = data.GetInt32(0);
            info.url = data.GetString(1);
            info.lastListPageUrl = data.GetString(2);
            info.assignedAccount = data.GetValue(3).ToString();

            data.Close();

            return info;
        }
    }
}
