using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;
using UpdateSystem.Download;
using UpdateSystem.Flow;

namespace UpdateSystem.Data
{
    public class MapFileData
    {
        int _begin;

        public int Begin
        {
            get { return _begin; }
            set { _begin = value; }
        }
        int _end;

        public int End
        {
            get { return _end; }
            set { _end = value; }
        }
        int _dirLen;

        public int DirLen
        {
            get { return _dirLen; }
            set { _dirLen = value; }
        }
        int _nameLen;

        public int NameLen
        {
            get { return _nameLen; }
            set { _nameLen = value; }
        }
        int _md5Len;

        public int Md5Len
        {
            get { return _md5Len; }
            set { _md5Len = value; }
        }
        int _fileSize;

        public int FileSize
        {
            get { return _fileSize; }
            set { _fileSize = value; }
        }
        string _dir;

        public string Dir
        {
            get { return _dir; }
            set { _dir = value; }
        }
        string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        string _md5;

        public string Md5
        {
            get { return _md5; }
            set { _md5 = value; }
        }

        string _resUrl;

        public string ResUrl
        {
            get { return _resUrl; }
            set { _resUrl = value; }
        }

        string _saveDir;

        public string SaveDir
        {
            get { return _saveDir; }
            set { _saveDir = value; }
        }

        internal object ArgObj;
        internal UpdateSystem.Delegate.UpdateAction<string, bool, object> DownloadCallBack;
        internal bool Downloaded;
        internal bool Downloading;
        internal string FullPath;
        internal DataLevel DataLevel = DataLevel.Low;
        //文件下载的错误代码
        internal int ErrorCode = CodeDefine.RET_INIT;
    }
}
