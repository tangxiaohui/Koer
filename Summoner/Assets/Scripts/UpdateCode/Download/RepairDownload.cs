using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using UpdateSystem.Log;
using UpdateSystem.Data;
using UpdateSystem.Delegate;
using UpdateSystem.Flow;

namespace UpdateSystem.Download
{
    /// <summary>
    /// 资源修复时的下载，是多线程下载
    /// </summary>
    public class RepairDownload
    {
        static string _TAG = "HttpDownload.cs ";

        //最大支持线程数
        private const int _MAX_THREAD_COUNT = 4;
        //下载文件失败超过10个，则表示本次更新失败
        private const int _MAX_ERROR_LIMIT = 10; 
        //线程休眠时间
        private const int _SLEEP_TIME = 1000;
        //每次下载失败，重试次数
        private const int _RETRAY_TIMES = 3;

        //下载实例列表，多线程中会用到，最多会有线程个数的实例
        private static List<Download> _httpInsList = new List<Download>();

        //保存下载失败的文件，用于重试
        private List<MapFileData> _failDownloadFile = new List<MapFileData>();
        //总需要下载的文件数
        private int _totalDownloadFileCount = 0;
        //当前已经下载的文件数
        private int _currentDownloadFileCount = 0;
        private bool _abort;

        //线程锁
        object m_locker = new object();

        private HttpThreadPool<MapFileData> _threadPool = null;

        public void AbortAll(AbortFinishCallback callback)
        {
            lock (m_locker)
            {
                _abort = true;
                if (_threadPool != null)
                {
                    _threadPool.stop();
                }

                for (int i = 0; i < _httpInsList.Count; ++i)
                {
                    lock (m_locker)
                    {
                        _httpInsList[i].Abort(null);
                    }
                }
            }

            if (callback != null)
            {
                callback(true);
            }
        }

        /// <summary>
        /// 多线程下载文件
        /// </summary>
        /// <param name="mapFileDataList">文件列表</param>
        /// <returns>小于0失败</returns>
        public int DownloadFileByMultiThread(List<MapFileData> mapFileDataList)
        {
            int ret = CodeDefine.RET_FAIL;
            int allRetryTimes = _RETRAY_TIMES;
            Download.MutiDownloadedSize = 0;
            _abort = false;

            if (mapFileDataList == null)
            {
                ret = CodeDefine.RET_FAIL;
                return ret;
            }
            while (_failDownloadFile.Count <= _MAX_ERROR_LIMIT && allRetryTimes > 0)
            {
                _currentDownloadFileCount = 0;
                _failDownloadFile.Clear();
                _totalDownloadFileCount = mapFileDataList.Count;

                _threadPool = new HttpThreadPool<MapFileData>(_MAX_THREAD_COUNT, ThreadCallBack);
                for (int i = 0; i < mapFileDataList.Count; i++)
                {
                    MapFileData fileData = mapFileDataList[i];
                    _threadPool.addTask(fileData);
                }

                //等待所有文件下载完
                _threadPool.waitWhileWorking();

                if (_abort)
                {
                    UpdateLog.DEBUG_LOG("abort repaire download!!!");
                    _abort = false;
                    return CodeDefine.RET_SKIP_BY_ABORT;
                }

                //当失败文件数小于_MAX_ERROR_LIMIT，则这些文件重新加到下载队列里面
                if (_failDownloadFile.Count != 0)
                {
                    for (int i = 0; i < _failDownloadFile.Count; i++)
                    {
                        UpdateLog.DEBUG_LOG("有文件下载失败" + _failDownloadFile[i].Name);
                    }
                    mapFileDataList.Clear();
                    mapFileDataList.AddRange(_failDownloadFile);
                    //_failDownloadFile.Clear();
                    ret = CodeDefine.RET_FAIL;
                }
                else
                {
                    mapFileDataList.Clear();
                    _failDownloadFile.Clear();
                    ret = CodeDefine.RET_SUCCESS;
                    break;
                }
                allRetryTimes--;
            } 

            //到这里还有文件没有下载成功，则表示下载失败了
            if (mapFileDataList.Count > 0)
            {
                UpdateLog.DEBUG_LOG("更新失败，有" + mapFileDataList.Count + "个文件下载失败");
            }

            return ret;
        }


