using Common;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Res
{
    /// <summary>
    /// 资源的通用加载和释放
    /// </summary>
    public partial class ResourcesManager
    {
        #region class
        class ResInfo
        {
            public string ResName;
            public ResourceType type;
        }

        /// <summary>
        /// 主要用于解决异步加载AB包时，出现执行匿名函数时，各种参数已被改变的情况
        /// </summary>
        class LoadingResInfo
        {
            /// <summary>
            /// 路径中可能包含"//","\\"
            /// </summary>
            public string ResName;
            public bool IsReference;
            public System.Type ResType;
            public ResourceType BigType;
            /// <summary>
            /// 加载完成，给物体赋值的回调
            /// </summary>
            public System.Action<string, Object> Cb;
            /// <summary>
            /// 仅用于依赖资源的路径
            /// </summary>
            public string SpecifyPath = string.Empty;
            /// <summary>
            /// 仅用于依赖资源的源资源路径
            /// </summary>
            public string SrcPath = string.Empty;
        }
        #endregion
        #region const
        #endregion
        #region property
        #region private
        /// <summary>
        /// 存储实例化对象以及不用实例化的资源的资源信息
        /// </summary>
        Dictionary<int, ResInfo> _resNameOfObject = new Dictionary<int, ResInfo>();

        /// <summary>
        /// 在内存中的，还未实例化的对象
        /// </summary>
        Dictionary<System.Type, Dictionary<string, Object>> _objPool = new Dictionary<System.Type, Dictionary<string, Object>>();

        /// <summary>
        /// 实例化对象的缓存池
        /// </summary>
        Dictionary<string, Queue<GameObject>> _instancePool = new Dictionary<string, Queue<GameObject>>();

        /// <summary>
        /// 主要用于AssetBundle加载
        /// </summary>
        Dictionary<string, List<string>> _referenceInfo = new Dictionary<string, List<string>>();
        /// <summary>
        /// 主要用于AssetBundle加载
        /// </summary>
        Dictionary<string, int> _referenceCount = new Dictionary<string, int>();

        /// <summary>
        /// GamesObject可能会有多个副本，所以要记个数
        /// </summary>
        Dictionary<string, int> _gameObjectRefCount = new Dictionary<string, int>();

        /// <summary>
        /// 记录还未加载的引用资源数量，key就是目标资源，value是剩余未加载完成的引用数量
        /// </summary>
        Dictionary<string, int> _referenceNotLoaded = new Dictionary<string, int>();

        List<string> _assetbundleList = new List<string>();
        #endregion
        #region public 
        public static readonly ResourcesManager Instance = new ResourcesManager();
        #endregion
        #endregion

        #region function
        #region private
        #region 对象池 

        /// <summary>
        /// 只有人物和特效需要对象池 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool NeedObjectPool(ResourceType type)
        {
            int offset = (int)type - (int)ResourceType.Role;
            return offset >= 0;
        }

        GameObject GetGameObject(string resName,ResourceType type)
        {
            if (_instancePool.ContainsKey(resName) == false)
            {
                Queue<GameObject> queue = new Queue<GameObject>();
                _instancePool.Add(resName, queue);
            }
            Queue<GameObject> pool = _instancePool[resName];
            GameObject result = null;
            if (pool.Count > 0)
            {
                result = pool.Dequeue();
                result.SetActive(true);
            }
            else
            {
                System.Type resType = typeof(GameObject);
                result = GameObject.Instantiate(_objPool[resType][resName]) as GameObject;
                ResInfo info = new ResInfo();
                info.ResName = resName;
                info.type = type;
                _resNameOfObject[result.GetInstanceID()] = info;
            }

            return result;
        }

        bool RecycleObject(string resName, GameObject go)
        {
            if (_instancePool.ContainsKey(resName) == false)
            {
                UDebug.LogWarning("res:" + resName + " doesn't have a Object Pool");
                return false;
            }

            Queue<GameObject> pool = _instancePool[resName];
            go.transform.SetParent(null);
            go.SetActive(false);
            pool.Enqueue(go);
            return true;
        }

        #endregion


        bool CheckPool(string resName, System.Type resType)
        {
            if (_objPool.ContainsKey(resType))
            {
                if (_objPool[resType].ContainsKey(resName))
                {
                    if (_objPool[resType][resName] != null)
                    {
                        return true;
                    }
                    else
                    {
                        //UDebug.LogWarning("Resource:" + resName + " already has been destroyed");
                        return false;
                    }
                }
                else
                {
                    //UDebug.LogWarning("Resource:" + resName + " is not exists!");
                    return false;
                }
            }
            else
            {
                //UDebug.Log("Resource Type:" + type + " doesn't have pool!");
                return false;
            }
        }

        void CheckPoolDic(System.Type type)
        {
            if (_objPool.ContainsKey(type) == false)
            {
                Dictionary<string, Object> subDic = new Dictionary<string, Object>();
                _objPool.Add(type, subDic);
            }
        }

        string GetExtensionName(ResourceType type)
        {
            return ".unity3d";

            //switch (type)
            //{
            //    case ResourceType.Audio:
            //        return ".mp3";
            //    case ResourceType.Config:
            //        return ".bin";
            //    case ResourceType.AssetBundle:
            //    case ResourceType.Lua:
            //        return ".unity3d";
            //    case ResourceType.Material:
            //        return ".mat";
            //    case ResourceType.Role:
            //    case ResourceType.UI:
            //    case ResourceType.Fx:
            //    case ResourceType.Pet:
            //        return ".prefab";
            //    case ResourceType.Scene:
            //        return ".unity";
            //    default:
            //        return string.Empty;
            //}
        }

        StringBuilder GetPathByResourceType(string resName, ResourceType type)
        {
            StringBuilder sb = new StringBuilder();
            bool lower = false;
            if ((DevelopSetting.isBeZip && type == ResourceType.Lua) || DevelopSetting.IsLoadAB)
            {
                sb.Append("ResourceAssetBundles");
                sb.Append(Path.DirectorySeparatorChar);
                lower = true;
                ////AssetBundle路径全是小写
                sb.Append(GetPrePathByResourceType(type).ToLower());
            }
            else
            {
                sb.Append(GetPrePathByResourceType(type));
            }

            sb.Append(Path.DirectorySeparatorChar);
            if (type == ResourceType.Map)
            {             
                ///map1_0,取_的前半部分
                sb.Append(resName.Substring(0,resName.IndexOf("_")));

                sb.Append(Path.DirectorySeparatorChar);
            }

            if (lower)
            {
                sb.Append(resName.ToLower());
            }
            else
            {
                sb.Append(resName);
            }
           
            if (DevelopSetting.IsLoadAB && type != ResourceType.Lua)
            {
                sb.Append("_ab");
            }
            return sb;
        }

        /// <summary>
        /// 当前认为同一种资源就放同一目录下，方便加载
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetPrePathByResourceType(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Texture:
                    return "Texture";
                case ResourceType.Sprite:
                    return "Sprite";
                case ResourceType.Audio:
                    return "Audio";
                case ResourceType.Config:
                    return "Config";
                case ResourceType.Material:
                    return "Material";
                case ResourceType.Role3D:
                case ResourceType.Role:
                    return "Role";
                case ResourceType.Pet:
                    return "Pet";
                case ResourceType.Scene:
                    return "Scene";
                case ResourceType.Fx:
                    return "FX";
                case ResourceType.SpineAltasData:
                case ResourceType.SpineAltasTxt:
                case ResourceType.SpineBoneData:
                case ResourceType.SpineJson:
                    return "SpineData";
                case ResourceType.UI:
                    return "UI";
                case ResourceType.Lua:
                    return "lua";
                case ResourceType.Map:
                    return "Map";
                default:
                    return string.Empty;
            }
        }

        System.Type GetTypeByResourceType(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Texture:
                    return typeof(Texture2D);
                case ResourceType.Sprite:
                    return typeof(Sprite);
                case ResourceType.Audio:
                    return typeof(AudioClip);
                case ResourceType.Config:
                case ResourceType.Lua:
                case ResourceType.AssetBundle:
                    return typeof(AssetBundle);
                case ResourceType.Material:
                    return typeof(Material);
                case ResourceType.Role:
                case ResourceType.UI:
                case ResourceType.Fx:
                case ResourceType.Pet:
                    return typeof(GameObject);
                case ResourceType.Scene:
                    return typeof(Scene);
                case ResourceType.Map:
                    return typeof(Texture2D);
                default:
                    return null;
            }
        }


        /// <summary>
        /// 简单的判断资源是否需要实例化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Object GetInstance(string resName, ResourceType type,System.Type resType)
        {
            if (CheckPool(resName, resType) == false)
            {
                return default(Object);
            }
            else if (NeedObjectPool(type))
            {
                return GetGameObject(resName, type);
            }
            else if (resType == typeof(GameObject))
            {
                GameObject go = GameObject.Instantiate(_objPool[resType][resName]) as GameObject;
                ResInfo info = new ResInfo();
                info.ResName = resName;
                info.type = type;
                if (_gameObjectRefCount.ContainsKey(resName) == false)
                    _gameObjectRefCount[resName] = 0;
                ++_gameObjectRefCount[resName];
                _resNameOfObject[go.GetInstanceID()] = info;
                return go;
            }
            else
            {
                Object value = _objPool[resType][resName];
                DealResAfterLoaded(resName, type, resType, value);
                //ResInfo info = new ResInfo();
                //info.ResName = resName;
                //info.type = type;
                //_resNameOfObject[value.GetInstanceID()] = info;
                //if (resType == typeof(AssetBundle) && _assetbundleList.Contains(resName) == false)
                //    _assetbundleList.Add(resName);
                return value;
            }
        }

        void DealResAfterLoaded(string resName,ResourceType type,System.Type resType,Object obj)
        {
            if (resType == typeof(AssetBundle) && _assetbundleList.Contains(resName) == false)
                _assetbundleList.Add(resName);
            ResInfo info = new ResInfo();
            info.ResName = resName;
            info.type = type;
            _resNameOfObject[obj.GetInstanceID()] = info;
        }

        void UnloadDependencyAssetBundle(string resName)
        {
            if (_referenceInfo.ContainsKey(resName))
            {
                for (int i = 0, count = _referenceInfo[resName].Count; i < count; ++i)
                {
                    --_referenceCount[_referenceInfo[resName][i]];
                    if (_referenceCount[_referenceInfo[resName][i]] == 0)
                    {
                        UnloadResource(_objPool[typeof(AssetBundle)][_referenceInfo[resName][i]]);
                    }
                }
            }
        }

        bool UnloadResource(ResInfo info, Object obj,bool forceUnload)
        {
            System.Type type;
            if (_assetbundleList.Contains(info.ResName))
            {
                type = typeof(AssetBundle);
                _assetbundleList.Remove(info.ResName);
            }
            else
            {
                type = obj.GetType();
            }

            UnloadDependencyAssetBundle(info.ResName);

            ///如果是卸载的图集，需要判断所有引用都卸载了，再卸载图集
            if (_altasReference.ContainsKey(info.ResName))
            {
                --_altasReference[info.ResName];

                if (_altasReference[info.ResName] == 0)
                {
                    if (DevelopSetting.IsLoadAB)
                    {
                        AssetBundle res = _objPool[type][info.ResName] as AssetBundle;
                        res.Unload(true);
                        _objPool[type].Remove(info.ResName);
                    }

                    if (_altasPool.ContainsKey(info.ResName))
                    {
                        _altasPool.Remove(info.ResName);
                    }
                    _altasReference.Remove(info.ResName);

                    Resources.UnloadUnusedAssets();
                }
                return true;
            }
            else if (CheckPool(info.ResName, type))
            {
                if (NeedObjectPool(info.type) && forceUnload == false)
                {
                    return RecycleObject(info.ResName, obj as GameObject);
                }
                else if (type == typeof(GameObject))
                {
                    GameObject.Destroy(obj);
                    if (_gameObjectRefCount.ContainsKey(info.ResName))
                    {
                        --_gameObjectRefCount[info.ResName];
                        if (_gameObjectRefCount[info.ResName] == 0)
                        {
                            _objPool[type].Remove(info.ResName);
                            _gameObjectRefCount.Remove(info.ResName);
                        }
                    }
                }
                else
                {
                    if (type == typeof(AssetBundle))
                    {
                        AssetBundle res = _objPool[type][info.ResName] as AssetBundle;
                        res.Unload(true);
                    }

                    else if (type == typeof(Sprite))
                    {
                        Object res = _objPool[type][info.ResName];
                        Sprite sprite = res as Sprite;
                        Resources.UnloadAsset(sprite.texture);
                        GameObject.Destroy(res);
                    }
                    else
                    {
                        Object res = _objPool[type][info.ResName];
                        Resources.UnloadAsset(res);
                        GameObject.Destroy(obj as GameObject);
                    }

                    _objPool[type].Remove(info.ResName);
                }
                Resources.UnloadUnusedAssets();
                return true;
            }

            Resources.UnloadUnusedAssets();
            return false;
        }
