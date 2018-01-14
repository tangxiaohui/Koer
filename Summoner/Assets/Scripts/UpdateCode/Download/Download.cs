using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using UpdateSystem.Log;
using UpdateSystem.Delegate;
using UpdateSystem.Flow;
using UpdateSystem.Data;

namespace UpdateSystem.Download
{
    /// <summary>
    /// http下载类， 支持单个文件下载和多线程下载
    /// </summary>
    public class Download
    {
        //备份cdn地址，台湾用, 第一个地址是默认地址
        public static string[] BackupCdn = null;

        static string _TAG = "HttpDownload.cs ";
        //写文件buffer
        private const int _BUFFER = 1024 * 5;
        //线程锁
        private object m_locker = new object();
        //http请求句柄列表，用于终止下载用
        private HttpWebRequest _requestIns;

        //是否中斷操作，如果是中斷，则下载结果置为success
        private bool _isAbortOption = false;

        //已下载
        public int DownloadedSize { get; set; }
        public string DownloadUrl { get; set; }

        //下载的数据
        public MapFileData MapFileData { get; set; }

        //多线程下载的文件大小,必须是静态的
        public static int MutiDownloadedSize { get; set; }
        //用来计算下载速度用的
        private static int _oldDownloadedSize;
        private static DateTime _time;

        public virtual void Abort(AbortFinishCallback callback)
        {
            lock (m_locker)
            {
                _isAbortOption = true;
                //if (_requestIns != null)
                //{
                //    _requestIns.Abort();
                //}

                _requestIns = null;
            }
            if(callback != null) callback(true);
        }

        /// <summary>
        /// 下载函数
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="outFile">输入路径</param>
        /// <param name="begin">起点</param>
        /// <param name="end">终点</param>
        /// <returns>小于0失败</returns>
        public int HttpDownload(string url, FileStream outFile, long begin = 0, long end = 0)
        {
            int num = CodeDefine.RET_INIT;
            _requestIns = null;
            try
            {
                DownloadUrl = url;
                _requestIns = createRequest(url);
                setRange(_requestIns, (int)begin, (int)end);
                requestData(_requestIns, outFile);
                num = CodeDefine.RET_SUCCESS;
            }
            catch (Exception exception)
            {
                UpdateLog.WARN_LOG("catch http instance exception :::" + exception.Message);
                if (!_isAbortOption)
                {
                    UpdateLog.ERROR_LOG(_TAG + exception.Message + "\n" + exception.StackTrace);
                    UpdateLog.EXCEPTION_LOG(exception);
                    num = CodeDefine.RET_FAIL_EXCEPTION_DOWNLOAD;
                }
                UpdateLog.ERROR_LOG("HttpDownload： " + outFile.Name);
            }
            finally
            {
                if ((outFile != null) && outFile.CanWrite)
                {
                    outFile.Close();
                }
                _requestIns = null;
                if (_isAbortOption)
                {
                    num = CodeDefine.RET_SKIP_BY_ABORT;
                }
            }
            _isAbortOption = false;
            if (MapFileData != null)
            {
                MapFileData.ErrorCode = num;
            }
            return num;
        }

        //使用备份cdn，比如台湾就有要求
        public static bool UseBackupCdn(string url, string[] backcdnList)
        {
            if (backcdnList != null && backcdnList.Length > 0 && !string.IsNullOrEmpty(backcdnList[0]) && url.IndexOf(backcdnList[0]) >= 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 重置已经下载的大小
        /// </summary>
        public static void ResetDownloadedSize()
        {
            MutiDownloadedSize = 0;
            _oldDownloadedSize = 0;
            _time = DateTime.Now;
        }

        /// <summary>
        /// 调用这个函数获取前后两次下载大小的差值，用来计算下载速度
        /// </summary>
        /// <returns>kb/s</returns>
        public static int GetDownloadSpeed()
        {
            if (_oldDownloadedSize == 0)
            {
                _oldDownloadedSize = MutiDownloadedSize;
                _time = DateTime.Now;
                return 0;
            }

            int deltaSize = MutiDownloadedSize - _oldDownloadedSize;
            float deltaTime = (float)(DateTime.Now - _time).TotalMilliseconds;
            _oldDownloadedSize = MutiDownloadedSize;
            _time = DateTime.Now;
            return (int)((deltaSize / 1024) / deltaTime * 1000);
        }
        
        private HttpWebRequest createRequest(string url)
        {
            HttpWebRequest myRequest = null;
            Uri uri = new Uri(url);
            myRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
            myRequest.Proxy = null;
            myRequest.ProtocolVersion = HttpVersion.Version10;
            myRequest.Method = "GET";
            myRequest.KeepAlive = false;
            myRequest.AllowAutoRedirect = false;
            myRequest.ServicePoint.Expect100Continue = false;
            //多线程使用http时，需要设置这个，默认是2
            ServicePointManager.DefaultConnectionLimit = 200;
            myRequest.Timeout = 10000;
            myRequest.ReadWriteTimeout = 10000;
            if (url.StartsWith("https://"))
            {
                ServicePointManager.ServerCertificateValidationCallback = 
                    new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                myRequest.ContentType = "application/x-www-form-urlencoded";
                myRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            }
            else
            {
                myRequest.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:19.0) Gecko/20100101 Firefox/19.0";
            }

            return myRequest;
        }

        public bool CheckValidationResult(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, 
            System.Security.Cryptography.X509Certificates.X509Chain chain, 
            System.Net.Security.SslPolicyErrors errors) 
        { 
                // Always accept
             return true; //总是接受
        }

        private void setRange(HttpWebRequest request, int begin, int end)
        {
            if (end > begin)
            {
                request.AddRange(begin, end);
            }
            else
                request.AddRange(begin);
        }

        private void requestData(HttpWebRequest myRequest, FileStream outFile)
        {
            byte[] btContent = new byte[_BUFFER];
            HttpWebResponse reponse = null;
            Stream myStream = null;
            try
            {
                using (reponse = myRequest.GetResponse() as HttpWebResponse)
                {
                    using (myStream = reponse.GetResponseStream())
                    {
                        int intSize = 0;
                        int readLen = 0;
                        intSize = myStream.Read(btContent, 0, _BUFFER);
                        while (intSize > 0)
                        {
                            readLen += intSize;
                            //已下载
                            DownloadedSize = readLen;

                            lock (m_locker)
                            {
                                MutiDownloadedSize += intSize;
                                //中断下载
                                if (_isAbortOption)
                                {
                                    UpdateLog.WARN_LOG("abort download : " + Thread.CurrentThread.Name);
                                    break;
                                }
                            }

                            outFile.Write(btContent, 0, intSize);
                            if (!myStream.CanRead)
                            {
                                break;
                            }
                            intSize = myStream.Read(btContent, 0, _BUFFER);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UpdateLog.ERROR_LOG(_TAG + ex.Message + "\n" + ex.StackTrace);
                if (reponse != null) UpdateLog.ERROR_LOG("Http status: " + reponse.StatusCode);
            }
            finally
            {
                if (reponse != null) reponse.Close();
                if (myStream != null) myStream.Close();
                if (outFile != null) outFile.Flush();
            }
            
            btContent = null;
        }
    }
}
