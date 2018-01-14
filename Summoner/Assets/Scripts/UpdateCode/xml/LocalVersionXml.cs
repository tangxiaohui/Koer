using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;

namespace UpdateSystem.Xml
{
    public partial class LocalVersionXml
    {
        private string _resourceVersionUrl;

        public string ResourceVersionUrl
        {
            get { return _resourceVersionUrl; }
            set { _resourceVersionUrl = value; }
        }

        private string _baseResVersion;

        public string BaseResVersion
        {
            get { return _baseResVersion; }
            set { _baseResVersion = value; }
        }

        private string _patchResVersion;

        public string PatchResVersion
        {
            get { return _patchResVersion; }
            set { _patchResVersion = value; }
        }

        private string _localAppVersion;

        public string LocalAppVersion
        {
            get { return _localAppVersion; }
            set { _localAppVersion = value; }
        }

        //没用
        private string _localResVersion;

        public string LocalResVersion
        {
            get { return _localResVersion; }
            set { _localResVersion = value; }
        }

        private string _fid;

        public string Fid
        {
            get { return _fid; }
            set { _fid = value; }
        }

        private string _fgi;

        public string Fgi
        {
            get { return _fgi; }
            set { _fgi = value; }
        }

        private string _hasCopy;

        public string HasCopy
        {
            get { return _hasCopy; }
            set { _hasCopy = value; }
        }

        private string _developer;

        public string Developer
        {
            get { return _developer; }
            set { _developer = value; }
        }

        private string _enableDownload;

        public string EnableDownload
        {
            get { return _enableDownload; }
            set { _enableDownload = value; }
        }

        public LocalVersionXml()
        {
            _hasCopy = "";
            _fid = "";
            _fgi = "";
            _baseResVersion = "";
            _patchResVersion = "";
            _localAppVersion = "";
            _resourceVersionUrl = "";
        }
    }
}
