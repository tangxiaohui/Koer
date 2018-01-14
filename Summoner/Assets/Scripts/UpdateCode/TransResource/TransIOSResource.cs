using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Threading;
using UpdateSystem.Log;


namespace UpdateSystem.Trans
{
    /// <summary>
    /// UILoginFormScript的逻辑处理类
    /// </summary>
    public class TransIOSResource : TransResource
    {
        //总
        List<string> _winFiles;

        private static TransIOSResource _instance;
        public static TransIOSResource Instance
        {
            get 
            { 
                if (_instance == null)
                {
                    _instance = new TransIOSResource();
                }
                return _instance;
            }
        }


        public override void BeginTransRes()
        {
            Thread thread = new Thread(transIOSRes);
            thread.Start();
            thread.Join();
        }

        
        private void transIOSRes()
        {
            UpdateLog.DEBUG_LOG("Trans resource from ipa!!!");
            string streamPath = _resourcePath;// Application.streamingAssetsPath;
            if (!Directory.Exists(streamPath))
            {
                UpdateLog.ERROR_LOG("转移资源无效，不是有效文件夹路径： " + streamPath);
                return;
            }
            _winFiles = new List<string>(Directory.GetFiles(streamPath, "*", SearchOption.AllDirectories));
            nReadCount = _winFiles.Count;
            while (true)
            {
                string file = null;
                if (_winFiles.Count > 0)
                {
                    file = _winFiles[0];
                    _winFiles.RemoveAt(0);
                }
                if (file != null)
                {
                    try
                    {
                        string oldFilePath = file.Replace('\\', '/');
                        string newFilePath = file.Replace(streamPath, _outPath).Replace('\\', '/');
                        string fullPath = newFilePath.Substring(0, newFilePath.LastIndexOf('/'));
                        if (!Directory.Exists(fullPath))
                        {
                            Directory.CreateDirectory(fullPath);
                        }
                        CopyFile(oldFilePath, newFilePath);
                        nWriteCount++;
                    }
                    catch (IOException ex)
                    {
                        UpdateLog.ERROR_LOG(ex.Message);
                    }
                }
                else
                {
                    break;
                }
                //Thread.Sleep(10);
            }

            Thread.Sleep(50);
            _success = (nWriteCount == nReadCount && nWriteCount != 0);

            UpdateLog.ERROR_LOG(string.Format("转移资源结束 {0}/{1}", nWriteCount, nReadCount));
        }

        

        public static void CopyFile(string sourceFileName, string destFileName)
        {
            try
            {
                File.Copy(sourceFileName, destFileName, true);
            }
            catch (IOException copyError)
            {
                UpdateLog.ERROR_LOG(copyError.Message);
            }
        }
    }
}