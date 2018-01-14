using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using UpdateSystem.Log;
using UpdateSystem.Delegate;
using UpdateSystem.Flow;

namespace UpdateSystem.Download
{
    /// <summary>
    /// http下载类， 支持单个文件下载和多线程下载
    /// </summary>
    public class FileDownload : Download
    {
        static string _TAG = "HttpDownload.cs ";
        //线程休眠时间
        private const int _SLEEP_TIME = 500;
        //每次下载失败，重试次数
        private const int _RETRAY_TIMES = 3;

        //文件总大小
        public int TotalSize;
        //断点续传的情况，本地已经存在的文件大小
        public int ExistFileSize;

        //正在下载
        public bool Downloading;

        public override void Abort(AbortFinishCallback callback)
        {
            base.Abort(callback);
        }

        /// <summary>
        /// 下载单个文件
        /// </summary>
        /// <param name="savePath">保存路径</param>
        /// <param name="url">下载地址</param>
        /// <param name="isContinue">是否断点续传</param>
        /// <param name="isRetryAllways">失败后是否一直重试</param>
        /// <returns>小于0失败</returns>
        public int DownloadFile(string savePath, string url, bool isContinue = true)
        {
            UpdateLog.INFO_LOG(_TAG + "DownloadFile(string strFileName, string url, bool isContinue = true) : " +
                savePath + "," +
                url + "," +
                isContinue);

            int downloadRet = CodeDefine.RET_FAIL;
            long SPosition = 0;

            //如果本地有下载文件，则判断是否断点续传
            FileStream FStream = null;
            try
            {
                ExistFileSize = 0;
                if (File.Exists(savePath) && isContinue)
                {
                    //定向文件写入位置
                    FStream = File.Open(savePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    SPosition = FStream.Length;
                    FStream.Seek(SPosition, SeekOrigin.Current);
                    //已存在文件大小
                    ExistFileSize = (int)SPosition;
                }
                else
                {
                    FStream = new FileStream(savePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    SPosition = 0;
                }

                //开始下载
                downloadRet = HttpDownload(url, FStream, SPosition);
                if (FStream.CanWrite)
                {
                    FStream.Close();
                }
                Thread.Sleep(_SLEEP_TIME);
            }
            catch (System.Exception ex)
            {
                UpdateLog.ERROR_LOG(ex.Message);
                UpdateLog.EXCEPTION_LOG(ex);
            }
            finally
            {
                if (FStream != null && FStream.CanWrite)
                {
                    FStream.Close();
                }
            }

            return downloadRet;
        }

        //下载文件，有重试次数
        public int DownloadFileEx(string savePath, int targetLen, string url, bool isContinue = true)
        {
            int ret = CodeDefine.RET_INIT;
            int retryTimes = _RETRAY_TIMES;
            FileInfo file = new FileInfo(savePath);

            ExistFileSize = 0;
            if (file.Exists && file.Length == targetLen && targetLen != 0)
            {
                ExistFileSize = (int)file.Length;
                ret = CodeDefine.RET_SUCCESS;
            }
            else
            {
                while (retryTimes > 0)
                {
                    retryTimes--;
                    ret = DownloadFile(savePath, url, isContinue);

                    if (ret == CodeDefine.RET_SKIP_BY_ABORT)
                    {
                        return ret;
                    }

                    if (ret <= CodeDefine.RET_INIT)
                        continue;

                    if (targetLen > 0)
                    {
                        //有可能中途断网或者网络波动导致下载中断，但是返回成功，需要判断下载文件的大小和目标大小是否一致
                        file = new FileInfo(savePath);
                        if (file.Length < targetLen)
                        {
                            //需要重置状态
                            ret = CodeDefine.RET_INIT;
                            continue;
                        }
                    }
                    break;
                }
            }

            return ret;
        }

        //使用备份cdn来下载，没有备份cdn则使用普通下载
        public int DownloadUseBackCdn(string savePath, string url, int targetLen, bool isContinue = true)
        {
            Downloading = true;
            DownloadUrl = url;
            ExistFileSize = 0;
            TotalSize = targetLen;

            int ret = CodeDefine.RET_INIT;
            string[] backupcdnList = BackupCdn;
            if (UseBackupCdn(url, backupcdnList))
            {
                for (int i = 0; i < backupcdnList.Length; ++i)
                {
                    string backUrl = url.Replace(backupcdnList[0], backupcdnList[i]);
                    ret = DownloadFileEx(savePath, targetLen, backUrl, isContinue);
                    if (ret >= CodeDefine.RET_SUCCESS) break;
                }
            }
            else
                ret = DownloadFileEx(savePath, targetLen, url, isContinue);

            Downloading = false;
            return ret;
        }

        //获取下载信息，链接、大小、已下载
        public void GetCurDownInfo(out string url, out int total, out int downloaded)
        {
            url = DownloadUrl;
            total = TotalSize;
            downloaded = ExistFileSize + DownloadedSize;
        }
    }
}
