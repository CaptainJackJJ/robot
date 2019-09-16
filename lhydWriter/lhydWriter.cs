using System;
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
            GotoLastCheckedUrl,
            LookForNewUrl,
            CheckAndGetArticle,
            GotoLhydLoginPage,
            LoginLhyd,
            GotoPostNewPage,
            EditTitle,
            EditContent,
            Publish,
            WaitPublishDone,
            Finished
        }

        EnumStep m_step = EnumStep.GotoLastCheckedUrl;
        EnumStep m_lastStep = EnumStep.None;

        Timer m_timerBrain;
        lhydWriterBrowser m_browser = null;
        Db m_DbCheckedUrl,m_DbPostedUrl;

        UInt16 m_timesOfStep = 0;
        readonly UInt16 m_maxSteps = 3 * 20;

        string m_lastCheckedUrl;

        public static string m_listPageUrlTail = "?orderby=ViewCount";

        public lhydWriter(lhydWriterBrowser w, Timer timerBrain)
        {
            m_browser = w;

            m_timerBrain = timerBrain;
            m_timerBrain.Enabled = true;

#if DEBUG
            m_timerBrain.Interval = 2000;
#else
            m_timerBrain.Interval = 3000;
#endif

            m_DbCheckedUrl = new Db("CheckedCsdnUrl.db");
            m_lastCheckedUrl = m_DbCheckedUrl.GetLastCheckedUrl();
            if (String.IsNullOrEmpty(m_lastCheckedUrl))
            {
                MessageBox.Show("m_lastCheckedUrl is empty");
                Environment.Exit(0);
            }

            m_DbPostedUrl = new Db("PostedCsdnUrl.db");
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

        public void timerBrain()
        {
            m_timesOfStep++;

            if (m_timesOfStep > m_maxSteps)
            {
                Environment.Exit(0);
            }

            m_browser.CloseSecurityAlert();

            Heartbeat();
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
                    case EnumStep.GotoLastCheckedUrl:
                        GotoLastCheckedUrl();
                        break;
                    case EnumStep.LookForNewUrl:
                        LookForNewUrl();
                        break;
                    case EnumStep.CheckAndGetArticle:
                        CheckAndGetArticle();
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
                    case EnumStep.WaitPublishDone:
                        WaitPublishDone();
                        break; 
                    case EnumStep.Finished:
                        Environment.Exit(0);
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

        private void GotoLastCheckedUrl()
        {
            m_browser.SafeNavigate(m_lastCheckedUrl);
            m_step = EnumStep.LookForNewUrl;
        }

        private void LookForNewUrl()
        {
            string url = m_browser.LookForNewUrl(m_DbCheckedUrl,m_DbPostedUrl);
            string csdn = "https://blog.csdn.net";
            if (url == "" || url.Substring(0, csdn.Length) != csdn)
            {
                Log.WriteLog(LogType.Error, "can not found new obj, so go to newarticles");
                m_browser.SafeNavigate("https://www.csdn.net/nav/newarticles");
                return;
                //Environment.Exit(0);
            }

            m_DbCheckedUrl.AddUrlToDb(url);
            m_lastCheckedUrl = url;
            m_step = EnumStep.CheckAndGetArticle;
        }

        private void CheckAndGetArticle()
        {
            if(m_browser.CheckAndGetArticle())
            {
                m_step = EnumStep.GotoLhydLoginPage;
            }
            else
            {
                m_step = EnumStep.LookForNewUrl;
            }
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
            m_step = EnumStep.WaitPublishDone;
        }

        private void WaitPublishDone()
        {
            if (m_browser.IsPublishing())
            {
                m_step = EnumStep.WaitPublishDone;
                return;
            }
            m_DbPostedUrl.AddUrlToDb(m_lastCheckedUrl);
            m_step = EnumStep.Finished;
        }

        private void CheckObjThenGoToFirstArticle()
        {
        }

        private void GoToObjArticleListPage()
        {
            m_browser.SafeNavigate(m_lastCheckedUrl + m_listPageUrlTail);
            m_step = EnumStep.CheckObjThenGoToFirstArticle;
        }
    }
}
