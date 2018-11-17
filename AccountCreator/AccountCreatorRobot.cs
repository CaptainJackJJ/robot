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
            Finished
        }

        public static string m_password = "FiSKpJuHc12345";

        EnumStep m_step = EnumStep.GoToLogoutPage;
        EnumStep m_lastStep = EnumStep.None;

        Timer m_timerBrain;
        AccountCreatorBrowser m_browser = null;
        AccountCreatorDb m_checkedObjDb, m_objDb;

        UInt16 m_timesOfStep = 0;
        readonly UInt16 m_maxSteps = 3 * 20;

        public AccountCreatorRobot(AccountCreatorBrowser w, Timer timerBrain)
        {
            m_browser = w;

            m_timerBrain = timerBrain;
            m_timerBrain.Enabled = true;
            m_timerBrain.Interval = 2000;
        }

        public void timerBrain()
        {
            m_timesOfStep++;

            if (m_timesOfStep > m_maxSteps)
            {
                Environment.Exit(0);
            }

            m_browser.CloseSecurityAlert();
            
            Log.WriteLog(LogType.Debug, "step is :" + m_step.ToString());
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
                    case EnumStep.Finished:
                        m_timerBrain.Stop();
                        MessageBox.Show("今天的工作已完成");
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
            finally
            {
                m_lastStep = m_step;
            }
        }
        private void ConfirmChangePassword()
        {
            m_browser.ConfirmChangePassword();
            m_step = EnumStep.None;
        }

        private void ChangePassword()
        {
            m_browser.ChangePassword();
            m_step = EnumStep.ConfirmChangePassword;
        }

        private void GoToChangePasswordPage()
        {
            m_browser.SafeNavigate("https://i.csdn.net/#/account/password");
            m_step = EnumStep.ChangePassword;
        }

        private void BeFans()
        {
            string account = m_browser.BeFans();
            m_step = EnumStep.GoToChangePasswordPage;
        }

        private void GoToMyAticle()
        {
            m_browser.SafeNavigate("https://blog.csdn.net/jiangjunshow/article/details/77338485");
            m_step = EnumStep.BeFans;
        }

        private void Login()
        {
            m_browser.Login();
            m_step = EnumStep.GoToMyAticle;
        }

        private void Logout()
        {
            m_browser.Logout();
            m_step = EnumStep.Login;
        }

        private void GoToLogoutPage()
        {
            m_browser.SafeNavigate("https://blog.csdn.net/jiangjunshow/article/details/77338485");
            m_step = EnumStep.Logout;
        }
    }
}
