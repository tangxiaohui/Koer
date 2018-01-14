using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Common
{
    public static class PathUtils
    {

        private readonly static string _persistentDataPath = Application.persistentDataPath;
        private readonly static string _streamingAssetsPath = Application.streamingAssetsPath;
        private readonly static string _dataPath = Application.dataPath;

        private static RuntimePlatform _platform = RuntimePlatform.WindowsEditor;
        static PathUtils()
        {
            _platform = Application.platform;
        }

        public static RuntimePlatform Platform
        {
            get { return _platform; }
        }

        public static string STREAMING_ASSET_PATH // streamingassets下
        {
            get
            {
                StringBuilder sb = new StringBuilder();
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                sb.Append("file://");
                sb.Append(_streamingAssetsPath); 
                sb.Append(Path.DirectorySeparatorChar);
#elif UNITY_ANDROID
                sb.Append(_streamingAssetsPath);
                sb.Append(Path.DirectorySeparatorChar);
#elif UNITY_IOS
                sb.Append("file://");
                sb.Append(_streamingAssetsPath);
                sb.Append(Path.DirectorySeparatorChar);
#endif
                return sb.ToString();
            }
        }

        public static string PERSISTENT_DATA_PATH  //persistentdata
        {
            get
            {
                StringBuilder sb = new StringBuilder();
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                sb.Append("file:///");
                sb.Append(_persistentDataPath);
                sb.Append(Path.DirectorySeparatorChar);
#elif UNITY_ANDROID
                sb.Append("file:///");
                sb.Append(_persistentDataPath);
                sb.Append(Path.DirectorySeparatorChar);
#elif UNITY_IOS
                sb.Append("file:///");
                sb.Append(_persistentDataPath);
                sb.Append(Path.DirectorySeparatorChar);
#endif
                return sb.ToString();
            }
        }

        public static String TEXT_ROOT
        {
            get
            {
                return PERSISTENT_DATA_PATH + "/Texts/";
            }
        }
        public static string AssetsPathMappingConfigPath
        {
            get
            {
                return PERSISTENT_DATA_PATH + "/AssetsPathMapping.fls";
            }
        }
        public static string GAME_DATA_ROOT
        {
            get
            {
                return PERSISTENT_DATA_PATH;
            }
        }
        public static string LocalPatchVersionPath
        {
            get
            {
                return PERSISTENT_DATA_PATH + "/Patch/LocalPatchVersion.xml";
            }
        }
        public static string RemoteSavePatchVersionPath
        {
            get
            {
                return PERSISTENT_DATA_PATH + "/Patch/RemotePatchVersion.xml";
            }
        }
        public static string ClassesResourcesFLSPath
        {
            get
            {
                var path1 = PERSISTENT_DATA_PATH + "/ClassesResources.fls";
                if (!System.IO.File.Exists(path1))
                {
                    path1 = _streamingAssetsPath + "/ClassesResources.fls";
                }
                return path1;
            }
        }

        static string _photoPath = string.Empty;

        public static string PhotoPath
        {
            get
            {
                if (string.IsNullOrEmpty(_photoPath))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(_persistentDataPath);
                    sb.Append(Path.DirectorySeparatorChar);
                    sb.Append("Photo");
                    sb.Append(Path.DirectorySeparatorChar);
                    _photoPath = sb.ToString();
                }
                return _photoPath;
            }
        }

        static string _accountInfo = string.Empty;

        public static string AccountInfo
        {
            get
            {
                if (string.IsNullOrEmpty(_accountInfo))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(_persistentDataPath);
                    sb.Append(Path.DirectorySeparatorChar);
                    sb.Append("AccountInfo.xml");
                    _accountInfo = sb.ToString();
                }
                return _accountInfo;
            }
        }

        static string _rechargeOrderPath = string.Empty;

        public static string RechargeOrderPath
        {
            get {
                if (string.IsNullOrEmpty(_rechargeOrderPath))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(_persistentDataPath);
                    sb.Append(Path.DirectorySeparatorChar);
                    sb.Append("Records.xml");
                    _rechargeOrderPath = sb.ToString();
                }
                return _rechargeOrderPath;
            }
        }

        static string _voicePath = string.Empty;

        public static string VoicePath
        {
            get {
                if (string.IsNullOrEmpty(_voicePath))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(_persistentDataPath);
                    sb.Append(Path.DirectorySeparatorChar);
                    sb.Append("Voice");
                    sb.Append(Path.DirectorySeparatorChar);
                    _voicePath = sb.ToString();
                }
                return _voicePath;
            }
        }

        public static string WriteablePath
        {
            get
            {
                string path = String.Empty;
                if (_platform == RuntimePlatform.IPhonePlayer)
                {
                    path = _persistentDataPath + "/";
                }
                else if (_platform == RuntimePlatform.Android)
                {
                    path = _persistentDataPath + "/";
                }
                else
                {
                    path = _dataPath + "/Documents/";
                }
                return path;
            }
        }
    }
}
