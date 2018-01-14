using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;
using System.IO;
using UpdateSystem.Log;
using UpdateSystem.Flow;

namespace UpdateSystem.Data
{
    class MapFileManage
    {
        string _TAG = "MapFileManage.cs ";
        List<MapFileData> _mapFileDataList = new List<MapFileData>();
        string _saveDir;

        public void setSaveDir(string saveDir)
        {
            _saveDir = saveDir;
        }

        public List<MapFileData> GetMapFileDataList()
        {
            return _mapFileDataList;
        }

        public int parseMapFile(string mapFile, string resUrl, string saveDir)
        {
            _saveDir = saveDir;
            UpdateLog.INFO_LOG(_TAG + "parseMapFile(string mapFile, string resUrl):" + mapFile + "," + resUrl);

            int ret = CodeDefine.RET_SUCCESS;

            List<MapFileData> mapFileDataList = _mapFileDataList;

            FileStream mapFileStream = null;
            try
            {
                mapFileStream = new FileStream(mapFile, FileMode.Open);

                long filePosition = 0;
                long mapFileSize = mapFileStream.Length;
                while (mapFileSize > 0 && filePosition != mapFileSize)
                {
                    if (filePosition >= mapFileSize)
                    {
                        UpdateLog.ERROR_LOG("解析出错了");
                        return ret;
                    }
                    MapFileData mapFileData = new MapFileData();
                    mapFileData.Begin = parseInt(read(mapFileStream, 10, filePosition, out filePosition));

                    if (mapFileData.Begin == -1)
                    {
                        UpdateLog.WARN_LOG("解析map文件，出现异常,请检查： " + mapFile);
                        return ret;
                    }

                    mapFileData.End = parseInt(read(mapFileStream, 10, filePosition, out filePosition));
                    mapFileData.DirLen = parseInt(read(mapFileStream, 10, filePosition, out filePosition));
                    mapFileData.NameLen = parseInt(read(mapFileStream, 10, filePosition, out filePosition));
                    mapFileData.Md5Len = parseInt(read(mapFileStream, 10, filePosition, out filePosition));
                    mapFileData.FileSize = parseInt(read(mapFileStream, 10, filePosition, out filePosition));
                    mapFileData.Dir = read(mapFileStream, mapFileData.DirLen, filePosition, out filePosition);
                    mapFileData.Name = read(mapFileStream, mapFileData.NameLen, filePosition, out filePosition);
                    mapFileData.Md5 = read(mapFileStream, mapFileData.Md5Len, filePosition, out filePosition);

                    mapFileData.ResUrl = resUrl;
                    mapFileData.SaveDir = _saveDir;

                    UnityEngine.Debug.Log("mapFileData.Name :" + mapFileData.Name);
                    mapFileDataList.Add(mapFileData);
                }
            }
            catch (System.Exception ex)
            {
                ret = CodeDefine.RET_FAIL_PARSE_MAP_FILE;
                UpdateLog.ERROR_LOG(_TAG + ex.Message + "\n" + ex.StackTrace);
                UpdateLog.EXCEPTION_LOG(ex);
            }
            finally
            {
                if (mapFileStream != null)
                {
                    mapFileStream.Close();
                }
            }
            return ret;
        }

        private int parseInt(string intStr)
        {
            int ret = 0;
            if (!int.TryParse(intStr, out ret))
            {
                return -1;
            }
            return ret;
        }

        private string read(FileStream mapFileStream, int readLen , long position, out long filePosition)
        {
            Byte[] beginBuf = new Byte[readLen];
            int readNum = mapFileStream.Read(beginBuf, 0, readLen);
            filePosition = position + readLen;
            if (readNum <= 0)
            {
                return null;
            }
            string retStr = System.Text.Encoding.Default.GetString(beginBuf);
            return retStr;
        }
    
    }
}
