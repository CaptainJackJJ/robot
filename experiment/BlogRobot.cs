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
                        m_browser.NavigateToLoginPage();
                        m_step = EnumStep.Login;
                        break;
                    case EnumStep.Login:
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
                        break;
                    case EnumStep.GoToListPage:
                        m_browser.SafeNavigate(m_workingObjectInfo.lastListPageUrl);
                        m_step = EnumStep.Finished;
                        break;
                    case EnumStep.Finished:
                        break;
                }
            }
            catch (Exception e)
            {
                Log.WriteLog(LogType.Warning, "Exception happened in step " + m_step.ToString() 
                    + ", Exception info: " + e.ToString());
                // this exception maybe just cause by doc which is not loaded complete. Network is not trustful.
            }
        }
    }
}
