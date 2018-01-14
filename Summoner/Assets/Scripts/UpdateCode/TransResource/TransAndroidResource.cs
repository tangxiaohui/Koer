using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Threading;
using UpdateSystem.Log;
using ICSharpCode.SharpZipLib.Zip;

namespace UpdateSystem.Trans
{
    /// <summary>
    /// 启动时，转移安卓资源
    /// </summary>
    public class TransAndroidResource : TransResource
    {
        public static bool ForTest = false;

        public override void BeginTransRes()
        {
            Thread thread = new Thread(unzipApk);
            thread.Start();
            thread.Join();
        }

        private void unzipApk()
        {
            UpdateLog.DEBUG_LOG("Trans resource from apk!!!");
            UpdateLog.DEBUG_LOG("_resourcePath=" + _resourcePath);

            //res开始路径，从assets开始
            string APKStreamingAsset = "assets/";
            //跳过的资源路径，bin目录不做拷贝
            string skipResDir = "/bin/";
            string apkPath = _resourcePath;
            if (!ForTest)
            {
                apkPath = apkPath.Replace("!/assets", "");
                apkPath = apkPath.Replace("jar:file://", "");
            }


            UpdateLog.DEBUG_LOG("apkPath=" + apkPath);
            UpdateLog.DEBUG_LOG("_outPath=" + _outPath);
            FileStream fileStream = null;
            try
            {
                fileStream = File.OpenRead(apkPath);// new FileStream(apkPath, FileMode.Open);
                ZipFile f = new ZipFile(fileStream);
                nReadCount = (Int32)f.Count;
                fileStream.Seek(0, SeekOrigin.Begin);
            }
            catch (System.Exception ex)
            {
                UpdateLog.ERROR_LOG(ex.Message);
            }

            if (fileStream == null)
            {
                UpdateLog.ERROR_LOG("文件读取失败");
                return;
            }
            
            using (ZipInputStream s = new ZipInputStream(fileStream))
            {
                ZipEntry entry = null;
                while ((entry = s.GetNextEntry()) != null)
                {
                    string unRootPath = _outPath;
                    string directoryName = Path.GetDirectoryName(entry.Name).Replace("\\", "/") + "/";
                    if (directoryName.Contains(skipResDir))
                    {
                        ++nWriteCount;
                        continue;
                    }
                    if (!directoryName.StartsWith(APKStreamingAsset))
                    {
                        ++nWriteCount;
                        continue;
                    }
                    
                    string fileName = Path.GetFileName(entry.Name);
                    // create directory;
                    if (!string.IsNullOrEmpty(directoryName))
                    {
                        unRootPath = Path.Combine(unRootPath, directoryName);
                        unRootPath = unRootPath.Replace(APKStreamingAsset, "");
                        unRootPath = unRootPath.Replace('\\', '/');
                        if (!Directory.Exists(unRootPath))
                        {
                            Directory.CreateDirectory(unRootPath);
                        }
                    }

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        try
                        {
                            fileName = Path.Combine(unRootPath, fileName);
                            fileName = fileName.Replace('\\', '/');
                            using (FileStream streamWriter = File.Create(fileName))
                            {
                                int size = 0;
                                int bufferSize = 512;
                                byte[] tempBuffer = new byte[bufferSize];
                                while (true)
                                {
                                    size = s.Read(tempBuffer, 0, bufferSize);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(tempBuffer, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                streamWriter.Flush();
                                ++nWriteCount;
                                streamWriter.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            UpdateLog.ERROR_LOG(ex.Message);
                            UpdateLog.ERROR_LOG("文件损坏： " + entry.Name);
                        }
                        finally
                        {

                        }
                    }
                    else
                    {
                        ++nWriteCount;
                        UpdateLog.ERROR_LOG("trans error， file name is empty： " + entry.Name);
                    }
                }

                Thread.Sleep(50);

                _success = (nWriteCount == nReadCount && nWriteCount != 0);
                
                UpdateLog.DEBUG_LOG(string.Format("转移资源结束 {0}/{1}", nWriteCount, nReadCount));
            }
        }
    }
}