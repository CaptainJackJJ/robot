using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace AccountCreator
{
    public static class Tools
    {
        #region Set working memory

        [DllImport("KERNEL32.DLL", EntryPoint = "SetProcessWorkingSetSize", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool SetProcessWorkingSetSize(IntPtr pProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

        #endregion Set working memory

        #region set browser version
        /// <summary>  
        /// 修改注册表信息来兼容当前程序  
        ///   
        /// </summary>  
        public static void SetWebBrowserFeatures(int ieVersion)
        {
            // don't change the registry if running in-proc inside Visual Studio  
            if (LicenseManager.UsageMode != LicenseUsageMode.Runtime)
                return;
            //获取程序及名称  
            var appName = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            //得到浏览器的模式的值  
            UInt32 ieMode = GeoEmulationModee(ieVersion);
            var featureControlRegKey = @"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\";
            //设置浏览器对应用程序（appName）以什么模式（ieMode）运行  
            Registry.SetValue(featureControlRegKey + "FEATURE_BROWSER_EMULATION",
                appName, ieMode, RegistryValueKind.DWord);
            // enable the features which are "On" for the full Internet Explorer browser  
            //不晓得设置有什么用  
            Registry.SetValue(featureControlRegKey + "FEATURE_ENABLE_CLIPCHILDREN_OPTIMIZATION",
                appName, 1, RegistryValueKind.DWord);


            //Registry.SetValue(featureControlRegKey + "FEATURE_AJAX_CONNECTIONEVENTS",  
            //    appName, 1, RegistryValueKind.DWord);  


            //Registry.SetValue(featureControlRegKey + "FEATURE_GPU_RENDERING",  
            //    appName, 1, RegistryValueKind.DWord);  


            //Registry.SetValue(featureControlRegKey + "FEATURE_WEBOC_DOCUMENT_ZOOM",  
            //    appName, 1, RegistryValueKind.DWord);  


            //Registry.SetValue(featureControlRegKey + "FEATURE_NINPUT_LEGACYMODE",  
            //    appName, 0, RegistryValueKind.DWord);  
        }
        /// <summary>  
        /// 获取浏览器的版本  
        /// </summary>  
        /// <returns></returns>  
        public static int GetBrowserVersion()
        {
            int browserVersion = 0;
            using (var ieKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer",
                RegistryKeyPermissionCheck.ReadSubTree,
                System.Security.AccessControl.RegistryRights.QueryValues))
            {
                var version = ieKey.GetValue("svcVersion");
                if (null == version)
                {
                    version = ieKey.GetValue("Version");
                    if (null == version)
                        throw new ApplicationException("Microsoft Internet Explorer is required!");
                }
                int.TryParse(version.ToString().Split('.')[0], out browserVersion);
            }
            //如果小于7  
            if (browserVersion < 7)
            {
                throw new ApplicationException("不支持的浏览器版本!");
            }
            return browserVersion;
        }
        /// <summary>  
        /// 通过版本得到浏览器模式的值  
        /// </summary>  
        /// <param name="browserVersion"></param>  
        /// <returns></returns>  
        static UInt32 GeoEmulationModee(int browserVersion)
        {
            UInt32 mode = 11000; // Internet Explorer 11. Webpages containing standards-based !DOCTYPE directives are displayed in IE11 Standards mode.   
            switch (browserVersion)
            {
                case 7:
                    mode = 7000; // Webpages containing standards-based !DOCTYPE directives are displayed in IE7 Standards mode.   
                    break;
                case 8:
                    mode = 8000; // Webpages containing standards-based !DOCTYPE directives are displayed in IE8 mode.   
                    break;
                case 9:
                    mode = 9000; // Internet Explorer 9. Webpages containing standards-based !DOCTYPE directives are displayed in IE9 mode.                      
                    break;
                case 10:
                    mode = 10000; // Internet Explorer 10.  
                    break;
                case 11:
                    mode = 11000; // Internet Explorer 11  
                    break;
            }
            return mode;
        }

        #endregion set browser version

        #region Close alert dlg
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("User32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]

        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        private static int WM_CLICK = 0x00F5;
        private static int WM_CLOSE = 0x10;

        public static void CloseSecurityAlert()
        {
            IntPtr hwnd = FindWindow(null, "安全警告");
            if (hwnd != IntPtr.Zero)
            {
                IntPtr btnhwnd = FindWindowEx(hwnd, IntPtr.Zero, "Button", "是(&Y)");
                if (btnhwnd != IntPtr.Zero)
                {
                    SendMessage(btnhwnd, WM_CLICK, 0, 0);//先移上去  
                    SendMessage(btnhwnd, WM_CLICK, 0, 0);//再点击  
                }
            }

            hwnd = FindWindow(null, "Windows Internet Explorer");
            if (hwnd != IntPtr.Zero)
            {
                SendKeys.SendWait("{Enter}");
                SendKeys.Flush();
            }

            hwnd = FindWindow(null, "来自网页的消息");
            if (hwnd != IntPtr.Zero)
            {
                SendKeys.SendWait("{Enter}");
                SendKeys.Flush();
            }

            hwnd = FindWindow(null, "安全警报");
            if (hwnd != IntPtr.Zero)
            {
                IntPtr btnhwnd = FindWindowEx(hwnd, IntPtr.Zero, "Button", "是(&Y)");
                if (btnhwnd != IntPtr.Zero)
                {
                    SendMessage(btnhwnd, WM_CLICK, 0, 0);//先移上去  
                    SendMessage(btnhwnd, WM_CLICK, 0, 0);//再点击  
                }
            }

            hwnd = FindWindow(null, "证书");
            if (hwnd != IntPtr.Zero)
            {
                IntPtr btnhwnd = FindWindowEx(hwnd, IntPtr.Zero, "Button", "确定");
                if (btnhwnd != IntPtr.Zero)
                {
                    SendMessage(btnhwnd, WM_CLICK, 0, 0);//先移上去  
                    SendMessage(btnhwnd, WM_CLICK, 0, 0);//再点击  
                }
                else
                {
                    SendMessage(hwnd, WM_CLOSE, 0, 0);
                }
            }

            //hwnd = FindWindow(null, "Web浏览器");
            //if (hwnd != IntPtr.Zero)
            //{
            //    SendKeys.SendWait("{Enter}");
            //    SendKeys.Flush();
            //}

            //hwnd = FindWindow(null, "Web 浏览器");
            //if (hwnd != IntPtr.Zero)
            //{
            //    SendKeys.SendWait("{Enter}");
            //    SendKeys.Flush();
            //}
        }
        #endregion Close alert dlg

        #region mouse control

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        [DllImport("user32.dll")]
        public static extern void SetCursorPos(int x, int y);//设置鼠标焦点

        const int MOUSEEVENTF_MOVE = 0x0001;      //移动鼠标 
        const int MOUSEEVENTF_LEFTDOWN = 0x0002; //模拟鼠标左键按下 
        const int MOUSEEVENTF_LEFTUP = 0x0004; //模拟鼠标左键抬起 
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008; //模拟鼠标右键按下 
        const int MOUSEEVENTF_RIGHTUP = 0x0010; //模拟鼠标右键抬起 
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; //模拟鼠标中键按下 
        const int MOUSEEVENTF_MIDDLEUP = 0x0040; //模拟鼠标中键抬起 
        const int MOUSEEVENTF_ABSOLUTE = 0x8000; //标示是否采用绝对坐标
        const int MOUSEEVENTF_WHEEL = 0x800;

        public static void Click(int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, x, y, 0, 0);
        }
        public static void DoubleClick(int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, x, y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, x, y, 0, 0);
        }

        #endregion mouse control

        public static void SafeClick(HtmlElement ele)
        {
            CloseSecurityAlert();
            ele.InvokeMember("click");
        }

    }
}
