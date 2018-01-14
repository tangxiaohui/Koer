using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;
using UnityEngine.UI;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace Utility
{
    /// <summary>
    /// 游戏工具类
    /// </summary>
    public static class GameUtility
    {
        #region 按钮点击相关
        public static UGUIEventListener.VoidDelegate GetClickFunc(GameObject go)
        {
            UGUIEventListener listerner = go.GetComponent<UGUIEventListener>();
            if (listerner)
                return listerner.onClick;
            else
                return null;
        }

        public static void SetClickFunc(GameObject go, UGUIEventListener.VoidDelegate cb)
        {
            UGUIEventListener listerner = GetOrAddComponent<UGUIEventListener>(go);
            listerner.onClick = cb;
        }

        public static void SetClickFunc(GameObject go)
        {
            UGUIEventListener listerner = GetOrAddComponent<UGUIEventListener>(go);
            listerner.onClick = delegate (GameObject obj)
            {
                if (go == null)
                {
                    return;
                }
            };
        }

        public static System.Action<GameObject> GetListClickFunc(GameObject go)
        {
            UGUIEventForList listerner = go.GetComponent<UGUIEventForList>();
            if (listerner)
                return listerner.onClick;
            else
                return null;
        }

        public static void SetListClickFunc(GameObject go, System.Action<GameObject> cb)
        {
            UGUIEventForList listerner = GetOrAddComponent<UGUIEventForList>(go);
            listerner.onClick = cb;
        }

        public static void SetListClickFunc(GameObject go)
        {
            UGUIEventForList listerner = GetOrAddComponent<UGUIEventForList>(go);
            listerner.onClick = delegate (GameObject obj)
            {
                if (go == null)
                {
                    return;
                }
            };
        }
        #endregion

 
        #region POST/GET
        public static bool MyRemoteCertificateValidationCallback(System.Object sender,
X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain,
            // look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        continue;
                    }
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                        break;
                    }
                }
            }
            return isOk;
        }

        public static string HttpGet(string Url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        public static string HttpPost(string Url, string postDataStr)
        {
            //Debug.LogError("url:" + Url + "  data:" + postDataStr);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postDataStr.Length;
            StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
            writer.Write(postDataStr);
            writer.Flush();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string encoding = response.ContentEncoding;
            if (encoding == null || encoding.Length < 1)
            {
                encoding = "UTF-8"; //默认编码  
            }
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
            string retString = reader.ReadToEnd();
            return retString;
        }
        #endregion
        public static string GetBase64String(string str)
        {
            byte[] bt = System.Text.Encoding.Default.GetBytes(str);
            return Convert.ToBase64String(bt);
        }

        public static string GetGuild()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }

        public static void DoContinuousSizeAnim(Transform trans,float size, float time)
        {
            trans.DOScale(size, time).SetLoops(-1);
        }

        public static void SetUIMatTex(UIImage img, Sprite sp)
        {
            img.material.SetTexture("_MainTex", sp.texture);
        }

        public static void DoRotateScaleAnim(Transform trans, Transform parentTrans, float time)
        {
            Transform parent = trans.parent;
            trans.gameObject.SetActive(true);
            trans.SetParent(parentTrans,true);
            Vector3 endValue = new Vector3(0f, 0f, 360.001f);
            trans.localEulerAngles = Vector3.zero;
            trans.localScale = Vector3.one * 7;

            trans.DOScale(Vector3.one, time).SetEase(Ease.InOutCubic);
            trans.DOLocalRotate(endValue, time, RotateMode.FastBeyond360).SetEase(Ease.InOutCubic).OnComplete(() =>
            {
                trans.SetParent(parent, true);
            });
        }

        public static void DoRotateScaleAnim(Transform trans, Transform realTrans, float time, Vector3 pos)
        {
            if (trans == null && realTrans == null)
                return;
            trans.gameObject.SetActive(true);
            realTrans.gameObject.SetActive(false);
            Vector3 endValue = new Vector3(0f, 0f, 360.001f);           
            trans.localEulerAngles = Vector3.zero;
            trans.localScale = Vector3.one * 7;
           // Debug.Log("trans Position  " + trans.position + "   realTrans.position " + realTrans.position);
            trans.position = pos;
          //  Debug.Log("trans.position   " + trans.position);
            trans.DOScale(Vector3.one, time).SetEase(Ease.InOutCubic);


            trans.DOLocalRotate(endValue, time, RotateMode.FastBeyond360).SetEase(Ease.InOutCubic).OnComplete(() => 
            {
                realTrans.position = trans.position;
                trans.gameObject.SetActive(false);
                realTrans.gameObject.SetActive(true);
            });
        }

        public static void DoLocalMoveAnim(Transform trans, Vector3 pos, float time, System.Action cb)
        {
            trans.DOLocalMove(pos, time).SetEase(Ease.OutBack).OnComplete(() =>
            {
                if (cb != null)
                    cb();
            });
        }

        public static void DoSizeYAnim(RectTransform trans, float size, float time)
        {
            if (trans == null)
                return;
            DOTween.To(x => trans.sizeDelta = new Vector2(trans.sizeDelta.x, x), trans.sizeDelta.y, size, time);
        }

        public static void DoScaleYAnim(Transform trans, float size, float time,System.Action cb)
        {
            if (trans == null)
                return;
            trans.DOScaleY(size, time).OnComplete(()=> {
                if (cb != null)
                    cb();
            });
        }

        public static List<UIText> GetUITextInchildren(GameObject go)
        {
            return GetComponentsInChildren<UIText>(go);
        }

        public static List<UIImage> GetUIImageInchildren(GameObject go)
        {
            return GetComponentsInChildren<UIImage>(go);
        }

        public static List<T> GetComponentsInChildren<T>(GameObject go)
        {
            T[] components = go.GetComponentsInChildren<T>();
            List<T> result = new List<T>();
            for (int i = 0, count = components.Length; i < count; ++i)
            {
                result.Add(components[i]);
            }

            return result;
        }

        public static string GetVoiceLength(string str)
        {
            return str.Substring(1, str.IndexOf("}") - 1);
        }

        public static List<string> DealChatMsg(string msg)
        {
            List<string> result = new List<string>();
            ///{#head:XXX}……
            result.Add(msg.Substring(msg.IndexOf(":") + 1, msg.IndexOf("}") - msg.IndexOf(":") - 1));
            result.Add(msg.Substring(msg.IndexOf("}") + 1));
            
            return result;
        }

        public static void ModifyTextAlignment(Text text,int anchor)
        {
            text.alignment = (TextAnchor)anchor;
        }

        public static List<string> StringSplit(string str, string spliter)
        {
            string[] arg = new string[1];
            arg[0] = spliter;
            string[] res = str.Split(arg, StringSplitOptions.None);
            List<string> result = new List<string>();
            for (int i = 0, count = res.Length; i < count; ++i)
            {
                result.Add(res[i]);
            }
            return result;
        }


        public static int GetStringLength(string str)
        {
            return str.Length;
        }

        public static string GetSubString(string str,int start,int length)
        {
            return str.Substring(start,length);
        }

        public static void DestroyGameObject(GameObject go)
        {
            GameObject.Destroy(go);
        }

        public static Transform CloneTransform(Transform trans)
        {
            return CloneGameObject(trans.gameObject).transform;
        }

        public static GameObject CloneGameObject(GameObject go)
        {
            GameObject clone = GameObject.Instantiate(go) as GameObject;
            clone.transform.SetParent(go.transform.parent);
            clone.transform.localPosition = go.transform.localPosition;
            clone.transform.localScale = go.transform.localScale;
            return clone;
        }

        public static DateTime ConvertStringToDateTime(Int32 timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime t = dtStart.AddSeconds((double)timeStamp);
            return t;
        }

        public static DateTime ConvertStringToDateTime(UInt64 timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime t = dtStart.AddSeconds((double)timeStamp);
            return t;
        }
        public static string ConvertStringToDateTime_YearsMonthDay(UInt64 timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime t = dtStart.AddSeconds((double)timeStamp);
            string timeTxt = t.ToString("yyyy年MM月dd日");
            return timeTxt;
        }

        public static int GetTimeStampSecond()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            int ret = Convert.ToInt32(ts.TotalSeconds);
            //Console.WriteLine("DateTime.Now" + DateTime.Now.ToString());
            return ret;
        }

        public static void ResetShader(UnityEngine.Object obj)
        {

            List<Material> listMat = new List<Material>();
            listMat.Clear();
            if (obj is Material)
            {
                Material m = obj as Material;
                listMat.Add(m);
            }
            else if (obj is GameObject)
            {
                GameObject go = obj as GameObject;
                Renderer[] rends = go.GetComponentsInChildren<Renderer>();
                if (null != rends)
                {
                    foreach (Renderer item in rends)
                    {
                        Material[] materialsArr = item.sharedMaterials;
                        foreach (Material m in materialsArr)
                            listMat.Add(m);
                    }
                }
            }
            for (int i = 0; i < listMat.Count; i++)
            {
                Material m = listMat[i];
                if (null == m)
                    continue;
                var shaderName = m.shader.name;
                var newShader = Shader.Find(shaderName);
                if (newShader != null)
                    m.shader = newShader;
            }
        }

        public static T DeepCopy<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                //序列化成流
                bf.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                //反序列化成对象
                retval = bf.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encode">0: utf8 1:unicode 2:ascii 3:BigEndianUnicode</param>
        /// <returns></returns>
        public static Encoding GetSpecifyEncode(int encode)
        {
            switch (encode)
            {
                case 1:
                    return Encoding.Unicode;
                case 2:
                    return Encoding.ASCII;
                case 3:
                    return Encoding.BigEndianUnicode;
                default:
                    return Encoding.UTF8;
            }
        }

        /// <summary>
        /// 网络是否可用
        /// </summary>
        /// <returns></returns>
        public static bool IsNetworkEnable()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }

        /// <summary>
        /// 查找子节点
        /// </summary>
        public static Transform FindDeepChild(GameObject _target, string _childName)
        {
            Transform resultTrs = null;
            resultTrs = _target.transform.Find(_childName);
            if (resultTrs == null)
            {
                foreach (Transform trs in _target.transform)
                {
                    resultTrs = GameUtility.FindDeepChild(trs.gameObject, _childName);
                    if (resultTrs != null)
                        return resultTrs;
                }
            }
            return resultTrs;
        }

        public static GameObject FindDeepChildGameObject(GameObject _target, string _childName)
        {
            Transform resultTrs = null;
            resultTrs = _target.transform.Find(_childName);
            if (resultTrs == null)
            {
                foreach (Transform trs in _target.transform)
                {
                    resultTrs = GameUtility.FindDeepChild(trs.gameObject, _childName);
                    if (resultTrs != null)
                        return resultTrs.gameObject;
                }
                return null;
            }
            return resultTrs.gameObject;
        }
        /// <summary>
        /// 查找子节点脚本
        /// </summary>
        public static T FindDeepChild<T>(GameObject _target, string _childName) where T : Component
        {
            Transform resultTrs = GameUtility.FindDeepChild(_target, _childName);
            if (resultTrs != null)
                return resultTrs.gameObject.GetComponent<T>();
            return default(T);
        }

        public static Component FindDeepChild(GameObject _target, string _childName, string com)
        {
            Transform resultTrs = GameUtility.FindDeepChild(_target, _childName);
            if (resultTrs != null)
                return resultTrs.gameObject.GetComponent(com);
            return null;
        }

        public static string GetUpperMd5(string str)
        {
            return GetMd5Hash(str).ToUpper();
        }

        public static string GetMd5Hash(string str)
        {
            using (System.Security.Cryptography.MD5 md5hash = System.Security.Cryptography.MD5.Create())
            {
                string hashstr = MD5Utils.GetMd5Hash(md5hash, str);
                return hashstr;
            }
        }

        public static int RondomExtremeRatio()
        {
            int min = 0;
            int max = 10001;
            return RandomRange(min, max);
        }

        public static bool RondomExtremeRatio(int rad)
        {
            int rod = RondomExtremeRatio();
            return rod <= rad;
        }

        public static int RandomRange(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        /// <summary>
        /// 添加子节点
        /// </summary>
        public static void AddChildToTargetWithLayer(Transform target, Transform child, bool withScale = true)
        {
            child.SetParent(target);
            if (withScale)
                child.localScale = Vector3.one;
            child.localPosition = Vector3.zero;
            child.localEulerAngles = Vector3.zero;
            ChangeChildLayer(child, target.gameObject.layer);
        }

        public static GameObject AddChild(GameObject parnet, GameObject prefab)
        {
            var child = GameObject.Instantiate<GameObject>(prefab);
            AddChildToTargetWithLayer(parnet.transform, child.transform);
            return child;
        }

        public static T AddChild<T>(GameObject parnet, GameObject prefab) where T : MonoBehaviour
        {
            var child = AddChild(parnet, prefab);
            return child.GetOrAddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this GameObject a) where T : Component
        {
            var temp = a.GetComponent<T>();
            if (temp == null) temp = a.AddComponent<T>();
            return (T)temp;
        }
        /// <summary>
        /// 添加到指定节点上面,找不到节点将子物体设置为 目标物体的节点
        /// </summary>
        /// <param name="target"></param>
        /// <param name="child"></param>
        /// <param name="nodes"></param>
        public static void AddChildToTargetWithLayer(Transform target, Transform child, string nodes, bool withScale = true)
        {
            var parent = FindDeepChild(target.gameObject, nodes);
            if (parent == null)
            {
                //Debug.Log(target.name+"===null==="+nodes);
                child.SetParent(target);
            }
            else
            {
                //Debug.Log(target.name + "===" + parent.name+"===" + nodes);
                child.SetParent(parent);
            }
            if (withScale)
                child.localScale = Vector3.one;
            child.localPosition = Vector3.zero;
            child.localEulerAngles = Vector3.zero;
            ChangeChildLayer(child, target.gameObject.layer);
        }
        public static void AddChildToTarget(Transform target, Transform child, bool withScale = true)
        {
            child.SetParent(target);
            if (withScale)
                child.localScale = Vector3.one;
            child.localPosition = Vector3.zero;
            child.localEulerAngles = Vector3.zero;
        }
        /// <summary>
        /// 修改子节点Layer  NGUITools.SetLayer();
        /// </summary>
        public static void ChangeChildLayer(Transform t, int layer)
        {
            t.gameObject.layer = layer;
            for (int i = 0; i < t.childCount; ++i)
            {
                Transform child = t.GetChild(i);
                child.gameObject.layer = layer;
                ChangeChildLayer(child, layer);
            }
        }
        /// <summary>
        /// 获取单例，如果没有创建一个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="centerName"></param>
        /// <returns></returns>
        public static T GetInstanceType<T>(string centerName) where T : MonoBehaviour
        {
            var instance = GameObject.FindObjectOfType<T>();
            if (instance == null)
            {
                var bb = new GameObject(centerName);
                instance = bb.AddComponent<T>();
            }
            return (T)instance;
        }

        #region 占时先不用 这些是和NGUI想关联的东西
        /// <summary>
        /// 根据最小depth设置目标所有Canvas层级，从小到大
        /// </summary>
        /// 
        private class CompareSubCanvas : IComparer<Canvas>
        {
            public int Compare(Canvas left, Canvas right)
            {
                return left.sortingOrder - right.sortingOrder;
            }
        }

        public static void SetTargetMinCanvas(GameObject obj, int sortingLayerID)
        {
            List<Canvas> lsCanvas = GetCanvasSorted(obj, true);
            if (lsCanvas != null)
            {
                int i = 0;
                while (i < lsCanvas.Count)
                {
                    lsCanvas[i].sortingOrder = sortingLayerID + i;
                    i++;
                }
            }
        }
        /// <summary>
        /// 获得指定目标最大depth值
        /// </summary>
        public static int GetMaxTargetDepth(GameObject obj, bool includeInactive = false)
        {
            int minDepth = -1;
            List<Canvas> lsCanvas = GetCanvasSorted(obj, includeInactive);
            if (lsCanvas != null)
                return lsCanvas[lsCanvas.Count - 1].sortingOrder;
            return minDepth;
        }

        /// <summary>
        /// 返回最大或者最小Depth界面
        /// </summary>
        public static GameObject GetCanvasDepthMaxMin(GameObject target, bool maxDepth, bool includeInactive)
        {
            List<Canvas> lsCanvas = GetCanvasSorted(target, includeInactive);
            if (lsCanvas != null)
            {
                if (maxDepth)
                    return lsCanvas[lsCanvas.Count - 1].gameObject;
                else
                    return lsCanvas[0].gameObject;
            }
            return null;
        }

        private static List<Canvas> GetCanvasSorted(GameObject target, bool includeInactive = false)
        {
            Canvas[] canvas = target.transform.GetComponentsInChildren<Canvas>(includeInactive);
            if (canvas.Length > 0)
            {
                List<Canvas> lsCanvas = new List<Canvas>(canvas);
                lsCanvas.Sort(new CompareSubCanvas());
                return lsCanvas;
            }
            return null;
        }
        #endregion

        public static string TimeFormatTime(int mini)
        {
            int minute = mini / 60;
            int second = mini % 60;
            System.Text.StringBuilder rTime = new System.Text.StringBuilder();

            rTime.Append(FormatTime(minute));
            rTime.Append(":");
            rTime.Append(FormatTime(second));
            return rTime.ToString();
        }
        static string FormatTime(int count)
        {
            string rTime;
            if (count >= 0 && count < 10)
                rTime = "0" + count;
            else if (count > 9)
                rTime = count.ToString();
            else
                rTime = "00";
            return rTime;
        }

        public static List<GameObject> GetAllChildItems(GameObject obj)
        {
            List<GameObject> tmpList = new List<GameObject>();
            if (obj == null)
            {
                return tmpList;
            }
            for (int i = 0; i < obj.transform.childCount; i++)
                tmpList.Add(obj.transform.GetChild(i).gameObject);
            return tmpList;
        }

        public static void ResetTransform(Transform t)
        {
            t.transform.localPosition = Vector3.zero;
            t.transform.localScale = Vector3.one;
            t.transform.localEulerAngles = Vector3.zero;
        }

        public static void ResetRectTransform(GameObject go)
        {
           if(go == null)
            {
                return;
            }
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector3.zero;
            rect.anchorMax = Vector3.one;
            rect.anchoredPosition = Vector3.zero;
            rect.offsetMin = Vector3.zero;
            rect.offsetMax = Vector3.zero;
            rect.transform.localScale = Vector3.zero;
        }

        public static bool isHasFlag(this Int32 a, Int32 b)
        {
            return (a & b) > 0;
        }

        /// <summary>
        /// 获取PC的mac地址
        /// </summary>
        /// <returns></returns>
        public static string GetMacAddress()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            string physicalAddress = "";

            NetworkInterface[] nice = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface adaper in nice)
            {
                if (adaper.Description == "en0")
                {
                    physicalAddress = adaper.GetPhysicalAddress().ToString();
                    break;
                }
                else
                {
                    physicalAddress = adaper.GetPhysicalAddress().ToString();

                    if (physicalAddress != "")
                    {
                        break;
                    };
                }
            }
            return physicalAddress;
