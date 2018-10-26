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

        public DataManager()
        {

        }

        public WorkingObjectInfo GetWorkingObjectInfo()
        {
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

        public void SetWorkingObjectInfo(WorkingObjectInfo info)
        {
            // save to database
        }

        public void UpdateLastFinishedArticleName(string lastFinishedArticleName)
        {

        }

        public void UpdateLastListPageUrl(string lastListPageUrl)
        {

        }
    }
}
