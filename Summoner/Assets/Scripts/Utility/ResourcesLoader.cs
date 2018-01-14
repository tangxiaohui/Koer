using Common;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace Res
{
    /// <summary>
    /// 主要是从Resources下，StreammingAssets下以及PersistentDataPath下读取资源
    /// </summary>
    public class ResourcesLoader : MonoBehaviour
    {
        /// <summary>
        /// 因为异步加载的延时性，导致一个资源还在第一次加载中，
        /// 又来第二次加载请求，就会加载两次，记录每次加载的回调，
        /// 只有第一次才真正加载，然后加载完成后，再执行所有回调
        /// </summary>
        Dictionary<string, List<System.Action<object>>> _loadingRes = new Dictionary<string, List<System.Action<object>>>();

        static ResourcesLoader _instance;
        public static ResourcesLoader Instance {
            get {
                if (_instance == null)
                {
                    GameObject go = new GameObject("[ResourcesLoader]");
                    go.transform.SetParent(Common.Root.root);
                    _instance = go.AddComponent<ResourcesLoader>();
                }
                return _instance;
            }
        }

        #region Resources
        public Object loadResources(string path)
        {
            Object obj = Resources.Load(path);
            if (obj == null)
            {
                UDebug.LogError("resName:" + path + " not found in Resources");
            }
            return obj;
        }

        public void AloadResources(string path, System.Action<Object> cb)
        {
            StartCoroutine(AsyncLoadResources(path, cb));
        }

        IEnumerator AsyncLoadResources(string path, System.Action<Object> cb)
        {
            ResourceRequest obj = Resources.LoadAsync(path);
            yield return obj;

            if (obj == null)
            {
                if(cb != null)
                {
                    cb(null);
                }
            }
            else
            {
                if (cb != null)
                {
                    cb(obj.asset);
                }
            }       
        }

        public Object[] loadAllResource(string path)
        {
            Object[] objs = Resources.LoadAll(path);
            if (objs == null)
            {
                UDebug.LogError("resName:" + path + " not found in Resources");
            }
            return objs;
        }
        #endregion

        public AssetBundle SyncLoadAssetBundle(string path)
        {
            try
            {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_IOS
                if (DevelopSetting.IsUsePersistent)
                    path = path.Replace("file:///", "");
                else
                    path = path.Replace("file://", "");
#elif UNITY_ANDROID
                 if (DevelopSetting.IsUsePersistent)
                    path = path.Replace("file:///", "");
#endif
                AssetBundle ab = AssetBundle.LoadFromFile(path);
                return ab;
            }
            catch (System.Exception e)
            {
                UDebug.LogError(e.Message);
                return null;
            }
        }

        #region StreamingAssets
        public AssetBundle SyncLoadStreamingAssetsAB(string path)
        {
            StringBuilder sb = new StringBuilder();
#if UNITY_ANDROID && !UNITY_EDITOR
            sb.Append(Application.dataPath);
            sb.Append("!assets/");         
#else          
            sb.Append(Application.streamingAssetsPath);
            sb.Append(System.IO.Path.DirectorySeparatorChar);
#endif
            sb.Append(path);

            try
            {
                AssetBundle ab = AssetBundle.LoadFromFile(sb.ToString());
                return ab;
            }
            catch (System.Exception e)
            {
                UDebug.LogError(e.Message);
                return null;
            }
            
        }
        #endregion
        #region PersistentDataPath
        public AssetBundle SyncLoadPersistentDataAB(string path)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(PathUtils.PERSISTENT_DATA_PATH);
            sb.Append(path);
            AssetBundle ab = AssetBundle.LoadFromFile(sb.ToString());

            return ab;
        }

        /// <summary>
        /// 是否是依赖的资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cb"></param>
        /// <param name="isReference"></param>
        public void AsyncLoadAssetBundle(string path, System.Action<AssetBundle> cb,bool isReference)
        {
            StartCoroutine(AsyncLoad<AssetBundle>(path, cb,isReference));
        }

        public void AsyncLoadTextAssets(string path, System.Action<string> cb)
        {
            StartCoroutine(AsyncLoad<string>(path, cb));
        }

        public void AsyncLoadSprite(string path, System.Action<Sprite> cb)
        {
            StartCoroutine(AsyncLoad<Sprite>(path, cb));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="cb"></param>
        /// <param name="isReference">是否是依赖的资源</param>
        /// <returns></returns>
        IEnumerator AsyncLoad<T>(string path, System.Action<T> cb, bool isReference = false) where T:class
        {

            ///处理一下因为异步加载的延时性，导致未加载完资源前加载同一资源的情况
            if (_loadingRes.ContainsKey(path))
            {
                if (cb != null)
                {
                    _loadingRes[path].Add((go) =>
                    {
                        cb(go as T);
                    });
                }
                yield break;
            }
            else
            {
                List<System.Action<object>> list = new List<System.Action<object>>();
                if (cb != null)
                {
                    list.Add((go) =>
                    {
                        cb(go as T);
                    });
                }
                _loadingRes[path] = list;
            }

            WWW www;
            //if (typeof(T) == typeof(Texture) || typeof(T) == typeof(Sprite))
            //{
            //    www = WWW.LoadFromCacheOrDownload(path, 1);
            //}
            //else
            //{
                www = new WWW(path);
            //}
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {

                if (typeof(T) == typeof(AssetBundle))
                {
                    AssetBundle ab = www.assetBundle;
                    for (int i = 0, count = _loadingRes[path].Count; i < count; ++i)
                    {
                        _loadingRes[path][i](ab);
                    }
                    ///不是依赖加载的AssetBundle，一个AB只包含一个资源，加载出来后就可以unload
                    if (isReference == false)
                        ab.Unload(false);
                }
                else if (typeof(T) == typeof(string))
                {
                    for (int i = 0, count = _loadingRes[path].Count; i < count; ++i)
                    {
                        _loadingRes[path][i](www.text);
                    }
                }
                else if (typeof(T) == typeof(Texture))
                {
                    for (int i = 0, count = _loadingRes[path].Count; i < count; ++i)
                    {
                        _loadingRes[path][i](www.texture);
                    }
                }
                else if (typeof(T) == typeof(Sprite))
                {
                    Texture2D texture = www.texture;
                    texture.wrapMode = TextureWrapMode.Clamp;
                    texture.filterMode = FilterMode.Bilinear;
                    texture.anisoLevel = 4;
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
                    for (int i = 0, count = _loadingRes[path].Count; i < count; ++i)
                    {
                        _loadingRes[path][i](sprite);
                    }
                }
            }
            else
            {
                Common.UDebug.LogError("Res:" + path + " is not exists!");
                for (int i = 0, count = _loadingRes[path].Count; i < count; ++i)
                {
                    _loadingRes[path][i](default(T));
                }
            }
            _loadingRes[path].Clear();
            _loadingRes.Remove(path);
            www.Dispose();
        }
        #endregion
    }
}
