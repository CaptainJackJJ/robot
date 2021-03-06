﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkObjCollector
{
    public class Log
    {
        private static string logPath = string.Empty;
        /// <summary>
        /// 保存日志的文件夹
        /// </summary>
        public static string LogPath
        {
            get
            {
                if (logPath == string.Empty)
                {
                    logPath = AppDomain.CurrentDomain.BaseDirectory;
                    //if (System.Web.HttpContext.Current == null)
                    //    // Windows Forms 应用
                    //    logPath = AppDomain.CurrentDomain.BaseDirectory;
                    //else
                    //    // Web 应用
                    //    logPath = AppDomain.CurrentDomain.BaseDirectory + @"bin\";
                }
                return logPath;
            }
            set { logPath = value; }
        }

        private static string logFielPrefix = string.Empty;
        /// <summary>
        /// 日志文件前缀
        /// </summary>
        public static string LogFielPrefix
        {
            get { return logFielPrefix; }
            set { logFielPrefix = value; }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        public static void WriteLog(string logType, string msg)
        {
            try
            {
                System.IO.StreamWriter sw = System.IO.File.AppendText(
                    LogPath + LogFielPrefix + logType + " " +
                    DateTime.Now.ToString("yyyyMMdd") + ".Log"
                    );
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss: ") + msg);
                sw.Close();
                sw.Dispose();
            }
            catch
            { }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        public static void WriteLog(LogType logType, string msg)
        {
            bool isDebug = false;
#if DEBUG
            isDebug = true;
#endif

            if (logType == LogType.Debug && !isDebug)
                return;
            WriteLog(logType.ToString(), msg);
        }
    }

    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogType
    {
        Debug,
        Trace,
        Exception,
        NetworkWarning,
        Warning,
        Notice,
        Error,
        SQL
    }
}
