using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace WorkObjCollector
{
    class CollectorRobot
    {
        enum EnumStep
        {
            None,
            GoToObjArticleListPage,
            CheckObjThenGoToFirstArticle,
            Login,
            WaitSucess,
            Finished
        }

        EnumStep m_step = EnumStep.GoToObjArticleListPage;
        EnumStep m_lastStep = EnumStep.None;

        Timer m_timerBrain;
        CollectorBrowser m_browser = null;
        ObjDb m_checkedObjDb,m_objDb;

        public static UInt64 m_MinReadCount = 3000;

        UInt16 m_tryTimes = 0;

        string m_lastObjArticleListPage;

        public CollectorRobot(CollectorBrowser w, Timer timerBrain)
        {
            m_browser = w;

            m_timerBrain = timerBrain;
            m_timerBrain.Enabled = true;
            m_timerBrain.Interval = 2000;

            m_checkedObjDb = new ObjDb("CheckedObj.db");
            m_lastObjArticleListPage = m_checkedObjDb.GetLastCheckedObject() + "?orderby=ViewCount";
        }

        public void timerBrain()
        {
            m_tryTimes++;
            if (m_tryTimes > 30)
                Environment.Exit(0);

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

        private void CheckObjThenGoToFirstArticle()
        {

        }

        private void GoToObjArticleListPage()
        {
            m_browser.SafeNavigate(m_lastObjArticleListPage);
            m_step = EnumStep.CheckObjThenGoToFirstArticle;
        }
    }
}
