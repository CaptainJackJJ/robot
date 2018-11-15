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
            LookForNewObj,
            Finished
        }

        EnumStep m_step = EnumStep.GoToObjArticleListPage;
        EnumStep m_lastStep = EnumStep.None;

        Timer m_timerBrain;
        CollectorBrowser m_browser = null;
        ObjDb m_checkedObjDb,m_objDb;

        UInt16 m_timesOfStep = 0;
        readonly UInt16 m_maxSteps = 3 * 20;

        string m_lastObjArticleListPage;

        readonly UInt64 m_minReadCount = 3000;
        readonly UInt16 m_minArticleCount = 5;
        public static string m_listPageUrlTail = "?orderby=ViewCount";

        public CollectorRobot(CollectorBrowser w, Timer timerBrain)
        {
            m_browser = w;

            m_timerBrain = timerBrain;
            m_timerBrain.Enabled = true;
            m_timerBrain.Interval = 2000;

            m_checkedObjDb = new ObjDb("CheckedObj.db");
            m_lastObjArticleListPage = m_checkedObjDb.GetLastCheckedObject() + m_listPageUrlTail;

            m_objDb = new ObjDb("Object.db");
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
            string objUrl = m_browser.LookForNewObj(m_checkedObjDb);
            if(objUrl == "")
            {
                Log.WriteLog(LogType.Error, "can not found new obj");
                Environment.Exit(0);
            }

            m_checkedObjDb.AddCheckedObject(objUrl);
            m_lastObjArticleListPage = objUrl + m_listPageUrlTail;
            m_step = EnumStep.GoToObjArticleListPage;
        }

        private void CheckObjThenGoToFirstArticle()
        {
            bool isNeedCheck = true;
            if(m_objDb.IsObjectCollected(m_lastObjArticleListPage))
            {
                isNeedCheck = false;
            }
            
            bool isNeedCollect = false;
            bool isDelay = false;
            m_browser.CheckObjThenGoToFirstArticle(isNeedCheck, m_minReadCount, m_minArticleCount, ref isNeedCollect, ref isDelay);
            
            if (isDelay)
                return;
            if(isNeedCollect)
            {
                if (!m_objDb.CollectObject(m_lastObjArticleListPage))
                    return;
            }

            m_step = EnumStep.LookForNewObj;
        }

        private void GoToObjArticleListPage()
        {
            m_browser.SafeNavigate(m_lastObjArticleListPage);
            m_step = EnumStep.CheckObjThenGoToFirstArticle;
        }
    }
}