#else
            return string.Empty;
#endif
        }

        public static string stringFormat(string str,object[] objs)
        {
            if(string.IsNullOrEmpty(str) || objs == null)
            {
                return str;
            }
            str = string.Format(str, objs);
            return str;
        }

        public static string stringFormat(string str, string replace)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(replace))
            {
                return str;
            }

            string[] tempStr = replace.Split('*');
            str = string.Format(str, tempStr);
            return str;
        }

        public static string GetIp()
        {
            string userIp = "";
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces(); ;
            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    UnicastIPAddressInformationCollection uniCast = adapter.GetIPProperties().UnicastAddresses;
                    if (uniCast.Count > 0)
                    {
                        foreach (UnicastIPAddressInformation uni in uniCast)
                        {
                            //得到IPv4的地址。 AddressFamily.InterNetwork指的是IPv4
                            if (uni.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                userIp = uni.Address.ToString();
                            }
                        }
                    }
                }
            }
            return userIp;
        }

       // 扩充字符
         public static void FixBrokenWord(Font font, string content)
        {
            if (string.IsNullOrEmpty(content) || font == null)
            {
                return;
            }
            font.RequestCharactersInTexture(content);
        }

        public static void FixBrokenWord()
        {
            Font font = Resources.Load<Font>("Font/Font2/HYWenHei-85W");
            if(font != null)
            {
                TextAsset txt = Resources.Load("Font/Font2/chinese") as TextAsset;
                if(txt != null)
                {
                    string chineseTxt = txt.ToString();
                    FixBrokenWord(font, chineseTxt);
                }
            }
        }

        public static void Fade(bool isIn, Transform trans)
        {
            Fade(isIn, trans, 1f);
        }

        public static void Fade(bool isIn, Transform trans, float time)
        {
            Image[] imgs = trans.GetComponentsInChildren<Image>();
            Color color;
            for (int i = 0; i < imgs.Length; ++i)
            {
                color = imgs[i].color;
                color.a = isIn ? 0 : 1;
                imgs[i].color = color;
                imgs[i].DOFade(isIn ? 1 : 0, time).OnComplete(() =>
                {
                    if (!isIn)
                    {
                        trans.gameObject.SetActive(false);
                    }
                });
            }
        }

        public static float DistanceX(this Vector3 a, Vector3 b)
        {
            return Math.Abs(a.x - b.x);
        }


        public static string GetAssetPath(string str)
        {
            int start = str.IndexOf("Assets");
            if (start >= 0)
            {
                string dir = str.Substring(start, str.Length - start);
                return dir;
            }
            return str;
        }

        public static string GetFullPath(string str)
        {
           string fullPath =  Path.GetFullPath(str);
            return fullPath;
        }

        public static void SetRaycastTarget(GameObject go, bool raycastTarget)
        {
            if (go == null)
            {
                return;
            }
            Graphic com = go.GetComponent<Graphic>();
            if (com != null)
            {
                com.raycastTarget = raycastTarget;
            }
        }

        public static void SetInputFilePassWord(InputField input)
        {
            SetInputFieldType(input, (int)InputField.ContentType.Password);
        }

        public static void SetInputFileNumber(InputField input)
        {
            SetInputFieldType(input, (int)InputField.ContentType.IntegerNumber);
        }

        public  static void CreateLink(Text text)
        {
            if (text == null)
                return;

            //克隆Text，获得相同的属性  
            Text underline = GameObject.Instantiate(text) as Text;
            underline.name = "Underline";

            underline.transform.SetParent(text.transform);
            underline.transform.localScale = Vector3.one;
            RectTransform rt = underline.rectTransform;

            //设置下划线坐标和位置  
            rt.anchoredPosition3D = Vector3.zero;
            rt.offsetMax = Vector2.zero;
            rt.offsetMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.anchorMin = Vector2.zero;

            underline.text = "_";
            float perlineWidth = underline.preferredWidth;      //单个下划线宽度  
            //Debug.Log(perlineWidth);

            float width = text.preferredWidth;
            //Debug.Log(width);
            int lineCount = (int)Mathf.Round(width / perlineWidth);
            //Debug.Log(lineCount);
            for (int i = 1; i < lineCount; i++)
            {
                underline.text += "_";
            }
        }


        public static void SetInputFieldType(InputField input, int index)
        {
            if (input == null)
            {
                return;
            }

            input.contentType = (InputField.ContentType)index;
        }

        public static void SetInputkeyboardType(InputField input, int index)
        {
            if (input == null)
            {
                return;
            }

            input.keyboardType = (UnityEngine.TouchScreenKeyboardType)index;
        }

        public static int UtilityRandom(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }


        #region int64算法
        public static UInt64 Int64ToUInt64(Int64 a)
        {
            return (UInt64)a;
        }

        public static Int64 UInt64ToInt64(UInt64 a)
        {
            return (Int64)a;
        }

        public static string Int64ToString(Int64 value)
        {
            return value.ToString();
        }

        public static int CompareInt64(Int64 a, Int64 b)
        {
            return a.CompareTo(b);
        }

        public static Int64  CopyInt64(Int64 a)
        {
            Int64 b = a;
            return b;
        }
        //减法
        public static Int64 SubtractionInt64(Int64 a, Int64 b)
        {
            return a - b;
        }

        //加法
        public static Int64 AddInt64(Int64 a, Int64 b)
        {
            return a - b;
        }

        //除法
        public static Int64 DivisionInt64(Int64 a, Int64 b)
        {
            if (b != 0)
            {
                return a / b;
            }
            return 0;
        }

        //Float除法
        public static float FloatDivisionInt64(Int64 a, Int64 b)
        {
            if (b != 0)
            {
                return (float)a / (float)b;
            }
            return 0;
        }

        public static float DivisionInt6432(Int64 a, int b)
        {
            if (b != 0)
            {
                return a / b;
            }
            return 0;
        }

        //除法
        public static float DivisionInt64Float(Int64 a, Int64 b)
        {
            if (b != 0)
            {
                return (float)a / b;
            }
            return 0;
        }

        //乘法
        public static Int64 MulInt64(Int64 a, Int64 b)
        {
            return a * b;
        }

        //取余
        public static UInt64 RemainderInt64(UInt64 a, UInt64 b)
        {
            return a % b;
        }
        #endregion


        #region uint64算法
        public static UInt64 stringToUInt64(string str)
        {
            UInt64 a = System.UInt64.Parse(str);
            return a;
        }

        public static Int64 stringToInt64(string str)
        {
            Int64 a = System.Int64.Parse(str);
            return a;
        }

        public static int Int64ToInt32(Int64 value)
        {
            return (int)value;
        }

        public static uint UInt64ToUInt32(UInt64 value)
        {
            return (uint)value;
        }

        public static string UInt64ToString(UInt64 value)
        {
            return value.ToString();
        }


        public static int CompareUInt64(UInt64 a, UInt64 b)
        {
            return a.CompareTo(b);
        }

        public static UInt64 SubtractionuInt64(UInt64 a, UInt64 b)
        {
            return a - b;
        }

        public static UInt64 AddUInt64(UInt64 a, UInt64 b)
        {
            return a + b;
        }

        //除法
        public static UInt64 DivisionUInt64(UInt64 a, UInt64 b)
        {
            if (b != 0)
            {
                return a / b;
            }
            return 0;
        }

        //除法
        public static float DivisionUInt64Float(UInt64 a, UInt64 b)
        {
            if (b != 0)
            {
                return (float)a / b;
            }
            return 0;
        }

        //乘法
        public static UInt64 MulUInt64(UInt64 a, UInt64 b)
        {
            return a * b;
        }

        //取余
        public static UInt64 RemainderUInt64(UInt64 a, UInt64 b)
        {
            return a % b;
        }
        #endregion

    }
}
