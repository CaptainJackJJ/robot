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
        bool m_isNewWorkingObject = true;
        bool m_needClickAccountLogin = false;
        bool m_needGoToListPage = false;
        bool m_needGetWorkingObjectInfo = false;

        string m_lastListPageUrl;

        CsdnBrowser m_browser = null;
        DataManager m_dataManager = null;
        DataManager.WorkingObjectInfo m_workingObjectInfo;

        public BlogRobot(CsdnBrowser w, Timer timerAfterDocCompleted)
        {
            m_dataManager = new DataManager();

            m_browser = w;
            m_browser.Init(timerAfterDocCompleted);

            m_needClickAccountLogin = true;
            m_browser.NavigateToLoginPage();
        }

        private void Logout()
        {
            m_browser.Logout();
        }


        public void timerAfterDocCompleted()
        {
            m_browser.timerAfterDocCompleted();
            if (m_isNewWorkingObject)
            {
                if (m_browser.IsLogedin())
                {
                    Logout();
                }
                else
                {
                    m_workingObjectInfo = m_dataManager.GetWorkingObjectInfo();                    
                    m_browser.Login(m_workingObjectInfo.userName, m_workingObjectInfo.password);
                    m_isNewWorkingObject = false;
                }
            }
        }
    }
}
