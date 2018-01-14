using System.Runtime.InteropServices;

namespace UpdateForm.Hardware
{
    ///// <summary>
    ///// 苹果设备信息
    ///// </summary>
    //public class IPhoneDeviceInfo : IDeviceInfo
    //{
    //    [DllImport("__Internal")]
    //    private static extern string get_model();          // 手机型号
    //    [DllImport("__Internal")]
    //    private static extern string get_imsi();           // imsi
    //    [DllImport("__Internal")]
    //    private static extern string get_number();         // 手机号
    //    [DllImport("__Internal")]
    //    private static extern string get_simOperator();    // 运营商
    //    [DllImport("__Internal")]
    //    private static extern string get_cpuName();        // 手机CPU型号
    //    [DllImport("__Internal")]
    //    private static extern string get_cpuFrequency();   // 手机CPU频率
    //    [DllImport("__Internal")]
    //    private static extern int get_cpuCores();          // 手机CPU个数

    //    [DllImport("__Internal")]
    //    private static extern string GetUID();              //获取UID --- ios7以上已经无用
    //    [DllImport("__Internal")]
    //    private static extern string GetImie();             //获取手机设备唯一码 
    //    [DllImport("__Internal")]
    //    private static extern string GetIdfa();             //获取广告ID,ios7以上有用 
    //    [DllImport("__Internal")]
    //    private static extern string GetMac();              //获取Mac地址,ios7以上已经无用     
    //    [DllImport("__Internal")]
    //    private static extern void set_net_handle(int handle, int msg_id, int head_size); //send net system handle to native code
    //    [DllImport("__Internal")]
    //    private static extern void init_sdk(string appVer, string platformID, int fgi);


    //    [DllImport("__Internal")]
    //    private static extern float get_screen_brightness();

    //    [DllImport("__Internal")]
    //    private static extern void set_screen_brightness(float val);

    //    [DllImport("__Internal")]
    //    private static extern float get_battery_remain();
       

    //    public float GetBatteryPower()
    //    {
    //        return get_battery_remain();
    //    }

    //    public float GetScreenBrightness()
    //    {
    //        return get_screen_brightness();
    //    }

    //    public string GetPhoneBrand()
    //    {
    //        return "Apple";
    //    }

    //    public string GetPhoneModel()
    //    {
    //        return get_model(); 
    //    }

    //    public string GetPhoneImei()
    //    {
    //        return GetImie();
    //    }

    //    public string GetPhoneImsi()
    //    {
    //        return get_imsi();
    //    }

    //    public string GetPhoneNumber()
    //    {
    //        return get_number(); 
    //    }

    //    public string GetSimOperator()
    //    {
    //        return get_simOperator();
    //    }

    //    public string GetPhoneCPUName()
    //    {
    //        return get_cpuName();
    //    }

    //    public string GetPhoneCPUFrequency()
    //    {
    //        return get_cpuFrequency();
    //    }

    //    public int GetPhoneCores()
    //    {
    //        return get_cpuCores();  
    //    }

    //    public string GetMacAddress()
    //    {
    //        return GetMac();
    //    }

    //    public string GetIDFA()
    //    {
    //        return GetIdfa();
    //    }

    //    public void SetScreenBrightness(float value)
    //    {
    //        set_screen_brightness(value);
    //    }
    //}
}
