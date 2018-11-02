﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace experiment
{
    class BlogRobot
    {
        enum EnumStep
        {
            None,
            GoToLoginPage,
            Login,
            GoToListPage,
            GoToArticlePage,
            GoToEditPage,
            LoginToEdit,
            Edit,
            Publish,
            WaitSucess,
            Finished
        }

        public struct ArticleInfo
        {
            public string url;
            public string title;
            public string content;
            public UInt64 readCount;
        }
                
        ArticleInfo m_articleInfo;
        EnumStep m_step = EnumStep.GoToLoginPage;
        EnumStep m_lastStep = EnumStep.None;

        Timer m_timerBrain;
        CsdnBrowser m_browser = null;
        DataManagerSqlLite m_DataManagerSqlLite = null;
        DataManagerSqlLite.WorkingObjectInfo m_workingObjectInfo;

        UInt16 m_timesOfSomeStep = 0;
        UInt16 m_goToArticleDelayTimes = 0;
        UInt16 m_maxSteps = 40;
        UInt16 m_publishedArticleNum = 0; // get quit after finish 5 articles
        UInt16 m_waitSuccessTimes = 0;

        public static UInt64 m_MinReadCount = 5000;
        
        public BlogRobot(CsdnBrowser w, Timer timerBrain)
        {
            m_DataManagerSqlLite = new DataManagerSqlLite("workingObject.db");

            m_browser = w;

            m_timerBrain = timerBrain;
            m_timerBrain.Enabled = true;
            m_timerBrain.Interval = 3000;
        }

        public void timerBrain()
        {
            if (m_publishedArticleNum > 4)
                Environment.Exit(0);

            m_browser.CloseSecurityAlert();

            if (m_lastStep == m_step && m_step != EnumStep.WaitSucess)
                m_timesOfSomeStep++;
            else
                m_timesOfSomeStep = 0;
            m_lastStep = m_step;

            if (m_timesOfSomeStep > m_maxSteps + 2) // +2 means give change to m_goToArticleDelayTimes;
            {
                Log.WriteLog(LogType.Notice, "same step is too much, maybe occurs some big error, so reset");
                // reset
                Environment.Exit(0);
                //m_step = EnumStep.GoToLoginPage;
            }

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
                    case EnumStep.GoToArticlePage:
                        GoToArticlePage();
                        break;
                    case EnumStep.GoToEditPage:
                        GoToEditPage();
                        break;
                    case EnumStep.LoginToEdit:
                        LoginToEdit();
                        break;
                    case EnumStep.Edit:
                        Edit();
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
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
                //Tools.SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
                // release idle memory
#if DEBUG
                Log.WriteLog(LogType.Exception, "Exception happened in step " + m_step.ToString()
                     + ", Exception info: " + e.ToString());
                // this exception maybe just cause by doc which is not loaded complete. Network is not trustful.
#endif              
            }
        }

        private void Edit()
        {
            m_browser.Edit(m_articleInfo);
            m_step = EnumStep.Publish;
        }

        private void WaitSucess()
        {
            if(m_browser.isPublishedMax())
            {
                m_DataManagerSqlLite.ZeroNeedFinishNum(m_workingObjectInfo.id);

                m_step = EnumStep.Login;
                m_waitSuccessTimes = 0;
                return;
            }

#if DEBUG
            if(true)
#else
            if(m_browser.isSuccess())
#endif
            {                
                m_publishedArticleNum++;

                m_workingObjectInfo.needFinishNum--;
                m_workingObjectInfo.lastFinishedArticleUrlInList = m_articleInfo.url;
                m_DataManagerSqlLite.SetWorkingObjectInfo(m_workingObjectInfo);

                Log.WriteLog(LogType.Trace, "published:" + m_articleInfo.title);
            }
            else
            {
                m_waitSuccessTimes++;
                if (m_waitSuccessTimes < m_maxSteps)
                    return; // Keep waiting

                Log.WriteLog(LogType.Notice, "WaitSucess too much times:" + m_articleInfo.title);
            }

            if (m_workingObjectInfo.needFinishNum <= 0)
            {
                m_step = EnumStep.Login;
            }
            else
            {
                m_step = EnumStep.GoToListPage;
            }

            m_waitSuccessTimes = 0;
        }

        private void Publish()
        {
            m_browser.Publish();
            m_step = EnumStep.WaitSucess;
        }

        private void GoToEditPage()
        {
            m_articleInfo = m_browser.GoToEditPage();
            if (m_articleInfo.readCount < m_MinReadCount)
            {                
                //this working object is done.
                m_workingObjectInfo.isObjectFinished = true;
                m_DataManagerSqlLite.SetWorkingObjectInfo(m_workingObjectInfo);
                m_step = EnumStep.Login;
                Log.WriteLog(LogType.Notice, "read count is too small, so object is done");
                return;
            }

            if (String.IsNullOrEmpty(m_articleInfo.title) || m_articleInfo.title == "undefined"
                || String.IsNullOrEmpty(m_articleInfo.content) || m_articleInfo.content == "undefined"
                || String.IsNullOrEmpty(m_articleInfo.url) || m_articleInfo.url == "undefined")
            {
                Log.WriteLog(LogType.NetworkWarning, "articleInfo is empty");
                return;
            }
            m_step = EnumStep.LoginToEdit;
            Log.WriteLog(LogType.Debug, m_articleInfo.title);
        }

        private void GoToArticlePage()
        {
            if (m_workingObjectInfo.lastFinishedArticleUrlInList == "") // Get into new list page, so update the list page url.
                m_workingObjectInfo.lastListPageUrl = m_browser.Url.ToString();
     
            bool isNetDealy = false;
            if (m_browser.GoToArticlePage(m_workingObjectInfo.lastFinishedArticleUrlInList, ref isNetDealy))
                m_step = EnumStep.GoToEditPage;
            else
            {
                if (isNetDealy)
                {
                    m_goToArticleDelayTimes++;
                    if (m_goToArticleDelayTimes > m_maxSteps) // article maybe deleted, so start from the first article in the list.
                    {
                        m_workingObjectInfo.lastFinishedArticleUrlInList = "";
                        m_goToArticleDelayTimes = 0;
                    }
                    return; // try goToArticlePage again
                }
                if(!m_browser.GoToNextPage())
                {
                    m_workingObjectInfo.isObjectFinished = true;
                    m_DataManagerSqlLite.SetWorkingObjectInfo(m_workingObjectInfo);
                    m_step = EnumStep.Login;
                    Log.WriteLog(LogType.Notice, "list is empty, so object is done");
                    //this working object is done.
                }
                m_workingObjectInfo.lastFinishedArticleUrlInList = "";
            }
            m_goToArticleDelayTimes = 0;
        }
        private void GoToListPage()
        {
            m_browser.SafeNavigate(m_workingObjectInfo.lastListPageUrl);
            m_step = EnumStep.GoToArticlePage;
        }

        private void GoToLoginPage()
        {
            m_browser.NavigateToLoginPage();
            m_step = EnumStep.Login;
        }

        private void LoginToEdit()
        {
            if (!m_browser.IsLogedin())
            {
                m_browser.Login(m_workingObjectInfo.userName, m_workingObjectInfo.password);
            }
            else
            {
                if(m_browser.IsInEditPage())
                {
                    m_step = EnumStep.Edit;
                }
            }            
        }

        private void Login()
        {
            if (m_browser.IsLogedin())
            {
                m_browser.Logout();
            }
            else
            {
                m_workingObjectInfo = m_DataManagerSqlLite.GetWorkingObjectInfo();
                if (m_workingObjectInfo == null)
                    m_step = EnumStep.Finished;
                else
                {
                    if(m_browser.Login(m_workingObjectInfo.userName, m_workingObjectInfo.password))
                    {
                        m_step = EnumStep.GoToListPage;  
                    }                    
                }        
            }
        }
    }
}
