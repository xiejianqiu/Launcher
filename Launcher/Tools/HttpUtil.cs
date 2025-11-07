using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tools;
using System.Reflection.Metadata;
using System.IO;
using System.Security.Cryptography;
using System.Windows;

namespace Launcher
{

    public static class HttpUtil
    {
        const int TIME_SPAN = 60;
        public class WebClientEx : WebClient
        {
            private readonly long from;
            private readonly long to;

            public WebClientEx(long from, long to)
            {
                this.from = from;
                this.to = to;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = (HttpWebRequest)base.GetWebRequest(address);
                request.AddRange(this.from, this.to);
                return request;
            }
        }
        public static async void DownloadFile(string url, Action<bool,string> OnCallBack)
        {
            try
            {
                using (var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(TIME_SPAN) })
                {
                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsByteArrayAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var content = Encoding.ASCII.GetString(responseBody);
                        if (null != OnCallBack)
                        {
                            OnCallBack(true, content);
                        }
                    }
                    else
                    {
                        LogTool.Instance.Error($"{response.StatusCode}, {url}");
                        OnCallBack(false, string.Empty);
                    }
                }
            } catch (Exception e)
            {
                LogTool.Instance.Error(e.ToString());
                if (null != OnCallBack)
                {
                    OnCallBack(false, string.Empty);
                }
            }
        }
        public static async void DownBigFile(string url,string savePath,string md5, Action<bool, string> dlFinish = null, DownloadProgressChangedEventHandler progressHandler = null)
        {

            DateTime curTime = DateTime.Now;
            bool IsSuccess = false;
            try
            {
                var tmpDownloadPath = savePath + ".tmp";
                using (WebClient client = new WebClient())
                {
                    if (File.Exists(tmpDownloadPath))
                    {
                        File.Delete(tmpDownloadPath);
                    }
                    client.DownloadProgressChanged += progressHandler;
                    await client.DownloadFileTaskAsync(url, tmpDownloadPath);
                    if (!string.IsNullOrEmpty(md5))
                    {
                        var tmd5 = GetMD5Hash(tmpDownloadPath);
                        IsSuccess = tmd5 == md5;
                    }
                    else
                    {
                        IsSuccess = true;
                    }
                    if (IsSuccess)
                    {
                        File.Move((string)tmpDownloadPath, (string)savePath);
                    }
                    else
                    {
                        if (File.Exists(tmpDownloadPath))
                        {
                            File.Delete(tmpDownloadPath);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogTool.Instance.Error($"DownUpDateeRes UseTime:{(DateTime.Now - curTime).Seconds} {url}\n {e.ToString()}");
                if (File.Exists((string)savePath))
                {
                    File.Delete((string)savePath);
                }
                var result = MessageBox.Show(TIPS.DOWNLOAD_FAILED);
                if (result == MessageBoxResult.OK)
                {
                    CommonTools.Exit();
                }

            }
            finally
            {
                dlFinish?.Invoke(IsSuccess, savePath);
            }
        }
        public static string GetMD5Hash(string pathName)
        {
            string result = string.Empty;
            string text = string.Empty;
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            try
            {
                FileStream fileStream = new FileStream(pathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                byte[] value = mD5CryptoServiceProvider.ComputeHash(fileStream);
                fileStream.Close();
                text = BitConverter.ToString(value);
                text = text.Replace("-", string.Empty);
                result = text;
            }
            catch
            {

            }
            return result;
        }
    }
}
