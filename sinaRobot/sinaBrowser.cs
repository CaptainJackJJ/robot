﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;

namespace experiment
{


    class sinaBrowser : WebBrowser
    {
        const string m_head = @"";
        const string m_tail = @"
<p>再分享一下我老师大神的人工智能教程吧。零基础！通俗易懂！风趣幽默！还带黄段子！希望你也加入到我们人工智能的队伍中来！<a href=""https://blog.csdn.net/jiangjunshow/article/details/77338485"">https://blog.csdn.net/jiangjunshow</a></p>";

        string m_articleContent = "";
        string m_articleTitle = "";

        public sinaBrowser()
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
            public WebBrowserSiteEx(sinaBrowser webBrowser)
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
            SafeNavigate("https://passport.cnblogs.com/user/signin");  
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

        public void EditTitle(sinaRobot.ArticleInfo articleInfo)
        {
            // <input id="articleTitle" name="blog_title" type="text" size="96" class="Fm_input1" value="">
            HtmlElement ele = this.Document.GetElementById("articleTitle"); 
            // This line makes title input success. 
            // Maybe bacuase this simulated human key press
            ele.Focus(); SendKeys.Send("");
            ele.InnerText = m_articleTitle;
            ele.SetAttribute("value", m_articleTitle);

            // <label for="SinaEditor_59_viewcodecheckbox">显示源代码</label>
            Tools.DoubleClick(266, 654);
        }

