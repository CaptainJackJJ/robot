﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace WorkObjCollector
{
    class lhydWriter
    {
        enum EnumStep
        {
            None,
            GoToObjArticleListPage,
            CheckObjThenGoToFirstArticle,
            LookForNewObj,
            GotoLhydLoginPage,
            LoginLhyd,
            GotoPostNewPage,
            EditTitle,
            EditContent,
            Publish,
            Finished
        }

        EnumStep m_step = EnumStep.GotoLhydLoginPage;
        EnumStep m_lastStep = EnumStep.None;

        Timer m_timerBrain;
        lhydWriterBrowser m_browser = null;
        Db m_DbCheckedUrl,m_DbPostedUrl;

        UInt16 m_timesOfStep = 0;
        readonly UInt16 m_maxSteps = 3 * 20;

        string m_lastCheckedUrl;

        readonly int m_minReadCount = 5000;
        readonly UInt16 m_minArticleCount = 1;
        public static string m_listPageUrlTail = "?orderby=ViewCount";

        public lhydWriter(lhydWriterBrowser w, Timer timerBrain)
        {
            m_browser = w;

            m_timerBrain = timerBrain;
            m_timerBrain.Enabled = true;
            m_timerBrain.Interval = 2000;

            m_DbCheckedUrl = new Db("CheckedCsdnUrl.db");
            m_lastCheckedUrl = m_DbCheckedUrl.GetLastCheckedUrl();
            if (String.IsNullOrEmpty(m_lastCheckedUrl))
            {
                MessageBox.Show("m_lastCheckedUrl is empty");
                Environment.Exit(0);
            }
            m_browser.SafeNavigate(m_lastCheckedUrl);

            m_DbPostedUrl = new Db("PostedCsdnUrl.db");
        }

        public void timerBrain()
        {
            m_timesOfStep++;

            if (m_timesOfStep > m_maxSteps)
            {
                Environment.Exit(0);
            }

            m_browser.CloseSecurityAlert();
            
            Log.WriteLog(LogType.Debug, "step is :" + m_step.ToString());
            try
            {
                switch(m_step)
                {
                    case EnumStep.GoToObjArticleListPage:
                        GoToObjArticleListPage();
                        break;
                    case EnumStep.CheckObjThenGoToFirstArticle:
                        CheckObjThenGoToFirstArticle();
                        break;
                    case EnumStep.LookForNewObj:
                        LookForNewObj();
                        break;
                    case EnumStep.GotoLhydLoginPage:
                        GotoLhydLoginPage();
                        break;
                    case EnumStep.LoginLhyd:
                        LoginLhyd();
                        break;
                    case EnumStep.GotoPostNewPage:
                        GotoPostNewPage();
                        break;
                    case EnumStep.EditTitle:
                        EditTitle();
                        break;
                    case EnumStep.EditContent:
                        EditContent();
                        break;
                    case EnumStep.Publish:
                        Publish();
                        break; 

                    case EnumStep.Finished:
                        m_timerBrain.Stop();
                        MessageBox.Show("今天的工作已完成");
                        return;
                }
            }
            catch (Exception e)
            {
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


        private void LookForNewObj()
        {
            string objUrl = m_browser.LookForNewObj(m_DbCheckedUrl);
            string csdn = "https://blog.csdn.net";
            if (objUrl == "" || objUrl.Substring(0, csdn.Length) != csdn)
            {
                Log.WriteLog(LogType.Error, "can not found new obj, so go to newarticles");
                m_browser.SafeNavigate("https://www.csdn.net/nav/newarticles");
                return;
                //Environment.Exit(0);
            }

            m_DbCheckedUrl.AddUrlToDb(objUrl);
            m_lastCheckedUrl = objUrl;
            m_step = EnumStep.GoToObjArticleListPage;
        }

        private void GotoLhydLoginPage()
        {
            m_browser.SafeNavigate("http://lhyd.top/wp-login.php?");
            m_step = EnumStep.LoginLhyd;
        }
        private void LoginLhyd()
        {
            if(!m_browser.LoginLhyd())
            {
                m_step = EnumStep.GotoLhydLoginPage;
                return;
            }
            m_step = EnumStep.GotoPostNewPage;
        }
        private void GotoPostNewPage()
        {
            m_browser.SafeNavigate("http://lhyd.top/wp-admin/post-new.php");
            m_step = EnumStep.EditTitle;
        }
        private void EditTitle()
        {
            if(!m_browser.EditTitle())
            {
                m_step = EnumStep.GotoPostNewPage;
                return;
            }
            m_step = EnumStep.EditContent;
        }
        private void EditContent()
        {
            m_browser.EditContent();
            m_step = EnumStep.Publish;
        }
        private void Publish()
        {
            if (!m_browser.Publish())
            {
                m_step = EnumStep.GotoPostNewPage;
                return;
            }
            m_step = EnumStep.Finished;
        }

        private void CheckObjThenGoToFirstArticle()
        {
            // https://passport.csdn.net/passport_fe/login.html
            if(m_browser.Url.ToString().Contains("passport.csdn.net"))
            {
                //m_browser.Login("sdhiiwfssf", "FiSKpJuHc12345");
                return;
            }

            bool isNeedCheck = true;
            if (m_DbPostedUrl.IsObjectPosted(m_lastCheckedUrl))
            {
                isNeedCheck = false;
            }
            
            bool isNeedCollect = false;
            bool isDelay = false;
            Int64 totalReadCount = 0; bool isExpert = false; int maxReadCount = 0;
            int OriginalArticleNum = 0;int FansNum= 0;int LikeNum= 0;int CommentsNum= 0;int Degree= 0; int Score= 0;int Ranking= 0;            

            m_browser.CheckObjThenGoToFirstArticle(isNeedCheck, m_minReadCount, m_minArticleCount, ref isNeedCollect, ref isDelay,
                ref totalReadCount,ref maxReadCount,ref OriginalArticleNum, ref FansNum, ref LikeNum, ref CommentsNum, ref Degree, ref Score, 
                ref Ranking, ref isExpert);
            
            if (isDelay)
                return;
            if(isNeedCollect)
            {
                if (!m_DbPostedUrl.AddUrlToDb(m_lastCheckedUrl))
                {
                    return;
                }
            }

            m_step = EnumStep.LookForNewObj;
        }

        private void GoToObjArticleListPage()
        {
            m_browser.SafeNavigate(m_lastCheckedUrl + m_listPageUrlTail);
            m_step = EnumStep.CheckObjThenGoToFirstArticle;
        }
    }
}
