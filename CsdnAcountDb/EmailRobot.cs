using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace CsdnAcountDb
{
    class EmailRobot
    {
        enum EnumStep
        {
            None,
            GoToLoginPage,
            Login,
            WaitSucess,
            Finished
        }

        EnumStep m_step = EnumStep.GoToLoginPage;
        EnumStep m_lastStep = EnumStep.None;

        Timer m_timerBrain;
        EmailBrowser m_browser = null;
        DbCsdnAccountDb m_DataManagerSqlLite = null;
        DbCsdnAccountDb.AccountInfo m_accounttInfo = null;

        public static UInt64 m_MinReadCount = 5000;

        UInt16 m_tryTimes = 0;
                
        public EmailRobot(EmailBrowser w, Timer timerBrain)
        {
            m_DataManagerSqlLite = new DbCsdnAccountDb("CsdnAccount.db");

            m_browser = w;

            m_timerBrain = timerBrain;
            m_timerBrain.Enabled = true;
            m_timerBrain.Interval = 2000;
        }

        public void timerBrain()
        {
            m_tryTimes++;
            if (m_tryTimes > 30)
                Environment.Exit(0);

            m_browser.CloseSecurityAlert();

            
            Log.WriteLog(LogType.Debug, "step is :" + m_step.ToString());
            try
            {
                switch(m_step)
                {
                    case EnumStep.GoToLoginPage:
                        GoToLoginPage();
                        break;
                    case EnumStep.Login:
                        Login();
                        break;
                    case EnumStep.WaitSucess:
                        WaitSucess();
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
                Log.WriteLog(LogType.Exception, "Exception happened in step " + m_step.ToString()
                     + ", Exception info: " + e.ToString());
                // this exception maybe just cause by doc which is not loaded complete. Network is not trustful.
#endif              
            }
            finally
            {
                m_lastStep = m_step;
            }
        }

        private void WaitSucess()
        {
            bool isDealy = false;
            bool isSuccess = m_browser.isSuccess(ref isDealy);

            if(isDealy)
                return;

            if (isSuccess)
            {
                Log.WriteLog(LogType.Notice, "email ok. email is " + m_accounttInfo.email);
                m_accounttInfo.emailPassword = m_accounttInfo.csdnPassword;
                m_step = EnumStep.GoToLoginPage;
            }
            else
            {
                m_accounttInfo.emailPassword = "";
                m_step = EnumStep.Login;
            }

            m_DataManagerSqlLite.SetAccountInfo(m_accounttInfo);

            
        }
     

        private void GoToLoginPage()
        {
            m_browser.NavigateToLoginPage();
            m_step = EnumStep.Login;
        }

        
        private void Login()
        {
            m_accounttInfo = m_DataManagerSqlLite.GetAccountInfo();
            if (m_accounttInfo == null)
            {
                m_step = EnumStep.Finished;
                return;
                //m_step = EnumStep.Finished;
            }
            else
            {
                if (m_browser.Login(m_accounttInfo.email, m_accounttInfo.csdnPassword))
                {
                    m_step = EnumStep.WaitSucess;
                }
            }        
        }
    }
}
