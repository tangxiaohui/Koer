using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;

namespace UpdateSystem.MonoXml
{
    /// <summary>
    /// MonoXml的一些工具操作
    /// </summary>
    public static class MonoXmlUtils
    {
        /// <summary>
        /// 默认的xml的头字符串
        /// </summary>
        private const string CN_XML_HEAD = "<?xml version=\"{0}\" encoding=\"{1}\"?>";
        /// <summary>
        /// 获取xml文件的根节点
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static SecurityElement GetRootNodeFromFile(string fullPath)
        {
            SecurityParser parser = new SecurityParser();
            parser.LoadXml(File.ReadAllText(fullPath));
            return parser.ToXml();
        }
        /// <summary>
        /// 通过字符串获取根节点
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static SecurityElement GetRootNodeFromString(string text)
        {
            SecurityParser parser = new SecurityParser();
            parser.LoadXml(text);
            return parser.ToXml();
        }

        /// <summary>
        /// //把一个xml节点写入文件中
        /// </summary>
        /// <param name="element">xml节点</param>
        /// <param name="fileName">文件名</param>
        /// <param name="version">xml的版本</param>
        /// <param name="encoding">xml的编码格式</param>
        public static void WriteSecurityElementToFile(SecurityElement element, string fileName, string version = "1.0", string encoding = "utf-8")
        {
            var sb = StringUtils.NewStringBuilder;
            sb.AppendLine(string.Format(CN_XML_HEAD, version, encoding));
            sb.AppendLine(element.ToString());
            File.WriteAllText(fileName,sb.ToString());
        }

        /// <summary>
        /// //把一个xml字符串写入文件中
        /// </summary>
        /// <param name="element">xml节点</param>
        /// <param name="fileName">文件名</param>
        /// <param name="version">xml的版本</param>
        /// <param name="encoding">xml的编码格式</param>
        public static void WriteXmlStringToFile(string xmlString, string fileName, string version = "1.0", string encoding = "utf-8")
        {
            var sb = StringUtils.NewStringBuilder;
            sb.AppendLine(string.Format(CN_XML_HEAD, version, encoding));            
            sb.AppendLine(xmlString);
            File.WriteAllText(fileName, sb.ToString());
        }


        /// <summary>
        /// 从Xml的属性中解析数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attrs"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T TryParseValue<T>(SecurityElement attrs, String name,T defaultValue)
        {
            var itemValue = attrs.Attribute(name);
            if (!string.IsNullOrEmpty(itemValue))
            {
                Type t = typeof(T);
                if (!t.IsEnum)
                {
                    return (T)Convert.ChangeType(itemValue, typeof(T));
                }
                else
                {
                    return (T)System.Enum.Parse(t, itemValue);
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// 从Xml的属性中解析数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attrs"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryParseValue<T>(SecurityElement attrs, String name, ref T value)
        {
            var itemValue = attrs.Attribute(name);
            if (!string.IsNullOrEmpty(itemValue))
            {
                Type t = typeof(T);
                if (!t.IsEnum)
                {                    
                    value = (T)Convert.ChangeType(itemValue, typeof(T));
                }
                else
                {
                    value = (T)System.Enum.Parse(t, itemValue);
                }
            }
            return false;
        }

        public static SecurityParser LoadXmlEx(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                return null;
            }

            SecurityParser retval = null;
            if (File.Exists(filepath))
            {
                try
                {
                    using (var stream = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        if (stream != null)
                        {
                            retval = new SecurityParser();
                            StreamReader reader = new StreamReader(stream);
                            retval.LoadXml(reader.ReadToEnd());
                        }
                        stream.Dispose();
                        stream.Close();
                    }
                }
                catch (System.Exception ex)
                {
                     UpdateSystem.Log.UpdateLog.ERROR_LOG(ex.Message);
                }
            }
            return retval;
        }

        public static bool SaveXml(string filepath, SecurityElement root)
        {
            if (string.IsNullOrEmpty(filepath) || root == null)
            {
                return false;
            }

            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

            var stream = File.Open(filepath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
            if (stream != null)
            {
                StreamWriter writer = new StreamWriter(stream);
                if (writer != null)
                {
                    writer.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n");
                    writer.Write(root.ToString());
                    writer.Flush();
                    writer.Close();
                }
                stream.Dispose();
                stream.Close();
            }
            return true;
        }
    }
}
