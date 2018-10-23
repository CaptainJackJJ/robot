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
            Login,
            GoToListPage,
            Finished
        }

        EnumStep m_step = EnumStep.Login;

        CsdnBrowser m_browser = null;
        DataManager m_dataManager = null;
        DataManager.WorkingObjectInfo m_workingObjectInfo;

        public BlogRobot(CsdnBrowser w, Timer timerAfterDocCompleted)
        {
            m_dataManager = new DataManager();

            m_browser = w;
            m_browser.Init(timerAfterDocCompleted);

            m_browser.NavigateToLoginPage();
        }

        public void timerAfterDocCompleted()
        {
            m_browser.CloseSecurityAlert();

            try
            {
                if (m_step == EnumStep.Login)
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
                else if (m_step == EnumStep.GoToListPage)
                {
                    m_browser.SafeNavigate(m_workingObjectInfo.lastListPageUrl);
                    m_step = EnumStep.Finished;
                }
            }
            catch (Exception e)
            {
                Log.WriteLog(LogType.Warning, "Exception happened in step " + m_step.ToString() 
                    + ", Exception info: " + e.ToString());
                return;
                // Exception happened, return directly.
                // Do not close timer, let timer keep try again.
                // Because this exception maybe just cause by doc which is not loaded complete.
                // Webbroswer's DocCompleted event is not trustful.
            }

            m_browser.timerAfterDocCompleted();
        }
    }
}
