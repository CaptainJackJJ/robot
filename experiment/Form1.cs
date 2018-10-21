using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Win32;
using System.Net.Security;


namespace experiment
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Tools.SetWebBrowserFeatures(11);
            this.Text = this.Text + "_IE" + Tools.GetBrowserVersion().ToString();

            SuppressScriptErrorsOnly(webBrowser1);

            webBrowser1.Navigate("https://blog.csdn.net/jiangjunshow/article/details/77711593");
        }

        #region Hide script error
        // Hides script errors without hiding other dialog boxes.
        private void SuppressScriptErrorsOnly(WebBrowser browser)
        {
            // Ensure that ScriptErrorsSuppressed is set to false.
            browser.ScriptErrorsSuppressed = false;

            // Handle DocumentCompleted to gain access to the Document object.
            browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);
        }

        private void Window_Error(object sender, HtmlElementErrorEventArgs e)
        {
            e.Handled = true;
        }
        #endregion Hide script error

        private void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            timerAfterDocCompleted.Enabled = true;
            
            ((WebBrowser)sender).Document.Window.Error += new HtmlElementErrorEventHandler(Window_Error);
        }


        private void timerAfterDocCompleted_Tick(object sender, EventArgs e)
        {
            Tools.CloseSecurityAlert();

            timerAfterDocCompleted.Enabled = false;
        }


        private void navigateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://blog.csdn.net/jiangjunshow/article/details/77711593");
        }

        private void submitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HtmlElementCollection collection = webBrowser1.Document.GetElementsByTagName("a");
            foreach(HtmlElement ele in collection)
            {
                if (ele.OuterHtml.Contains("https://passport.csdn.net/account/login"))
                {
                    Tools.SafeClick(ele);
                }
            }
        }
    }
}
