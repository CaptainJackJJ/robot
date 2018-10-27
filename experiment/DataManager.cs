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
        public struct WorkingObjectInfo
        {
            public string userName;
            public string password;
            public string lastListPageUrl;
            public string lastFinishedArticleUrlInList;
            public short needFinishNum;
        }

        private bool m_bSwitch = false;
        private string connStr = @"Provider= Microsoft.ACE.OLEDB.12.0;Data Source = workingObject.accdb";
        private const short m_MaxFinishedNum = 10;

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
            string today = DateTime.Today.ToString(new CultureInfo("zh-CHS"));
            string sql = "SELECT TOP 1 * FROM objectInfo WHERE isReadyForWork = YES AND"
                + " (lastWorkingDay < #" + today + "# OR lastWorkingDay IS NULL OR"
                + " (lastWorkingDay = #" + today + "# AND needFinishNum < " + m_MaxFinishedNum.ToString() + "))";

            OleDbDataReader data = ExecuteReader(sql);

            WorkingObjectInfo info = new WorkingObjectInfo();
            return info;

            //if(m_bSwitch)
            //{
            //    m_bSwitch = false;

            //    WorkingObjectInfo info;
            //    info.userName = "werfhksdhf";
            //    info.password = "Ct@z7h3LFt4q";
            //    info.lastListPageUrl = "https://blog.csdn.net/laoyang360?orderby=ViewCount";
            //    // The last finished article url in the list page. 
            //    // if it is empty means either this is a new working object or new list page.
            //    info.lastFinishedArticleUrlInList = "https://blog.csdn.net/laoyang360/article/details/51824617";
            //    info.needFinishNum = 2;
            //    return info;
            //}
            //else
            //{
            //    m_bSwitch = true;

            //    WorkingObjectInfo info;
            //    info.userName = "sdhiiwfssf";
            //    info.password = "Cq&86tjUKHEG";
            //    info.lastListPageUrl = "https://blog.csdn.net/jxw167?orderby=ViewCount";
            //    // The last finished article url in the list page. 
            //    // if it is empty means either this is a new working object or new list page.
            //    info.lastFinishedArticleUrlInList = "";
            //    info.needFinishNum = 2;
            //    return info;
            //}            
        }

        public void SetWorkingObjectInfo(WorkingObjectInfo info)
        {
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
