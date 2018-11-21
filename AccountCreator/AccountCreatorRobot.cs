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
            Unbind,
        }

        enum EnumStep
        {
            None,
            GoToLogoutPage,
            Logout,
            Login,
            GoToMyAticle,
            BeFans,
            GoToChangePasswordPage,
            ConfirmChangePassword,
            ChangePassword,
            GoToAccountLoginPage,
            LoginWithAccount,
            ConfirmLoginWithAccount,
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

            m_accountDb = new AccountCreatorDb("Account.db");
        }

        public void SetTaskType(EnumTaskType type)
        {
            m_taskType = type;
            if (m_taskType == EnumTaskType.Create)
            {
                m_step = EnumStep.GoToLogoutPage;
            }
            else if (m_taskType == EnumTaskType.Set)
            {
                m_step = EnumStep.GoToAccountLoginPage;
            }
            else
            {
                m_browser.NavigateToLoginPage();
                m_step = EnumStep.Login;
            }
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
                        m_timerBrain.Stop();
                        MessageBox.Show("已完成");
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
        }
        
        private void ConfirmChangeCodeStyle()
        {
            m_browser.ConfirmChangeCodeStyle();
            m_step = EnumStep.GoToBindPage;
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
            
            m_timerBrain.Enabled = false;
            System.Media.SystemSounds.Beep.Play();
        }

        private void ConfirmLoginWithAccount()
        {
            // <input class="logging" accesskey="l" value="登 录" tabindex="6" type="button">
            if (!m_browser.MouseClickEle("input", "登 录"))
            {
                //<button data-type="account" class="btn btn-primary">登录</button>
                m_browser.MouseClickEle("button", "登录");
            }

            m_step = EnumStep.GoToProfile;
        }

        private void LoginWithAccount()
        {
            if (m_browser.IsLogedin())
            {
                m_browser.Logout(false);
            }
            else
            {
                m_accountInfo = m_accountDb.GetUnsetAccount();
                if (m_accountInfo == null)
                {
                    m_timerBrain.Stop();
                    MessageBox.Show("all account is set");
                    
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
                m_browser.Logout(false);
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
            m_step = EnumStep.Finished;

            m_accountDb.AddAccountInfo(m_accountInfo);
            EmptyPhone();
            System.Media.SystemSounds.Beep.Play();
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
            m_accountInfo.userName = m_browser.BeFans();
            m_step = EnumStep.GoToConfigurePage;
        }

        private void GoToMyAticle()
        {
            m_browser.SafeNavigate("https://blog.csdn.net/jiangjunshow/article/details/77338485");
            if (m_taskType == EnumTaskType.Create)
                m_step = EnumStep.BeFans;
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
            m_browser.Logout();
            if(m_taskType == EnumTaskType.Create)
                m_step = EnumStep.Login;
            else if(m_taskType == EnumTaskType.Unbind)
            {
                m_timerBrain.Enabled = false;
                System.Media.SystemSounds.Beep.Play();
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
            return "蚂蚁短信记录" + DateTime.Now.ToString("yyyyMM") + DateTime.Now.ToString("yyyy年MM月dd日") + ".txt";
        }

        private string GetPhone()
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

        private void GoToLogoutPage()
        {
            m_accountInfo.phone = GetPhone();
            if (m_accountInfo.phone == "")
            {
                m_timerBrain.Enabled = false;
                MessageBox.Show("phone is empty");
                return;
            }

            m_browser.SafeNavigate("https://blog.csdn.net/jiangjunshow/article/details/77338485");
            m_step = EnumStep.Logout;
        }
    }
}
