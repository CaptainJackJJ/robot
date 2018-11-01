using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;
using System.Globalization;
using System.Windows.Forms;

namespace assigner
{
    class DataManagerSqlLiteAssigner
    {
        public class ObjectInfo
        {
            public long id;
            public string url;
            public string lastListPageUrl;
            public string assignedAccount;
        }

        public class AccountInfo
        {
            public long id;
            public string userName;
            public string password;
            public Int16 assignedNum;
            public string workStation;
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

        private string connStr = @"data source=";

        public DataManagerSqlLiteAssigner(string dbName)
        {
            connStr += dbName;
        }

        // 执行增加、删除、修改指令
        public int ExecuteNonQuery(string sql/*, params OleDbParameter[] param*/)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;

                        int res = cmd.ExecuteNonQuery();
                        conn.Close();
                        conn.Dispose();

                        return res;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return -1;
        }


        // 执行查询指令，获取返回的datareader
        public SQLiteDataReader ExecuteReader(string sql/*, params OleDbParameter[] param*/)
        {
            try
            {
                SQLiteConnection conn = new SQLiteConnection(connStr);
                conn.Open();

                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;

                SQLiteDataReader reader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                return reader;
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
            string sql = "SELECT * FROM [object] WHERE assignedAccount IS NULL ORDER BY ID ASC LIMIT 1";

            SQLiteDataReader data = ExecuteReader(sql);

            ObjectInfo info = new ObjectInfo();
            data.Read();
            if (!data.HasRows)
                return null;

            info.id = data.GetInt32(0);
            info.url = data.GetString(1);
            info.lastListPageUrl = data.GetString(2);
            info.assignedAccount = data.GetValue(3).ToString();

            data.Close();
            data.Dispose();

            return info;
        }

        public AccountInfo GetUnAssignedAccountInfo()
        {
            string sql = "SELECT * FROM [account] WHERE assignedNum = 0 ORDER BY ID ASC LIMIT 1";

            SQLiteDataReader data = ExecuteReader(sql);

            AccountInfo info = new AccountInfo();
            data.Read();
            if (!data.HasRows)
                return null;

            info.id = data.GetInt32(0);
            info.userName = data.GetString(1);
            info.password = data.GetString(2);
            info.assignedNum = data.GetInt16(3);
            info.workStation = data.GetValue(4).ToString();

            data.Close();
            data.Dispose();

            return info;
        }

        public bool AddWorkingObjectInfo(WorkingObjectInfo info)
        {
            string sql = "INSERT INTO objectInfo ( objectUrl, userName, [password], lastListPageUrl, isReadyForWork )"
            + " VALUES ('" + info.url + "','" + info.userName + "','" + info.password + "','" + info.lastListPageUrl + "',TRUE)";

            if (ExecuteNonQuery(sql) <= 0)
            {
                MessageBox.Show("add work obj is failed");
                return false;
            }

            return true;
        }

        public bool UpdateAccountInfo(AccountInfo info)
        {
            string sql = "UPDATE account SET"
+ " assignedNum = " + info.assignedNum + ","
+ " workStation = '" + info.workStation + "'"
+ " WHERE ID = " + info.id;

            if (ExecuteNonQuery(sql) <= 0)
            {
                MessageBox.Show("UpdateAccountInfo is failed");
                return false;
            }

            return true;
        }

        public bool UpdateObjInfo(ObjectInfo info)
        {
            string sql = "UPDATE [object] SET"
+ " assignedAccount = '" + info.assignedAccount + "'"
+ " WHERE ID = " + info.id;

            if (ExecuteNonQuery(sql) <= 0)
            {
                MessageBox.Show("UpdateObjInfo is failed");
                return false;
            }

            return true;
        }
    }
}
