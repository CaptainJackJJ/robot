using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace AccountCreator
{
    class AccountCreatorRobot
    {
        public enum EnumTaskType
        {
            Create,
            Set,
            BeFan,
        }

        enum EnumStep
        {
            None,
            GoToLogoutPage,
            Logout,
            Login,
            GoToMyAticle,
            BeFans,
            GetUsernameForQQ,
            GoToChangePasswordPage,
            ConfirmChangePassword,
            ChangePassword,
            GoToAccountLoginPage,
            LoginWithAccount,
            ConfirmLoginWithAccount,
            GoToListPage,
            GoToNextArticlePage,
            GoToProfile,
            GoToBindPage,
            Unbind,
            ConfirmUnbind,
            GoToConfigurePage,
            MoveToCodeStyle,
            ChangeCodeStyle,
            ConfirmChangeCodeStyle,
            Finished
        }

        public static string m_password = "FiSKpJuHc12345";

        EnumTaskType m_taskType = EnumTaskType.Create;
        EnumStep m_step = EnumStep.GoToAccountLoginPage;
        EnumStep m_lastStep = EnumStep.None;

        Timer m_timerBrain;
        AccountCreatorBrowser m_browser = null;
        AccountCreatorDb m_accountDb;

        public AccountCreatorDb.AccountInfo m_accountInfo = new AccountCreatorDb.AccountInfo();

        bool m_isFirstTimeTryLogin = true;
        UInt16 m_TimesTryUnbind = 0;
        

        public AccountCreatorRobot(AccountCreatorBrowser w, Timer timerBrain)
        {
            m_browser = w;

            m_timerBrain = timerBrain;
            m_timerBrain.Interval = 2000;
#if DEBUG
            m_accountDb = new AccountCreatorDb("Account_debug.db");
#else
            m_accountDb = new AccountCreatorDb("Account.db");
#endif
        }

        public void SetTaskType(EnumTaskType type)
        {

#if DEBUG
                m_timerBrain.Interval = 2000;
#else
                m_timerBrain.Interval = 10000;
#endif

            m_taskType = type;
            if (m_taskType == EnumTaskType.Create)
            {
                m_step = EnumStep.GoToLogoutPage;
            }
            else if (m_taskType == EnumTaskType.Set || m_taskType == EnumTaskType.BeFan)
            {
                m_step = EnumStep.GoToAccountLoginPage;
            }
            //else
            //{
            //    m_browser.NavigateToLoginPage();
            //    m_step = EnumStep.Login;
            //}
        }

        public void timerBrain()
        {
            m_browser.CloseSecurityAlert();
            
            try
            {
                switch(m_step)
                {
                    case EnumStep.GoToLogoutPage:
                        GoToLogoutPage();
                        break;
                    case EnumStep.Logout:
                        Logout();
                        break;
                    case EnumStep.Login:
                        Login();
                        break;
                    case EnumStep.GoToMyAticle:
                        GoToMyAticle();
                        break;
                    case EnumStep.BeFans:
                        BeFans();
                        break;
                    case EnumStep.GetUsernameForQQ:
                        GetUsernameForQQ();
                        break;
                    case EnumStep.GoToChangePasswordPage:
                        GoToChangePasswordPage();
                        break;
                    case EnumStep.ChangePassword:
                        ChangePassword();
                        break;
                    case EnumStep.ConfirmChangePassword:
                        ConfirmChangePassword();
                        break;
                    case EnumStep.GoToAccountLoginPage:
                        GoToAccountLoginPage();
                        break;
                    case EnumStep.LoginWithAccount:
                        LoginWithAccount();
                        break;
                    case EnumStep.ConfirmLoginWithAccount:
                        ConfirmLoginWithAccount();
                        break;
                    case EnumStep.GoToListPage:
                        GoToListPage();
                        break;
                    case EnumStep.GoToNextArticlePage:
                        GoToNextArticlePage();
                        break;
                    case EnumStep.GoToProfile:
                        GoToProfile();
                        break;
                    case EnumStep.GoToBindPage:
                        GoToBindPage();
                        break;
                    case EnumStep.Unbind:
                        Unbind();
                        break;
                    case EnumStep.ConfirmUnbind:
                        ConfirmUnbind();
                        break;
                    case EnumStep.GoToConfigurePage:
                        GoToConfigurePage();
                        break;
                    case EnumStep.MoveToCodeStyle:
                        MoveToCodeStyle();
                        break;
                    case EnumStep.ChangeCodeStyle:
                        ChangeCodeStyle();
                        break;
                    case EnumStep.ConfirmChangeCodeStyle:
                        ConfirmChangeCodeStyle();
                        break;
                    case EnumStep.Finished:
                        System.Media.SystemSounds.Beep.Play();
                        m_timerBrain.Stop();                        
                        return;
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Log.WriteLog(LogType.Exception, "Exception happened in step " + m_step.ToString() + ", Exception info: " + e.ToString());
                // this exception maybe just cause by doc which is not loaded complete. Network is not trustful.
#endif              
            }
            m_browser.CloseSecurityAlert();
        }
        
        private void ConfirmChangeCodeStyle()
        {
            m_browser.ConfirmChangeCodeStyle();
            //m_step = EnumStep.GoToBindPage;

            if (m_taskType == EnumTaskType.Create)
                m_step = EnumStep.GoToChangePasswordPage;
            else
            {
                m_step = EnumStep.Logout;
            }
        }

        private void ChangeCodeStyle()
        {
            m_browser.ChangeCodeStyle();
            m_step = EnumStep.ConfirmChangeCodeStyle;
        }

        private void MoveToCodeStyle()
        {
            if (m_browser.MoveToCodeStyle())
                m_step = EnumStep.ChangeCodeStyle;
        }

        private void GoToConfigurePage()
        {
            if (m_browser.Url.ToString().Contains("https://passport.csdn.net"))
            {
                m_step = EnumStep.LoginWithAccount;
                return;
            }

            m_browser.SafeNavigate("https://mp.csdn.net/configure");
            m_step = EnumStep.MoveToCodeStyle;
        }

        private void ConfirmUnbind()
        {
            if (m_browser.ConfirmUnbind())
            {
                //m_TimesTryUnbind++;
                //if (m_TimesTryUnbind < 2)
                //{
                //    m_step = EnumStep.Unbind;
                //    return;
                //}
                //m_TimesTryUnbind = 0;
                if (m_taskType == EnumTaskType.Create)
                    m_step = EnumStep.GoToChangePasswordPage;
                else
                {
                    m_step = EnumStep.Logout;
                }
            }
        }

        private void Unbind()
        {
            if (!m_browser.Url.ToString().Contains("account/bind"))
            {
                return;
            }

            if (m_browser.Unbind())
            {
                //m_browser.SafeNavigate("https://i.csdn.net/#/uc/profile");
                m_step = EnumStep.ConfirmUnbind;
            }
        }

        private void GoToBindPage()
        {
            m_browser.SafeNavigate("https://i.csdn.net/#/account/bind");
            m_step = EnumStep.Unbind;
        }
        private void GoToProfile()
        {
            m_browser.SafeNavigate("https://i.csdn.net/#/uc/profile");

            //m_timerBrain.Enabled = false;
            m_step = EnumStep.Finished;
            SetDoneUnsetAccount();
        }

        private void ConfirmLoginWithAccount()
        {
            // <input class="logging" accesskey="l" value="登 录" tabindex="6" type="button">
            if (!m_browser.MouseClickEle("input", "登 录"))
            {
                //<button data-type="account" class="btn btn-primary">登录</button>
                m_browser.MouseClickEle("button", "登录");
            }

            if (m_taskType == EnumTaskType.BeFan)
            {
                m_step = EnumStep.GoToListPage;
            }
            else
            {
                m_step = EnumStep.GoToProfile;
            }
        }

        private void GoToListPage()
        {
            m_browser.SafeNavigate(m_accountInfo.fanToListPage);
            m_step = EnumStep.GoToNextArticlePage;
        }

        private void GoToNextArticlePage()
        {
            if (m_accountInfo.fanToArticle == "") // Maybe get into new list page, so update the list page url.
            {
                string url = m_browser.Url.ToString();
                if (url.Trim() == "https://www.csdn.net/")
                {
                    Log.WriteLog(LogType.Warning, "last url is https://www.csdn.net/. obj is " + m_accountInfo.fanToAccount);
                    m_step = EnumStep.GoToListPage;
                    return;
                }
                m_accountInfo.fanToListPage = m_browser.Url.ToString();
            }

            bool isNetDealy = false;
            if (m_browser.GoToNextArticlePage(m_accountInfo.fanToArticle, ref isNetDealy))
                m_step = EnumStep.BeFans;
            else // no next article
            {
                if (isNetDealy)
                {
                    return; // try GoToNextArticlePage again
                }
                if (!m_browser.GoToNextPage())
                {
                    Log.WriteLog(LogType.Notice, "list is empty, so object is done");
                    m_step = EnumStep.Finished;

                    m_accountInfo.isFaning = false;
                    m_accountInfo.fanToNum++;
                    m_accountDb.SetFan(m_accountInfo);

                    return;
                    //this working object is done.
                }

                m_accountInfo.fanToArticle = "";
            }
        }

        private void LoginWithAccount()
        {
            if (m_browser.IsLogedin())
            {
                m_browser.Logout();
                if(m_taskType == EnumTaskType.BeFan)
                {
                    m_step = EnumStep.GoToAccountLoginPage;
                    return;
                }
            }
            else
            {
                if (m_taskType == EnumTaskType.Set)
                {
                    m_accountInfo = m_accountDb.GetUnsetAccount();
                }
                else
                {
                    m_accountInfo = m_accountDb.GetFan();
                }

                if (m_accountInfo == null)
                {
                    m_timerBrain.Stop();
                    MessageBox.Show("account is null");
                    
                    return;
                }
                else
                {
                    if (m_browser.LoginWithAccount(m_accountInfo.userName, m_accountInfo.password))
                    {
                        m_step = EnumStep.ConfirmLoginWithAccount;
                    }
                }
            }
        }

        public void SetDoneUnsetAccount()
        {
            if(m_accountInfo != null)
            {
                m_accountDb.SetUnsetAccount(m_accountInfo);
                m_browser.Logout();
            }
        }

        private void GoToAccountLoginPage()
        {
            m_browser.SafeNavigate("https://passport.csdn.net/account/login");
            m_step = EnumStep.LoginWithAccount;
        }

        private void ConfirmChangePassword()
        {
            m_browser.MouseClickEle("button", "确认");
            //m_step = EnumStep.Finished;            

            m_accountDb.AddAccountInfo(m_accountInfo);
            EmptyPhone();
            //System.Media.SystemSounds.Beep.Play();
            SetTaskType(EnumTaskType.Set);
        }

        private void ChangePassword()
        {
            if (!m_browser.Url.ToString().Contains("account/password"))
                m_step = EnumStep.GoToChangePasswordPage;
            if (!m_browser.ChangePassword())
                return;
            m_step = EnumStep.ConfirmChangePassword;
        }

        private void GoToChangePasswordPage()
        {
            m_browser.SafeNavigate("https://i.csdn.net/#/account/password");
            m_step = EnumStep.ChangePassword;
        }

        private void BeFans()
        {
            string strUrl = m_browser.Url.ToString();
            if (strUrl.Contains("orderby=ViewCount")) // still at list page
                return;

            m_browser.BeFans();

            m_accountInfo.fanToArticle = m_browser.Url.ToString();
            m_accountInfo.isFaning = true;
            m_accountDb.SetFan(m_accountInfo);

            m_step = EnumStep.GoToListPage;
        }

        private void GetUsernameForQQ()
        {
            if (!m_browser.IsLogedin())
            {
                m_browser.NavigateToLoginPage();
                m_step = EnumStep.Login;
                return;
            }
            m_accountInfo.userName = m_browser.GetUsernameForQQ();
            m_step = EnumStep.GoToConfigurePage;
        }

        private void GoToMyAticle()
        {
            m_browser.SafeNavigate("https://blog.csdn.net/jiangjunshow/article/details/77338485");
            if (m_taskType == EnumTaskType.Create)
                m_step = EnumStep.GetUsernameForQQ;
            else
                m_step = EnumStep.GoToBindPage;            
        }

        private void Login()
        {
            if (m_isFirstTimeTryLogin)
            {
                m_isFirstTimeTryLogin = false;
                return;
            }

            m_browser.Login();
            m_step = EnumStep.GoToMyAticle;

            m_isFirstTimeTryLogin = true;
        }

        private void Logout()
        {
            if (m_browser.IsLogedin())
            {
                m_browser.Logout();
            }
            else
            {
                m_browser.NavigateToLoginPage();
                if (m_taskType == EnumTaskType.Create)
                    m_step = EnumStep.Login;
                else if (m_taskType == EnumTaskType.BeFan)
                {
                    m_timerBrain.Enabled = false;
                    System.Media.SystemSounds.Beep.Play();
                }
            }
        }

        private void EmptyPhone()
        {
            System.IO.FileStream stream = System.IO.File.Open(GetPhoneFileName(), System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            stream.SetLength(0);
            stream.Close();
            stream.Dispose();
        }

        private string GetPhoneFileName()
        {
            // 蚂蚁短信记录2018112018年11月19日
            return "蚂蚁短信记录" + DateTime.Now.ToString("yyyy%M") + DateTime.Now.ToString("yyyy年%M月%d日") + ".txt";
        }

        private string GetPhone()
        {
            try
            {
                using (System.IO.StreamReader sr = System.IO.File.OpenText(GetPhoneFileName()))
                {
                    string s = sr.ReadLine();
                    if (s == null)
                    {
                        return "";
                    }
                    //2018年11月19日20时37分,17045676221,
                    int indexS = s.IndexOf(",") + 1;
                    int indexE = s.IndexOf(",", indexS);
                    return s.Substring(indexS, indexE - indexS);
                }
            }
            catch(Exception e)
            {
                m_timerBrain.Stop();
                MessageBox.Show("读取手机号时出错。要先获取验证码，并在手机上登录后再打开本软件。错误信息是：" + e.Message);
                Environment.Exit(0);
            }
            return "";
        }

        private void GoToLogoutPage()
        {
#if DEBUG
#else
            m_accountInfo.phone = GetPhone();
            if (m_accountInfo.phone == "")
            {
                m_timerBrain.Enabled = false;
                MessageBox.Show("phone is empty");
                return;
            }
#endif

            m_browser.SafeNavigate("https://blog.csdn.net/jiangjunshow/article/details/77338485");
            m_step = EnumStep.Logout;
        }
    }
}
