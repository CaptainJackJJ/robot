using System;
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
            GoToLoginPage,
            Login,
            GoToListPage,
            GoToNextArticle,
            Finished
        }

        EnumStep m_step = EnumStep.GoToLoginPage;

        Timer m_timerBrain;
        CsdnBrowser m_browser = null;
        DataManager m_dataManager = null;
        DataManager.WorkingObjectInfo m_workingObjectInfo;

        public BlogRobot(CsdnBrowser w, Timer timerBrain)
        {
            m_dataManager = new DataManager();

            m_browser = w;

            m_timerBrain = timerBrain;
            m_timerBrain.Enabled = true;
            m_timerBrain.Interval = 2000;
        }

        public void timerBrain()
        {
            m_browser.CloseSecurityAlert();

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
                    case EnumStep.GoToNextArticle:
                        GoToNextArticle();
                        break;
                    case EnumStep.Finished:
                        m_timerBrain.Stop();
                        return;
                }
            }
            catch (Exception e)
            {
                Log.WriteLog(LogType.Warning, "Exception happened in step " + m_step.ToString() 
                    + ", Exception info: " + e.ToString());
                // this exception maybe just cause by doc which is not loaded complete. Network is not trustful.
            }
        }
        private void GoToNextArticle()
        {
            if (m_browser.NavToNextArticle(m_workingObjectInfo.lastFinishedArticleUrl))
                m_step = EnumStep.Finished;
            else
            {
                // go next list page, or this working object is done.
            }
        }
        private void GoToListPage()
        {
            m_browser.SafeNavigate(m_workingObjectInfo.lastListPageUrl);
            m_step = EnumStep.GoToNextArticle;
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
            else
            {
                m_workingObjectInfo = m_dataManager.GetWorkingObjectInfo();
                m_browser.Login(m_workingObjectInfo.userName, m_workingObjectInfo.password);
                m_step = EnumStep.GoToListPage;
            }
        }
    }
}
