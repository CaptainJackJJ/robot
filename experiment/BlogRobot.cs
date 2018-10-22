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
        bool m_bNeedClickAccountLogin = false;

        CsdnBrowser m_browser = null;

        public BlogRobot(WebBrowser w, Timer timerAfterDocCompleted)
        {
            m_browser = new CsdnBrowser(w, timerAfterDocCompleted);

            m_bNeedClickAccountLogin = true;
            m_browser.NavigateToLoginPage();
        }

        public void timerAfterDocCompleted()
        {
            m_browser.timerAfterDocCompleted();

            if (m_bNeedClickAccountLogin && !m_browser.IsLogedin())
            {
                m_browser.ClickAccountLogin();
                m_bNeedClickAccountLogin = false;
                m_browser.Login("sdhiiwfssf", "Cq&86tjUKHEG");
            }
        }
    }
}
