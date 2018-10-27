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
            public string userName;
            public string password;
            public string lastListPageUrl;
            public string lastFinishedArticleUrlInList;
            public short needFinishNum;
        }

        private bool m_bSwitch = false;
        private string connStr = @"Provider= Microsoft.ACE.OLEDB.12.0;Data Source = workingObject.accdb";
        private const short m_MaxFinishedNum = 3;

        public DataManager()
        {

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

        public WorkingObjectInfo GetWorkingObjectInfo()
        {
            string today = DateTime.Today.ToString(new CultureInfo("zh-CHS")).Substring(0,10);
            string sql = "SELECT TOP 1 * FROM objectInfo WHERE isReadyForWork = YES AND"
                + " (lastWorkingDay < #" + today + "# OR lastWorkingDay IS NULL OR"
                + " (lastWorkingDay = #" + today + "# AND needFinishNum > 0))";

            OleDbDataReader data = ExecuteReader(sql);

            WorkingObjectInfo info = new WorkingObjectInfo();
            data.Read();
            if (!data.HasRows)
                return null;
            
            info.userName = data.GetString(2);
            info.password = data.GetString(3);
            info.lastListPageUrl = data.GetString(4);
            info.lastFinishedArticleUrlInList = data.GetValue(5).ToString();
            info.needFinishNum = data.GetInt16(6);
            string lastWorkingDay = data.GetValue(7).ToString();

            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.LongDatePattern = "yyyy/MM/dd";
            if (lastWorkingDay == "" || Convert.ToDateTime(lastWorkingDay.Substring(0,10), dtFormat) < Convert.ToDateTime(today, dtFormat))
                info.needFinishNum = m_MaxFinishedNum; // This is new day.

            data.Close();


            return info;       
        }

        public void SetWorkingObjectInfo(WorkingObjectInfo info)
        {
           // ExecuteNonQuery();
            // save to database

            // if needFinishNum is 0, so we need to change the flag that indicate the daily work with this object is done.
        }

        public void UpdateLastFinishedArticleName(string lastFinishedArticleName)
        {

        }

        public void UpdateLastListPageUrl(string lastListPageUrl)
        {

        }
    }
}
