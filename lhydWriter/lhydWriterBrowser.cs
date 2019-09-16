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
    class lhydWriterBrowser : WebBrowser
    {
        string m_articleContent = "";
        string m_articleTitle = "";
        string m_head = @"
<p>首先给大家分享一个巨牛巨牛的人工智能教程，是我无意中发现的。教程不仅零基础，通俗易懂，而且非常风趣幽默，还时不时有内涵段子，像看小说一样，叫做床长人工智能教程，哈哈～我正在学习中，觉得太牛了，所以分享给大家！<a href=""https://www.captainbed.net"">点这里可以跳转到教程</a></p>";

        string m_tail = @"
<p><a href=""https://www.captainbed.net"">点这里可以跳转到床长人工智能教程</a></p>";


        public lhydWriterBrowser()
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
            public WebBrowserSiteEx(lhydWriterBrowser webBrowser)
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


        public string LookForNewUrl(Db DbCheckedUrl, Db DbPostedUrl)
        {
            int indexStart, indexEnd;
            string url = "";
            
            // <a href="https://blog.csdn.net/fangqun663775/article/details/73614850" target="_blank" title="Java高级程序员（5年左右）面试的题目集 - F &amp; Q的专栏">	
            HtmlElementCollection collection = this.Document.GetElementsByTagName("a");
            foreach (HtmlElement ele in collection)
            {
                if (ele.OuterHtml.Contains("https://blog.csdn.net/") && ele.OuterHtml.Contains("article/details"))
                {
                    indexStart = ele.OuterHtml.IndexOf("https://blog.csdn.net/");
                    indexEnd = ele.OuterHtml.IndexOf("target=",indexStart);
                    if (indexEnd <= 0)
                        continue;
                    url = ele.OuterHtml.Substring(indexStart, indexEnd - indexStart - 2);
                    if (url.Length > 100)
                        return "";
                    if (!DbCheckedUrl.IsUrlExisted(url) && !DbPostedUrl.IsUrlExisted(url))
                    {
                        SafeNavigate(url);
                        return url;
                    }
                }
            }

            return "";
        }

        public bool CheckAndGetArticle()
        {
            HtmlElementCollection collection = null;

            // <span class="read-count">阅读数：884</span>
            collection = this.Document.GetElementsByTagName("span");

            foreach (HtmlElement ele in collection)
            {
                if (ele.OuterHtml.Contains("阅读数"))
                {
                    int indexStart = ele.OuterHtml.IndexOf("阅读数") + 4;
                    int indexEnd = ele.OuterHtml.LastIndexOf("</span>");
                    string count = ele.OuterHtml.Substring(indexStart, indexEnd - indexStart);
                    if(Convert.ToUInt64(count) < 10000)
                    {
                        return false;
                    }
                    else
                    {
                        break;
                    }
                }
            }            

            collection = this.Document.GetElementsByTagName("h1");
            foreach (HtmlElement ele in collection)
            {
                if (ele.OuterHtml.Contains("title-article"))
                {
                    if (ele.InnerText.Length > 60)
                    {
                        m_articleTitle = ele.InnerText.Substring(0, 60);
                    }
                    else
                        m_articleTitle = ele.InnerText;
                    m_articleTitle = Regex.Replace(m_articleTitle, "[ \\[ \\] \\^ _*×――(^)^$%~!@#$…&%￥=<>《》!！??？:：•`·、。；,.;\"‘’“”]", " ");
                    break;
                }
            }

            collection = this.Document.GetElementsByTagName("div");
            foreach (HtmlElement ele in collection)
            {
                if (!ele.OuterHtml.Contains("阅读数") && !ele.OuterHtml.Contains("rel=\"stylesheet\"") &&
                    (ele.OuterHtml.Contains("markdown_views") || ele.OuterHtml.Contains("htmledit_views")))
                {
                    if (ele.OuterHtml.Length > 100000)
                    {
                        Log.WriteLog(LogType.Notice, "article too large, skip it. url is :"
                            + this.Url.ToString() + " , original len is " + ele.OuterHtml.Length);

                        return false;
                    }
                    else
                        m_articleContent = ele.OuterHtml;
                    break;
                }
            }

            return true;
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

        public bool LoginLhyd()
        {
            string uName = "captainjackjj@outlook.com";
            string password = "Kila1987219";

            // <input type="text" name="log" id="user_login" class="input" value="" size="20" autocapitalize="off">
            HtmlElement ele = this.Document.GetElementById("user_login");
            if (ele == null)
            {
                return false;
            }
            ele.SetAttribute("value", uName);


            // <input type="password" name="pwd" id="user_pass" class="input" value="" size="20">
            ele = this.Document.GetElementById("user_pass");
            if (ele == null)
            {
                return false;
            }
            ele.SetAttribute("value", password);

            // <input type="submit" name="wp-submit" id="wp-submit" class="button button-primary button-large" value="登录">
            ele = this.Document.GetElementById("wp-submit");
            if (ele == null)
            {
                return false;
            }
            SafeClick(ele);
            return true;
        }
        public bool EditTitle()
        {
            // <textarea id="post-title-0" class="editor-post-title__input" placeholder="添加标题" rows="1" style="overflow: hidden; overflow-wrap: break-word; resize: none; height: 95px;"></textarea>
            HtmlElement ele = this.Document.GetElementById("post-title-0");
            if (ele == null)
            {
                return false;
            }

            Tools.Click(500, 200);
            Clipboard.SetDataObject(m_articleTitle);
            Tools.ctrlV(); 
            
            return true;
        }

        public void EditContent()
        {   
            Tools.Click(500, 700);

            Clipboard.SetDataObject(m_head + m_articleContent + m_tail);
            m_articleContent = "";
            Tools.ctrlV();           
        }

        public bool Publish()
        {
            // <button type="button" aria-disabled="false" class="components-button editor-post-publish-button is-button is-default is-primary is-large">发布</button>
            HtmlElement ele = GetEleByTagAndOuterHtml("button", "发布");
            if (ele == null)
            {
                return false;
            }
            SafeClick(ele);
            return true;
        }

        public bool IsPublishing()
        {
            // <button type="button" aria-disabled="true" class="components-button editor-post-publish-button is-button is-default is-primary is-large is-busy">正在发布…</button>
            HtmlElement ele = GetEleByTagAndOuterHtml("button", "正在发布…");
            if (ele == null)
            {
                return false;
            }
            return true;
        }

        public void CheckObjThenGoToFirstArticle(bool isNeedCheck, int minReadCount, UInt16 minArticleCount,
            ref bool isNeedCollect, ref bool isNetDealy, ref Int64 totalReadCount, ref int maxReadCount, 
            ref int OriginalArticleNum, ref int FansNum, ref int LikeNum, ref int CommentsNum
            , ref int Degree, ref int Score, ref int Ranking, ref bool isExpert)
        {
            isNetDealy = isNeedCollect = false;

            short timesFoundReadCount = 0;
            string outerHtmlFirstArticle = "";

            int indexStart,indexEnd;
            int readCount;

            // <div class="error_text">404
            // <div class="text">页面找不到了
            HtmlElement element1 = GetEleByTagAndOuterHtml("div", "页面找不到了");
            if (element1 != null)
            { return ; }

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
                    readCount = Convert.ToInt32(str);
                    if (readCount < minReadCount)
                        break;

                    if (timesFoundReadCount == minArticleCount)
                    {
                        if (maxReadCount <= 0)
                            maxReadCount = readCount;
                        isNeedCollect = true;
                        break;
                    }
                }
            }

            if (timesFoundReadCount == 0)
            {
                // <h6>空空如也</h6>
                HtmlElement element = GetEleByTagAndOuterHtml("h6", "空空如也");
                if (element == null)
                { isNetDealy = true; }
                else
                { isNetDealy = false; }
                
            }
            else
            {
                totalReadCount = GetTotalReadCount();
                if (totalReadCount < 500000)
                {
                    isNeedCollect = false;
                }
                else
                {
                    OriginalArticleNum = GetOriginalArticleNum();
                    FansNum = GetFansNum();
                    LikeNum = GetLikeNum();
                    CommentsNum = GetCommentsNum();
                    Degree = GetDegree();
                    Score = GetScore();
                    Ranking = GetRanking();
                    isExpert = IsExpert();
                }

                ClickArticleInList(outerHtmlFirstArticle);
            }
        }

        bool IsExpert()
        {
            //<p class="flag expert">
            //    <svg class="icon" aria-hidden="true">
            //      <use xlink:href="#csdnc-blogexpert"></use>
            //    </svg>
            //    博客专家
            // </p>
            HtmlElement element = GetEleByTagAndOuterHtml("p", "博客专家");
            if(element == null)
            { return false; }
            else
            { return true; }
        }

        int GetRanking()
        {
            // <dl title="173">
            //      <dt>排名：</dt>
            //  <dd>173</dd>
            // </dl>     
            HtmlElement element = GetEleByTagAndOuterHtml("dt", "<dt>排名：</dt>");
            string html = element.Parent.OuterHtml;
            int start = html.IndexOf("\"");
            int end = html.IndexOf("\"", start + 1);
            string str = html.Substring(start + 1, end - start - 1);
            return Convert.ToInt32(str);
        }

        int GetScore()
        {
            // <dl class="text-center" title="5080">
            //      <dt>积分：</dt>
            //      <dd><span class="count">5080</span></dd>
            // </dl>       
            HtmlElement element = GetEleByTagAndOuterHtml("dt", "<dt>积分：</dt>");
            // <dl> 
            //  <dt>积分：</dt>
            //  <dd title="38924">
            string html = element.Parent.OuterHtml;
            int start = html.IndexOf("\"");
            int end = html.IndexOf("\"", start + 1);
            string str = html.Substring(start + 1, end - start - 1);
            return Convert.ToInt32(str);
        }

        int GetDegree()
        {
            //<a href="https://blog.csdn.net/home/help.html#level" title="8级,点击查看等级说明" target="_blank">      
            HtmlElement element = GetEleByTagAndOuterHtml("a", "点击查看等级说明");
            string html = element.OuterHtml;
            int index = html.IndexOf("级");
            string str = html.Substring(index-1, 1);
            return Convert.ToInt32(str);
        }

        int GetCommentsNum()
        {
            // <dl class="text-center" title="5080">
            //      <dt>评论</dt>
            //      <dd><span class="count">5080</span></dd>
            // </dl>       
            HtmlElement element = GetEleByTagAndOuterHtml("dt", "<dt>评论</dt>");
            // <dl> 
            //  <dt>评论</dt>
            //  <dd title="38924">
            string html = element.Parent.OuterHtml;
            int start = html.IndexOf("\"");
            int end = html.IndexOf("\"", start + 1);
            string str = html.Substring(start + 1, end - start - 1);
            return Convert.ToInt32(str);
        }

        int GetLikeNum()
        {
            // <dl class="text-center" title="835">
            //      <dt>喜欢</dt>
            //      <dd><span class="count">835</span></dd>
            // </dl>           
            HtmlElement element = GetEleByTagAndOuterHtml("dt", "<dt>喜欢</dt>");
            // "喜欢835"
            string html = element.Parent.OuterHtml;
            int start = html.IndexOf("\"");
            int end = html.IndexOf("\"", start + 1);
            string str = html.Substring(start + 1, end - start - 1);
            return Convert.ToInt32(str);
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
            if (element == null)
                return 0;
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
