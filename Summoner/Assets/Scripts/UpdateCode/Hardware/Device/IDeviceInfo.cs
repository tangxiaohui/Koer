namespace UpdateForm.Hardware
{
    /// <summary>
    /// 设备的信息
    /// </summary>
    public interface IDeviceInfo
    {
        //获取当前电量
        float GetBatteryPower();

        //获取屏幕的亮度
        float GetScreenBrightness();

        //手机品牌
        string GetPhoneBrand();

        //手机型号
        string GetPhoneModel();

        //imei -- 手机设备唯一码
        string GetPhoneImei();

        //imsi --- 手机入网注册码
        string GetPhoneImsi();

        //手机号码
        string GetPhoneNumber();

        //SIM卡运营商
        string GetSimOperator();

        //手机CPU型号
        string GetPhoneCPUName();

        //手机CPU频率
        string GetPhoneCPUFrequency();

        string GetMacAddress();

        //ios才有idfa
        string GetIDFA();

        //手机CPU个数
        int GetPhoneCores();

        //设置屏幕的亮度
        void SetScreenBrightness(float value);
    }
}
