﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;

namespace WorkObjCollector
{
    class CollectorBrowser : WebBrowser
    {
        public CollectorBrowser()
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
            public WebBrowserSiteEx(CollectorBrowser webBrowser)
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
            SafeNavigate("http://mail.sina.com.cn/?from=mail#");  
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

        private bool ClickEleByTagAndOuterHtml(string tag,string html)
        {
            HtmlElement ele = GetEleByTagAndOuterHtml(tag, html);
            if (ele == null)
                return false;
            SafeClick(ele);
            return true;
        }

        private Point GetOffset(HtmlElement el)
        {
            //get element pos
            Point pos = new Point(el.OffsetRectangle.Left, el.OffsetRectangle.Top);

            //get the parents pos
            HtmlElement tempEl = el.OffsetParent;
            while (tempEl != null)
            {
                pos.X += tempEl.OffsetRectangle.Left;
                pos.Y += tempEl.OffsetRectangle.Top;
                tempEl = tempEl.OffsetParent;
            }

            pos.X += (el.OffsetRectangle.Width / 2);
            pos.Y += (el.OffsetRectangle.Height / 2);

            pos = this.PointToScreen(pos);
            return pos;
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

        private void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Tools.CloseSecurityAlert();
        }

        public void CloseSecurityAlert()
        {
            Tools.CloseSecurityAlert();
        }


        public string LookForNewObj(ObjDb checkedObjDb)
        {
            int indexStart, indexEnd;
            string objUrl = "";
            
            // <a href="https://blog.csdn.net/fangqun663775/article/details/73614850" target="_blank" title="Java高级程序员（5年左右）面试的题目集 - F &amp; Q的专栏">	
            HtmlElementCollection collection = this.Document.GetElementsByTagName("a");
            foreach (HtmlElement ele in collection)
            {
                if (ele.OuterHtml.Contains("https://blog.csdn.net/"))
                {
                    indexStart = ele.OuterHtml.IndexOf("https:");
                    indexEnd = ele.OuterHtml.IndexOf("/article/details");
                    if (indexEnd <= 0)
                        continue;
                    objUrl = ele.OuterHtml.Substring(indexStart, indexEnd - indexStart);
                    if (!checkedObjDb.IsObjectChecked(objUrl))
                        return objUrl;
                }
            }

            return "";
        }

        private bool ClickAccountLogin()
        {
            //<a href="javascript:void(0);" class="login-code__open js_login_trigger login-user__active">账号登录</a>
            bool isOk = ClickEleByTagAndOuterHtml("a", "账号登录");
            if (!isOk)
            {
                // <a href="">帐号登录</a>
                isOk = ClickEleByTagAndOuterHtml("a", "帐号登录");
            }
            return isOk;
        }

        public bool Login(string uName, string password)
        {
            if (!ClickAccountLogin())
            {
                Log.WriteLog(LogType.NetworkWarning, "ClickAccountLogin failed");
                return false;
            }

            HtmlElement ele = this.Document.GetElementById("username");
            if (ele == null)
            {
                ele = this.Document.GetElementById("all");
                ele.Focus(); SendKeys.Send(" ");
            }
            ele.SetAttribute("value", uName);


            // <input type="password" placeholder="密码" id="password-number" autocomplete="false" class="form-control form-control-icon">
            ele = this.Document.GetElementById("password-number");
            if (ele != null)
            {
                ele.Focus(); SendKeys.Send(" ");
            }
            else
            {
                ele = this.Document.GetElementById("password");
            }
            ele.SetAttribute("value", password);

            // <input class="logging" accesskey="l" value="登 录" tabindex="6" type="button">
            if (!ClickEleByTagAndOuterHtml("input", "登 录"))
            {
                //<button data-type="account" class="btn btn-primary">登录</button>
                ClickEleByTagAndOuterHtml("button", "登录");
            }
            Log.WriteLog(LogType.Trace, "logged in with username " + uName);
            return true;
        }
        

