using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;
using System.IO;
using UpdateSystem.Log;
using UpdateSystem.Flow;

namespace UpdateSystem.Data
{
    /// <summary>
    /// 解压下载的二进制基础资源文件
    /// </summary>
    public class UnzipResource
    {
        string _TAG = "UnzipResource.cs ";

        string _resPath;

        object _lockObj = new object();

        public string ResPath
        {
            get { return _resPath; }
            set { _resPath = value; }
        }
        string _outPath;

        public string OutPath
        {
            get { return _outPath; }
            set { _outPath = value; }
        }

        public UnzipResource(string resFile, string unzipPath)
        {
            _resPath = resFile;
            _outPath = unzipPath;
        }

        /// <summary>
        /// 释放资源
        /// 文件结构 4x32 文件头 |32位记录路径长度|32位记录文件名|32位记录md5长度|32位记录文件大小|
        /// 然后根据这4个结构获取具体的路径、文件名、md5值、文件内容
        /// </summary>
        /// <returns></returns>
        public int UnzipRes()
        {
            UpdateLog.INFO_LOG(_TAG + "unzipRes()");

            int ret = CodeDefine.RET_SUCCESS;
            FileStream resFileStream = null;
            long filePostion = 0;
            long fileSize = 0;
            try
            {
                resFileStream = new FileStream(_resPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                fileSize = resFileStream.Length;

                List<ResourceFileData> fileDataList = new List<ResourceFileData>();

                //启动线程池
                // var threadPool = new HttpThreadPool<UnzipData>(4, ThreadFunc);
                while (fileSize > 0 && filePostion < fileSize)
                {
                    ResourceFileData fileData = new ResourceFileData();

                    //4x32长度的头
                    fileData.DirLen = int.Parse(read(resFileStream, 32, filePostion, out filePostion));
                    fileData.NameLen = int.Parse(read(resFileStream, 32, filePostion, out filePostion));
                    fileData.Md5Len = int.Parse(read(resFileStream, 32, filePostion, out filePostion));
                    fileData.FileSize = long.Parse(read(resFileStream, 32, filePostion, out filePostion));

                    //读取内容
                    fileData.Dir = read(resFileStream, fileData.DirLen, filePostion, out filePostion);
                    fileData.Name = read(resFileStream, fileData.NameLen, filePostion, out filePostion);
                    fileData.Md5 = read(resFileStream, fileData.Md5Len, filePostion, out filePostion);

                    //跳过localversion的释放
                    if (fileData.Name.ToLower().Equals("localversion.xml"))
                    {
                        resFileStream.Seek(fileData.FileSize, SeekOrigin.Current);
                    }
                    else
                    {

                        writeFile(resFileStream, fileData.FileSize, fileData.Dir, fileData.Name);

                        //resFileStream.Seek(fileData.FileSize, SeekOrigin.Current);
                        //UnzipData ud = new UnzipData(fileData, filePostion, fileSize);
                        //threadPool.addTask(ud);
                    }
                    filePostion += fileData.FileSize;
                }

                //等待所有文件下载完
                //threadPool.waitWhileWorking();
            }
            catch (System.Exception ex)
            {
                ret = CodeDefine.RET_FAIL_UNZIP_RES_FILE;
                UpdateLog.ERROR_LOG(_TAG + ex.Message + "\n" + ex.StackTrace);
                UpdateLog.EXCEPTION_LOG(ex);
            }
            finally
            {
                if (resFileStream != null)
                {
                    resFileStream.Close();
                }
            }
            return ret;
        }

        long _unzipSizeCounter = 0;
        private void ThreadFunc(UnzipBaseResData data)
        {
            var resFileStream = new FileStream(_resPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            resFileStream.Seek(data.StartPos, SeekOrigin.Begin);
            writeFile(resFileStream, data.FileData.FileSize, data.FileData.Dir, data.FileData.Name);
            resFileStream.Close();
            lock (_lockObj)
            {
                _unzipSizeCounter += data.FileData.FileSize;
            }
        }

        private void writeFile(FileStream resFileStream, long fileLen, string dir, string name)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(_outPath + "/" + dir);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            string filePath = _outPath + "/" + dir + name;
            FileStream writeStream = new FileStream(filePath, FileMode.Create);

            byte[] buffer = null;
            long bufferSize = 1024;
            if (fileLen < bufferSize)
            {
                bufferSize = fileLen;
            }

            buffer = new byte[bufferSize];
            do
            {
                resFileStream.Read(buffer, 0, (int)bufferSize);
                writeStream.Write(buffer, 0, (int)bufferSize);

                fileLen -= bufferSize;
                if (fileLen < 1024)
                {
                    bufferSize = fileLen;
                }
            } while (fileLen > 0);

            writeStream.Close();
        }

        private string read(FileStream resFileStream, int readLen, long position, out long filePosition)
        {
            Byte[] beginBuf = new Byte[readLen];
            resFileStream.Read(beginBuf, 0, readLen);
            filePosition = position + readLen;
            string retStr = System.Text.Encoding.Default.GetString(beginBuf);
            return retStr;
        }

        //内嵌数据结构
        private class UnzipBaseResData
        {
            public ResourceFileData FileData { get; set; }
            //指向stream中的起始位置
            public long StartPos { get; set; }
            ////源文件，base资源
            //public string SourcePath { get; set; }
            ////源文件总大小
            public long SourceSize { get; set; }

            public UnzipBaseResData(ResourceFileData fileData, long pos, long sourceSize)
            {
                FileData = fileData;
                StartPos = pos;
                //             SourcePath = path;
                SourceSize = sourceSize;
            }
        }
    }
}