﻿using System;
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
            public string lastFinishedArticleUrl;
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
            info.lastFinishedArticleUrl = "https://blog.csdn.net/laoyang360/article/details/51931981";
            info.needFinishNum = 2;

            return info;
        }

        public void UpdateLastFinishedArticleName(string lastFinishedArticleName)
        {

        }

        public void UpdateLastListPageUrl(string lastListPageUrl)
        {

        }
    }
}
