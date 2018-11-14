using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;

namespace WorkObjCollector
{
    class ObjDb
    {
        public class ObjectInfo
        {
            public long id;
            public string url;
            public string lastListPageUrl;
            public string assignedAccount;
        }

        private string connStr = @"data source=";

        public ObjDb(string dbName)
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
                Log.WriteLog(LogType.Error, e.Message);
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
                Log.WriteLog(LogType.Error, e.Message);
            }
            return null;
        }

        public string GetLastCheckedObject()
        {
            string sql = "SELECT * FROM obj WHERE ORDER BY rowid DESC LIMIT 1";

            SQLiteDataReader data = ExecuteReader(sql);

            data.Read();
            if (!data.HasRows)
                return null;

            string url = data["url"].ToString();

            data.Close();
            data.Dispose();

            return url;
        }
        public bool AddCheckedObject(string url)
        {
            string sql = "INSERT INTO obj ( url)"
            + " VALUES ('" + url + "')";

            if (ExecuteNonQuery(sql) <= 0)
            {
                Log.WriteLog(LogType.Error, "AddCheckedObject is failed");
                return false;
            }

            return true;
        }

        public bool IsObjectExisted(string url)
        {
            string sql = "SELECT * FROM object WHERE objectUrl = '" + url + "'";

            SQLiteDataReader data = ExecuteReader(sql);

            ObjectInfo info = new ObjectInfo();
            data.Read();
            if (!data.HasRows)
                return false;

            data.Close();
            data.Dispose();

            return true;
        }

        public bool AddObject(ObjectInfo info)
        {
            string sql = "INSERT INTO object ( objectUrl, lastListPageUrl )"
            + " VALUES ('" + info.url + "','" + info.lastListPageUrl + "')";

            if (ExecuteNonQuery(sql) <= 0)
            {
                Log.WriteLog(LogType.Error, "AddObject is failed");
                return false;
            }

            return true;
        }
    }
}
