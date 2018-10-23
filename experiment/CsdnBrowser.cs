using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace experiment
{
    class CsdnBrowser
    {
        public WebBrowser m_browser = null;
        Timer m_timerAfterDocCompleted = null;

        public CsdnBrowser(WebBrowser w, Timer timerAfterDocCompleted)
        {
            m_browser = w;
            m_timerAfterDocCompleted = timerAfterDocCompleted;
            
            m_timerAfterDocCompleted.Enabled = false;
            m_timerAfterDocCompleted.Interval = 3000;

            // This flag make script error dlg disappear.
            m_browser.ScriptErrorsSuppressed = true;

            m_browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);
        }

        public void NavigateToLoginPage()
        {
            m_browser.Navigate("https://passport.csdn.net/account/login");  
        }

        private HtmlElement GetEleByTagAndOuterHtml(string tag,string html)
        {
            HtmlElementCollection collection = m_browser.Document.GetElementsByTagName(tag);
            foreach (HtmlElement ele in collection)
            {
                if (ele.OuterHtml.Contains(html))
                {
                    return ele;
                }
            }
            return null;
        }

        private void ClickEleByTagAndOuterHtml(string tag,string html)
        {
            Tools.SafeClick(GetEleByTagAndOuterHtml(tag, html));
        }

        private void ClickAccountLogin()
        {
            ClickEleByTagAndOuterHtml("a", "账号登录");
        }

        public void ClickLogin()
        {
            ClickEleByTagAndOuterHtml("a", "https://passport.csdn.net/account/login");
        }

        // do not show scriptError dlg. But seems does not work
        //private void Window_Error(object sender, HtmlElementErrorEventArgs e)
        //{
        //    e.Handled = true;
        //}

        private void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            m_timerAfterDocCompleted.Enabled = true;

            //((WebBrowser)sender).Document.Window.Error += new HtmlElementErrorEventHandler(Window_Error);
        }

        public void timerAfterDocCompleted()
        {
            m_timerAfterDocCompleted.Enabled = false;
            Tools.CloseSecurityAlert();
        }

        public bool IsLogedin()
        {
            if (GetEleByTagAndOuterHtml("img", "login_img") != null)
                return true;
            return false;
        }

        public bool Login(string uName,string password)
        {
            ClickAccountLogin();

            HtmlElement ele = m_browser.Document.GetElementById("username");
            if (ele == null)
            {
                Log.WriteLog(LogType.Error, "did not found ele username");
                return false;
            }
            ele.SetAttribute("value", uName);

            ele = m_browser.Document.GetElementById("password");
            if (ele == null)
            {
                Log.WriteLog(LogType.Error, "did not found ele password");
                return false;
            }
            ele.SetAttribute("value", password);

            ClickEleByTagAndOuterHtml("input", "登 录");
            Log.WriteLog(LogType.Trace, "logged in with username " + uName + " password " + password);
            return true;
        }

        public void Logout()
        {
            ClickEleByTagAndOuterHtml("a", "退出");
            NavigateToLoginPage();
        }
    }
}
