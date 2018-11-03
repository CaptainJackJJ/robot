﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Drawing;

namespace experiment
{


    class CsdnBrowser : WebBrowser
    {
        int m_articleTypeOffset;
        int m_articleFieldOffset;

        public CsdnBrowser()
        {
            this.ScriptErrorsSuppressed = false;
            this.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);

            DataManagerSqlLite dm = new DataManagerSqlLite("parameters.db");
            dm.GetParams(ref m_articleTypeOffset, ref m_articleFieldOffset);
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

        private bool ClickEleByTagAndOuterHtml(string tag,string html)
        {
            HtmlElement ele = GetEleByTagAndOuterHtml(tag, html);
            if (ele == null)
                return false;
            SafeClick(ele);
            return true;
        }

        public void Edit(BlogRobot.ArticleInfo articleInfo)
        {
            articleInfo.title = articleInfo.title.Replace(@"\", "斜杠");

            HtmlElement ele = GetEleByTagAndOuterHtml("input", "article-bar__title");
            // This line makes title input success. 
            // Maybe bacuase this simulated human key press
            ele.Focus(); SendKeys.Send(" "); 
            ele.SetAttribute("value", articleInfo.title);

            ele = GetEleByTagAndOuterHtml("pre", "editor__inner");

            string head = @"<p><strong>分享一下我老师大神的人工智能教程！零基础，通俗易懂！<a href=""https://blog.csdn.net/jiangjunshow/article/details/77338485"">http://blog.csdn.net/jiangjunshow</a></strong></p>
<p></p>
<p><strong>也欢迎大家转载本篇文章。分享知识，造福人民，实现我们中华民族伟大复兴！</strong></p>
<p></p>
";
            string tail = @"<p></p>
<strong><h4>给我老师的人工智能教程打call！<a href=""https://blog.csdn.net/jiangjunshow/article/details/77338485"">http://blog.csdn.net/jiangjunshow</a></h4></strong>
<div align=""center""><img src=""https://img-blog.csdn.net/20161220210733446?watermark/2/text/aHR0cDovL2Jsb2cuY3Nkbi5uZXQvc3VuaHVhcWlhbmcx/font/5a6L5L2T/fontsize/400/fill/I0JBQkFCMA==/dissolve/70/gravity/SouthEast"" alt=""这里写图片描述"" title=""""></div>
";

            if (articleInfo.content.Length > 300000)
            {
                Log.WriteLog(LogType.Notice, "article too large, cut end content. url is :"
                    + articleInfo.url + " , original len is " + articleInfo.content.Length);

                articleInfo.content = articleInfo.content.Substring(0, 300000);
            }
            articleInfo.content = head + articleInfo.content + tail;
            ele.FirstChild.InnerText = articleInfo.content;
            articleInfo.content = "";

            SafeClick(GetEleByTagAndOuterHtml("button", "发布文章"));

            ele = GetEleByTagAndOuterHtml("select", "原创");
            Point p = GetOffset(ele);
            Tools.DoubleClick(p.X, p.Y);
            Tools.Click(p.X, p.Y + m_articleTypeOffset);

            ele = GetEleByTagAndOuterHtml("select", "编程语言");
            p = GetOffset(ele);
            Tools.DoubleClick(p.X, p.Y);
            Tools.Click(p.X, p.Y + m_articleFieldOffset);
        }

        public bool isPublishedMax()
        {
            HtmlElementCollection collection = this.Document.GetElementsByTagName("span");
            foreach (HtmlElement ele in collection)
            {
                if (ele.OuterHtml.Contains("今天发表文章数量已达到限制的 10 篇"))
                {
                    return true;
                }
            }
            return false;
        }

        public bool isSuccess()
        {
            HtmlElement ele = this.Document.GetElementById("alertSuccess");
            return (ele != null && !ele.OuterHtml.Contains("display: none"));
        }

        public void Publish()
        {
#if DEBUG
            HtmlElement ele = GetEleByTagAndOuterHtml("button", "保存为草稿");
#else
            HtmlElement ele = GetEleByTagAndOuterHtml("button", "button btn-c-blue\">发布文章");
#endif
            SafeClick(ele);
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

        public BlogRobot.ArticleInfo GoToEditPage()
        {
            BlogRobot.ArticleInfo info = new BlogRobot.ArticleInfo();

            // <span class="read-count">阅读数：884</span>
            HtmlElementCollection collection = this.Document.GetElementsByTagName("span");
            foreach (HtmlElement ele in collection)
            {
                if (ele.OuterHtml.Contains("阅读数"))
                {
                    int indexStart = ele.OuterHtml.IndexOf("阅读数：") + 4;
                    int indexEnd = ele.OuterHtml.LastIndexOf("</span>");
                    string count = ele.OuterHtml.Substring(indexStart, indexEnd - indexStart);
                    info.readCount = Convert.ToUInt64(count);
                    if (info.readCount < BlogRobot.m_MinReadCount)
                        return info;
                    else
                        break;
                }
            }

            info.url = this.Document.Url.ToString();

            collection = this.Document.GetElementsByTagName("h1");
            foreach (HtmlElement ele in collection)
            {
                if (ele.OuterHtml.Contains("title-article"))
                {
                    info.title = ele.InnerText;
                    break;
                }
            }

            collection = this.Document.GetElementsByTagName("div");
            foreach (HtmlElement ele in collection)
            {
                if (!ele.OuterHtml.Contains("阅读数") && !ele.OuterHtml.Contains("rel=\"stylesheet\"") &&
                    (ele.OuterHtml.Contains("markdown_views") || ele.OuterHtml.Contains("htmledit_views")))
                {
                    info.content = ele.OuterHtml;
                    break;
                }
            }

            SafeNavigate("https://mp.csdn.net/mdeditor");

            return info;
        }

        public bool GoToNextPage()
        {
            //<li class="js-page-next js-page-action ui-pager">下一页</li>
            //<li class="js-page-next js-page-action ui-pager ui-pager-disabled">下一页</li>
            HtmlElement ele = GetEleByTagAndOuterHtml("li", "下一页");
            if (ele == null || ele.OuterHtml.Contains("disabled"))
                return false;
            SafeClick(ele);
            return true;
        }

        // return false means no next article anymore.
        public bool GoToArticlePage(string lastArticleUrl, ref bool isNetDealy)
        {
            isNetDealy = false;

            short timesOfFindLastArticle = 0;
            string lastArticleId = "";
            if (!String.IsNullOrEmpty(lastArticleUrl))
            {
                lastArticleId = lastArticleUrl.Substring(lastArticleUrl.LastIndexOf("/") + 1);
                // add "target" string to avoid next article's id is include in front article's content.

                // This link of article is not same as the link of artcile which is in list, but the ID is same
                /* https://blog.csdn.net/laoyang360/article/details/52244917 */
                // <a href="https://blog.csdn.net/rlhua/article/details/16961497" target="_blank">
            }

            HtmlElementCollection collection = this.Document.GetElementsByTagName("a");
            foreach (HtmlElement ele in collection)
            {
                // reach the list end.
                if (timesOfFindLastArticle == 2 && !ele.OuterHtml.Contains("article/details"))
                {
                    return false;
                }
                if (ele.OuterHtml.Contains("article/details"))
                {
                    if (ele.Parent.Parent.OuterHtml.Contains("display: none"))
                        continue; // this ele is hidden                    

                    if (lastArticleId == "" || timesOfFindLastArticle == 2) // Use first article || every article has two link
                    {
                        ClickArticleInList(ele.OuterHtml);
                        return true;
                    }

                    /* <a href="https://blog.csdn.net/rlhua/article/details/16961497" target="_blank">
                        Oracle OCP 11G &nbsp;052答案解析目录(V8.02&amp;V9.02)V8.02
                        1：http://blog.csdn.net/rlhua/article/details/12624275
                        2：http://blog.csdn.net/rlhua/articl...      </a>
                    */
                    // use substring to avoid OuterHtml contarin's other article's link
                    if (ele.OuterHtml.Substring(0, ele.OuterHtml.IndexOf(">")).Contains(lastArticleId))
                        timesOfFindLastArticle++;
                }
            }

            isNetDealy = true;
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

        private bool ClickAccountLogin()
        {
            return ClickEleByTagAndOuterHtml("a", "账号登录");
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
            if (!ClickAccountLogin())
            {
                Log.WriteLog(LogType.NetworkWarning, "ClickAccountLogin failed");
                return false;
            }

            HtmlElement ele = this.Document.GetElementById("username");
            ele.SetAttribute("value", uName);

            ele = this.Document.GetElementById("password");
            ele.SetAttribute("value", password);

            ClickEleByTagAndOuterHtml("input", "登 录");
            Log.WriteLog(LogType.Trace, "logged in with username " + uName);
            return true;
        }

        public void Logout()
        {
            ClickEleByTagAndOuterHtml("a", "退出");
            NavigateToLoginPage();
        }
    }
}
