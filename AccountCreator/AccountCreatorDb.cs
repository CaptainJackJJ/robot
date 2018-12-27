using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;
using System.Globalization;
using System.Windows.Forms;

namespace AccountCreator
{
    class AccountCreatorDb
    {
        public class AccountInfo
        {
            public long id;
            public string userName;
            public string password;
            public Int16 assignedNum;
            public string workStation;
            public string phone;
            public Int32 fanToNum;
            public bool isFaning;
            public string fanToAccount;
            public string fanToListPage;
            public string fanToArticle;
        }
        
        private string connStr = @"data source=";
        private readonly string m_myAccount = @"jiangjunshow";

        public AccountCreatorDb(string dbName)
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

        public AccountInfo GetFan()
        {
            string sql = "SELECT * FROM [account] ORDER BY fanToNum ASC LIMIT 1";

            SQLiteDataReader data = ExecuteReader(sql);           
            data.Read();
            if (!data.HasRows)
            {
                return null;
            }

            AccountInfo info = new AccountInfo();
            info.id = data.GetInt32(0);
            info.userName = data.GetString(1);
            info.password = data.GetString(2);
            info.assignedNum = data.GetInt16(3);
            info.workStation = data.GetValue(4).ToString();
            info.phone = data["phone"].ToString();
            info.fanToNum = Convert.ToInt32(data["fanToNum"]);
            info.isFaning = Convert.ToBoolean(data["isFaning"]);
            info.fanToAccount = Convert.ToString(data["fanToAccount"]);
            info.fanToListPage = Convert.ToString(data["fanToListPage"]);
            info.fanToArticle = Convert.ToString(data["fanToArticle"]);

            if (info.fanToAccount == "")
            {
                info.fanToAccount = m_myAccount;
            }
            if (info.fanToListPage == "")
            {
                info.fanToListPage = "https://blog.csdn.net/" + m_myAccount + "?orderby=ViewCount";
            }

            data.Close();
            data.Dispose();

            return info;
        }

        public bool SetFan(AccountInfo info)
        {
            string sql = "UPDATE account SET"
+ " fanToNum = " + info.fanToNum + ","
+ " isFaning = " + info.isFaning + ","
+ " fanToListPage = '" + info.fanToListPage + "',"
+ " fanToArticle = '" + info.fanToArticle + "'"
+ " WHERE ID = " + info.id;

            if (ExecuteNonQuery(sql) <= 0)
            {
                MessageBox.Show("SetFan is failed");
                return false;
            }

            return true;
        }

        public AccountInfo GetUnsetAccount()
        {
            string sql = "SELECT * FROM [account] WHERE assignedNum < 1";

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
            info.phone = data["phone"].ToString();

            data.Close();
            data.Dispose();

            return info;
        }

        public bool SetUnsetAccount(AccountInfo info)
        {
            info.assignedNum++;

            string sql = "UPDATE account SET"
+ " assignedNum = " + info.assignedNum
+ " WHERE ID = " + info.id;

            if (ExecuteNonQuery(sql) <= 0)
            {
                MessageBox.Show("SetUnsetAccount is failed");
                return false;
            }

            return true;
        }

        public AccountInfo GetAccountInfoByName(string username)
        {
            string sql = "SELECT * FROM [account] WHERE username = '" + username + "'";

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
            info.phone = data["phone"].ToString();

            data.Close();
            data.Dispose();

            return info;
        }

        public bool AddAccountInfo(AccountInfo info)
        {
            string sql = "INSERT INTO account ( username, phone )"
            + " VALUES ('" + info.userName + "','" + info.phone + "')";

            if (ExecuteNonQuery(sql) <= 0)
            {
                Log.WriteLog(LogType.Error, "sql: " + sql);
                return false;
            }

            return true;
        }
    }
}
