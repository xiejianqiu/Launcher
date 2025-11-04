using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
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
                                if (!zipArchiveEntry.FullName.EndsWith("/"))
                                {
                                    var entryFilePath = Regex.Replace(zipArchiveEntry.FullName.Replace("/", @"\"),
                                        @"^\\*", "");
                                    var filePath = directoryInfo + entryFilePath; //设置解压路径
                                    var content = new byte[zipArchiveEntry.Length];
                                    zipArchiveEntry.Open().Read(content, 0, content.Length);

                                    //if (File.Exists(filePath) && content.Length == new FileInfo(filePath).Length)
                                    //    continue; //跳过相同的文件，否则覆盖更新

                                    var sameDirectoryNameFilePath = new DirectoryInfo(filePath);
                                    if (sameDirectoryNameFilePath.Exists)
                                    {
                                        sameDirectoryNameFilePath.Delete(true);
                                        DeleteDirectoryWithCmd(filePath);
                                        /*if (!DeleteDirectoryWithCmd(filePath))
                                        {
                                            Console.WriteLine(filePath + "删除失败");
                                            resualt = false;
                                            break;
                                        }*/
                                    }
                                    var sameFileNameFilePath = new FileInfo(filePath);
                                    if (sameFileNameFilePath.Exists)
                                    {
                                        sameFileNameFilePath.Delete();
                                        DelFileWithCmd(filePath);
                                        /*if (!DelFileWithCmd(filePath))
                                        {
                                            Console.WriteLine(filePath + "删除失败");
                                            resualt = false;
                                            break;
                                        }*/
                                    }
                                    var greatFolder = Directory.GetParent(filePath);
                                    if (!greatFolder.Exists) greatFolder.Create();
                                    File.WriteAllBytes(filePath, content);
                                    OnProgressHandler?.Invoke(nofUnzip * 1f / nOfFiles);
                                    if (0 == nofUnzip % 10)
                                        Thread.Sleep(100);
                                }
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