        public void EditHtml()
        {
            // <iframe id="mce_61_ifr" src="https://common.cnblogs.com/editor/tiny_mce/themes/advanced/source_editor.htm?mce_rdomain=cnblogs.com&amp;id=20170728" frameborder="0" style="border: 0px; width: 720px; height: 580px;"></iframe>
            HtmlElement ele = GetEleByTagAndOuterHtml("iframe", "source_editor");
            HtmlDocument htmlDocDlg = this.Document.Window.Frames[ele.Id.ToString()].Document;

            // <textarea name="htmlSource" id="htmlSource" rows="15" cols="100" style="width: 700px; height: 515px; font-family: &quot;Courier New&quot;, Courier, monospace; font-size: 12px; white-space: pre-wrap;" dir="ltr" wrap="off" class="mceFocus"></textarea>
            ele = htmlDocDlg.GetElementById("htmlSource");
            ele.InnerText = m_articleContent + m_tail;
            ele.SetAttribute("value", ele.InnerText);

            // <input type="submit" role="button" name="insert" value="更新" id="insert">
            ele = htmlDocDlg.GetElementById("insert");
            SafeClick(ele);
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

        public bool isSuccess()
        {
            // <div>发布成功</div>
            HtmlElement ele = GetEleByTagAndOuterHtml("div", "成功");
            bool s = (ele != null && ele.Style == null);
            if(s)
            {
                m_articleContent = "";
            }
            return s;
        }

        public bool isUnexpectError()
        {            
            HtmlElementCollection collection = this.Document.GetElementsByTagName("div");
            foreach (HtmlElement ele in collection)
            {
                // <div>博文内容中不允许有包含js代码!<script type="text/javascript">g_blnCheckUnload=true;</script>
                if (ele.OuterHtml.Contains("博文内容中不允许"))
                {
                    Log.WriteLog(LogType.Error, "occur unexpect error. error msg is " + ele.InnerText);
                    return true;
                }
            }
            return false;
        }

        public void PrePublish()
        {
        //    SafeClick(GetEleByTagAndOuterHtml("button", "发布文章"));

        //    HtmlElement ele = GetEleByTagAndOuterHtml("select", "原创");
        //    Point p = GetOffset(ele);
        //    Tools.DoubleClick(p.X, p.Y);
        //    Tools.Click(p.X, p.Y + m_articleTypeOffset);

        //    ele = GetEleByTagAndOuterHtml("select", "编程语言");
        //    p = GetOffset(ele);
        //    Tools.DoubleClick(p.X, p.Y);
        //    Tools.Click(p.X, p.Y + m_articleFieldOffset);
        }

        public void Publish()
        {
#if DEBUG
            // <input type="submit" name="Editor$Edit$lkbDraft" value="存为草稿" onclick="return CheckInput();" id="Editor_Edit_lkbDraft" class="Button">
            HtmlElement ele = this.Document.GetElementById("Editor_Edit_lkbDraft");
#else
            // <input type="submit" name="Editor$Edit$lkbPost" value="发布" onclick="return CheckInput();" id="Editor_Edit_lkbPost" class="Button">
            HtmlElement ele = this.Document.GetElementById("Editor_Edit_lkbPost");
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

        public sinaRobot.ArticleInfo GetArticleInfo()
        {
            sinaRobot.ArticleInfo info = new sinaRobot.ArticleInfo();

            info.url = this.Document.Url.ToString();

            // <h2 id="t_4c8a693a0102yeu0" class="titName SG_txta">保险行业也暴雷，A股还可有安全的？</h2>
            HtmlElementCollection collection = this.Document.GetElementsByTagName("h2");
            foreach (HtmlElement ele1 in collection)
            {
                if (ele1.OuterHtml.Contains("titName SG_txta"))
                {
                    info.title = ele1.InnerText;
                    m_articleTitle = info.title;
                    break;
                }
            }

            //<div id="sina_keyword_ad_area2" class="articalContent   newfont_family">
            HtmlElement ele = this.Document.GetElementById("sina_keyword_ad_area2");
            if (ele.OuterHtml.Length > 100000)
            {
                Log.WriteLog(LogType.Notice, "article too large, cut end content. url is :"
                    + info.url + " , original len is " + ele.OuterHtml.Length);

                m_articleContent = ele.OuterHtml.Substring(0, 100000);
            }
            else
                m_articleContent = ele.OuterHtml;

            return info;
        }

        public bool GoToNextPage()
        {
            //<a href="http://blog.sina.com.cn/s/articlelist_1284139322_3_2.html" title="跳转至第 2 页">下一页&nbsp;&gt;</a>
            HtmlElement ele = GetEleByTagAndOuterHtml("a", "下一页");
            if (ele == null || ele.OuterHtml.Contains("disabled"))
                return false;
            ClickLinkInEle(ele.OuterHtml);
            return true;
        }

        // return false means no next article anymore.
        public bool GoToNextArticlePage(string lastArticleUrl, ref bool isNetDealy)
        {
            m_articleContent = "";
            isNetDealy = true;

            //<span class="atc_title">
            // <a title="" target="_blank" href="http://blog.sina.com.cn/s/blog_4c8a693a0102yeu0.html">保险行业也暴雷，A股还可有安全的…</a>
            HtmlElementCollection collection = this.Document.GetElementsByTagName("a");
            foreach (HtmlElement ele in collection)
            {
                if (ele.OuterHtml.Contains("blog.sina.com.cn/s/blog"))
                {
                    isNetDealy = false;
                    if (lastArticleUrl == ""
                        || ele.OuterHtml.Contains(lastArticleUrl))
                    {
                        ClickLinkInEle(ele.OuterHtml);
                        return true;
                    }
                }
            }
            
            return false;
        }

        private void ClickLinkInEle(string EleOuterHtml)
        {
            // <a title="" target="_blank" href="http://blog.sina.com.cn/s/blog_4c8a693a0102yeu0.html">保险行业也暴雷，A股还可有安全的…</a>       
            int startIndex = EleOuterHtml.IndexOf("http");
            int endIndex = EleOuterHtml.IndexOf(".html") + 5;
            string ArticleUrl = EleOuterHtml.Substring(startIndex, endIndex - startIndex);
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
            //TODO: This is not trustful
            if (GetEleByTagAndOuterHtml("img", "avatar.csdn.net") != null)
                return true;
            return false;
        }

        public bool Login(string uName,string password)
        {
            // <input type="text" id="input1" value="" class="input-text" onkeydown="check_enter(event)">
            HtmlElement ele = this.Document.GetElementById("input1");
            if(ele == null)
            {
                ele = this.Document.GetElementById("all");
                ele.Focus(); SendKeys.Send(" ");
            }
            ele.SetAttribute("value", uName);


            // <input type="password" id="input2" value="" class="input-text" onkeydown="check_enter(event)">
            ele = this.Document.GetElementById("input2");
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
            ClickEleByTagAndOuterHtml("a", "退出");
            //SafeNavigate("//passport.csdn.net/account/logout");
            //if(ClickEleByTagAndOuterHtml("a", "退出"))
            //    NavigateToLoginPage();
        }

        public bool ClickVerify()
        {
            // <span class="geetest_wait_dot geetest_dot_1"></span>
            HtmlElement ele = GetEleByTagAndOuterHtml("span", "geetest_wait_dot geetest_dot_1");
            if (ele == null)
                return false;
            Point p = GetOffset(ele);
            Tools.DoubleClick(p.X + 1, p.Y + 1);
            return true;
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
