﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;

using System.Collections;

namespace experiment
{


    class CsdnBrowser : WebBrowser
    {
        const string m_head = @"
<p>分享一下我老师大神的人工智能教程。零基础！通俗易懂！风趣幽默！还带黄段子！希望你也加入到我们人工智能的队伍中来！<a href=""https://blog.csdn.net/jiangjunshow/article/details/77338485"">https://blog.csdn.net/jiangjunshow</a></p>";

        const string m_tail = @"
<p>分享一下我老师大神的人工智能教程。零基础！通俗易懂！风趣幽默！还带黄段子！希望你也加入到我们人工智能的队伍中来！<a href=""https://blog.csdn.net/jiangjunshow/article/details/77338485"">https://blog.csdn.net/jiangjunshow</a></p>";


        string m_articleContent = "";
        string m_articleTitle = "";

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

        public bool ClickEleByTagAndOuterHtml(string tag,string html)
        {
            // <a href="">帐号登录</a>
            HtmlElement ele = GetEleByTagAndOuterHtml(tag, html);
            if (ele == null)
                return false;
            SafeClick(ele);
            return true;
        }

        public bool isSuccess()
        {
            // <span class="comment">博主您好！您的博文非常棒！我们想与您进行商务合作。若有意合作，请加V：CaptainJackJJ。若有打扰，望博友们海涵！期待更多博主加入我们！</span>
            HtmlElement ele = GetEleByTagAndOuterHtml("span", "CaptainJackJJ");
            bool s = (ele != null && ele.Style == null);
            if (!s)
            {
                // is 
            }
            return s;
        }

        public bool isCommentSuccess()
        {
            // <span class="comment">博主您好！您的博文非常棒！我们想与您进行商务合作。若有意合作，请加V：CaptainJackJJ。若有打扰，望博友们海涵！期待更多博主加入我们！</span>
            HtmlElement ele = GetEleByTagAndOuterHtml("span", "CaptainJackJJ");
            bool s = (ele != null && ele.Style == null);
            if (!s)
            {
                return false;
            }
            return s;
        }

        public void EditComment(ref bool isNotAllowComment)
        {
            isNotAllowComment = false;
            //<textarea class="comment-content open" name="comment_content" id="comment_content" placeholder="想对作者说点什么"></textarea>
            HtmlElement ele = this.Document.GetElementById("comment_content");
            if(ele == null)
            {
                 Log.WriteLog(LogType.Error, "comment_content is not found");
                //<div class="unlogin-box text-center">博主设置当前文章不允许评论。
	            HtmlElement ele1 = GetEleByTagAndOuterHtml("div","博主设置当前文章不允许评论。");
                if (ele1 != null)
                {
                    Log.WriteLog(LogType.Error, "isNotAllowComment");
                    isNotAllowComment = true;
                    return;
                }
            }
            // This line makes title input success. 
            // Maybe bacuase this simulated human key press
            string str = "博主您好！您的博文非常棒！我们想与您进行商务合作。若有意合作，请加V：CaptainJackJJ。若有打扰，望博友们海涵！期待更多博主加入我们！大家为写博付出了精力，应当获得收入作为回报！";
            ele.Focus(); SendKeys.Send(" ");
            ele.InnerText = str;
            ele.SetAttribute("value", str);
        }

        public void commitCommnet()
        {
            //<input type="submit" class="btn btn-sm btn-red btn-comment" value="发表评论">
            if (!ClickEleByTagAndOuterHtml("input", "发表评论"))
            {
                Log.WriteLog(LogType.Error, "comment submit botton is not found");
            }
        }

