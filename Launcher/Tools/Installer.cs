using System;
using System.IO.Compression;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using Tools;

namespace Launcher
{
    public sealed class Installer
    {
        /// <summary>
        ///     Zip解压并更新目标文件
        /// </summary>
        /// <param name="zipFilePath">Zip压缩包路径</param>
        /// <param name="unZipDir">解压目标路径</param>
        /// <returns></returns>
        public static void UnZip(string zipFilePath, string unZipDir,Action<float> OnProgressHandler, Action<bool, string> onUnzipAppResult)
        {
            Task.Run(() =>
            {
                bool IsUnzipFinish = false;
                try
                {
                    unZipDir = unZipDir.EndsWith(@"\") ? unZipDir : unZipDir + @"\";
                    var directoryInfo = new DirectoryInfo(unZipDir);
                    if (!directoryInfo.Exists)
                        directoryInfo.Create();
                    var fileInfo = new FileInfo(zipFilePath);
                    if (!fileInfo.Exists)
                        return;
                    using (
                        var zipToOpen = new FileStream(zipFilePath, FileMode.Open, FileAccess.ReadWrite,
                            FileShare.Read))
                    {
                        using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                        {
                            int nOfFiles = archive.Entries.Count;
                            int nofUnzip = 0;
                            foreach (var zipArchiveEntry in archive.Entries)
                            {
                                nofUnzip += 1;

                                var entryFilePath = Regex.Replace(zipArchiveEntry.FullName.Replace("/", @"\"),
                                        @"^\\*", "");
                                var filePath = directoryInfo + entryFilePath; //设置解压路径
                                if (zipArchiveEntry.FullName.EndsWith("/"))
                                {
                                    
                                    if (!Directory.Exists(filePath))
                                    {
                                        Directory.CreateDirectory(filePath);
                                    }
                                    continue;
                                }
                                zipArchiveEntry.ExtractToFile(filePath);
                                OnProgressHandler?.Invoke(nofUnzip * 1f / nOfFiles);
                                if (0 == nofUnzip % 1000)
                                    Thread.Sleep(100);
                            }
                            IsUnzipFinish = true;
                        }
                    }
                }
                catch (Exception exception)
                {
                    LogTool.Instance.Error(exception.ToString());
                    IsUnzipFinish = false;
                }
                finally
                { 
                    onUnzipAppResult?.Invoke(IsUnzipFinish, zipFilePath);
                }
            });
        }
        /// <summary>
        /// 调用bat删除目录，以防止系统底层的异步删除机制
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static bool DeleteDirectoryWithCmd(string dirPath)
        {
            var process = new Process(); //string path = ...;//bat路径  
            var processStartInfo = new ProcessStartInfo("CMD.EXE", "/C rd /S /Q \"" + dirPath + "\"")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true
            }; //第二个参数为传入的参数，string类型以空格分隔各个参数  
            process.StartInfo = processStartInfo;
            process.Start();
            process.WaitForExit();
            var output = process.StandardOutput.ReadToEnd();
            if (string.IsNullOrWhiteSpace(output))
                return true;
            return false;
        }
        /// <summary>
        /// 调用bat删除文件，以防止系统底层的异步删除机制
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool DelFileWithCmd(string filePath)
        {
            var process = new Process(); //string path = ...;//bat路径  
            var processStartInfo = new ProcessStartInfo("CMD.EXE", "/C del /F /S /Q \"" + filePath + "\"")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true
            }; //第二个参数为传入的参数，string类型以空格分隔各个参数  
            process.StartInfo = processStartInfo;
            process.Start();
            process.WaitForExit();
            var output = process.StandardOutput.ReadToEnd();
            if (output.Contains(filePath))
                return true;
            return false;
        }
    }
}
