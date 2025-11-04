using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using TinyJson;
using Tools;

namespace Launcher
{
    /// <summary>
    /// 1.获取CDN上最新版本信息
    /// 2.和本地版本对比，如果比大就走版本更新流程
    /// 3.版本更新完后显示渠道选服界面
    /// </summary>
    public class Index
    {
        private static Index _Index;
        public static Index Inst {
            get {
                if (null == _Index)
                {
                    _Index = new Index();
                }
                return _Index;
            }
        }
        public event Action<int> DownloadProgressEvent;
        public event Action<string> StepChangeEvent;
        public event Action OpenChannelLoginWindow;
        void UpdateProgerss(int progress)
        {
            DownloadProgressEvent?.Invoke(progress);
        }
        void UpdateTips(string msg)
        {
            StepChangeEvent?.Invoke(msg);
        }
        public void Start()
        {
            UpdateProgerss(GameConfig.ProgressOfStartUp);
            this.UpdateTips(TIPS.GET_VERSION_INFO);
            LogTool.Instance.Info("HttpUtil.DownloadFile");
            HttpUtil.DownloadFile(GameConfig.newAppVerUrl, OnGetNewAppVerInfo);
        }

        private void OnGetNewAppVerInfo(bool result, string jsonStr)
        {
            LogTool.Instance.Info("OnGetNewAppVerInfo");
            try
            {
                if (!result)
                {
                    MessageBox.Show("获取资源版本信息失败，请检查网络是否畅通!");
                    return;
                }
                bool NeedUpdate = false;
                var newAppVer = jsonStr.FromJson<AppVer>();
                if (File.Exists(GameConfig.LocaGameAppInfo))
                {
                    string jsonContent = File.ReadAllText(GameConfig.LocaGameAppInfo, Encoding.UTF8);
                    var localAppVer = jsonContent.FromJson<Localver>();
                    if (IsNewAppVersion(localAppVer.version, newAppVer.version))
                    {
                        NeedUpdate = true;
                    }
                }
                else
                {
                    NeedUpdate = true;
                }
                if (!NeedUpdate)
                {
                    ShowGameServerUI();
                }
                else
                {
                    
                    StartGameUpdate(newAppVer);
                }
            }
            catch (Exception e)
            {
                LogTool.Instance.Error(e.ToString());
                MessageBox.Show($"游戏更新出现异常，{e.ToString()}");
            }
        }
        /// <summary>
        /// 开始游戏更新
        /// </summary>
        void StartGameUpdate(AppVer newAppVer)
        {
            LogTool.Instance.Info("StartGameUpdate");
            var savePath = $"{newAppVer.version}{newAppVer.md5}_{newAppVer.size}.zip";
            savePath = Path.Combine(GameConfig.GameRoot,savePath);
            if (!Directory.Exists(GameConfig.GameRoot))
            {
                Directory.CreateDirectory(GameConfig.GameRoot);
            }
            if (File.Exists(savePath))
            {
                UnzipNewAppVers(savePath, GameConfig.GameSavePath);
                return;
            }
            this.UpdateTips(TIPS.IN_DOWNLOAD_ZIPFILE);
            //CommonTools.DelAndCreate(GameConfig.GameRoot);
            HttpUtil.DownBigFile(newAppVer.url, savePath, newAppVer.md5, OnNewGameVerDL, OnDLGameZipProgress);
        }
        /// <summary>
        /// 游戏新版本下载进度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDLGameZipProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            var progress = GameConfig.ProgressOfStartUp + GameConfig.ProgressOfDLZip * (e.ProgressPercentage * 1f / 100);
            UpdateProgerss(Convert.ToInt16(progress));
        }
        /// <summary>
        /// App版本压缩包下载成功
        /// </summary>
        /// <param name="result"></param>
        /// <param name="savePath"></param>
        void OnNewGameVerDL(bool result, string savePath)
        {
            LogTool.Instance.Info($"OnNewGameVerDL {result}");
            if (!result)
            {
                LogTool.Instance.Error("OnNewGameVerDL Fail");
                CommonTools.Exit();
                return;
            }
            UnzipNewAppVers(savePath, GameConfig.GameSavePath);
        }
        /// <summary>
        /// 将资源解压缩到游戏目录
        /// </summary>
        /// <param name="zipFile"></param>
        /// <param name="gamePath"></param>
        void UnzipNewAppVers(string zipFile, string gamePath)
        {
            LogTool.Instance.Info($"UnzipNewAppVers {zipFile}");
            this.UpdateTips(TIPS.IN_UNZIP);
            if (Directory.Exists(gamePath))
            {
                Directory.Delete(gamePath, true);
            }
            if (File.Exists(GameConfig.LocaGameAppInfo))
            {
                File.Delete(GameConfig.LocaGameAppInfo);
            }
            Installer.UnZip(zipFile, gamePath, OnUnzipProgresssHandler, OnUnzipAppResult);
        }
        void OnUnzipProgresssHandler(float progress)
        {
            UpdateProgerss(Convert.ToInt16(GameConfig.ProgressOfStartUp + GameConfig.ProgressOfDLZip + GameConfig.ProgressOfUnzip * progress));
        }
        void OnUnzipAppResult(bool result, string zipFile)
        {
            LogTool.Instance.Info($"OnUnzipAppResult {result}");
            if (!result)
            {
                LogTool.Instance.Error("OnUnzipAppResult Fail");
                CommonTools.Exit();
                return;
            }
            if (File.Exists(GameConfig.LocaGameAppInfo))
            {
                File.Delete(GameConfig.LocaGameAppInfo);
            }
            File.Move(GameConfig.GameAppInfo, GameConfig.LocaGameAppInfo);
            if (GameConfig.DelZipAfterUnzip)
            {
                File.Delete(zipFile);
            }
            ShowGameServerUI();
        }
        /// <summary>
        /// 显示渠道区服界面
        /// </summary>
        void ShowGameServerUI()
        {
            LogTool.Instance.Info($"ShowGameServerUI");
            this.UpdateTips(TIPS.START_GAME);
            UpdateProgerss(100);
            OpenChannelLoginWindow?.Invoke();
        }
        bool IsNewAppVersion(string oldVersion, string newVersion)
        {
            return VersionCheck(oldVersion, newVersion, 0) || VersionCheck(oldVersion, newVersion, 1) || VersionCheck(oldVersion, newVersion, 2) || VersionCheck(oldVersion, newVersion, 3);
        }
        bool VersionCheck(string oldVersion, string newVersion, int nIdx)
        {

            bool bRet = false;

            string[] vecOld = oldVersion.Split('.');
            string[] vecNew = newVersion.Split('.');
            try
            {
                if (nIdx < vecOld.Length && nIdx < vecNew.Length)
                {
                    if (int.Parse(vecNew[nIdx]) > int.Parse(vecOld[nIdx]))
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception e)
            {
                var msg = string.Format("oldVer:{0}, newVer:{1}, idx:{2}", oldVersion, newVersion, nIdx);
                LogTool.Instance.Error(msg);
            }
            return bRet;
        }
    }
}
