﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace experiment
{
    class KeyRobot
    {
        enum EnumStep
        {
            None,
            Back1AfterComment,
            Back2AfterComment,
            RemoveOldUrl,
            EnterUrl,
            Search,
            GoToUser,
            ClickUser,
            ClickArticle,
            ClickComment,
            EnterComment,
            ClickSend,

            GoToLoginPageForLockCheck,
            LoginForLockCheck,
            ConfirmLoginForLockCheck,
            CheckLock,

            GoToLoginPage,
            Login,
            ConfirmLogin,
            GoToListPage,
            GoToArticlePage,
            GoToEditPage,
            LoginToEdit,
            Edit,
            PrePublish, // can not direct click publish buttion, if so article type droplist would failed. so need PrePublsih
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
        EnumStep m_step = EnumStep.Back1AfterComment;
        EnumStep m_lastStep = EnumStep.None;

        Timer m_timerBrain;
        CsdnBrowser m_browser = null;
        dbKeyRobot m_DataManagerSqlLite = null;
        dbKeyRobot.WorkingObjectInfo m_workingObjectInfo;
        dbKeyRobot.BloggerInfo m_bloggerInfo;

        UInt16 m_timesOfSomeStep = 0;
        UInt16 m_goToArticleDelayTimes = 0;
        UInt16 m_maxSteps = 20;
        //UInt16 m_publishedArticleNum = 0; // get quit after finish 5 articles
        UInt16 m_waitSuccessTimes = 0;
        UInt16 m_timesDetectLessMinReadCount = 0;
        UInt16 m_timesCantGoToNext = 0;

        public static UInt64 m_MinReadCount = 3000;
                
        public KeyRobot(Timer timerBrain)
        {
            m_DataManagerSqlLite = new dbKeyRobot("CsdnBloger_invite.db");

            m_timerBrain = timerBrain;
            m_timerBrain.Enabled = false;
#if DEBUG
            m_timerBrain.Interval = 2000;
#else
            m_timerBrain.Interval = 3000;
#endif
        }

        public void timerBrain()
        {
            //if (m_publishedArticleNum > 4)
            //    Environment.Exit(0);

            //m_browser.CloseSecurityAlert();

            if (m_lastStep == m_step && m_step != EnumStep.WaitSucess)
                m_timesOfSomeStep++;
            else
                m_timesOfSomeStep = 0;            

            if (m_timesOfSomeStep > m_maxSteps + 2) 
            {
                //if (m_goToArticleDelayTimes > 10 && m_timesOfSomeStep < m_maxSteps * 2)
                //{
                //    // give change to m_goToArticleDelayTimes;
                //}
                //else
                //{
                //    Log.WriteLog(LogType.Notice, "same step is too much, maybe occurs some big error, so reset");
                //    // reset
                //    Environment.Exit(0);
                //    //m_step = EnumStep.GoToLoginPage;
                //}
            }

            Heartbeat();
            Log.WriteLog(LogType.Debug, "step is :" + m_step.ToString());
            try
            {
                switch(m_step)
                {
                    case EnumStep.Back1AfterComment:
                        Back1AfterComment();
                        break;
                    case EnumStep.Back2AfterComment:
                        Back2AfterComment();
                        break;
                    case EnumStep.RemoveOldUrl:
                        RemoveOldUrl();
                        break;
                    case EnumStep.EnterUrl:
                        EnterUrl();
                        break;
                    case EnumStep.Search:
                        Search();
                        break;
                    case EnumStep.GoToUser:
                        GoToUser();
                        break;
                    case EnumStep.ClickUser:
                        ClickUser();
                        break;
                    case EnumStep.ClickArticle:
                        ClickArticle();
                        break;
                    case EnumStep.ClickComment:
                        ClickComment();
                        break;
                    case EnumStep.EnterComment:
                        EnterComment();
                        break;
                   case EnumStep.ClickSend:
                        ClickSend();
                        break;
                    case EnumStep.GoToLoginPageForLockCheck:
                        GoToLoginPageForLockCheck();
                        break;
                    case EnumStep.LoginForLockCheck:
                        LoginForLockCheck();
                        break;
                    case EnumStep.ConfirmLoginForLockCheck:
                        ConfirmLoginForLockCheck();
                        break;
                    case EnumStep.CheckLock:
                        CheckLock();
                        break;
                    case EnumStep.GoToLoginPage:
                        GoToLoginPage();
                        break;
                    case EnumStep.Login:
                        Login();
                        break;
                    case EnumStep.ConfirmLogin:
                        ConfirmLogin();
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
                    case EnumStep.PrePublish:
                        PrePublish();
                        break;
                    case EnumStep.Publish:
                        Publish();
                        break;
                    case EnumStep.WaitSucess:
                        WaitSucess();
                        break;
                    case EnumStep.Finished:
                        //m_timerBrain.Stop();
                        //MessageBox.Show("今天的工作已完成");
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

        public void Start()
        {
            m_step = EnumStep.Back1AfterComment;
            m_timerBrain.Start();
        }

        private void Edit()
        {
            if (!m_browser.IsInEditPage())
            {
                return;
            }

            // two step do one edit. because this maybe avoid title empty bug. to give more time to load edit page.
            if (m_lastStep == EnumStep.Edit) 
            {
                m_browser.Edit(m_articleInfo);
                m_step = EnumStep.PrePublish;
            }
        }

        private void WaitSucess()
        {
            if(m_browser.isMissContent())
            {
                m_step = EnumStep.GoToListPage;
                m_waitSuccessTimes = 0;
                return;
            }

            if(m_browser.isPublishedMax())
            {
                m_DataManagerSqlLite.SetObjDailyJobDone(m_workingObjectInfo.id);

                Environment.Exit(0);

                //m_step = EnumStep.Login;
                //m_waitSuccessTimes = 0;
                //return;
            }

#if DEBUG
            if(true)
#else
            if(m_browser.isSuccess())
#endif
            {                
                //m_publishedArticleNum++;

                m_workingObjectInfo.needFinishNum--;
                m_workingObjectInfo.lastFinishedArticleUrlInList = m_articleInfo.url;
                if(!m_DataManagerSqlLite.SetWorkingObjectInfo(m_workingObjectInfo))
                {
                    UseBackupObj();
                }

                Log.WriteLog(LogType.Trace, "published:" + m_articleInfo.title);

            }
            else
            {
                if(m_browser.isUnexpectError())
                {
                    Log.WriteLog(LogType.Error, "occur unexpect error, so jump over this article. lastListUrl is "
                        + m_workingObjectInfo.lastListPageUrl + ", article is " + m_articleInfo.url);

                    m_workingObjectInfo.lastFinishedArticleUrlInList = m_articleInfo.url;
                    m_step = EnumStep.GoToListPage;
                    m_waitSuccessTimes = 0;

                    return;
                }

                m_waitSuccessTimes++;
                if (m_waitSuccessTimes < m_maxSteps)
                    return; // Keep waiting

                Log.WriteLog(LogType.Notice, "WaitSucess too much times:" + m_articleInfo.title);
            }

            if (m_workingObjectInfo.needFinishNum <= 0)
            {
                Environment.Exit(0);
            }
            else
            {
                m_step = EnumStep.GoToListPage;
            }

            m_waitSuccessTimes = 0;
        }

        private void PrePublish()
        {
            m_browser.PrePublish();
            m_step = EnumStep.Publish;
        }

        private void Publish()
        {
            m_browser.Publish();
            m_step = EnumStep.WaitSucess;
        }

        private void GoToEditPage()
        {
            if(dbKeyRobot.bRandon)
            {
                m_browser.BeFans();
                m_browser.Follow();
            }

            m_articleInfo = m_browser.GoToEditPage();
            if (!dbKeyRobot.bRandon && m_articleInfo.readCount < m_MinReadCount)
            {
                //m_timesDetectLessMinReadCount++;
                //if (m_timesDetectLessMinReadCount < 3)
                //{
                //    m_step = EnumStep.GoToListPage;
                //    return;
                //}

                //this working object is done.
                m_timesDetectLessMinReadCount = 0;

                UseBackupObj();

                Log.WriteLog(LogType.Trace, "read count is too small, so object is done");
                return;
            }

            m_timesDetectLessMinReadCount = 0;

            if (String.IsNullOrEmpty(m_articleInfo.title) || m_articleInfo.title == "undefined"
                || String.IsNullOrEmpty(m_articleInfo.url) || m_articleInfo.url == "undefined")
            {
                Log.WriteLog(LogType.NetworkWarning, "articleInfo is empty");
                return;
            }
            m_step = EnumStep.Edit;
            Log.WriteLog(LogType.Debug, m_articleInfo.title);
        }

        private void UseBackupObj()
        {
            dbKeyRobot objDB = new dbKeyRobot("ObjectBackup.db");
            dbKeyRobot.ObjectInfo backupObj = objDB.GetBackupObj();
            if (backupObj == null)
            {
                Log.WriteLog(LogType.Warning, "backup db is empty");
                m_workingObjectInfo.needFinishNum = 0;
            }
            else
            {
                m_workingObjectInfo.url = backupObj.url;
                m_workingObjectInfo.lastListPageUrl = backupObj.lastListPageUrl;
                m_workingObjectInfo.lastFinishedArticleUrlInList = "";
            }
            objDB.DeleteBackupObj(backupObj.id);

            m_DataManagerSqlLite.SetWorkingObjectInfo(m_workingObjectInfo);

            m_step = EnumStep.Login;
        }

        private void GoToArticlePage()
        {
            if (!m_browser.Url.ToString().Contains("orderby=UpdateTime"))
            {
                m_step = EnumStep.GoToListPage;
                return;
            }

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
                        m_timesOfSomeStep = 0; // to avoid shut domn.
                    }
                    return; // try goToArticlePage again
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
            m_step = EnumStep.GoToArticlePage;
        }
        private bool GetFirstBlogger()
        {
            m_bloggerInfo = m_DataManagerSqlLite.GetFirstBlogger();
            if(m_bloggerInfo == null)
            {
                m_step = EnumStep.Finished;
                MessageBox.Show("No more blogger");
                return false;
            }
            if(m_DataManagerSqlLite.HasMany(m_bloggerInfo.ranking))
            {
                m_DataManagerSqlLite.DeleteBlogger(m_bloggerInfo.id);;
                return false;
            }
            return true;
        }
        private void Back1AfterComment()
        {
            Tools.Click(511,77);
            m_step = EnumStep.Back2AfterComment;
        }
        private void Back2AfterComment()
        {
            Tools.Click(511, 77);
            m_step = EnumStep.RemoveOldUrl;
        }
        private void RemoveOldUrl()
        {
            Tools.Click(813, 75);
            m_step = EnumStep.EnterUrl;
        }

        private void EnterUrl()
        {
            if (!GetFirstBlogger())
                return;
            Clipboard.SetDataObject(m_bloggerInfo.url);
            Tools.ctrlV();
            m_step = EnumStep.Search;
        }
        private void Search()
        {
            Tools.Click(850, 77);
            m_step = EnumStep.GoToUser;
        }

        private void GoToUser()
        {
            Tools.Click(578, 116);
            m_step = EnumStep.ClickUser;
        }
        private void ClickUser()
        {
            Tools.Click(583, 222);
            m_step = EnumStep.ClickArticle;
        }
        private void ClickArticle()
        {
            Tools.Click(616, 616);
            m_step = EnumStep.ClickComment;
        }

        private void ClickComment()
        {
            Tools.Click(647, 704);
            m_step = EnumStep.EnterComment;
        }

        private void EnterComment()
        {
            Clipboard.SetDataObject("博主您好！您的博文非常棒！我们想与您进行商务合作。若有意合作，请加V：CaptainJackJJ。若有打扰，望博友们海涵！期待更多博主加入我们！大家为写博付出了精力，应当获得收入作为回报！");
            Tools.ctrlV();
            m_step = EnumStep.ClickSend;
        }

        private void ClickSend()
        {
            Tools.Click(856, 495);
            m_DataManagerSqlLite.SetBloggerInvited(m_bloggerInfo.id);
            m_step = EnumStep.Back1AfterComment;
        }

        private void GoToLoginPageForLockCheck()
        {
            m_browser.NavigateToLoginPage();
            m_step = EnumStep.LoginForLockCheck;
        }
        private void LoginForLockCheck()
        {
            if (DateTime.Now.Hour < 9 || DateTime.Now.Hour > 22)
                return;

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
                m_workingObjectInfo = m_DataManagerSqlLite.GetFirstWorkingObject();
                if (m_workingObjectInfo == null)
                {
                    return;
                    //m_step = EnumStep.Finished;
                }
                else
                {
                    if (m_browser.Login(m_workingObjectInfo.userName, m_workingObjectInfo.password))
                    {
                        m_step = EnumStep.ConfirmLoginForLockCheck;
                    }
                }
            }
        }
        private void ConfirmLoginForLockCheck()
        {
            // <input class="logging" accesskey="l" value="登 录" tabindex="6" type="button">
            if (!m_browser.MouseClickEle("input", "登 录"))
            {
                //<button data-type="account" class="btn btn-primary">登录</button>
                if (!m_browser.MouseClickEle("button", "登录"))
                    return;
            }

            m_step = EnumStep.CheckLock;
        }

        private void CheckLock()
        {
            //https://passport.csdn.net/passport_fe/sign.html
            if (!m_browser.Url.ToString().Contains("sign"))
            {
                m_browser.Logout();
                m_step = EnumStep.GoToLoginPage;
            }
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

        private void ConfirmLogin()
        {
            // <input class="logging" accesskey="l" value="登 录" tabindex="6" type="button">
            if (!m_browser.MouseClickEle("input", "登 录"))
            {
                //<button data-type="account" class="btn btn-primary">登录</button>
                if (!m_browser.MouseClickEle("button", "登录"))
                    return;
            }

            m_step = EnumStep.GoToListPage;
        }

        public void ResetNeedFinishNum()
        {
            m_DataManagerSqlLite.ResetNeedFinishNum();
        }

        private void Login()
        {
#if DEBUG

#else
            if (DateTime.Now.Hour < 9 || DateTime.Now.Hour >= 22)
                return;
#endif


            if (m_browser.IsLogedin())
            {
                m_browser.Logout();
            }
            else if(!m_browser.Url.ToString().Contains("/login"))
            {
                m_browser.NavigateToLoginPage();
            }
            else
            {
                m_workingObjectInfo = m_DataManagerSqlLite.GetWorkingObjectInfo();
                if (m_workingObjectInfo == null)
                {
                    m_DataManagerSqlLite.ResetNeedFinishNum();
                    return;
                    //m_step = EnumStep.Finished;
                }
                else
                {
                    if (m_browser.Login(m_workingObjectInfo.userName, m_workingObjectInfo.password))
                    {
                        m_step = EnumStep.ConfirmLogin;
                    }
                }        
            }
        }
    }
}
