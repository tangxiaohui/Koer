using UpdateSystem.Xml;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

namespace UpdateSystem.Data
{
    public class BackDownloadData
    {
        //下载的url
        private string _downloadUrl;

        public string DownloadUrl
        {
            get { return _downloadUrl; }
            set { _downloadUrl = value; }
        }
        //下载文件存放路径
        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }
        //资源版本，分段版本号
        private string _resVersion;

        public string ResVersion
        {
            get { return _resVersion; }
            set { _resVersion = value; }
        }
        //已下载文件大小
        private int _existSize;

        public int ExistSize
        {
            get { return _existSize; }
            set { _existSize = value; }
        }
        //当前下载大小
        private int _downloadSize;

        public int DownloadSize
        {
            get { return _downloadSize; }
            set { _downloadSize = value; }
        }
        //总文件大小
        private int _totalSize;

        public int TotalSize
        {
            get { return _totalSize; }
            set { _totalSize = value; }
        }

        FileInfo _fileInfo;

        public void Init(string storePath, VersionModel model)
        {
            DownloadUrl = model.ResourceUrl.Replace("\\", "/");
            FilePath = System.IO.Path.Combine(storePath, DownloadUrl.Substring(DownloadUrl.LastIndexOf("/") + 1));
            ResVersion = model.ToVersion;

            FileInfo fileInfo = new FileInfo(FilePath);
            if (fileInfo.Exists)
            {
                _fileInfo = fileInfo;
                ExistSize = (int)fileInfo.Length;
            }

            DownloadSize = ExistSize;
            TotalSize = int.Parse(model.FileSize);
        }

        /// <summary>
        /// 1秒钟调用一次即可
        /// </summary>
        /// <returns></returns>
        public bool Finish()
        {
            if (_fileInfo == null || !_fileInfo.Exists)
            {
                _fileInfo = new FileInfo(FilePath);
            }

            if (_fileInfo.Exists)
            {
                DownloadSize = (int)_fileInfo.Length;
            }
            

            return DownloadSize == TotalSize;
        }

        public void GetDownloadSize(out int downloaded, out int total)
        {
            total = TotalSize;
            downloaded = ExistSize;
            FileInfo fileInfo = new FileInfo(_filePath);
            if (fileInfo != null && fileInfo.Exists)
            {
                downloaded = (int)fileInfo.Length;
            }
        }
    }
}
