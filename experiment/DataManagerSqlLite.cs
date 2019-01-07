using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;
using System.Globalization;

namespace experiment
{
    class DataManagerSqlLite
 {
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

        public class ObjectInfo
        {
            public long id;
            public string url;
            public string lastListPageUrl;
            public string assignedAccount;
        }

        private string connStr = @"data source=";

#if DEBUG
                    private const short m_MaxFinishedNum = 3;
#else
        private const short m_MaxFinishedNum = 3;
#endif

        public DataManagerSqlLite(string dbName)
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
            catch(Exception e)
            {
                Log.WriteLog(LogType.SQL, e.Message);
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
                Log.WriteLog(LogType.SQL, e.Message);
            }
            return null;
        }

        public void GetParams(ref int articleTypeOffset, ref int articleFieldOffset)
        {
            string sql = "SELECT * FROM params LIMIT 1";

            SQLiteDataReader data = ExecuteReader(sql);

            data.Read();
            articleTypeOffset = Convert.ToInt32(data.GetValue(1));
            articleFieldOffset = Convert.ToInt32(data.GetValue(2));            
            data.Close();
            data.Dispose();
        }

        public WorkingObjectInfo GetWorkingObjectInfo()
        {
            string today = DateTime.Today.ToString(new CultureInfo("ko")).Substring(0,10) + " 00:00:00.000";
            string sql = "SELECT * FROM objectInfo WHERE"
                + " (lastWorkingDay < '" + today + "' OR lastWorkingDay IS NULL OR"
                + " (lastWorkingDay = '" + today + "' AND needFinishNum > 0 AND isObjectFinished = 0)) LIMIT 1";

            SQLiteDataReader data = ExecuteReader(sql);

            WorkingObjectInfo info = new WorkingObjectInfo();
            data.Read();
            if (!data.HasRows)
                return null;

            info.id = data.GetInt32(0);
            info.url = data.GetString(1);
            info.userName = data.GetString(2);
            info.password = data.GetString(3);
            info.lastListPageUrl = data.GetString(4);
            info.lastFinishedArticleUrlInList = data.GetValue(5).ToString();
            info.needFinishNum = data.GetInt16(6);
            info.lastWorkingDay = data.GetValue(7).ToString();
            if (info.lastWorkingDay != "")
                info.lastWorkingDay = Convert.ToDateTime(info.lastWorkingDay).ToShortDateString();
            info.isObjectFinished = data.GetBoolean(8);
            info.isReadyForWork = data.GetBoolean(9);

            today = Convert.ToDateTime(today).ToShortDateString();
            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.LongDatePattern = "yyyy-MM-dd";
            if (info.lastWorkingDay == ""
                || Convert.ToDateTime(info.lastWorkingDay, dtFormat) < Convert.ToDateTime(today, dtFormat))
            {
                info.needFinishNum = m_MaxFinishedNum; // This is new day.
            }

            //if (info.needFinishNum > 1)
            //    info.needFinishNum = 1;

            data.Close();
            data.Dispose();

            return info;       
        }

        public WorkingObjectInfo GetFirstWorkingObject()
        {
            string sql = "SELECT * FROM objectInfo ORDER BY ID ASC LIMIT 1";

            SQLiteDataReader data = ExecuteReader(sql);

            WorkingObjectInfo info = new WorkingObjectInfo();
            data.Read();
            if (!data.HasRows)
                return null;

            info.id = data.GetInt32(0);
            info.url = data.GetString(1);
            info.userName = data.GetString(2);
            info.password = data.GetString(3);
            info.lastListPageUrl = data.GetString(4);
            info.lastFinishedArticleUrlInList = data.GetValue(5).ToString();
            info.needFinishNum = data.GetInt16(6);
            info.lastWorkingDay = data.GetValue(7).ToString();
            if (info.lastWorkingDay != "")
                info.lastWorkingDay = Convert.ToDateTime(info.lastWorkingDay).ToShortDateString();
            info.isObjectFinished = data.GetBoolean(8);
            info.isReadyForWork = data.GetBoolean(9);

            data.Close();
            data.Dispose();

            return info;
        }

        public void SetObjDailyJobDone(long id)
        {
            string sql = "UPDATE objectInfo SET"
            + " isObjectFinished = 1"
            + " WHERE id = " + id;

            if (ExecuteNonQuery(sql) <= 0)
            {
                Log.WriteLog(LogType.SQL, "SetObjDailyJobDone error. sql is " + sql);
            }
        }

        public void ZeroNeedFinishNum(long id)
        {
            string sql = "UPDATE objectInfo SET"
            + " needFinishNum = 0"
            + " WHERE id = " + id;

            if (ExecuteNonQuery(sql) <= 0)
            {
                Log.WriteLog(LogType.SQL, "ZeroNeedFinishNum error. sql is " + sql);
            }
        }

        public void ResetNeedFinishNum()
        {
            string sql = "UPDATE objectInfo SET needFinishNum = 1";

            if (ExecuteNonQuery(sql) <= 0)
            {
                Log.WriteLog(LogType.SQL, "ResetNeedFinishNum error. sql is " + sql);
            }
        }

        public bool SetWorkingObjectInfo(WorkingObjectInfo info)
        {
            string today = DateTime.Today.ToString(new CultureInfo("ko")).Substring(0, 10) + " 00:00:00.000";
            if (String.IsNullOrEmpty(info.lastWorkingDay)
                || info.lastWorkingDay != Convert.ToDateTime(today).ToShortDateString())
                info.isObjectFinished = false; // new day. so reset daily finish flag

            string sql = "UPDATE objectInfo SET"
            + " objectUrl = '" + info.url + "',"
            + " lastListPageUrl = '" + info.lastListPageUrl + "',"
            + " lastFinishedArticleUrlInList = '" + info.lastFinishedArticleUrlInList + "',"
            + " needFinishNum = " + info.needFinishNum + ","
            + " lastWorkingDay = '" + today + "',"
            + " isObjectFinished = " + info.isObjectFinished + ","
            + " isReadyForWork = " + info.isReadyForWork
            + " WHERE id = " + info.id;

            if(ExecuteNonQuery(sql) <= 0)
            {
                Log.WriteLog(LogType.SQL, "SetWorkingObjectInfo error. sql is " + sql);
                return false;
            }
            return true;
        }

        public ObjectInfo GetBackupObj()
        {
            string sql = "SELECT * FROM object LIMIT 1";

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

        public void DeleteBackupObj(long id)
        {
            string sql = "DELETE FROM object WHERE id = " + id;

            if (ExecuteNonQuery(sql) <= 0)
            {
                Log.WriteLog(LogType.SQL, "DeleteObj error. sql is " + sql);
            }
        }
    }
}
