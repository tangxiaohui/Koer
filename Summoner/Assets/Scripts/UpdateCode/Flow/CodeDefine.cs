using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

namespace UpdateSystem.Flow
{
    /// <summary>
    /// 更新各流程的具体实现
    /// 1. 解析local xml
    /// 2. 下载和解析RemoteVersion
    /// 3. 下载执行端
    /// 4. 下载base基础资源
    /// 5. 资源校验
    /// 6. 下载补丁资源
    /// </summary>
    public class CodeDefine
    {
        /// <summary>
        /// 中断操作，中断下载
        /// </summary>
        public const int RET_SKIP_BY_ABORT = -17;
        /// <summary>
        /// 资源转移失败，不完全
        /// </summary>
        public const int RET_FAIL_TRANS_FAIL = -16;
        /// <summary>
        /// 下载失败
        /// </summary>
        public const int RET_FAIL_EXCEPTION_DOWNLOAD = -15;
        /// <summary>
        /// 解析remoteVersion.xml失败
        /// </summary>
        public const int RET_FAIL_PARSE_RES_XML_FILE = -14;
        /// <summary>
        /// 保存LocalVersion.xml失败
        /// </summary>
        public const int RET_FAIL_SAVE_LOCAL_XML_FILE = -13;
        /// <summary>
        /// 解析LocalVersion.xml失败
        /// </summary>
        public const int RET_FAIL_PARSE_LOCAL_XML_FILE = -12;
        /// <summary>
        /// 释放base基础资源
        /// </summary>
        public const int RET_FAIL_UNZIP_RES_FILE = -11;
        /// <summary>
        /// 解析map文件失败
        /// </summary>
        public const int RET_FAIL_PARSE_MAP_FILE = -10;
        /// <summary>
        /// 对比资源md5失败
        /// </summary>
        public const int RET_FAIL_RES_FILE_MD5_ERROR = -9;
        /// <summary>
        /// 资源不存在
        /// </summary>
        public const int RET_FAIL_RES_FILE_NOT_EXIST = -8;
        /// <summary>
        /// LocalVersion.xml文件不存在
        /// </summary>
        public const int RET_FAIL_LOCAL_XML_NOT_EXIST = -7;
        /// <summary>
        /// map文件的MD5值不正确
        /// </summary>
        public const int RET_FAIL_MAP_MD5_ERROR = -6;
        /// <summary>
        /// 下载map文件失败
        /// </summary>
        public const int RET_FAIL_DOWNLOAD_MAP_FILE = -5;
        /// <summary>
        /// RemoteVersion.xml文件名字不正确
        /// </summary>
        public const int RET_FAIL_RES_XML_PATH_ERROR = -4;
        /// <summary>
        /// 下载RemoteVersion.xml失败
        /// </summary>
        public const int RET_FAIL_DOWNLOAD_RES_XML = -3;
        /// <summary>
        /// 下载LocalVersion.xml失败
        /// </summary>
        public const int RET_FAIL_DOWNLOAD_LOCAL_XML = -2;
        /// <summary>
        /// 默认失败
        /// </summary>
        public const int RET_FAIL = -1;

        /// <summary>
        /// 初始化
        /// </summary>
        public const int RET_INIT = 0;
        /// <summary>
        /// 操作成功
        /// </summary>
        public const int RET_SUCCESS = 1;
        /// <summary>
        /// 因为取消操作，跳过正常流程，后续流程不执行，游戏内要判断这个值，不让进入游戏
        /// </summary>
        public const int RET_SKIP_BY_CANCEL = 2;

        /// <summary>
        /// 因为下载执行端，跳过后续更新流程
        /// </summary>
        public const int RET_SKIP_BY_DOWNLOAD_APP = 3;
        /// <summary>
        /// 因为下载执行端，跳过后续更新流程
        /// </summary>
        public const int RET_SKIP_BY_FORCE_TRANS_RESOURCE = 4;
        /// <summary>
        /// 因为后台下载分段资源，跳过资源释放
        /// </summary>
        public const int RET_SKIP_BY_BACKDOWNLOAD = 5;
        /// <summary>
        /// enableDownload字段true为支持下载，false不支持
        /// </summary>
        public const int RET_SKIP_BY_DISABLEDOWNLOAD = 5;

        /// <summary>
        /// 已经存在
        /// </summary>
        public const int RET_BACKDOWNLOAD_ALREADYEXIST = 1;

        /// <summary>
        /// 后台更新，当个文件下载成功
        /// </summary>
        public const int RET_BACKDOWNLOAD_SUCCESS = 0;
     
        /// <summary>
        /// http下载失败，包括MD5校验失败
        /// </summary>
        public const int RET_BACKDOWNLOAD_HTTPFAIL = -1;
        /// <summary>
        /// 因暂停跳过
        /// </summary>
        public const int RET_BACKDOWNLOAD_SKIPBYPAUSE = -2;
        /// <summary>
        /// 无效请求，map中不存在这个文件
        /// </summary>
        public const int RET_BACKDOWNLOAD_INVALIDFILE = -3;
    }
}
