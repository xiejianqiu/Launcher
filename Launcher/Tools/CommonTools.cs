using Launcher;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Runtime.InteropServices.ComTypes;
using Launcher;

namespace Tools
{
    public static class CommonTools
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        public static void Exit()
        {
            Environment.Exit(0);
        }
        public static void RunExe(string exePath, string arguments)
        {
            var startIinfo = new ProcessStartInfo(exePath, arguments)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                WindowStyle = ProcessWindowStyle.Normal
            };
            using (var process = Process.Start(startIinfo))
            {
                process.WaitForInputIdle();
                SetForegroundWindow(process.MainWindowHandle);
            }
        }
        public static void RunURL()
        {
            Process.Start("https://jq.qq.com/?_wv=1027&k=FMIcacvI");
        }
        public static void DelAndCreate(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            Directory.CreateDirectory(path);
        }
        public static void OpenCHannelWindow<T>() where T:Window
        {
            try
            {
                Window newWindow = Activator.CreateInstance<T>() as Window;
                newWindow.Show();
                foreach (Window window in App.Current.Windows)
                {
                    if (window != newWindow)
                    {
                        window.Close();
                    }
                }

            }
            catch (Exception e)
            {
                LogTool.Instance.Error($"OpenCHannelWindow {e.ToString()}");
            }
        }
        /// <summary>
        /// 在创建app的快捷方式,在中文路径会出现乱码
        /// </summary>
        public static void CreateDesktopShortcut2()
        {
            try
            {
                if (System.IO.File.Exists(GameConfig.GameExeLnkPath))
                    System.IO.File.Delete(GameConfig.GameExeLnkPath);
                string processName = Process.GetCurrentProcess().ProcessName + ".exe";
                string linkName = GameConfig.AppName;
                string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                using (StreamWriter writer = new StreamWriter(deskDir + "\\" + linkName + ".url"))
                {
                    var pathBytes = Environment.CurrentDirectory;
                    string exepath = Path.Combine(pathBytes, processName);
                    LogTool.Instance.Info($"CreateAppShortCut ExePath:{exepath}");
                    writer.WriteLine("[InternetShortcut]");
                    writer.WriteLine($"URL=file:///{exepath}");
                    writer.WriteLine("IconIndex=0");
                    string icon = exepath.Replace('\\', '/');
                    writer.WriteLine("IconFile=" + icon);
                }
            }
            catch (Exception e)
            {
                LogTool.Instance.Error($"CreateAppShortCut {e.ToString()}");
            }
        }
        /// <summary>
        /// 在桌面创建快捷方式
        /// </summary>
        static public void CreateDesktopShortcut()
        {
            
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            //从桌面打开则不创建快捷方式
            if (Environment.CurrentDirectory == desktopPath)
                return;
            string processName = Process.GetCurrentProcess().ProcessName + ".exe";
            string tarExePath = Path.Combine(Environment.CurrentDirectory, processName);
            if (!File.Exists(tarExePath))
            {
                LogTool.Instance.Info($"CreateDesktopShortcut 目标程序不存在，无法创建快捷方式 {tarExePath}");
                return;
            }
            if (File.Exists(GameConfig.GameExeLnkPath))
            {
                File.Delete(GameConfig.GameExeLnkPath);
            }
            IShellLink link = (IShellLink)new ShellLink();
            link.SetDescription("点我进凡人修仙世界");
            link.SetPath(tarExePath);
            IPersistFile file = (IPersistFile)link;
            file.Save(GameConfig.GameExeLnkPath, false);
        }
    }
}
