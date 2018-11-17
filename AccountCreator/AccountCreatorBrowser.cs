﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;

namespace AccountCreator
{
    class AccountCreatorBrowser : WebBrowser
    {
        public AccountCreatorBrowser()
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
            public WebBrowserSiteEx(AccountCreatorBrowser webBrowser)
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


        public void ChangePassword()
        {
            // <input data-v-08c18f7a="" type="password" name="password" id="password" placeholder="11-20位数字和字母组合" autocomplete="off" validate="true" data-rule="['password']" oncopy="return false" class="inpt">
            HtmlElement ele = this.Document.GetElementById("password");
            ele.Focus(); //SendKeys.Send(" ");
            ele.SetAttribute("value", AccountCreatorRobot.m_password);

            //<input data-v-08c18f7a="" id="confirmPwd" type="password" placeholder="确认新密码" autocomplete="off" oncopy="return false" class="inpt">
            ele = this.Document.GetElementById("confirmPwd");
            ele.Focus(); //SendKeys.Send(" ");
            ele.SetAttribute("value", AccountCreatorRobot.m_password);
        }

        public void ConfirmChangePassword()
        {
            //<button data-v-08c18f7a="" class="confirm_btn confirm_primary">确认</button>
            HtmlElement ele = GetEleByTagAndOuterHtml("button", "确认");
            Point p = GetOffset(ele);
            Tools.DoubleClick(p.X + 3, p.Y + 1);
        }

        public string BeFans()
        {
            // <a class="btn btn-sm btn-red-hollow attention" id="btnAttent" target="_blank">关注</a>
            HtmlElement ele = this.Document.GetElementById("btnAttent");
            if (ele.OuterHtml.Contains(">关注<"))
                SafeClick(ele);

            // <button class=" long-height hover-box btn-like " title="点赞">
            ClickEleByTagAndOuterHtml("button", "\"点赞\"");

            // <button class="btn-bookmark low-height hover-box" title="收藏">
            ClickEleByTagAndOuterHtml("button", "\"收藏\"");

            // <img class="login_img" src="//avatar.csdn.net/A/7/B/2_qq_43717814.jpg">
            ele = GetEleByTagAndOuterHtml("img", "login_img");
            int indexS = ele.OuterHtml.IndexOf("qq");
            int indexE = ele.OuterHtml.IndexOf(".jpg");
            return ele.OuterHtml.Substring(indexS, indexE - indexS);
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

        public bool Login()
        {
            Tools.Click(666, 300);

            return true;
        }

        public void NavigateToLoginPage()
        {
            SafeNavigate("https://graph.qq.com/oauth2.0/show?which=Login&display=pc&client_id=100270989&response_type=code&redirect_uri=https%3A%2F%2Fpassport.csdn.net%2Faccount%2Flogin%3FpcAuthType%3Dqq%26state%3Dtest");
        }

        public void Logout()
        {
            ClickEleByTagAndOuterHtml("a", "退出");
            NavigateToLoginPage();
        }
    }
}
