using UpdateForm.Center;
using UnityEngine;
using UpdateSystem;
using UnityEngine.UI;

namespace UpdateForm.Form
{
    /// <summary>
    /// 弹出窗，作为消息提示和退出框
    /// </summary>
    public class UIMsgBoxForm : MonoBehaviour
    {
        //窗体路径
        private static string UIPath = UpdateCenter.MyMsgBoxUIPath;

        GameObject _uploadUI;   //窗体

        Button _okBtn;        //确定按钮
        Button _noBtn;        //取消按钮
        Text _msgLabel;
        Text _okLabel;
        Text _noLabel;
        EventDelegate.Callback _okCallback;
        EventDelegate.Callback _noCallback;
        //确认按钮原始位置
        Vector3 _okBtnOldPos;
        //确认按钮居中位置
        Vector3 _okBtnXZeroPos;

        static UIMsgBoxForm Instance { get; set; }

        private static void Initialize()
        {
            Object obj = Resources.Load(UIPath);
            if (obj == null)
            {
                UnityEngine.Debug.LogError("UIExitForm: 没有找到 " + UIPath);
                return;
            }
            GameObject go = GameObject.Instantiate(obj) as GameObject;
            Instance = go.AddComponent<UIMsgBoxForm>();
            go.transform.parent = GetParent();
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            //go.layer = UpdateCenter.UIRootTrans.gameObject.layer;
        }

        private static Transform GetParent()
        {
            Transform parent = null;
            if (UpdateCenter.UIRootTrans != null)
            {
                parent = UpdateCenter.UIRootTrans.transform.Find("Canvas");
            }
            else
            {
                GameObject parentGo = GameObject.Find("UIRoot(Clone)");
                if (parentGo == null)
                {
                    parentGo = GameObject.Find("UIRoot");
                }
                if (parentGo != null)
                {
                    parent = parentGo.transform;
                }
            }

            return parent;
        }

        public static void Open(string msg, string okbtnStr, string nobtnStr,
            EventDelegate.Callback okAction = null, EventDelegate.Callback noAction = null)
        {
            if (Instance == null)
            {
                Initialize();
            }

            if (Instance == null)
            {
                UnityEngine.Debug.LogError("初始化失败");
                return;
            }

            Instance.showUI(msg, okbtnStr, nobtnStr, okAction, noAction);
        }

        public static void Close()
        {
            if (Instance != null) Instance.hideUI();
        }

        public static bool IsOpening()
        {
            if (Instance == null)
            {
                return false;
            }

            return Instance._uploadUI.activeSelf;
        }


        void Awake()
        {
            DontDestroyOnLoad(this);
            _uploadUI = gameObject;
            _uploadUI.transform.position = new Vector3(0, 2000, 0);
            _uploadUI.SetActive(false);

            _okBtn = transform.Find("OkBtn").GetComponent<Button>();
            _noBtn = transform.Find("NoBtn").GetComponent<Button>();
            _msgLabel = transform.Find("Label").GetComponent<Text>();
            _okLabel = _okBtn.transform.Find("Text").GetComponent<Text>();
            _noLabel = _noBtn.transform.Find("Text").GetComponent<Text>();

            _okBtnOldPos = _okBtn.transform.localPosition;
            _okBtnXZeroPos = new Vector3(0, _okBtnOldPos.y, _okBtnOldPos.z);

            _okBtn.onClick.AddListener(delegate (){ OnClick(true); });
            _noBtn.onClick.AddListener(delegate () { OnClick(false); });
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnDestory()
        {
            Instance = null;
            _uploadUI = null;
        }


        private void showUI(string msg, string okbtnStr = "Ok", string nobtnStr = "No",
            EventDelegate.Callback okAction = null, EventDelegate.Callback noAction = null)
        {
            if (_uploadUI == null || _uploadUI.activeSelf)
            {
                return;
            }

            _msgLabel.text = msg;

            if (!string.IsNullOrEmpty(okbtnStr))
            {
                _okLabel.text = okbtnStr;
                _okBtn.transform.localPosition = _okBtnOldPos;
            }
            else
                _okLabel.gameObject.SetActive(false);

            if (!string.IsNullOrEmpty(nobtnStr))
            {
                _noLabel.text = nobtnStr;
                _noBtn.gameObject.SetActive(true);
            }
            else
            {
                _noBtn.gameObject.SetActive(false);
                _okBtn.transform.localPosition = _okBtnXZeroPos;
            }

            _okCallback = okAction;
            _noCallback = noAction;

            _uploadUI.SetActive(true);
        }

        private void OnClick(bool ok)
        {
            if (ok)
            {
                if (_okCallback != null) _okCallback();
            }
            else
            {
                if (_noCallback != null) _noCallback();
            }

            hideUI();
        }

        private void hideUI()
        {
            if (_uploadUI != null)
            {
                _uploadUI.SetActive(false);
            }
        }
    }
}