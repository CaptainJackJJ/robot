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
        WebBrowser m_browser = null;
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
        }

        private HtmlElement GetEleByTagAndOuterHtml(string tag,string html)
        {
            HtmlElementCollection collection = m_browser.Document.GetElementsByTagName("a");
            foreach (HtmlElement ele in collection)
            {
                if (ele.OuterHtml.Contains("账号登录"))
                {
                    return ele;
                }
            }
            MessageBox.Show("did not find ele tag is '" + tag + "' outerHtml is " + html);
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

            ((WebBrowser)sender).Document.Window.Error += new HtmlElementErrorEventHandler(Window_Error);
        }


        public void timerAfterDocCompleted()
        {
            Tools.CloseSecurityAlert();

            m_timerAfterDocCompleted.Enabled = false;
        }
    }
}
