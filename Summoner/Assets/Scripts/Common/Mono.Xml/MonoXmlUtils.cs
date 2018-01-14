using System.IO;
using System.Security;
using System.Collections.Generic;

namespace Mono.Xml {
    public class MonoXmlUtils {

        public static SecurityElement LoadXmlSE(string filepath) {
            return LoadXml(filepath).ToXml();
        }

        public static SecurityParser LoadXml(string filepath) {
            if( string.IsNullOrEmpty( filepath ) ) {
                return null;
            }

            SecurityParser retval = null;
            if( Common.FileUtils.Exist( filepath ) )
            {
                try {
                    using (var stream = Common.FileUtils.OpenFileStream(filepath))
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
                } catch( System.Exception ex ) {
					UnityEngine.Debug.LogError (ex.ToString ());
                    //Common.ULogFile.sharedInstance.LogErrorEx( LogFile.AssetsLog, "load xml failed! path:{0}, exception:{1}", filepath, ex );
                }
            }
            return retval;
        }

        public static SecurityElement LoadXmlContentSE(string content)
        {
            return LoadXmlContent(content).ToXml();
        }

        public static SecurityParser LoadXmlContent(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            SecurityParser retval = null;
            try
            {
                retval = new SecurityParser();
                retval.LoadXml(content);
            }
            catch (System.Exception ex)
            {
                Common.ULogFile.sharedInstance.LogErrorEx(LogFile.AssetsLog, "load xml content failed! path:{0}, exception:{1}", content, ex);
            }
            return retval;
        }

        public static bool SaveXml(string filepath, SecurityElement root) {
            if( string.IsNullOrEmpty( filepath ) || root == null ) {
                return false;
            }

            if(Common.FileUtils.Exist( filepath ) ) {
                File.Delete( filepath );
            }

            Common.FileUtils.CreateDirectory( filepath );
            var stream = File.Open( filepath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write );
            if( stream != null ) {
                StreamWriter writer = new StreamWriter( stream );
                if( writer != null ) {
                    
                    writer.Write( "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" );
                    writer.Write( root.ToString() );
                    writer.Flush();
                    writer.Close();
                }
                stream.Dispose();
                stream.Close();
            }
            return true;
        }

        public static string Parse(SecurityElement dom, string domPath)
        {
            if (dom == null || string.IsNullOrEmpty(domPath))
            {
                return string.Empty;
            }
            SecurityElement node = dom;
            var subpaths = domPath.Split('/');
            for (int i = 1; node != null && i < subpaths.Length; ++i)
            {
                var tag = subpaths[i];
                node = node.SearchForChildByTag(tag);
            }
            return node != null ? node.Text : string.Empty;
        }

        public static bool Set(SecurityElement dom, string domPath, string value)
        {
            if (dom == null || string.IsNullOrEmpty(domPath))
            {
                return false;
            }
            SecurityElement node = dom;
            var subpaths = domPath.Split('/');
            for (int i = 1; node != null && i < subpaths.Length; ++i)
            {
                var tag = subpaths[i];
                node = node.SearchForChildByTag(tag);
            }
            if (node != null)
            {
                node.Text = value;
                return true;
            }
            return false;
        }

		public static bool SetRemoteVersion(SecurityElement dom, string tag, Dictionary<string, string> dic)
		{
			if (dom == null || string.IsNullOrEmpty(tag) || dic.Count == 0)
			{
				return false;
			}
			SecurityElement node = dom;
			SecurityElement child = new SecurityElement (tag);
			foreach (KeyValuePair<string, string> pair in dic)
			{
				SecurityElement _child = new SecurityElement (pair.Key, pair.Value);
				child.AddChild (_child);
			}
			node.AddChild(child);
			return true;
		}

        public static bool RemoveByTag(SecurityElement se, string tag)
        {
            SecurityElement item;
            for (int i = 0, count = se.Children.Count; i < count; ++i)
            {
                item = se.Children[i] as SecurityElement;
                if (string.IsNullOrEmpty(item.Tag) == false && item.Tag.CompareTo(tag) == 0)
                {
                    se.Children.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public static bool RemoveByValue(SecurityElement se, string value)
        {
            SecurityElement item;
            for (int i = 0, count = se.Children.Count; i < count; ++i)
            {
                item = se.Children[i] as SecurityElement;
                if (string.IsNullOrEmpty(item.Text) == false && item.Text.CompareTo(value) == 0)
                {
                    se.Children.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public static bool Add(SecurityElement se,string tag, string value)
        {
            SecurityElement item = new SecurityElement(tag, value);
            se.AddChild(item);
            return true;
        }

        public static bool AddAttr(SecurityElement se, string account, string value)
        {
            SecurityElement item = new SecurityElement("account", account);
            item.AddAttribute("pwd", value);
            se.AddChild(item);
            return true;
        }

		public static SecurityElement Create(string rootStr = "")
        {
			if (rootStr == "")
				rootStr = "root";
			SecurityElement root = new SecurityElement(rootStr);
            return root;
        }
    }
};