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
            GoToArticlePage,
            GoToEditPage,
            LoginToEdit,
            Edit,
            Publish,
            Finished
        }

        public struct ArticleInfo
        {
            public string url;
            public string title;
            public string content;
        }

        ArticleInfo m_articleInfo;
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
                Log.WriteLog(LogType.Warning, "Exception happened in step " + m_step.ToString() 
                    + ", Exception info: " + e.ToString());
                // this exception maybe just cause by doc which is not loaded complete. Network is not trustful.
            }
        }

        private void Edit()
        {
            m_browser.Edit(m_articleInfo);
            m_step = EnumStep.Publish;
        }

        private void Publish()
        {
            m_browser.Publish();

            m_workingObjectInfo.needFinishNum--;
            m_workingObjectInfo.lastFinishedArticleUrlInList = m_articleInfo.url;
            m_dataManager.SetWorkingObjectInfo(m_workingObjectInfo);

            if (m_workingObjectInfo.needFinishNum <= 0)
            {
                m_step = EnumStep.Login;
            }
            else
            {
                m_step = EnumStep.GoToListPage; 
            }
        }

        private void GoToEditPage()
        {
            m_articleInfo = m_browser.GoToEditPage();
            if (String.IsNullOrEmpty(m_articleInfo.title) || m_articleInfo.title == "undefined"
                || String.IsNullOrEmpty(m_articleInfo.content) || m_articleInfo.content == "undefined"
                || String.IsNullOrEmpty(m_articleInfo.url) || m_articleInfo.url == "undefined")
            {
                Log.WriteLog(LogType.NetworkWarning, "articleInfo is empty");
                return;
            }
            m_step = EnumStep.LoginToEdit;
            Log.WriteLog(LogType.Test, m_articleInfo.title);
        }

        private void GoToArticlePage()
        {
            if (m_workingObjectInfo.lastFinishedArticleUrlInList == "") // Get into new list page, so update the list page url.
                m_workingObjectInfo.lastListPageUrl = m_browser.Url.ToString();

            if (m_browser.GoToArticlePage(m_workingObjectInfo.lastFinishedArticleUrlInList))
                m_step = EnumStep.GoToEditPage;
            else
            {
                if(!m_browser.GoToNextPage())
                {
                    // TODO: this working object is done.
                }
                m_workingObjectInfo.lastFinishedArticleUrlInList = "";
            }
        }
        private void GoToListPage()
        {
            m_browser.SafeNavigate(m_workingObjectInfo.lastListPageUrl);
            m_step = EnumStep.GoToArticlePage;
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

        private void Login()
        {
            if (m_browser.IsLogedin())
            {
                m_browser.Logout();
            }
            else
            {
                m_workingObjectInfo = m_dataManager.GetWorkingObjectInfo();
                if (m_workingObjectInfo == null)
                    m_step = EnumStep.Finished;
                else
                {
                    m_browser.Login(m_workingObjectInfo.userName, m_workingObjectInfo.password);
                    m_step = EnumStep.GoToListPage;  
                }        
            }
        }
    }
}
