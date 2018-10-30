using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.OleDb;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

namespace experiment
{
    class DataManager
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

        private string connStr = @"Provider= Microsoft.ACE.OLEDB.12.0;Data Source = ";

#if DEBUG
                    private const short m_MaxFinishedNum = 5;
#else
        private const short m_MaxFinishedNum = 10;
#endif

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
                Log.WriteLog(LogType.SQL, e.Message);
            }
            return null;
        }

        public void GetParams(ref int articleTypeOffset, ref int articleFieldOffset)
        {
            string sql = "SELECT TOP 1 * FROM params";

            OleDbDataReader data = ExecuteReader(sql);

            data.Read();
            articleTypeOffset = Convert.ToInt32(data.GetValue(1));
            articleFieldOffset = Convert.ToInt32(data.GetValue(2));            
            data.Close();
            data.Dispose();
        }

        public WorkingObjectInfo GetWorkingObjectInfo()
        {
            string today = DateTime.Today.ToString(new CultureInfo("zh-CHS")).Substring(0,10);
            string sql = "SELECT TOP 1 * FROM objectInfo WHERE isReadyForWork = YES AND isObjectFinished = NO AND"
                + " (lastWorkingDay < #" + today + "# OR lastWorkingDay IS NULL OR"
                + " (lastWorkingDay = #" + today + "# AND needFinishNum > 0))";

            OleDbDataReader data = ExecuteReader(sql);

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
            info.isObjectFinished = data.GetBoolean(8);
            info.isReadyForWork = data.GetBoolean(9);

            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.LongDatePattern = "yyyy/MM/dd";
            if (info.lastWorkingDay == "" || Convert.ToDateTime(info.lastWorkingDay.Substring(0,10), dtFormat) < Convert.ToDateTime(today, dtFormat))
                info.needFinishNum = m_MaxFinishedNum; // This is new day.

            data.Close();
            data.Dispose();

            return info;       
        }

        public void SetWorkingObjectInfo(WorkingObjectInfo info)
        {
            string today = DateTime.Today.ToString(new CultureInfo("zh-CHS")).Substring(0, 10);
            if (info.isObjectFinished)
            {
                info.isReadyForWork = false;
                Log.WriteLog(LogType.Trace, "workingObj is done. obj url is " + info.url);
            }

            string sql = "UPDATE objectInfo SET"
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
            }
        }
    }
}