        public void Edit(csdnCommenter.ArticleInfo articleInfo)
        {           
            HtmlElement ele = GetEleByTagAndOuterHtml("input", "article-bar__title");
            // This line makes title input success. 
            // Maybe bacuase this simulated human key press
            ele.Focus(); SendKeys.Send(" ");
            ele.InnerText = m_articleTitle;
            ele.SetAttribute("value", m_articleTitle);

            ele = GetEleByTagAndOuterHtml("pre", "editor__inner");
            //ele.InnerText = m_head + m_articleContent + m_tail;

            int s = m_articleContent.IndexOf("<svg");
            int e = m_articleContent.IndexOf("</svg");
            if(e > 0 && s > 0)
                m_articleContent = m_articleContent.Remove(s, e - s + 6);


            //int s = m_articleContent.IndexOf("-->");
            //int e = m_articleContent.IndexOf("<div class=\"htmledit_views");
            //if(e < 0)
            //{
            //    e = m_articleContent.IndexOf("<div class=\"markdown_views");
            //}
            //m_articleContent = m_articleContent.Remove(s+3, e - s-3);

            ele.InnerText = m_articleContent;
            //ele.FirstChild.InnerText = m_head + m_articleContent + m_tail; // this makes csdn default first text remain

            //if (!DataManagerSqlLite.bRandon)
            {
                SafeClick(GetEleByTagAndOuterHtml("button", "摘要"));
                // <textarea rows="7" maxlength="256" class="textfield" id="BYKAfYzlCEVs2ygo"></textarea>
                ele = GetEleByTagAndOuterHtml("textarea", "rows=\"7");
                ele.InnerText = m_articleTitle;
                //SafeClick(GetEleByTagAndOuterHtml("button", "保存摘要"));
                ele = GetEleByTagAndOuterHtml("button", "保存摘要");
                Point p = GetOffset(ele);
                Tools.DoubleClick(p.X + 3, p.Y + 1);
            }
        }