        public void CheckObjThenGoToFirstArticle(bool isNeedCheck, UInt64 minReadCount, UInt16 minArticleCount,
            ref bool isNeedCollect, ref bool isNetDealy)
        {
            isNetDealy = isNeedCollect = false;

            short timesFoundReadCount = 0;
            string outerHtmlFirstArticle = "";

            int indexStart,indexEnd;
            UInt64 readCount;

            // <span class="read-num">阅读数：139843</span>
            HtmlElementCollection collection = this.Document.GetElementsByTagName("span");
            foreach (HtmlElement ele in collection)
            {
                if (ele.OuterHtml.Contains("阅读数"))
                {
                    if (ele.Parent.Parent.Parent.OuterHtml.Contains("display: none"))
                        continue; // this ele is hidden                    

                    timesFoundReadCount++;

                    if (timesFoundReadCount == 1) // save first article outerHtml
                    {
                        outerHtmlFirstArticle = ele.Parent.Parent.Parent.OuterHtml;
                        if (!isNeedCheck)
                            break;
                    }

                    // <span class="read-num">阅读数 <span class="num">571586</span> </span>
                    indexStart = ele.OuterHtml.IndexOf("\"num\">") + 6;
                    indexEnd = ele.OuterHtml.LastIndexOf("</span>");
                    string str = ele.OuterHtml.Substring(indexStart, indexEnd - indexStart - 8);
                    readCount = Convert.ToUInt64(str);
                    if (readCount < minReadCount)
                        break;

                    if (timesFoundReadCount == minArticleCount)
                    {
                        isNeedCollect = true;
                        break;
                    }
                }
            }

            if (timesFoundReadCount == 0)
                isNetDealy = true;
            else
            {
                Int64 totalReadCount = GetTotalReadCount();
                if (totalReadCount < 500000)
                {
                    isNeedCollect = false;
                }
                else
                {
                    int OriginalArticleNum = GetOriginalArticleNum();
                    int FansNum = GetFansNum();
                }

                ClickArticleInList(outerHtmlFirstArticle);
            }
        }

        int GetFansNum()
        {
            // <dl title="21161" class="text-center" id="fanBox">            
            HtmlElement element = this.Document.GetElementById("fanBox");
            string html = element.OuterHtml;
            int start = html.IndexOf("\"");
            int end = html.IndexOf("\"", start + 1);
            string str = html.Substring(start + 1, end - start - 1);
            return Convert.ToInt32(str);
        }

        int GetOriginalArticleNum()
        {
            //<a href="https://blog.csdn.net/morewindows?t=1"><span class="count">156</span></a>
            HtmlElement element = GetEleByTagAndOuterHtml("a", "t=1\"><span");
            return Convert.ToInt32(element.InnerText);
        }

        Int64 GetTotalReadCount()
        {
            //<dl>
            //  <dt>访问：</dt>
            //  <dd title="8285617">
            HtmlElement element = GetEleByTagAndOuterHtml("dt", "访问：");
            string html = element.Parent.Children[1].OuterHtml;
            int start = html.IndexOf("\"");
            int end = html.LastIndexOf("\"");
            string str = html.Substring(start + 1, end - start - 1);
            return Convert.ToInt64(str);
        }

        private void ClickArticleInList(string ArticleOuterHtml)
        {
            int startIndex = ArticleOuterHtml.IndexOf("https://blog.csdn.net/");
            int endIndex = ArticleOuterHtml.IndexOf("target") - 2;
            string ArticleUrl = ArticleOuterHtml.Substring(startIndex, endIndex - startIndex);
            /* outerhtml
            <a href="https://blog.csdn.net/wojiushiwo987/article/details/52244917" target="_blank"><span class="article-type type-1">原        </span>Elasticsearch学习，请先看这一篇！      </a>
             */
            SafeNavigate(ArticleUrl);// Do not use ele click, beacuse click will jump to other browser.
        }
    }
}
