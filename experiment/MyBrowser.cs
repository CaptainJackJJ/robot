using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace experiment
{
    class MyBrowser
    {
        bool m_bNeedClickAccountLogin = false;
        bool m_bNeedClickLogin = false;

        public WebBrowser m_browser = null;
        Timer m_timerAfterDocCompleted = null;

        public MyBrowser(WebBrowser w, Timer timerAfterDocCompleted)
        {
            m_browser = w;
            m_timerAfterDocCompleted = timerAfterDocCompleted;
            
            m_timerAfterDocCompleted.Enabled = false;
            m_timerAfterDocCompleted.Interval = 1000;

            // Ensure that ScriptErrorsSuppressed is set to false.
            m_browser.ScriptErrorsSuppressed = false;

            // Handle DocumentCompleted to gain access to the Document object.
            m_browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);

            m_bNeedClickAccountLogin = true;
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

        public void ClickAccountLogin()
        {
            ClickEleByTagAndOuterHtml("a", "账号登录");
        }

        public void ClickLogin()
        {
            ClickEleByTagAndOuterHtml("a", "https://passport.csdn.net/account/login");
        }

        // do not show scriptError dlg
        private void Window_Error(object sender, HtmlElementErrorEventArgs e)
        {
            e.Handled = true;
        }

        private void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            m_timerAfterDocCompleted.Enabled = true;

            ((WebBrowser)m_browser).Document.Window.Error += new HtmlElementErrorEventHandler(Window_Error);
        }

        private bool IsLogedin()
        {
            if (GetEleByTagAndOuterHtml("a", "账号登录") == null)
                return true;
            return false;
        }

        public void timerAfterDocCompleted()
        {
            Tools.CloseSecurityAlert();

            m_timerAfterDocCompleted.Enabled = false;

            if (m_bNeedClickAccountLogin && !IsLogedin())
            {                
                ClickAccountLogin();
                m_bNeedClickAccountLogin = false;
                Login("sdhiiwfssf", "Cq&86tjUKHEG");
            }            
        }

        public bool Login(string uName,string password)
        {
            HtmlElement ele = m_browser.Document.GetElementById("username");            
            if (ele == null) return false;
            ele.SetAttribute("value", uName);

            ele = m_browser.Document.GetElementById("password");
            if (ele == null) return false;
            ele.SetAttribute("value", password);

            ClickEleByTagAndOuterHtml("input", "登 录");
            return false;
        }
    }
}
