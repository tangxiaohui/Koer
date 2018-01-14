using UnityEngine;

namespace UpdateForm.Hardware
{
#if !UNITY_IPHONE
    ///// <summary>
    ///// 安卓的设备信息
    ///// </summary>
    //public class AndroidDeviceInfo:IDeviceInfo
    //{
    //    public const string CN_PLUGIN_DEVICE_INFO_CLASS_NAME = "com.haowan123.plugin.utils.DeviceInfo";        

    //    private IAndroidPlayer _player;

    //    private string _brand;          // 手机品牌
    //    private string _model;          // 手机型号
    //    private string _imei;           // imei
    //    private string _imsi;           // imsi
    //    private string _number;         // 手机号
    //    private string _simOperator;    // 运营商
    //    private string _cpuName;        // 手机CPU型号
    //    private string _cpuFrequency;   // 手机CPU频率
    //    private int _cpuCores;          // 手机CPU个数
    //    private string _macAddress;     // mac地址

    //    public AndroidDeviceInfo()
    //    {
    //        _player = AndroidPlayer.ShareInstance;

    //        UnityEngine.Debug.Log("初始化AndroidDeviceInfo");
    //        //AndroidJavaClass naticeJC = new AndroidJavaClass("com.liushanmen.UnityPlayerNativeActivity");
    //        //AndroidJavaObject contextJO = naticeJC.GetStatic<AndroidJavaObject>("currentActivity");
    //        //using (AndroidJavaClass clazz = new AndroidJavaClass("com.liushanmen.Utils"))
    //        //{
    //        //    UnityEngine.Debug.Log("InitMobileInfoInterface");
    //        //    _player.CallActionStatic(CN_PLUGIN_DEVICE_INFO_CLASS_NAME, "InitMobileInfoInterface", contextJO);
    //        //}
    //        //naticeJC.Dispose();
    //        //contextJO.Dispose();


    //        getPhoneInfo();
    //        getCpuInfo();
    //        getNumCores();
    //        getMacAddress();
    //    }

    //    public float GetBatteryPower()
    //    {
    //        return GetBatteryRemain();
    //    }

    //    public float GetScreenBrightness()
    //    {
    //        //float brightness = _player.CallFuncStatic<float>("com.haowan123.plugin.utils.Utils", "getAppBrightness");
    //        //return brightness < 0 ? 1 / 255 : brightness;
    //        return 1.0f;
    //    }

    //    public string GetPhoneBrand()
    //    {
    //        return _brand;
    //    }

    //    public string GetPhoneModel()
    //    {
    //        return _model;
    //    }

    //    public string GetPhoneImei()
    //    {
    //        return _imei;
    //    }

    //    public string GetPhoneImsi()
    //    {
    //        return _imsi;
    //    }

    //    public string GetPhoneNumber()
    //    {
    //        return _number;
    //    }

    //    public string GetSimOperator()
    //    {
    //        return _simOperator;
    //    }

    //    public string GetPhoneCPUName()
    //    {
    //        return _cpuName;
    //    }

    //    public string GetPhoneCPUFrequency()
    //    {
    //        return _cpuFrequency;
    //    }

    //    public int GetPhoneCores()
    //    {
    //        return _cpuCores;
    //    }

    //    public string GetMacAddress()
    //    {
    //        return _macAddress;
    //    }

    //    //安卓和pc直接返回空
    //    public string GetIDFA()
    //    {
    //        return string.Empty;
    //    }

    //    public void SetScreenBrightness(float value)
    //    {
    //        //_player.CallActionStatic("com.haowan123.plugin.utils.Utils", "changeAppBrightness");
            
    //    }
    //    // 获取手机信息
    //    private string[] getPhoneInfo()
    //    {
    //        /* return phone infos with special struct:
    //         * mtyb + "/" + mtype + "/" + imei + "/" + imsi + "/" + numer + "/" + serviceName
    //         */
    //        string infos = _player.CallFuncStatic<string>(CN_PLUGIN_DEVICE_INFO_CLASS_NAME, "getPhoneInfo") ?? string.Empty;
    //        string[] splitedInfo = infos.Split('/');
    //        if (splitedInfo.Length == 6)
    //        {
    //            _brand = splitedInfo[0];
    //            _model = splitedInfo[1];
    //            _imei = splitedInfo[2];
    //            _imsi = splitedInfo[3];
    //            _number = splitedInfo[4];
    //            _simOperator = splitedInfo[5];
    //        }
    //        return splitedInfo;
    //    }

    //    // 获取手机CPU信息
    //    private string[] getCpuInfo()
    //    {
    //        /*"CPU型号 " -> cpuInfo[0]
    //         *"CPU频率:" -> cpuInfo[1]
    //        */
    //        string infos = _player.CallFuncStatic<string>(CN_PLUGIN_DEVICE_INFO_CLASS_NAME, "getCpuInfo") ?? string.Empty;
    //        string[] splitedInfo = infos.Split('/');
    //        if (splitedInfo.Length == 2)
    //        {
    //            _cpuName = splitedInfo[0];
    //            _cpuFrequency = splitedInfo[1];
    //        }
    //        return splitedInfo;
    //    }

    //    // 获取CPU核心数
    //    private int getNumCores()
    //    {
    //        _cpuCores = _player.CallFuncStatic<int>(CN_PLUGIN_DEVICE_INFO_CLASS_NAME, "getNumCores");
    //        return _cpuCores;
    //    }

    //    // 获取电池信息
    //    private float GetBatteryRemain()
    //    {
    //        return _player.CallFuncStatic<float>(CN_PLUGIN_DEVICE_INFO_CLASS_NAME, "GetBatteryRemain");
    //    }

    //    //是否正在充电
    //    private bool GetBatteryCharging()
    //    {
    //        return _player.CallFuncStatic<int>(CN_PLUGIN_DEVICE_INFO_CLASS_NAME, "GetBatteryCharging") == 1;
    //    }

    //    // 获取手机信息
    //    private string getMacAddress()
    //    {
    //        /* return phone infos with special struct:
    //         * mtyb + "/" + mtype + "/" + imei + "/" + imsi + "/" + numer + "/" + serviceName
    //         */
    //        _macAddress = _player.CallFuncStatic<string>(CN_PLUGIN_DEVICE_INFO_CLASS_NAME, "GetMacAddress") ?? string.Empty;

    //        return _macAddress;
    //    }
    //}
#endif
}
