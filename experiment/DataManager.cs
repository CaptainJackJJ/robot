using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace experiment
{
    class DataManager
    {
        public struct WorkingObjectInfo
        {
            public string userName;
            public string password;
            public string lastListPageUrl;
            public string lastFinishedArticleUrlInList;
            public short needFinishNum;
        }

        private bool m_bSwitch = false;

        public DataManager()
        {

        }

        public WorkingObjectInfo GetWorkingObjectInfo()
        {
            if(m_bSwitch)
            {
                m_bSwitch = false;

                WorkingObjectInfo info;
                info.userName = "werfhksdhf";
                info.password = "Ct@z7h3LFt4q";
                info.lastListPageUrl = "https://blog.csdn.net/laoyang360?orderby=ViewCount";
                // The last finished article url in the list page. 
                // if it is empty means either this is a new working object or new list page.
                info.lastFinishedArticleUrlInList = "https://blog.csdn.net/laoyang360/article/details/51824617";
                info.needFinishNum = 2;
                return info;
            }
            else
            {
                m_bSwitch = true;

                WorkingObjectInfo info;
                info.userName = "sdhiiwfssf";
                info.password = "Cq&86tjUKHEG";
                info.lastListPageUrl = "https://blog.csdn.net/jxw167?orderby=ViewCount";
                // The last finished article url in the list page. 
                // if it is empty means either this is a new working object or new list page.
                info.lastFinishedArticleUrlInList = "";
                info.needFinishNum = 2;
                return info;
            }            
        }

        public void SetWorkingObjectInfo(WorkingObjectInfo info)
        {
            // save to database

            // if needFinishNum is 0, so we need to change the flag that indicate the daily work with this object is done.
        }

        public void UpdateLastFinishedArticleName(string lastFinishedArticleName)
        {

        }

        public void UpdateLastListPageUrl(string lastListPageUrl)
        {

        }
    }
}