        public bool isMissContent()
        {
            HtmlElementCollection collection = this.Document.GetElementsByTagName("span");
            foreach (HtmlElement ele in collection)
            {
                //<span class="notice">文章标题不能为空</span>
                // <span class="notice">文章分类为必选项</span>
                //<span class="notice">博客分类为必选项</span>
                if (ele.OuterHtml.Contains("文章标题不能为空") || ele.OuterHtml.Contains("文章分类为必选项")
                    || ele.OuterHtml.Contains("博客分类为必选项") || ele.OuterHtml.Contains("请勿使用默认标题"))
                {
                    return true;
                }


            }
            return false;
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

        public bool isUnexpectError()
        {
            HtmlElementCollection collection = this.Document.GetElementsByTagName("i");
            foreach (HtmlElement ele in collection)
            {
                // <i class="mr8 notice-icon type-error"></i>
                if (ele.OuterHtml.Contains("type-error"))
                {
                    Log.WriteLog(LogType.Error, "occur unexpect error. error msg is " + ele.NextSibling.InnerText);
                    if (ele.NextSibling.InnerText.Contains("没有权限执行操作"))
                    {
                        Environment.Exit(0);
                    }
                    return true;
                }
            }
            return false;
        }

        public void PrePublish()
        {
            SafeClick(GetEleByTagAndOuterHtml("button", "发布文章"));

            HtmlElement ele = GetEleByTagAndOuterHtml("select", "原创");
            Point p = GetOffset(ele);
            Tools.DoubleClick(p.X, p.Y);
            Tools.Click(p.X, p.Y + 17);

            string lng = "编程语言";
            int lngPos = 200;
            if (dbCsdnCommenter.bRandon)
            {
                lng = "人工智能";
                lngPos = 20;
            }
            ele = GetEleByTagAndOuterHtml("select", lng);
            p = GetOffset(ele);
            Tools.DoubleClick(p.X, p.Y);
            Tools.Click(p.X, p.Y + lngPos);
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

        public void BeFans()
        {
            // <button class=" long-height hover-box btn-like " title="点赞">
            ClickEleByTagAndOuterHtml("button", "\"点赞\"");

            // <button class="btn-bookmark low-height hover-box" title="收藏">
            ClickEleByTagAndOuterHtml("button", "\"收藏\"");
        }

        public void Follow()
        {
            // <a class="btn btn-sm btn-red-hollow attention" id="btnAttent" target="_blank">关注</a>
            HtmlElement ele = this.Document.GetElementById("btnAttent");
            if (ele.OuterHtml.Contains(">关注<"))
                SafeClick(ele);
        }

        public csdnCommenter.ArticleInfo GoToEditPage()
        {
            csdnCommenter.ArticleInfo info = new csdnCommenter.ArticleInfo();
            HtmlElementCollection collection = null;

            // <span class="read-count">阅读数：884</span>
            collection = this.Document.GetElementsByTagName("span");
            if (dbCsdnCommenter.bRandon)
            {
                info.readCount = 0;
            }
            else
            {
                foreach (HtmlElement ele in collection)
                {
                    if (ele.OuterHtml.Contains("阅读数"))
                    {
                        int indexStart = ele.OuterHtml.IndexOf("阅读数：") + 4;
                        int indexEnd = ele.OuterHtml.LastIndexOf("</span>");
                        string count = ele.OuterHtml.Substring(indexStart, indexEnd - indexStart);
                        info.readCount = Convert.ToUInt64(count);
                        if (dbCsdnCommenter.bRandon)
                            break;
                        else
                        {
                            if (info.readCount < csdnCommenter.m_MinReadCount)
                                return info;
                            else
                                break;
                        }
                    }
                }
            }

            info.url = this.Document.Url.ToString();

            collection = this.Document.GetElementsByTagName("h1");
            foreach (HtmlElement ele in collection)
            {
                if (ele.OuterHtml.Contains("title-article"))
                {
                    if (ele.InnerText.Length > 60)
                    {
                        info.title = ele.InnerText.Substring(0, 60);
                    }
                    else
                        info.title = ele.InnerText;
                    info.title = Regex.Replace(info.title, "[ \\[ \\] \\^ _*×――(^)^$%~!@#$…&%￥=<>《》!！??？:：•`·、。；,.;\"‘’“”]", " ");
                    m_articleTitle = info.title;
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
                        Log.WriteLog(LogType.Notice, "article too large, cut end content. url is :"
                            + info.url + " , original len is " + ele.OuterHtml.Length);

                        m_articleContent = ele.OuterHtml.Substring(0, 100000);
                    }
                    else
                        m_articleContent = ele.OuterHtml;
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
            m_articleContent = "";
            isNetDealy = false;

            if (dbCsdnCommenter.bRandon)
            {
                ArrayList articles = new ArrayList(); 
                HtmlElementCollection collection = this.Document.GetElementsByTagName("a");
                foreach (HtmlElement ele in collection)
                {
                    // reach the list end.
                    if (articles.Count > 0 && !ele.OuterHtml.Contains("article/details"))
                    {
                        break;
                    }
                    if (ele.OuterHtml.Contains("article/details"))
                    {
                        if (ele.Parent.Parent.OuterHtml.Contains("display: none"))
                            continue; // this ele is hidden    
                        articles.Add(ele.OuterHtml);
                    }
                }

                if (articles.Count > 0)
                {
                    Random rnd = new Random();
                    int index = rnd.Next(articles.Count);
                    ClickArticleInList(articles[index].ToString());
                    return true;
                }
            }
            else
            {
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
            //<a href="javascript:void(0);" class="login-code__open js_login_trigger login-user__active">账号登录</a>
            bool isOk = ClickEleByTagAndOuterHtml("a", "账号登录");
            if (!isOk)
            {
                isOk = ClickEleByTagAndOuterHtml("a", "帐号登录");
            }
            return isOk;
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
            // <img class="login_img" src="//profile.csdnimg.cn/9/5/2/2_ugghhj">
            if (GetEleByTagAndOuterHtml("img", "login_img\" src=\"//profile") != null)
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
            if(ele == null)
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

            Log.WriteLog(LogType.Trace, "logged in with username " + uName);
            return true;
        }

        public void Logout()
        {
            //<a href="//passport.csdn.net/account/logout">退出</a>
            //ClickEleByTagAndOuterHtml("a", "退出");
            //SafeNavigate("//passport.csdn.net/account/logout");
            if (ClickEleByTagAndOuterHtml("a", "退出"))
                NavigateToLoginPage();
        }

        public bool MouseClickEle(string tag, string outerhtml)
        {
            HtmlElement ele = GetEleByTagAndOuterHtml(tag, outerhtml);
            if (ele == null)
                return false;
            Point p = GetOffset(ele);
            Tools.DoubleClick(p.X + 3, p.Y + 1);
            return true;
        }
    }
}
