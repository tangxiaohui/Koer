using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using ICSharpCode.SharpZipLib.Zip;

namespace Common
{
    public static class FileUtils
    {
        private static byte[] TEMPBUFFER = new byte[1024];

#if UNITY_ANDROID && !UNITY_EDITOR
     private static ZipFile m_AssetsJarFile;
     private static ZipFile AssetsJarFile
     {
         get
         {
             if (null == m_AssetsJarFile)
             {
                 m_AssetsJarFile = new ZipFile(Application.dataPath);
             }
             return m_AssetsJarFile;
         }
     }
#endif
        #region File function
        public static System.IO.Stream OpenFileStream(string fullPath) 
        {
            long streamLength = 0;
            return OpenFileStream(fullPath, out streamLength);
        }

        public static System.IO.Stream OpenFileStream(string fullPath, out long streamLength)
        {
            Stream filestream = null;
            streamLength = 0;
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
             if (fullPath.StartsWith(PathUtils.STREAMING_ASSET_PATH))
 		    {
 			    ZipEntry zipEntry = AssetsJarFile.GetEntry(fullPath.Replace(Application.streamingAssetsPath, "assets"));
 			    filestream = AssetsJarFile.GetInputStream(zipEntry);
                 streamLength = zipEntry.Size;
             }
             else
#endif
                {
                    filestream = File.OpenRead(fullPath);
                    streamLength = filestream.Length;
                }
#if UNITY_EDITOR
                if (filestream == null || streamLength == 0)
                {
                    UnityEngine.Debug.LogError("OpenFile fail: " + fullPath);
                }
#endif
                return filestream;
            }
            catch (Exception e)
            {
                filestream = null;
                UnityEngine.Debug.LogException(e);
            }
            return filestream;
        }

        public static System.IO.MemoryStream OpenFileWithMemoryStream(string fullPath)
        {
            Stream stream = OpenFileStream(fullPath);
            try
            {
                if (stream != null)
                {
                    MemoryStream memStream = new MemoryStream((int)stream.Length);
                    while (true)
                    {
                        int bytesRead = stream.Read(TEMPBUFFER, 0, TEMPBUFFER.Length);
                        if (bytesRead > 0)
                        {
                            memStream.Write(TEMPBUFFER, 0, bytesRead);
                        }
                        else
                        {
                            memStream.Position = 0;
                            return memStream;
                        }
                    }
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream = null;
                }
            }
            return null;
        }

