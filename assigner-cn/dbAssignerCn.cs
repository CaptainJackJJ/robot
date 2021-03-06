﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;
using System.Globalization;
using System.Windows.Forms;

namespace assigner
{
    class dbAssignerCn
    {
        public class ObjectInfo
        {
            public long id;
            public string url;
            public string lastListPageUrl;
        }

        public class AccountInfo
        {
            public long id;
            public string userName;
            public string password;
            public Int16 assignedNum;
            public string workStation;
            public string phone;
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

        public dbAssignerCn(string dbName)
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
            string sql = "SELECT * FROM [object] WHERE isAssigned = 0 ORDER BY ID ASC LIMIT 1";

            SQLiteDataReader data = ExecuteReader(sql);

            ObjectInfo info = new ObjectInfo();
            data.Read();
            if (!data.HasRows)
                return null;

            info.id = data.GetInt32(0);
            info.url = data.GetString(1);
            info.lastListPageUrl = data.GetString(2);

            data.Close();
            data.Dispose();

            return info;
        }

        public AccountInfo GetUnAssignedAccountInfo()
        {
            string sql = "SELECT [rowid],* FROM [account] WHERE isReady = 1 AND isAssigned = 0 ORDER BY [rowid] ASC LIMIT 1";

            SQLiteDataReader data = ExecuteReader(sql);

            AccountInfo info = new AccountInfo();
            data.Read();
            if (!data.HasRows)
                return null;

            info.id = data.GetInt32(0);
            info.userName = data.GetString(1);
            info.password = data.GetString(2);

            data.Close();
            data.Dispose();

            return info;
        }

        public bool AddWorkingObjectInfo(WorkingObjectInfo info)
        {
            string sql = "INSERT INTO objectInfo ( objectUrl, userName, [password], lastListPageUrl )"
            + " VALUES ('" + info.url + "','" + info.userName + "','" + info.password + "','" + info.lastListPageUrl + "')";

            if (ExecuteNonQuery(sql) <= 0)
            {
                MessageBox.Show("add work obj is failed");
                return false;
            }

            return true;
        }

        public bool UpdateAccountInfo(AccountInfo info)
        {
            string sql = "UPDATE account SET isAssigned = 1 WHERE [rowid] = " + info.id;

            if (ExecuteNonQuery(sql) <= 0)
            {
                MessageBox.Show("UpdateAccountInfo is failed");
                return false;
            }

            return true;
        }

        public bool UpdateObjInfo(ObjectInfo info)
        {
            string sql = "UPDATE [object] SET isAssigned = 1 WHERE ID = " + info.id;

            if (ExecuteNonQuery(sql) <= 0)
            {
                MessageBox.Show("UpdateObjInfo is failed");
                return false;
            }

            return true;
        }
    }
}