        //线程回调方法，用来做具体的下载动作
        public void ThreadCallBack(object state)
        {
            if (_abort)
            {
                return;
            }

            MapFileData fileData = (MapFileData)state;
            string saveFilePath = fileData.SaveDir + "/" + fileData.Dir + fileData.Name;

            //如果下载的文件失败个数超过10个，那么后面的文件就没有必要再下载了，需要检查网络
            lock (this._failDownloadFile)
            {
                if (_failDownloadFile.Count >= _MAX_ERROR_LIMIT)
                {
                    _failDownloadFile.Add(fileData);
                    return;
                }
            }

            int ret = downloadMapData(fileData, saveFilePath);

            lock (m_locker)
            {
                _currentDownloadFileCount++;

                if (ret == CodeDefine.RET_SKIP_BY_ABORT)
                {
                    UpdateLog.DEBUG_LOG("abort download: " + Thread.CurrentThread.Name);
                    return;
                }

                //当前文件下载失败，放到失败列表中
                if (ret <= CodeDefine.RET_FAIL)
                {
                    _failDownloadFile.Add(fileData);
                    return;
                }

                //文件下载成功后，对比md5是否正确，不正确也放到失败队列中
                string downloadFileMD5 = MD5.MD5File(saveFilePath);
                if (string.IsNullOrEmpty(downloadFileMD5) || !fileData.Md5.Equals(downloadFileMD5))
                {
                    _failDownloadFile.Add(fileData);
                }
            }

        }

        private int downloadMapData(MapFileData fileData, string saveFilePath)
        {
            int ret = CodeDefine.RET_FAIL;

            try
            {
                lock (m_locker)
                {
                    if (!Directory.Exists(fileData.SaveDir + "/" + fileData.Dir))
                    {
                        Directory.CreateDirectory(fileData.SaveDir + "/" + fileData.Dir);
                    }
                }

                //计算下载点，在资源包中，每个文件都有4个32字节的数据头加上文件名、md5长度，要跳过
                long begin = fileData.Begin + 32 * 4 + fileData.DirLen + fileData.NameLen + fileData.Md5Len;
                //http的AddRange方法是闭包的，所以减一。（[from, to])
                long end = fileData.End - 1;

                //每个文件有3次下载机会
                int i = _RETRAY_TIMES;
                while (i > 0)
                {
                    i--;
                    if (fileData.Name.Contains("RemoteVersion.xml") || fileData.Name.ToLower().Contains("localversion.xml"))
                    {
                        ret = CodeDefine.RET_SUCCESS;
                        return ret;
                    }

                    if (Download.UseBackupCdn(fileData.ResUrl, Download.BackupCdn))
                    {
                        //使用台湾备份cdn地址
                        for (int cdnIndex = 0; cdnIndex < Download.BackupCdn.Length; ++cdnIndex)
                        {
                            string backupUrl = fileData.ResUrl.Replace(Download.BackupCdn[0], Download.BackupCdn[cdnIndex]);
                            using (FileStream outFile = new FileStream(saveFilePath, FileMode.Create))
                            {
                                ret = httpDownload(backupUrl, outFile, begin, end);
                                if (ret >= CodeDefine.RET_SUCCESS)
                                {
                                    break;
                                }

                                if (ret == CodeDefine.RET_SKIP_BY_ABORT)
                                {
                                    return ret;
                                }
                            }
                        }

                        if (ret >= CodeDefine.RET_SUCCESS)
                        {
                            break;
                        }
                    }
                    else
                    {
                        using (FileStream outFile = new FileStream(saveFilePath, FileMode.Create))
                        {
                            //UpdateLog.INFO_LOG(_TAG + " download: " + saveFilePath);
                            ret = httpDownload(fileData.ResUrl, outFile, begin, end);
                            if (ret >= CodeDefine.RET_SUCCESS)
                            {
                                break;
                            }

                            if (ret == CodeDefine.RET_SKIP_BY_ABORT)
                            {
                                return ret;
                            }

                            UpdateLog.WARN_LOG(_TAG + " try download i = " + i);
                            Thread.Sleep(_SLEEP_TIME);
                        }
                    }
                }

            }
            catch (System.Exception ex)
            {
                if (saveFilePath.Contains("ClassesResources.xml"))
                {
                    ret = CodeDefine.RET_SUCCESS;
                }
                UpdateLog.ERROR_LOG(_TAG + "ThreadCallBack(object state) download fail: file= " + saveFilePath + "\n error" + ex.Message + "\n" + ex.StackTrace);
                UpdateLog.EXCEPTION_LOG(ex);
            }

            return ret;
        }

        private int httpDownload(string url, FileStream outFile, long begin = 0, long end = 0)
        {
            lock (m_locker)
            {
                if (_abort)
                {
                    return CodeDefine.RET_SUCCESS;
                }
            }
            Download downloadIns = new Download();
            addIns(downloadIns);
            int ret = downloadIns.HttpDownload(url, outFile, begin, end);
            removeIns(downloadIns);

            return ret;
        }

        private void addIns(Download ins)
        {
            lock (m_locker)
            {
                _httpInsList.Add(ins);
            }
        }

        private void removeIns(Download ins)
        {
            lock (m_locker)
            {
                _httpInsList.Remove(ins);
            }
        }
    }
}