        static public bool Exist(string finalFullPath)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_IOS
            if (DevelopSetting.IsUsePersistent)
                finalFullPath = finalFullPath.Replace("file:///", "");
            else
                finalFullPath = finalFullPath.Replace("file://", "");
#elif UNITY_ANDROID
            if (DevelopSetting.IsUsePersistent)
                finalFullPath = finalFullPath.Replace("file:///", "");
#endif
            bool retval = File.Exists(finalFullPath);
#if UNITY_ANDROID && !UNITY_EDITOR
         if (!retval)
         {
             try 
             {
                 var path = finalFullPath.Replace(Application.streamingAssetsPath, "assets");
                 ZipEntry zipEntry = AssetsJarFile.GetEntry(path);
                 retval = zipEntry != null && zipEntry.IsFile;
             }
             catch(Exception e)
             {
                    Common.UDebug.LogException(e);
                    retval = false;
             }
         }
#endif
            return retval;
        }

        public static void ReadToBytes(ref Stream fs, out byte[] buffer, long length)
        {
            buffer = null;
            if (fs != null)
            {
                buffer = new byte[length];
                try
                {
                    fs.Read(buffer, 0, buffer.Length);
                    fs.Close();
                    fs.Dispose();
                    fs = null;
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                    buffer = null;
                }
            }
        }

        public static byte[] OpenFile(String filePath, out long length)
        {
            length = 0;
            Stream fs = OpenFileStream(filePath, out length);
            if (fs != null && length > 0)
            {
                Byte[] buff = null;
                ReadToBytes(ref fs, out buff, length);
                return buff;
            }
            return null;
        }

        // create a directory
        // each sub directories will be created if any of them don't exist.
        public static bool CreateDirectory(String dirName)
        {
            // first remove file name and extension;
            string fileNameAndExt = Path.GetFileName(dirName);
            if (!string.IsNullOrEmpty(fileNameAndExt))
            {
                dirName = dirName.Substring(0, dirName.Length - fileNameAndExt.Length);
            }

            StringBuilder sb = new StringBuilder();
            String[] dirs = dirName.Split('/');
            if (dirs.Length > 0)
            {
                if (dirName[0] == '/')
                {
                    // abs path tag on Linux OS
                    dirs[0] = "/" + dirs[0];
                }
            }
            for (int i = 0; i < dirs.Length; ++i)
            {
                if (dirs[i].Length == 0)
                {
                    continue;
                }
                if (sb.Length != 0)
                {
                    sb.Append('/');
                }
                sb.Append(dirs[i]);
                String cur = sb.ToString();
                if (String.IsNullOrEmpty(cur))
                {
                    continue;
                }
                if (!Directory.Exists(cur))
                {
                    var info = Directory.CreateDirectory(cur);
                    if (null == info)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static List<String> GetFileList(String path, Func<String, Boolean> filter, bool recursive = true)
        {
            var ret = new List<String>();
            var fs = new FileScanner(ret, filter, recursive);
            WalkTree(path, fs);
            return ret;
        }

        public static List<String> GetDirectoryList(String path, Func<String, Boolean> filter, bool recursive = true)
        {
            var ret = new List<String>();
            var fs = new DirectoryScanner(ret, filter, recursive);
            WalkTree(path, fs);
            return ret;
        }

        public static bool CreateFolder(string path)
        {
            try
            {
                int length = 0;
                while (length < path.Length)
                {
                    if (((path[length] == '\\') || (path[length] == '/')) && (length != 0))
                    {
                        string str = path.Substring(0, length);
                        if (!Directory.Exists(str))
                        {
                            Directory.CreateDirectory(str);
                        }
                    }
                    length++;
                }
                if ((length >= (path.Length - 1)) && !Directory.Exists(path) && (!path.Contains(".")))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception exception)
            {
                Common.ULogFile.sharedInstance.LogExceptionEx(LogFile.Exception, exception);
            }
            return Directory.Exists(path);
        }

        public static void SaveStringFile(string file, string cont)
        {
            SaveByteFile(file, System.Text.Encoding.UTF8.GetBytes(cont));
        }

        public static void SaveByteFile(string file, byte[] byteFile)
        {
            string path = file.Substring(0, file.LastIndexOf("/"));
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (File.Exists(file))
                {
                    File.Delete(file);
                    File.WriteAllBytes(file, byteFile);
                }
                else
                {
                    File.WriteAllBytes(file, byteFile);
                }
            }
            catch (Exception exception)
            {
                Common.ULogFile.sharedInstance.LogExceptionEx(LogFile.Exception, exception);
            }
        }

        public static bool rname(string from, string to)
        {
            try
            {
                if (!File.Exists(from))
                {
                    return false;
                }
                if (File.Exists(to))
                {
                    File.Delete(to);
                }
                File.Move(from, to);
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
            return false;
        }

        public static string GetWritePath(string path)
        {
            return Common.StringUtils.newStringBuilder.Append(PathUtils.PERSISTENT_DATA_PATH).Append("/").Append(path).ToString().Replace("//", "/");
        }

        #region internal function

        private interface ITreeWalker
        {
            bool IsRecursive();
            // will be called for each file while WalkTree is running
            bool DoFile(String name);
            // will be called for each directory while WalkTree is running
            bool DoDirectory(String name);
            // wildmatch pattern
            String FileSearchPattern();
            String DirectorySearchPattern();
        }
        private class BaseTreeWalker : ITreeWalker
        {
            public virtual bool IsRecursive() { return true; }
            public virtual bool DoFile(String name) { return true; }
            public virtual bool DoDirectory(String name) { return true; }
            public virtual String FileSearchPattern() { return "*"; }
            public virtual String DirectorySearchPattern() { return "*"; }
        }
        private class TreeDeleter : BaseTreeWalker, IDisposable
        {
            public List<String> fileList = new List<String>();
            public List<String> dirList = new List<String>();
            public override bool IsRecursive() { return true; }
            public override bool DoFile(String name)
            {
                fileList.Add(name);
                return true;
            }
            public override bool DoDirectory(String name)
            {
                dirList.Add(name);
                return true;
            }
            public void Dispose()
            {
                for (int i = 0; i < fileList.Count; ++i)
                {
                    File.Delete(fileList[i]);
                }
                for (int i = dirList.Count - 1; i >= 0; --i)
                {
                    Directory.Delete(dirList[i]);
                }
            }
        }
        public static bool RemoveTree(String dirName, bool delSelf = false)
        {
            int count = 0;
            using (var td = new TreeDeleter())
            {
                WalkTree(dirName, td);
                count = td.dirList.Count + td.fileList.Count;
            }
            if (delSelf)
            {
                if (Directory.Exists(dirName))
                {
                    Directory.Delete(dirName);
                }
            }
            return count != 0;
        }
        public static bool CopyTree(String from, String to)
        {
            int count = 0;
            return count != 0;
        }
        private static void WalkTree(String dirName, ITreeWalker walker)
        {
            dirName = Common.StringUtils.StandardisePath(dirName);
            LinkedList<String> dirStack = new LinkedList<String>();
            dirStack.AddLast(dirName);
            while (dirStack.Count > 0)
            {
                String lastPath = dirStack.Last.Value;
                dirStack.RemoveLast();
                DirectoryInfo di = new DirectoryInfo(lastPath);
                if (!di.Exists || (di.Attributes & FileAttributes.Hidden) != 0)
                {
                    continue;
                }
                foreach (FileInfo fileInfo in di.GetFiles(walker.FileSearchPattern()))
                {
                    // compose full file name from dirName
                    String f = lastPath;
                    if (f[f.Length - 1] == '/')
                    {
                        f += fileInfo.Name;
                    }
                    else
                    {
                        f = f + "/" + fileInfo.Name;
                    }
                    if (!walker.DoFile(f))
                    {
                        goto EXIT;
                    }
                }
                if (walker.IsRecursive())
                {
                    foreach (DirectoryInfo dirInfo in di.GetDirectories(walker.DirectorySearchPattern()))
                    {
                        // compose full path name from dirName
                        String p = lastPath;
                        if (p[p.Length - 1] == '/')
                        {
                            p += dirInfo.Name;
                        }
                        else
                        {
                            p = p + "/" + dirInfo.Name;
                        }
                        dirStack.AddLast(p);
                        FileAttributes fa = File.GetAttributes(p);
                        if ((fa & FileAttributes.Hidden) == 0)
                        {
                            if (!walker.DoDirectory(p))
                            {
                                goto EXIT;
                            }
                        }
                    }
                }
            }
            EXIT:
            ;
        }
        private class DirectoryScanner : BaseTreeWalker
        {
            List<String> m_allDirs;
            Func<String, Boolean> m_filter;
            bool m_recursive;
            public DirectoryScanner(List<String> fs, Func<String, Boolean> filter, bool recursive)
            {
                m_allDirs = fs;
                m_filter = filter;
                m_recursive = recursive;
            }
            public override bool IsRecursive()
            {
                return m_recursive;
            }
            public override bool DoDirectory(String name)
            {
                if (m_filter == null || !m_filter(name))
                {
                    m_allDirs.Add(name);
                }
                return true;
            }
        }
        private class FileScanner : BaseTreeWalker
        {
            List<String> m_allFiles;
            Func<String, Boolean> m_filter;
            bool m_recursive;
            public FileScanner(List<String> fs, Func<String, Boolean> filter, bool recursive)
            {
                m_allFiles = fs;
                m_filter = filter;
                m_recursive = recursive;
            }
            public override bool IsRecursive()
            {
                return m_recursive;
            }
            public override bool DoFile(String name)
            {
                if (m_filter == null || !m_filter(name))
                {
                    m_allFiles.Add(name);
                }
                return true;
            }
        }
        #endregion
        #endregion
    }

}
