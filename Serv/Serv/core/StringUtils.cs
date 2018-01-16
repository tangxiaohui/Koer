using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Common
{
    public static class StringUtils
    {
#if UNITY_EDITOR 
        static StringBuilder m_strBuilder = new StringBuilder(1024);  //初始为256，减少扩容的性能开销！
#else
        static StringBuilder m_strBuilder = new StringBuilder(256);  //初始为256，减少扩容的性能开销！
#endif
        static Byte[] m_bytes = new Byte[1024];

        public static StringBuilder stringBuilder
        {
            get
            {
                return m_strBuilder;
            }
        }

        public static StringBuilder newStringBuilder
        {
            get
            {
                m_strBuilder.Length = 0;
                return m_strBuilder;
            }
        }

        public static void SplitFilename(String qualifiedName, out String outBasename, out String outPath)
        {
            String path = qualifiedName.Replace('\\', '/');
            int i = path.LastIndexOf('/');
            if (i == -1)
            {
                outPath = String.Empty;
                outBasename = qualifiedName;
            }
            else
            {
                outBasename = path.Substring(i + 1, path.Length - i - 1);
                outPath = path.Substring(0, i + 1);
            }
        }

        public static String StandardisePath(String init)
        {
            String path = init.Replace('\\', '/');
            if (path.Length > 0 && path[path.Length - 1] != '/')
            {
                path += '/';
            }
            return path;
        }

        public static String StandardisePathWithoutSlash(String init)
        {
            String path = init.Replace('\\', '/');
            while (path.Length > 0 && path[path.Length - 1] == '/')
            {
                path = path.Remove(path.Length - 1);
            }
            return path;
        }

        public static void SplitBaseFilename(String fullName, out String outBasename, out String outExtention)
        {
            int i = fullName.LastIndexOf('.');
            if (i == -1)
            {
                outExtention = String.Empty;
                outBasename = fullName;
            }
            else
            {
                outExtention = fullName.Substring(i + 1);
                outBasename = fullName.Substring(0, i);
            }
        }

        public static int CountOf(String str, char what)
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

        public static String SafeFormat(String format, params object[] args)
        {
            if (format != null && args != null)
            {
                try
                {
                    return String.Format(format, args);
                }
                catch (Exception e)
                {
                }
            }
            return String.Empty;
        }

        public static void SplitFullFilename(String qualifiedName, out String outBasename, out String outExtention, out String outPath)
        {
            String fullName = String.Empty;
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
                for (int i = 0; i < utf8String.Length; ++i)
                {
                    m_bytes[i] = (byte)utf8String[i];
                }
                return Encoding.UTF8.GetString(m_bytes, 0, utf8String.Length);
            }
            return utf8String;
        }

        public static string CombineString(params string[] strs)
        {
            m_strBuilder.Length = 0;
            var length = strs.Length;
            for (int i = 0; i < length; i++)
            {
                m_strBuilder.Append(strs[i]);
            }
            return m_strBuilder.ToString();
        }

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
                catch (FormatException)
                {
                    /*print(ex.ToString());*/
                }
            }
            return outStr;
        }

        // string to md5
        public static string convertToMd5(string sourceStr)
        {
            MD5 newMd5 = new MD5CryptoServiceProvider();
            byte[] sourceBit = Encoding.Default.GetBytes(sourceStr);
            byte[] directBit = newMd5.ComputeHash(sourceBit);
            string directStr = BitConverter.ToString(directBit).Replace("-", "");
            return directStr;
        }
    }
}