#region 同步加载
        void SyncLoadDependencyAssetBundle(string path,string srcRes)
        {
            StringBuilder sb = new StringBuilder();
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_IOS
            if (DevelopSetting.IsUsePersistent)
                path = path.Replace("file:///", "");
            else
                path = path.Replace("file://", "");
#elif UNITY_ANDROID
            if (DevelopSetting.IsUsePersistent)
                path = path.Replace("file:///", "");
#endif
            sb.Append(path.ToString());
            sb.Append(".manifest");

            if (Common.FileUtils.Exist(sb.ToString()) == false)
            {
                return;
            }
            
            using (StreamReader sr = new StreamReader(Common.FileUtils.OpenFileStream(sb.ToString())))
            {
                string line = sr.ReadToEnd();

                line = line.Substring(line.LastIndexOf("Dependencies:") + "Dependencies:".Length);
                line = line.Replace("\n", "");
                if (line.Contains("-"))
                {
                    string[] dependencyPath = line.Split('-');
                    string resName;
                    string realPath;
                    if (_referenceInfo.ContainsKey(srcRes) == false)
                    {
                        List<string> reference = new List<string>();
                        _referenceInfo[srcRes] = reference;
                    }
                    _referenceInfo[srcRes].Clear();
                    ///0是 空
                    for (int i = 1, count = dependencyPath.Length; i < count; ++i)
                    {
                        dependencyPath[i] = dependencyPath[i].Replace("//", "/");
                        resName = dependencyPath[i].Substring(dependencyPath[i].LastIndexOf("/") + 1);
                        resName = resName.Substring(0, resName.LastIndexOf("_"));
                        _referenceInfo[srcRes].Add(resName);
                        if (_referenceCount.ContainsKey(resName) == false)
                            _referenceCount[resName] = 0;
                        ++_referenceCount[resName];
                        realPath = dependencyPath[i].Substring(dependencyPath[i].IndexOf("ResourceAssetBundles"));
                        if (DevelopSetting.IsUsePersistent)
                        {
                            SyncGetPersistentAB(resName, ResourceType.AssetBundle, realPath);
                        }
                        else
                        {
                            SyncGetStreamingAssetsAB(resName, ResourceType.AssetBundle, realPath);
                        }
                        
                    }
                }
            }
        }
        
        AssetBundle SyncGetStreamingAssetsAB(string resName, ResourceType type,string SpecifyPath = "")
        {
            System.Type resType = typeof(AssetBundle);

            if (CheckPool(resName, resType))
            {
                return _objPool[resType][resName] as AssetBundle;
            }
            else
            {
                StringBuilder path = new StringBuilder();

                path.Append(PathUtils.STREAMING_ASSET_PATH);

                if (string.IsNullOrEmpty(SpecifyPath))
                {
                    StringBuilder subPath = GetPathByResourceType(resName, type);
                    subPath.Append(GetExtensionName(type));
                    path.Append(subPath);
                }
                else
                {
                    path.Append(SpecifyPath);
                }
                SyncLoadDependencyAssetBundle(path.ToString(),resName);

                AssetBundle ab = ResourcesLoader.Instance.SyncLoadAssetBundle(path.ToString());
                ///SpecifyPath不为空表示是依赖资源，需要加入objpool中
                if (string.IsNullOrEmpty(SpecifyPath) == false)
                {
                    CheckPoolDic(resType);
                    _objPool[resType][resName] = ab;
                    DealResAfterLoaded(resName, type, resType, ab);
                }
                return ab;
            }
        }

        AssetBundle SyncGetPersistentAB(string resName, ResourceType type, string SpecifyPath = "")
        {
            System.Type resType = typeof(AssetBundle);

            if (CheckPool(resName, resType))
            {
                return _objPool[resType][resName] as AssetBundle;
            }
            else
            {
                StringBuilder path = new StringBuilder();

                path.Append(PathUtils.PERSISTENT_DATA_PATH);
                if (string.IsNullOrEmpty(SpecifyPath))
                {
                    StringBuilder subPath = GetPathByResourceType(resName, type);
                    subPath.Append(GetExtensionName(type));
                    path.Append(subPath);
                }
                else
                {
                    path.Append(SpecifyPath);
                }

                SyncLoadDependencyAssetBundle(path.ToString(), resName);
                AssetBundle ab = ResourcesLoader.Instance.SyncLoadAssetBundle(path.ToString());
                ///SpecifyPath不为空表示是依赖资源，需要加入objpool中
                if (string.IsNullOrEmpty(SpecifyPath) == false)
                {
                    CheckPoolDic(resType);
                    _objPool[resType][resName] = ab;
                    DealResAfterLoaded(resName, type, resType, ab);
                }
                return ab;
            }
        }
        #endregion
        #region 异步加载
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// /// <param name="srcResInfo">目标资源的相关信息</param>
        /// <param name="cb">所有依赖资源加载完毕，再加载目标资源</param>
        void AsyncLoadDependencyAssetBundle(string path,LoadingResInfo srcResInfo,System.Action<LoadingResInfo> cb)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(path.ToString());
            sb.Append(".manifest");

            if (Common.FileUtils.Exist(sb.ToString()) == false)
            {
                return;
            }

            ResourcesLoader.Instance.AsyncLoadTextAssets(sb.ToString(), (line) =>
            {
                line = line.Substring(line.LastIndexOf("Dependencies:") + "Dependencies:".Length);
                line = line.Replace("\n", "");
                if (line.Contains("-"))
                {
                    string[] dependencyPath = line.Split('-');
                    string resName;
                    string realPath;

                    _referenceNotLoaded[path] = dependencyPath.Length - 1;
                    if (_referenceInfo.ContainsKey(srcResInfo.ResName) == false)
                    {
                        List<string> reference = new List<string>();
                        _referenceInfo[srcResInfo.ResName] = reference;
                    }
                    _referenceInfo[srcResInfo.ResName].Clear();

                    LoadingResInfo resInfo;
                    ///0是 空
                    for (int i = 1, count = dependencyPath.Length; i < count; ++i)
                    {
                        dependencyPath[i] = dependencyPath[i].Replace("//", "/");
                        resName = dependencyPath[i].Substring(dependencyPath[i].LastIndexOf("/") + 1);
                        resName = resName.Substring(0, resName.LastIndexOf("_"));
                        _referenceInfo[srcResInfo.ResName].Add(resName);
                        if (_referenceCount.ContainsKey(resName) == false)
                            _referenceCount[resName] = 0;
                        ++_referenceCount[resName];
                        realPath = dependencyPath[i].Substring(dependencyPath[i].IndexOf("ResourceAssetBundles"));
                        resInfo = new LoadingResInfo();
                        resInfo.ResName = resName;
                        resInfo.BigType = ResourceType.AssetBundle;
                        resInfo.SpecifyPath = realPath;
                        resInfo.SrcPath = path;
                        resInfo.IsReference = true;


                        if (DevelopSetting.IsUsePersistent)
                        {
                            AsyncLoadPersistentAssets(resInfo, null, srcResInfo, cb);
                        }
                        else
                        {
                            AsyncLoadStreamingAssets(resInfo, null, srcResInfo, cb);
                        }

                    }

                }
                else
                {
                    cb(srcResInfo);
                }
            });
        }

        //Object,System.Type, ResourceType, string, System.Action<string, Object>
        void AsyncLoadABResources(LoadingResInfo resInfo,System.Action<LoadingResInfo, AssetBundle> cb)
        {
            if (DevelopSetting.IsUsePersistent)
            {
                AsyncLoadPersistentAssets(resInfo, cb);
            }
            else
            {
                AsyncLoadStreamingAssets(resInfo, cb);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="resInfo"></param>
        /// <param name="cb"></param>
        /// <param name="SpecifyPath">仅用于依赖资源的路径</param>
        /// <param name="srcPath">加载依赖资源的源资源路径</param>
        void AsyncLoadStreamingAssets(LoadingResInfo resInfo, System.Action<LoadingResInfo,AssetBundle> cb,LoadingResInfo srcResInfo = null, System.Action<LoadingResInfo> allLoadedcb = null)
        {
           
            System.Action afterAllLoaded = () =>
            {
                if (_referenceNotLoaded.ContainsKey(resInfo.SrcPath))
                {
                    --_referenceNotLoaded[resInfo.SrcPath];
                    ///加载完所有依赖后执行
                    if (_referenceNotLoaded[resInfo.SrcPath] == 0 && allLoadedcb != null)
                    {
                        allLoadedcb(srcResInfo);
                    }
                }
            };

            System.Type resType = typeof(AssetBundle);
            if (CheckPool(resInfo.ResName, resType))
            {
                if (cb != null)
                    cb(resInfo, _objPool[resType][resInfo.ResName] as AssetBundle);
                afterAllLoaded();
            }
            else
            {
                StringBuilder path = new StringBuilder();
               
                path.Append(PathUtils.STREAMING_ASSET_PATH);

                if (string.IsNullOrEmpty(resInfo.SpecifyPath))
                {
                    StringBuilder subPath = GetPathByResourceType(resInfo.ResName, resInfo.BigType);
                    subPath.Append(GetExtensionName(resInfo.BigType));
                    path.Append(subPath);
                }
                else
                {
                    path.Append(resInfo.SpecifyPath);
                }

                ///所有依赖资源加载完毕，再加载目标资源
                AsyncLoadDependencyAssetBundle(path.ToString(), resInfo, (subResInfo)=> {
                    ResourcesLoader.Instance.AsyncLoadAssetBundle(path.ToString(), (ab) => {
                        ///SpecifyPath不为空表示是依赖资源，需要加入objpool中
                        if (string.IsNullOrEmpty(subResInfo.SpecifyPath) == false)
                        {
                            CheckPoolDic(resType);
                            _objPool[resType][subResInfo.ResName] = ab;
                            DealResAfterLoaded(subResInfo.ResName, subResInfo.BigType, subResInfo.ResType, ab);
                        }

                        if (cb != null)
                        {
                            cb(subResInfo, ab);
                        }
                        afterAllLoaded();
                    },subResInfo.IsReference);
                });             
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resInfo"></param>
        /// <param name="cb"></param>
        /// <param name="SpecifyPath">仅用于依赖资源的路径</param>
        /// <param name="srcPath">加载依赖资源的源资源路径</param>
        void AsyncLoadPersistentAssets(LoadingResInfo resInfo, System.Action<LoadingResInfo, AssetBundle> cb, LoadingResInfo srcResInfo = null, System.Action<LoadingResInfo> allLoadedcb = null)
        {
            System.Action afterAllLoaded = () =>
            {
                if (_referenceNotLoaded.ContainsKey(resInfo.SrcPath))
                {
                    --_referenceNotLoaded[resInfo.SrcPath];
                    ///加载完所有依赖后执行
                    if (_referenceNotLoaded[resInfo.SrcPath] == 0 && allLoadedcb != null)
                    {
                        allLoadedcb(srcResInfo);
                    }
                }
            };

            System.Type resType = typeof(AssetBundle);
            if (CheckPool(resInfo.ResName, resType))
            {
                if (cb != null)
                    cb(resInfo,_objPool[resType][resInfo.ResName] as AssetBundle);
                afterAllLoaded();
            }
            else
            {
                StringBuilder path = new StringBuilder();
                path.Append(PathUtils.PERSISTENT_DATA_PATH);
                if (string.IsNullOrEmpty(resInfo.SpecifyPath))
                {
                    StringBuilder subPath = GetPathByResourceType(resInfo.ResName, resInfo.BigType);
                    subPath.Append(GetExtensionName(resInfo.BigType));
                    path.Append(subPath);
                }
                else
                {
                    path.Append(resInfo.SpecifyPath);
                }
                AsyncLoadDependencyAssetBundle(path.ToString(), resInfo, (subResInfo)=> {
                    ResourcesLoader.Instance.AsyncLoadAssetBundle(path.ToString(), (ab) => {
                        ///SpecifyPath不为空表示是依赖资源，需要加入objpool中
                        if (string.IsNullOrEmpty(subResInfo.SpecifyPath) == false)
                        {
                            CheckPoolDic(resType);
                            _objPool[resType][subResInfo.ResName] = ab;
                            DealResAfterLoaded(subResInfo.ResName, subResInfo.BigType, subResInfo.ResType, ab);
                        }
                        if (cb != null)
                            cb(subResInfo, ab);
                        afterAllLoaded();
                    },subResInfo.IsReference);
                });             
            }
        }
#endregion
#endregion
#region public
#region  同步加载
        /// <summary>
        /// 用反射的话，比较消耗，所以就自己写个枚举判断了
        /// </summary>
        /// <param name="classType"></param>
        /// <param name="resName"></param>
        /// <param name="resType"></param>
        /// <returns></returns>
        public Object SyncGetResource(ClassType classType, string resName, ResourceType resType)
        {
            switch (classType)
            {
                case ClassType.Sprite:
                    return SyncGetResource<Sprite>(resName, resType);
                case ClassType.Texture:
                    return SyncGetResource<Texture>(resName, resType);
                case ClassType.GameObject:
                    return SyncGetResource<GameObject>(resName, resType);
                case ClassType.AudioClip:
                    return SyncGetResource<AudioClip>(resName, resType);
                case ClassType.Material:
                    return SyncGetResource<Material>(resName, resType);
                case ClassType.TextAsset:
                    return SyncGetResource<TextAsset>(resName, resType);
                default:
                    return SyncGetResource<Object>(resName, resType);
            }
        }

        public Object SyncGetResource(int Index, string resName, int IndexType)
        {
            ClassType classType = (ClassType)Index;
            ResourceType resType = (ResourceType)IndexType;
            return SyncGetResource(classType, resName, resType);
        }

        public T SyncGetResource<T>(string resName,ResourceType type) where T : Object
         {
            System.Type resType = typeof(T);

            if (CheckPool(resName, resType))
            {
                return GetInstance(resName, type, resType) as T;
            }
            else
            {
                Object obj = null;
                if (DevelopSetting.IsLoadAB || resType == typeof(AssetBundle))
                {
                    string realResName = resName.Replace("//","/");
                    int index = realResName.LastIndexOf("/");
                    if (index != -1)
                    {
                        realResName = resName.Substring(index + 1);
                    }
                    if (resType != typeof(AssetBundle))
                    {
                        AssetBundle ab = SyncGetABResources(resName, type);
                        obj = ab.LoadAsset(realResName);
                        ///不是依赖加载的AssetBundle，一个AB只包含一个资源，加载出来后就可以unload
                        ab.Unload(false);
                    }
                    else
                    {
                        obj = SyncGetABResources(resName, type);
                    }
                }
                else
                {
                    StringBuilder path = GetPathByResourceType(resName, type);
                    obj = ResourcesLoader.Instance.loadResources(path.ToString());
                }
                if (obj)
                {
                    //Utility.GameUtility.ResetShader(obj);
                    CheckPoolDic(resType);
                    if (typeof(T) == typeof(Sprite))
                    {
                        Texture2D texture = obj as Texture2D;
                        texture.wrapMode = TextureWrapMode.Clamp;
                        texture.filterMode = FilterMode.Bilinear;
                        texture.anisoLevel = 4;
                        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100f, 0, SpriteMeshType.FullRect);
                        _objPool[resType][resName] = sprite;
                    }
                    else
                        _objPool[resType][resName] = obj;
                    return GetInstance(resName, type, resType) as T;
                }
                else
                    return default(T);
            }
        }

        AssetBundle SyncGetABResources(string resName, ResourceType type)
        {
            if (DevelopSetting.IsUsePersistent)
            {
                return SyncGetPersistentAB(resName, type);
            }
            else
            {
                return SyncGetStreamingAssetsAB(resName, type);
            }
        }
#endregion
#region 异步加载
        public Object GetAsyncLoadResource(ClassType classType, string resName, ResourceType resType)
        {
            switch (classType)
            {
                case ClassType.Sprite:
                    return GetAsyncLoadResource<Sprite>(resName, resType);
                case ClassType.Texture:
                    return GetAsyncLoadResource<Texture>(resName, resType);
                case ClassType.GameObject:
                    return GetAsyncLoadResource<GameObject>(resName, resType);
                case ClassType.AudioClip:
                    return GetAsyncLoadResource<AudioClip>(resName, resType);
                case ClassType.Material:
                    return GetAsyncLoadResource<Material>(resName, resType);
                case ClassType.TextAsset:
                    return GetAsyncLoadResource<TextAsset>(resName, resType);
                default:
                    return GetAsyncLoadResource<Object>(resName, resType);
            }
        }

        public void GetAsyncLoadResource(int Index, string resName, int IndexType)
        {
            ClassType classType = (ClassType)Index;
            ResourceType resType = (ResourceType)IndexType;
            GetAsyncLoadResource(classType, resName, resType);
        }

        public T GetAsyncLoadResource<T>(string resName, ResourceType type) where T : Object
        {
            return GetInstance(resName, type, typeof(T)) as T;
        }

        public void AsyncLoadResource(ClassType classType, string resName, ResourceType resType,System.Action<string,Object> cb = null)
        {
            switch (classType)
            {
                case ClassType.Sprite:
                    AsyncLoadResource<Sprite>(resName, resType, cb);
                    break;
                case ClassType.Texture:
                    AsyncLoadResource<Texture>(resName, resType, cb);
                    break;
                case ClassType.GameObject:
                    AsyncLoadResource<GameObject>(resName, resType, cb);
                    break;
                case ClassType.AudioClip:
                    AsyncLoadResource<AudioClip>(resName, resType, cb);
                    break;
                case ClassType.Material:
                    AsyncLoadResource<Material>(resName, resType, cb);
                    break;
                case ClassType.TextAsset:
                    AsyncLoadResource<TextAsset>(resName, resType, cb);
                    break;
                default:
                    AsyncLoadResource<Object>(resName, resType, cb);
                    break;
            }
        }

        public void AsyncLoadResource(int Index, string resName, int IndexType,System.Action<string,Object> cb = null)
        {
            ClassType classType = (ClassType)Index;
            ResourceType resType = (ResourceType)IndexType;
            AsyncLoadResource(classType, resName, resType, cb);
        }

        public bool isExistResource(string resName, int type)
        {
            return isExistResource(resName, (ResourceType)type);
        }

        public bool isExistResource(string resName, ResourceType type)
        {
            string realResName = resName.Replace("//", "/");
            realResName = resName.Replace("\\", "/");

            int index = realResName.LastIndexOf("/");
            if (index != -1)
            {
                realResName = resName.Substring(index + 1);
            }

            string path = string.Empty;
            StringBuilder sb = GetPathByResourceType(resName, type);
            if(DevelopSetting.IsLoadAB)
            {
                if (DevelopSetting.IsUsePersistent)
                {
                    path = Common.StringUtils.CombineString(PathUtils.PERSISTENT_DATA_PATH, sb.ToString(), GetExtensionName(type));
                }
                else
                {
                    path = Common.StringUtils.CombineString(PathUtils.STREAMING_ASSET_PATH, sb.ToString(), GetExtensionName(type));
                }
            }
            else
            {
                path = Common.StringUtils.CombineString(Application.dataPath,Path.DirectorySeparatorChar.ToString(),"Resources" , Path.DirectorySeparatorChar.ToString(), sb.ToString(), ".prefab");
            }
            return Common.FileUtils.Exist(path);
        }

        public void AsyncLoadResource<T>(string resName, ResourceType type,System.Action<string,Object> cb = null) where T : Object
        {
            System.Type resType = typeof(T);

            if (CheckPool(resName, resType))
            {
                if (cb != null)
                    cb(resName, GetInstance(resName,type,resType));
                return;
            }
            else
            {
                string realResName = resName.Replace("//", "/");
                realResName = resName.Replace("\\", "/");

                int index = realResName.LastIndexOf("/");
                if (index != -1)
                {
                    realResName = resName.Substring(index + 1);
                }

                if (DevelopSetting.IsLoadAB)
                {
                    LoadingResInfo resInfo = new LoadingResInfo();
                    resInfo.ResName = resName;
                    resInfo.ResType = resType;
                    resInfo.BigType = type;
                    resInfo.Cb = cb;
                    resInfo.IsReference = false;
 
                    AsyncLoadABResources(resInfo, (subResInfo,ab) =>
                    {
                        try
                        {
                            Object obj = ab.LoadAsset(realResName);

                            AsyncLoaded(obj, subResInfo.ResType, subResInfo.BigType, subResInfo.ResName, subResInfo.Cb);
                        }
                        catch (System.Exception e)
                        {
                            Common.UDebug.LogError("res load failed:" + resName + " message:" + e.Message);
                        }
                    });
                }
                else
                {
                    StringBuilder path = GetPathByResourceType(resName, type);
                    //用来测试 
                    ResourcesLoader.Instance.AloadResources(path.ToString(), (obj) =>
                    {
                        AsyncLoaded(obj,resType,type,resName,cb);
                    });

                }
            }
        }

        void AsyncLoaded(Object obj, System.Type resType,ResourceType type,string resName, System.Action<string, Object> cb)
        {
            if (obj)
            {
                CheckPoolDic(resType);
                if (resType == typeof(Sprite))
                {
                    Texture2D texture = obj as Texture2D;
                    texture.wrapMode = TextureWrapMode.Clamp;
                    texture.filterMode = FilterMode.Bilinear;
                    texture.anisoLevel = 4;
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100f, 0, SpriteMeshType.FullRect);
                    _objPool[resType][resName] = sprite;
                }
                else
                    _objPool[resType][resName] = obj;
            }
            else
            {
                Common.UDebug.LogError("null res:" + resName);
            }
            if (cb != null)
                cb(resName, GetInstance(resName, type,resType));
        }

#endregion

        public void UnloadAll()
        {
            foreach (var item in _instancePool)
            {
                while (item.Value.Count > 0)
                {
                    GameObject.Destroy(item.Value.Dequeue());
                }
            }

            _instancePool.Clear();

            foreach (var item in _objPool)
            {
                if (item.Key == typeof(Sprite))
                {
                    foreach (var subItem in item.Value)
                    {
                        Sprite sprite = subItem.Value as Sprite;
                        Resources.UnloadAsset(sprite.texture);
                        GameObject.Destroy(subItem.Value);
                    }
                }
                else if (item.Key == typeof(GameObject) || item.Key == typeof(Component))
                {
                    //foreach (var subItem in item.Value)
                    //{
                    //    GameObject.DestroyImmediate(subItem.Value, false);
                    //}
                }
                else if (item.Key == typeof(AssetBundle))
                {
                    foreach (var subItem in item.Value)
                    {
                        if(subItem.Key.Contains("lua") == false)
                            (subItem.Value as AssetBundle).Unload(false);
                    }
                }
                else
                {
                    foreach (var subItem in item.Value)
                    {
                        Resources.UnloadAsset(subItem.Value);
                    }
                }
                item.Value.Clear();
            }

            _objPool.Clear();
            Resources.UnloadUnusedAssets();
        }

        public bool UnloadResource(Object obj,bool forceUnload = false)
        {
            if(obj == null)
            {
                return false;
            }
            int id = obj.GetInstanceID();
            if (_resNameOfObject.ContainsKey(id))
            {
                return UnloadResource(_resNameOfObject[id],obj,forceUnload);
            }
            else
            {
                if (obj is GameObject)
                {
                    GameObject.Destroy(obj);
                }
                UDebug.LogWarning("Object:" + obj + " is not in the Dic");

                return false;
            }
        }
#endregion
#endregion

    }
}