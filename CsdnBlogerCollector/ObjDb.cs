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
            string sql = "SELECT * FROM bloger ORDER BY rowid DESC LIMIT 1";

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
            string sql = "INSERT INTO bloger ( url )"
            + " VALUES ('" + url + "')";

            if (ExecuteNonQuery(sql) <= 0)
            {
                Log.WriteLog(LogType.Error, "AddCheckedObject is failed");
                return false;
            }

            return true;
        }

        public bool IsObjectChecked(string url)
        {
            string sql = "SELECT * FROM bloger WHERE url = '" + url + "'";

            SQLiteDataReader data = ExecuteReader(sql);

            data.Read();
            if (!data.HasRows)
                return false;

            data.Close();
            data.Dispose();

            return true;
        }

        public bool IsObjectCollected(string urlBloger)
        {
            string sql = "SELECT * FROM bloger WHERE bloger_url = '" + urlBloger + "'";

            SQLiteDataReader data = ExecuteReader(sql);

            data.Read();
            if (!data.HasRows)
                return false;

            data.Close();
            data.Dispose();

            return true;
        }

        public bool CollectObject(string urlBloger,Int64 totalReadCount, int maxReadCount,int OriginalArticleNum,int FansNum,int LikeNum,
            int CommentsNum,int Degree,int Score,int Ranking,bool isExpert)
        {
            string sql = "INSERT INTO bloger ( bloger_url,total_read_count,max_read_count,is_expert,original_article_num,degree,fans_num"
                + ",like_num,comment_num,ranking,score )"
                + " VALUES ('" + urlBloger + "'," + totalReadCount + "," + maxReadCount + "," + isExpert + "," + OriginalArticleNum + ","
             + Degree + "," + FansNum + "," + LikeNum + "," + CommentsNum + ","+ Ranking + ","+ Score + ")";

            if (ExecuteNonQuery(sql) <= 0)
            {
                Log.WriteLog(LogType.Error, "AddObject is failed");
                return false;
            }

            return true;
        }
    }
}
