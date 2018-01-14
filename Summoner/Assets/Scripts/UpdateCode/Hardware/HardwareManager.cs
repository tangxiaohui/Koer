using UnityEngine;

namespace UpdateForm.Hardware
{
    /// <summary>
    /// 硬件管理器 -- 用于访问硬件信息的类
    /// 现在只有:内存,硬盘存储器,其他设备的管理,其中其他设备中有:cpu,电池,屏幕,芯片设备等
    /// </summary>
    public static class HardwareManager
    {        
        //内存信息
        private static IMemoryInfo _memoryInfo = null;
        //存储器信息
        private static IDiskInfo _diskInfo = null;     
        //其他设备信息
        private static IDeviceInfo _deviceInfo = null;
        
        //内存信息
        public static IMemoryInfo MemoryInfo
        {
            get { return _memoryInfo; }
        }

        //存储器信息
        public static IDiskInfo DiskInfo
        {
            get { return _diskInfo; }
        }

        //其他设备信息
        public static IDeviceInfo DeviceInfo
        {
            get { return _deviceInfo; }            
        }        

        //初始化
        static HardwareManager()
        {
            return;
//            switch (Application.platform)
//            {
//                case RuntimePlatform.Android:
//#if !UNITY_IPHONE
//                    _memoryInfo = new AndroidMemoryInfo();
//                    _diskInfo = new AndroidDiskInfo();
//                    _deviceInfo = new AndroidDeviceInfo();
//#endif
//                    break;
//                case RuntimePlatform.IPhonePlayer:
//                    _memoryInfo = new IPhoneMemoryInfo();
//                    _diskInfo = new IPhoneDiskInfo();
//                    _deviceInfo = new IPhoneDeviceInfo();
//                    break;
//                default:
//                    _memoryInfo = new DefaultMemoryInfo();
//                    _diskInfo = new DefaultDiskInfo();
//                    _deviceInfo = new DefaultDeviceInfo();
//                    break;
//            }

//            UnityEngine.Debug.Log(string.Format("手机内存：{0}  已用内存：{1}  可用内存：{2}", _memoryInfo.GetSumMemory(), _memoryInfo.GetAppUsedMemory(), _memoryInfo.GetFreeMemory()));
        }

        /// <summary>
        /// 返回mac地址、imei或者idfa，默认是空的
        /// </summary>
        /// <returns></returns>
        public static string GetMacOrImeiOrIdfa()
        {
            return string.Empty;
            string mac = _deviceInfo.GetMacAddress();
            string imei = _deviceInfo.GetPhoneImei();
            string idfa = _deviceInfo.GetIDFA();

            if (!string.IsNullOrEmpty(mac)) return mac;
            if (!string.IsNullOrEmpty(imei)) return imei;
            if (!string.IsNullOrEmpty(idfa)) return idfa;

            return "";
        }
    }
}
