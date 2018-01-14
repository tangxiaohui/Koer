using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;
using System.IO;
using System.Security.Cryptography;
using UpdateSystem.Log;

namespace UpdateSystem.Data
{
    /// <summary>
    /// 文件工具类，主要是计算md5
    /// </summary>
    public class MD5
    {
        //文件md5
        public static string MD5File(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return "";
            }

//             MD5 md5 = new MD5CryptoServiceProvider();
//             byte[] data = System.Text.Encoding.Default.GetBytes(filePath);//将字符编码为一个字节序列 
//             byte[] md5data = md5.ComputeHash(data);//计算data字节数组的哈希值 
//             md5.Clear();
//             string str = "";
//             for (int i = 0; i < md5data.Length; i++)
//             {
//                 str += md5data[i].ToString("x2");
//             }
//             return str;

            try
            {
                FileStream file = new FileStream(filePath, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                UpdateLog.ERROR_LOG("calc md5 fail : " + ex.Message + "\n" + ex.StackTrace);
                UpdateLog.EXCEPTION_LOG(ex);
            }

            return "";
        }

        //字符串md5
        public static string MD5String(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            try
            {
                byte[] data = System.Text.Encoding.Default.GetBytes(text);//将字符编码为一个字节序列 
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(data);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                UpdateLog.ERROR_LOG("calc md5 fail : " + ex.Message + "\n" + ex.StackTrace);
                UpdateLog.EXCEPTION_LOG(ex);
            }

            return "";
        }
    }
}
