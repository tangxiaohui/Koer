using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

namespace Res
{
    /// <summary>
    /// 各种类型资源的特殊处理 
    /// </summary>
    public partial class ResourcesManager
    {
        #region 物品图标    
        /// <summary>
        /// item表里面的
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public Sprite GetItemIcon(int itemID)
        {
            //Item item = Item.Get(itemID);
            //if (item == null)
            //{
            //    Common.UDebug.LogError("item:" + itemID + " is not exist!");
            //    return null;
            //}

            //if (item.iItemPage == 1)//装备
            //{
            //    return SyncGetSpecifyIcon(item.itemicon, IconType.Equipment);
            //}
            //else
            //{
            //return SyncGetSpecifyIcon(item.itemicon, IconType.Item);
            //}

            return SyncGetSpecifyIcon("", IconType.Item);
        }
       

        #endregion
        #region 图集
        Dictionary<string, Object[]> _altasPool = new Dictionary<string, Object[]>();
        /// <summary>
        /// 一个图集里面的所有图片的引用都一起统计了，不分开
        /// </summary>
        Dictionary<string, int> _altasReference = new Dictionary<string, int>();

        public Sprite SyncGetSpecifyIcon(string resName, int type)
        {
            return SyncGetSpecifyIcon(resName, (IconType)type);
        }

        public Sprite SyncGetSpecifyIcon(string resName, IconType type)
        {

            int realType = (int)type;
            return SyncGetSpecifyImgInAltas(resName, realType);

        }

        //         public Sprite SyncGetSpecifyImgInAltas(string resName, int type)
        //         {
        //             return SyncGetSpecifyImgInAltas(resName, type);
        //         }
        public Sprite SyncGetSpecifyImgInAltas(string resName, AltasType type)
        {
            return SyncGetSpecifyImgInAltas(resName, (int)type);
        }

        public Sprite SyncGetCardImgInAltas(int CardId)
        {
            int type = 0;
            if (CardId <= 24)
                type = (int)AltasType.Card1;
            else if (CardId <= 48)
                type = (int)AltasType.Card2;
            else if (CardId <= 72)
                type = (int)AltasType.Card3;
            else if (CardId <= 96)
                type = (int)AltasType.Card4;
            else
                type = (int)AltasType.Card5;
            return SyncGetSpecifyImgInAltas(CardId.ToString(), type);
        }

        public Sprite SyncGetSpecifyImgInAltas(string resName, int type)
        {
            if(string.IsNullOrEmpty(resName))
            {
                return null;
            }

            StringBuilder path = GetAltasPath(resName, type);
            string pathStr = path.ToString();

            if (_altasPool.ContainsKey(pathStr) == false)
            {
                SyncLoadSpecifyAltas(path);
            }

            for (int i = 0, count = _altasPool[pathStr].Length; i < count; ++i)
            {
                if (_altasPool[pathStr][i].name.CompareTo(resName) == 0 && _altasPool[pathStr][i] is Sprite)
                {
                    int instanceId = _altasPool[pathStr][i].GetInstanceID();
                    if (_resNameOfObject.ContainsKey(instanceId) == false)
                    {
                        ResInfo res = new ResInfo();
                        res.ResName = pathStr;
                        res.type = ResourceType.Sprite;
                        _resNameOfObject[instanceId] = res;
                    }
                    if (_altasReference.ContainsKey(pathStr) == false)
                        _altasReference[pathStr] = 1;
                    else
                        ++_altasReference[pathStr];
                    return _altasPool[pathStr][i] as Sprite;
                }
            }

            return null;
        }

        void SyncLoadSpecifyAltas(StringBuilder path)
        {
            string pathStr = path.ToString();
            if (DevelopSetting.IsLoadAB)
            {
                AssetBundle ab = SyncGetResource<AssetBundle>(pathStr, ResourceType.Sprite);
                _altasPool[pathStr] = ab.LoadAllAssets();
                //ab.Unload(false);
            }
            else
            {
                path = GetPathByResourceType(pathStr, ResourceType.Sprite);
                _altasPool[pathStr] = ResourcesLoader.Instance.loadAllResource(path.ToString());
            }
        }

        StringBuilder GetAltasPath(string resName, int type)
        {
            StringBuilder sb = new StringBuilder();
            if (type > (int)AltasType.LoadingForm)
            {
                sb.Append("JPG");
                sb.Append(Path.DirectorySeparatorChar);
                sb.Append("Loading");
                sb.Append(Path.DirectorySeparatorChar);
                sb.Append(resName);
            }
            else
            {
                sb.Append("Altas");
                sb.Append(Path.DirectorySeparatorChar);
                sb.Append(GetAltasName(type));
            }

            return sb;
        }

        string GetAltasName(int aType)
        {

            //ClientAltas altas = ClientAltas.Get((int)aType);
            //if (altas != null)
            //{
            //    return altas.name;
            //}
            //else
            //{
                return string.Empty;
            //}
        }
        #endregion



        public Texture2D SyncGetTexture(string resName)
        {
            return SyncGetResource<Texture2D>(resName, ResourceType.Texture);
        }

        public Sprite SyncGetJPGSprite(string resName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("JPG");
            sb.Append(Path.DirectorySeparatorChar);
            sb.Append(resName);
            return SyncGetResource<Sprite>(sb.ToString(), ResourceType.Sprite);
        }

        public void ASyncGetJPGSprite(string resName, System.Action<string, Object> cb = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("JPG");
            sb.Append(Path.DirectorySeparatorChar);
            sb.Append(resName);
            AsyncLoadResource<Sprite>(sb.ToString(), ResourceType.Sprite, cb);
        }

        public Sprite SyncGetSprite(string resName)
        {
            return SyncGetResource<Sprite>(resName, ResourceType.Sprite);
        }
    }
}