using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace experiment
{
    class sinaRobot
    {
        enum EnumStep
        {
            None,
            GoToLoginPage,
            Login,
            GoToListPage,
            GoToNextArticlePage,
            GetArticleInfo,
            GoToEditPage,
            EditTitle,
            EditHtml,
            Publish,
            WaitSucess,
            Finished
        }

        public struct ArticleInfo
        {
            public string url;
            public string title;
            public UInt64 readCount;
        }
                
        ArticleInfo m_articleInfo;
        EnumStep m_step = EnumStep.GoToLoginPage;
        EnumStep m_lastStep = EnumStep.None;

        Timer m_timerBrain;
        sinaBrowser m_browser = null;
        sinaDb m_workingObjDb = null;
        public sinaDb.WorkingObjectInfo m_workingObjectInfo;

        UInt16 m_timesOfSomeStep = 0;
        UInt16 m_goToArticleDelayTimes = 0;
        UInt16 m_maxSteps = 40000;
        UInt16 m_timesCantGoToNext = 0;

        public static UInt64 m_MinReadCount = 3000;
                
        public sinaRobot(sinaBrowser w, Timer timerBrain)
        {
            m_workingObjDb = new sinaDb("workingObject-sina.db");

            m_browser = w;

            m_timerBrain = timerBrain;
            m_timerBrain.Enabled = true;

#if DEBUG
            m_timerBrain.Interval = 3000;
#else
            m_timerBrain.Interval = 8888;
#endif
        }

        public void timerBrain()
        {
            //if (m_publishedArticleNum > 4)
            //    Environment.Exit(0);

            m_browser.CloseSecurityAlert();

            if (m_lastStep == m_step && m_step != EnumStep.WaitSucess)
                m_timesOfSomeStep++;
            else
                m_timesOfSomeStep = 0;            

            if (m_timesOfSomeStep > m_maxSteps + 2) 
            {
                if (m_goToArticleDelayTimes > 10 && m_timesOfSomeStep < m_maxSteps * 2)
                {
                    // give change to m_goToArticleDelayTimes;
                }
                else
                {
                    Log.WriteLog(LogType.Notice, "same step is too much, maybe occurs some big error, so reset");
                    // reset
                    Environment.Exit(0);
                    //m_step = EnumStep.GoToLoginPage;
                }
            }

            Heartbeat();
            Log.WriteLog(LogType.Debug, "step is :" + m_step.ToString());
            try
            {
                switch(m_step)
                {
                    case EnumStep.GoToLoginPage:
                        GoToLoginPage();
                        break;
                    case EnumStep.Login:
                        Login();
                        break;
                    case EnumStep.GoToListPage:
                        GoToListPage();
                        break;
                    case EnumStep.GoToNextArticlePage:
                        GoToNextArticlePage();
                        break;
                    case EnumStep.GetArticleInfo:
                        GetArticleInfo();
                        break;
                    case EnumStep.GoToEditPage:
                        GoToEditPage();
                        break;
                    case EnumStep.EditTitle:
                        EditTitle();
                        break;
                    case EnumStep.EditHtml:
                        EditHtml();
                        break;
                    case EnumStep.Publish:
                        Publish();
                        break;
                    case EnumStep.WaitSucess:
                        WaitSucess();
                        break;
                    case EnumStep.Finished:
                        m_timerBrain.Stop();
                        MessageBox.Show("今天的工作已完成");
                        return;
                }
            }
            catch (Exception e)
            {
                m_step = EnumStep.GoToListPage;
#if DEBUG
                Log.WriteLog(LogType.Exception, "Exception happened in step " + m_step.ToString()
                     + ", Exception info: " + e.ToString());
                // this exception maybe just cause by doc which is not loaded complete. Network is not trustful.
#endif              
            }
            finally
            {
                m_lastStep = m_step;
            }
        }

        private void Heartbeat()
        {
            try
            {
                string fileName = "Heartbeat.txt";

                System.IO.FileStream stream = System.IO.File.Open(fileName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                stream.SetLength(0);
                stream.Close();
                stream.Dispose();

                System.IO.StreamWriter sw = System.IO.File.AppendText(fileName);
                sw.WriteLine(DateTime.Now.ToString("HH:mm"));
                sw.Close();
                sw.Dispose();
            }
            catch
            { }
        }

        private void EditTitle()
        {
            // two step do one edit. because this maybe avoid title empty bug. to give more time to load edit page.
            //if (m_lastStep == EnumStep.Edit) 
            {
                m_browser.EditTitle(m_articleInfo);
                m_step = EnumStep.EditHtml;
            }
        }

        private void EditHtml()
        {
            {
                m_browser.EditHtml();
                m_step = EnumStep.Publish;
            }
        }

        private void WaitSucess()
        {
            if (m_browser.isPublishTooMuch())
            {
                m_workingObjDb.SetWorkingObjDone(m_workingObjectInfo.id);
                m_step = EnumStep.GoToLoginPage;
            }
            else
            {
                m_workingObjectInfo.lastFinishedArticleUrlInList = m_articleInfo.url;
                m_workingObjDb.SetWorkingObjectInfo(m_workingObjectInfo);

                Log.WriteLog(LogType.Trace, "published:" + m_articleInfo.title);
                m_step = EnumStep.GoToListPage;
            }     
        }

        private void Publish()
        {
            m_browser.Publish();
            m_step = EnumStep.WaitSucess;
        }

        private void GoToEditPage()
        {
            m_browser.SafeNavigate("http://control.blog.sina.com.cn/admin/article/article_add.php");
            m_step = EnumStep.EditTitle;        
        }

        private void GetArticleInfo()
        {
            m_articleInfo = m_browser.GetArticleInfo();

            if (String.IsNullOrEmpty(m_articleInfo.title) || m_articleInfo.title == "undefined"
                || String.IsNullOrEmpty(m_articleInfo.url) || m_articleInfo.url == "undefined")
            {
                Log.WriteLog(LogType.NetworkWarning, "articleInfo is empty");
                return;
            }
            m_step = EnumStep.GoToEditPage;
            Log.WriteLog(LogType.Debug, m_articleInfo.title);
        }

        private void UseBackupObj()
        {
            sinaDb objDB = new sinaDb("ObjectBackup-cn.db");
            sinaDb.ObjectInfo backupObj = objDB.GetBackupObj();
            if (backupObj == null)
            {
                Log.WriteLog(LogType.Warning, "backup db is empty");
            }
            else
            {
                m_workingObjectInfo.url = backupObj.url;
                m_workingObjectInfo.lastListPageUrl = backupObj.lastListPageUrl;
                m_workingObjectInfo.lastFinishedArticleUrlInList = "";
            }
            objDB.DeleteBackupObj(backupObj.id);

            m_workingObjDb.SetWorkingObjectInfo(m_workingObjectInfo);

            m_step = EnumStep.GoToListPage;
        }

        private void GoToNextArticlePage()
        {
            if (DateTime.Now.Hour < 9 || DateTime.Now.Hour >= 22)
                return;

            if (m_workingObjectInfo.lastFinishedArticleUrlInList == "") // Get into new list page, so update the list page url.
            {
                string url = m_browser.Url.ToString();
                if (url.Trim() == "https://www.csdn.net/")
                {
                    Log.WriteLog(LogType.Warning, "last url is https://www.csdn.net/. obj is " + m_workingObjectInfo.url);
                    m_step = EnumStep.GoToListPage;
                    return;
                }
                m_workingObjectInfo.lastListPageUrl = m_browser.Url.ToString();
            }

            bool isNetDealy = false;
            if (m_browser.GoToNextArticlePage(m_workingObjectInfo.lastFinishedArticleUrlInList, ref isNetDealy))
                m_step = EnumStep.GetArticleInfo;
            else
            {
                if (isNetDealy)
                {
                    m_goToArticleDelayTimes++;
                    if (m_goToArticleDelayTimes > m_maxSteps) // article maybe deleted, so start from the first article in the list.
                    {
                        m_workingObjectInfo.lastFinishedArticleUrlInList = "";
                        m_goToArticleDelayTimes = 0;
                        m_timesOfSomeStep = 0; // to avoid shut domn.
                    }
                    return; // try GoToNextArticlePage again
                }
                if(!m_browser.GoToNextPage())
                {
                    m_timesCantGoToNext++;
                    if (m_timesCantGoToNext < 3)
                    {
                        m_step = EnumStep.GoToListPage;
                        return;
                    }

                    UseBackupObj();

                    Log.WriteLog(LogType.Notice, "list is empty, so object is done");
                    //this working object is done.
                }
                
                m_workingObjectInfo.lastFinishedArticleUrlInList = "";
            }
            m_timesCantGoToNext = 0;
            m_goToArticleDelayTimes = 0;
        }
        private void GoToListPage()
        {
            m_browser.SafeNavigate(m_workingObjectInfo.lastListPageUrl);
            m_step = EnumStep.GoToNextArticlePage;
        }

        private void GoToLoginPage()
        {
            m_browser.NavigateToLoginPage();
            m_step = EnumStep.Login;
        }

        private void Login()
        {
            if (m_browser.IsLogedin())
            {
                m_browser.Logout();
            }
            else if (!m_browser.Url.ToString().Contains("/login"))
            {
                m_browser.NavigateToLoginPage();
            }
            else
            {
                m_workingObjectInfo = m_workingObjDb.GetWorkingObjectInfo();
                if (m_workingObjectInfo == null)
                {
                    //m_workingObjDb.ResetWorkingObjDone();
                    MessageBox.Show("all done at this ip");
                }
                else
                {
                    if (m_browser.Login(m_workingObjectInfo.userName, m_workingObjectInfo.password))
                    {
                        m_step = EnumStep.GoToListPage;
                    }
                }
            }
        }
    }
}
