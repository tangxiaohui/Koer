using System;
using System.Collections.Generic;

using System.Text;
using System.Security.Cryptography;

namespace UpdateSystem.MonoXml
{
    /// <summary>
    /// 字符串工具类
    /// </summary>
    public static class StringUtils
    {

        static StringBuilder _strBuilder = new StringBuilder();
        static byte[] _bytes = new Byte[1024];

        public static StringBuilder StringBuilder
        {
            get
            {
                return _strBuilder;
            }
        }

        public static StringBuilder NewStringBuilder
        {
            get
            {
                _strBuilder.Length = 0;
                return _strBuilder;
            }
        }

        public static void SplitFilename(string qualifiedName, out string outBasename, out string outPath)
        {
            string path = qualifiedName.Replace('\\', '/');
            int i = path.LastIndexOf('/');
            if (i == -1)
            {
                outPath = string.Empty;
                outBasename = qualifiedName;
            }
            else
            {
                outBasename = path.Substring(i + 1, path.Length - i - 1);
                outPath = path.Substring(0, i + 1);
            }
        }

        public static string StandardisePath(string init)
        {
            string path = init.Replace('\\', '/');
            if (path.Length > 0 && path[path.Length - 1] != '/')
            {
                path += '/';
            }
            return path;
        }

        public static string StandardisePathWithoutSlash(string init)
        {
            string path = init.Replace('\\', '/');
            while (path.Length > 0 && path[path.Length - 1] == '/')
            {
                path = path.Remove(path.Length - 1);
            }
            return path;
        }

        /// <summary>
        /// 分割文件名
        /// </summary>
        /// <param name="fullName">文件名的全程</param>
        /// <param name="outBasename">基础文件名</param>
        /// <param name="outExtention">带"."的扩展名 </param>
        public static void SplitBaseFilename(string fullName, out string outBasename, out string outExtention)
        {
            int i = fullName.LastIndexOf('.');
            if (i == -1)
            {
                outExtention = string.Empty;
                outBasename = fullName;
            }
            else
            {
                outExtention = fullName.Substring(i);
                outBasename = fullName.Substring(0, i);
            }
        }

        public static int CountOf(string str, char what)
        {
            int count = 0;
            for (int i = 0; i < str.Length; ++i)
            {
                if (str[i] == what)
                {
                    ++count;
                }
            }
            return count;
        }

        public static string SafeFormat(string format, params object[] args)
        {
            if (format != null && args != null)
            {
                try
                {
                    return string.Format(format, args);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.Fail(e.Message, e.StackTrace);
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 分割全路径文件名
        /// </summary>
        /// <param name="qualifiedName">合格的名字</param>
        /// <param name="outBasename">文件名</param>
        /// <param name="outExtention">带有"."的扩展名</param>
        /// <param name="outPath">路径的目录</param>
        public static void SplitFullFilename(string qualifiedName, out string outBasename, out string outExtention, out string outPath)
        {
            string fullName = string.Empty;
            SplitFilename(qualifiedName, out fullName, out outPath);
            SplitBaseFilename(fullName, out outBasename, out outExtention);
        }

        public static string DecodeFromUtf8(string utf8String)
        {
            if (utf8String == null)
            {
                return string.Empty;
            }
            if (utf8String.Length > 0)
            {
                System.Diagnostics.Debug.Assert(_bytes.Length > utf8String.Length);
                for (int i = 0; i < utf8String.Length; ++i)
                {
                    _bytes[i] = (byte)utf8String[i];
                }
                return Encoding.UTF8.GetString(_bytes, 0, utf8String.Length);
            }
            return utf8String;
        }

        public static string EncodingEscape(string str, int startIndex = 0, int endIndex = -1)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            var s = new StringBuilder(str.Length + str.Length / 10 + 4);
            if (endIndex < 0)
            {
                endIndex = str.Length;
            }
            for (int i = startIndex; i < endIndex; ++i)
            {
                var c = str[i];
                switch (c)
                {
                    case '\'':
                        s.Append('\\');
                        s.Append('\'');
                        break;
                    case '\"':
                        s.Append('\\');
                        s.Append('\"');
                        break;
                    case '\a':
                        s.Append('\\');
                        s.Append('a');
                        break;
                    case '\b':
                        s.Append('\\');
                        s.Append('b');
                        break;
                    case '\f':
                        s.Append('\\');
                        s.Append('f');
                        break;
                    case '\n':
                        s.Append('\\');
                        s.Append('n');
                        break;
                    case '\r':
                        s.Append('\\');
                        s.Append('r');
                        break;
                    case '\t':
                        s.Append('\\');
                        s.Append('t');
                        break;
                    case '\v':
                        s.Append('\\');
                        s.Append('v');
                        break;
                    case '\\':
                        s.Append('\\');
                        s.Append('\\');
                        break;
                    default:
                        s.Append(c);
                        break;
                }
            }
            return s.ToString();
        }

        public static string CombineString(params string[] strs)
        {
            _strBuilder.Length = 0;
            for (int i = 0; i < strs.Length; i++)
            {
                _strBuilder.Append(strs[i]);
            }
            return _strBuilder.ToString();
        }

        /// <summary>
        /// 把Word转换为等Unicode字符串(\u09f9)
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string WordToUnicode(string word)
        {
            string outStr = "";
            if (!string.IsNullOrEmpty(word))
            {
                for (int i = 0; i < word.Length; i++)
                {
                    //将中文字符转为10进制整数，然后转为16进制unicode字符
                    outStr += "\\u" + ((int)word[i]).ToString("x");
                }
            }
            return outStr;
        }
        /// <summary>
        /// 把Unicode字符串(\u09f9)转换为word字符串
        /// </summary>
        /// <param name="unicode"></param>
        /// <returns></returns>
        public static string UnicodeToWord(string unicode)
        {
            string str = unicode;
            string outStr = "";
            if (!string.IsNullOrEmpty(str))
            {
                string[] strlist = str.Replace("\\", "").Split('u');
                try
                {
                    for (int i = 1; i < strlist.Length; i++)
                    {
                        //将unicode字符转为10进制整数，然后转为char中文字符
                        outStr += (char)int.Parse(strlist[i], System.Globalization.NumberStyles.HexNumber);
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.Fail(e.Message, e.StackTrace);
                }
            }
            return outStr;
        }

        /// <summary>
        /// 对字符串进行翻转
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static string Reverse(string original)
        {
            char[] arr = original.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
    }
}
