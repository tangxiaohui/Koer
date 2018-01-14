using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpdateSystem
{
    public class UpdateCenter : MonoBehaviour
    {
        //TODO::Update 这几个UI，都是放在Resource目录下的，只需要修改Layer和panel的depth
        //更新流程中的UIRoot，这时游戏逻辑还没启动
        public const string MyUIRootPath = "Default/UpdateUI/UIUpdateCanvas";
        //更新流程中的消息框
        public const string MyMsgBoxUIPath = "Default/UpdateUI/UIMsgBoxForm";
        //更新流程界面
        public const string MyUpdateFormUIPath = "Default/UpdateUI/UpdatePanel";
		public static bool bDownBaseRes = false;
		public static bool bBackGround = false;
		public static bool bRestart = false;
        public const string GameUIRootName = "UIUpdateCanvas";
        public static Transform UIRootTrans;
        void Awake()
        {
            //屏幕常亮
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        void Start()
        {
			if(DevelopSetting.isDevelop)
			{
				Common.ULogFile.sharedInstance.enable = true;
			}

            if (!DevelopSetting.HotFix/* || DevelopSetting.isDevelop*/)
            {
                Launcher.Launcher.Launch();
                return;
            }
			LoadUpdateForm();
        }
			
		public static void LoadUpdateForm(bool _bDownBaseRes = false, bool _bBackGround = false, bool _bRestart = false)
        {
			bDownBaseRes = _bDownBaseRes;
			bBackGround = _bBackGround;
			bRestart = _bRestart;
            GameObject uiRoot = LoadPrefabFromResources(MyUIRootPath, "UIUpdateCanvas");
            UIRootTrans = uiRoot.transform;
            GameObject updateFormGo = null;
            Transform uiFormTrans = uiRoot.transform.Find(MyUpdateFormUIPath);
            if (uiFormTrans == null)
                updateFormGo = LoadPrefabFromResources(MyUpdateFormUIPath, "UIUpdateForm");
            else
                updateFormGo = uiFormTrans.gameObject;
            AddChild(uiRoot.transform.Find("Canvas").gameObject, updateFormGo);
            var com = updateFormGo.GetComponent<UIUpdateForm>();
            if(com  == null)
            {
                updateFormGo.AddComponent<UIUpdateForm>();
            }
        }

        public static GameObject LoadPrefabFromResources(string path, string goName)
        {
            GameObject go = GameObject.Instantiate(Resources.Load(path)) as GameObject;
            go.name = goName;

            return go;
        }

        public static void AddChild(GameObject parent, GameObject child)
        {
            child.transform.SetParent(parent.transform);
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;
            child.transform.localScale = Vector3.one;
        }

        #region 销毁更新界面UI
        public static void Close()
        {
            GameObject uiRoot = GameObject.Find(GameUIRootName);
            if (uiRoot != null)
            {
                GameObject.DestroyImmediate(uiRoot);
            }
            UIRootTrans = null;
        }
        #endregion

    }
}
