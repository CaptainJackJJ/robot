using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;
using System.Globalization;
using System.Windows.Forms;

namespace CsdnAcountDb
{
    class DbCsdnAccountDb
    {
        public class AccountInfo
        {
            public long id;
            public string csdnUsername;
            public string csdnPassword;
            public string email;
            public string emailPassword;
            public string emailServer;
        }

        private string connStr = @"data source=";

        public DbCsdnAccountDb(string dbName)
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
                Log.WriteLog(LogType.Error, "sql: " + sql + ", e msg" + e.Message);
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


        public bool AddAccountInfo(AccountInfo info)
        {
            string sql = "INSERT INTO account ( csdnUsername, csdnPassword, email, emailServer )"
            + " VALUES ('" + info.csdnUsername + "','" + info.csdnPassword + "','" + info.email + "','" + info.emailServer + "')";

            if (ExecuteNonQuery(sql) <= 0)
            {
                Log.WriteLog(LogType.Error, "sql: " + sql );
                return false;
            }

            return true;
        }
    }
}
