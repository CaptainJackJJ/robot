﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace experiment
{
    class CsdnBrowser : WebBrowser
    {
        public CsdnBrowser()
        {
            this.ScriptErrorsSuppressed = false;
            this.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);
        }

        // Override to allow custom script error handling.
        protected override WebBrowserSiteBase CreateWebBrowserSiteBase()
        {
            return new WebBrowserSiteEx(this);
        }

        #region Inner Class [WebBrowserSiteEx]

        //Sub-class to allow custom script error handling.
        protected class WebBrowserSiteEx : WebBrowserSite, NativeMethods.IOleCommandTarget
        {
            /// <summary>
            /// Default constructor.
            /// </summary>
            public WebBrowserSiteEx(CsdnBrowser webBrowser)
                : base(webBrowser)
            {
            }

            /// <summary>Queries the object for the status of one or more commands generated by user interface events.</summary>
            /// <param name="pguidCmdGroup">The GUID of the command group.</param>
            /// <param name="cCmds">The number of commands in <paramref name="prgCmds" />.</param>
            /// <param name="prgCmds">An array of OLECMD structures that indicate the commands for which the caller needs status information. This method fills the <paramref name="cmdf" /> member of each structure with values taken from the OLECMDF enumeration.</param>
            /// <param name="pCmdText">An OLECMDTEXT structure in which to return name and/or status information of a single command. This parameter can be null to indicate that the caller does not need this information.</param>
            /// <returns>This method returns S_OK on success. Other possible return values include the following.
            /// E_FAIL The operation failed.
            /// E_UNEXPECTED An unexpected error has occurred.
            /// E_POINTER The <paramref name="prgCmds" /> argument is null.
            /// OLECMDERR_E_UNKNOWNGROUP The <paramref name="pguidCmdGroup" /> parameter is not null but does not specify a recognized command group.</returns>
            public int QueryStatus(ref Guid pguidCmdGroup, int cCmds, NativeMethods.OLECMD prgCmds, IntPtr pCmdText)
            {
                if ((int)NativeMethods.OLECMDID.OLECMDID_SHOWSCRIPTERROR == prgCmds.cmdID)
                {   // Do nothing (suppress script errors)
                    return NativeMethods.S_OK;
                }

                // Indicate that command is unknown. The command will then be handled by another IOleCommandTarget.
                return NativeMethods.OLECMDERR_E_UNKNOWNGROUP;
            }

            /// <summary>Executes the specified command.</summary>
            /// <param name="pguidCmdGroup">The GUID of the command group.</param>
            /// <param name="nCmdID">The command ID.</param>
            /// <param name="nCmdexecopt">Specifies how the object should execute the command. Possible values are taken from the <see cref="T:Microsoft.VisualStudio.OLE.Interop.OLECMDEXECOPT" /> and <see cref="T:Microsoft.VisualStudio.OLE.Interop.OLECMDID_WINDOWSTATE_FLAG" /> enumerations.</param>
            /// <param name="pvaIn">The input arguments of the command.</param>
            /// <param name="pvaOut">The output arguments of the command.</param>
            /// <returns>This method returns S_OK on success. Other possible return values include 
            /// OLECMDERR_E_UNKNOWNGROUP The <paramref name="pguidCmdGroup" /> parameter is not null but does not specify a recognized command group.
            /// OLECMDERR_E_NOTSUPPORTED The <paramref name="nCmdID" /> parameter is not a valid command in the group identified by <paramref name="pguidCmdGroup" />.
            /// OLECMDERR_E_DISABLED The command identified by <paramref name="nCmdID" /> is currently disabled and cannot be executed.
            /// OLECMDERR_E_NOHELP The caller has asked for help on the command identified by <paramref name="nCmdID" />, but no help is available.
            /// OLECMDERR_E_CANCELED The user canceled the execution of the command.</returns>
            public int Exec(ref Guid pguidCmdGroup, int nCmdID, int nCmdexecopt, object[] pvaIn, int pvaOut)
            {
                if ((int)NativeMethods.OLECMDID.OLECMDID_SHOWSCRIPTERROR == nCmdID)
                {   // Do nothing (suppress script errors)
                    return NativeMethods.S_OK;
                }

                // Indicate that command is unknown. The command will then be handled by another IOleCommandTarget.
                return NativeMethods.OLECMDERR_E_UNKNOWNGROUP;
            }
        }

        #endregion Inner Class [WebBrowserSiteEx]


        public void NavigateToLoginPage()
        {
            SafeNavigate("https://passport.csdn.net/account/login");  
        }

        private HtmlElement GetEleByTagAndOuterHtml(string tag,string html)
        {
            HtmlElementCollection collection = this.Document.GetElementsByTagName(tag);
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
            SafeClick(GetEleByTagAndOuterHtml(tag, html)); 
        }

        public BlogRobot.ArticleInfo GoToEditPage()
        {
            BlogRobot.ArticleInfo info = new BlogRobot.ArticleInfo();

            info.url = this.Document.Url.ToString();

            HtmlElementCollection collection = this.Document.GetElementsByTagName("h1");
            foreach (HtmlElement ele in collection)
            {
                string outerHtml = ele.OuterHtml;
                if (outerHtml.Contains("title-article"))
                {
                    info.title = ele.InnerText;
                }
            }

            collection = this.Document.GetElementsByTagName("div");
            foreach (HtmlElement ele in collection)
            {
                string outerHtml = ele.OuterHtml;
                if (outerHtml.Contains("markdown_views") || outerHtml.Contains("htmledit_views"))
                {
                    info.content = ele.OuterHtml;
                }
            }

            SafeNavigate("https://mp.csdn.net/mdeditor");

            return info;
        }

        // return false means no next article anymore.
        public bool NavToArticlePage(string lastArticleUrl)
        {
            short timesOfFindLastArticle = 0;
            string lastArticleId = "";
            if (!String.IsNullOrEmpty(lastArticleUrl))
            {
                lastArticleId = lastArticleUrl.Substring(lastArticleUrl.LastIndexOf("/") + 1);
                // This link of article is not same as the link of artcile which is in list, but the ID is same
                /* https://blog.csdn.net/laoyang360/article/details/52244917 */
            }

            HtmlElementCollection collection = this.Document.GetElementsByTagName("a");
            foreach (HtmlElement ele in collection)
            {
                string outerHtml = ele.OuterHtml;
                if (outerHtml.Contains("article/details"))
                {
                    if (ele.Parent.Parent.OuterHtml.Contains("display: none"))
                        continue; // this ele is hidden                    

                    if (lastArticleId == "" || timesOfFindLastArticle == 2) // Use first article || every article has two link
                    {
                        ClickArticleInList(outerHtml);
                        return true;
                    }
                    if (outerHtml.Contains(lastArticleId))
                        timesOfFindLastArticle++;
                }
            }

            return false;
        }

        private void ClickArticleInList(string ArticleOuterHtml)
        {
            int startIndex = ArticleOuterHtml.IndexOf("http");
            int endIndex = ArticleOuterHtml.IndexOf("target") - 2;
            string ArticleUrl = ArticleOuterHtml.Substring(startIndex, endIndex - startIndex);
            /* outerhtml
            <a href="https://blog.csdn.net/wojiushiwo987/article/details/52244917" target="_blank"><span class="article-type type-1">原        </span>Elasticsearch学习，请先看这一篇！      </a>
             */

            SafeNavigate(ArticleUrl);// Do not use ele click, beacuse click will jump to other browser.
        }

        private void SafeClick(HtmlElement ele)
        {
            Tools.CloseSecurityAlert();
            ele.InvokeMember("click"); 
        }

        public void SafeNavigate(string url)
        {
            Tools.CloseSecurityAlert();
            this.Navigate(url); 
        }

        private void ClickAccountLogin()
        {
            ClickEleByTagAndOuterHtml("a", "账号登录");
        }

        public void ClickLogin()
        {
            ClickEleByTagAndOuterHtml("a", "https://passport.csdn.net/account/login");
        }

        private void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Tools.CloseSecurityAlert();
        }

        public void CloseSecurityAlert()
        {
            Tools.CloseSecurityAlert();
        }

        public bool IsInEditPage()
        {
            if (GetEleByTagAndOuterHtml("pre", "editor__inner") != null)
                return true;
            return false;
        }

        public bool IsLogedin()
        {
            if (GetEleByTagAndOuterHtml("img", "avatar.csdn.net") != null)
                return true;
            return false;
        }

        public bool Login(string uName,string password)
        {
            ClickAccountLogin();

            HtmlElement ele = this.Document.GetElementById("username");
            if (ele == null)
            {
                Log.WriteLog(LogType.Error, "did not found ele username");
                return false;
            }
            ele.SetAttribute("value", uName);

            ele = this.Document.GetElementById("password");
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
