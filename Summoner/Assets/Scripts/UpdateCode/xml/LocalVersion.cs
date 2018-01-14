using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;
using System.Security;
using UpdateSystem.MonoXml;
using UpdateSystem.Log;
using UpdateSystem.Flow;

namespace UpdateSystem.Xml
{
    public partial class LocalVersionXml
    {
        string _TAG = "LocalVersion.cs ";
        SecurityElement dom = null;
        string _localVersionXml;

        /// <summary>
        /// 从xml内容解析
        /// </summary>
        /// <param name="xmlText"></param>
        /// <returns></returns>
        public int ParseFromText(string xmlText)
        {
            try
            {
                var rootNode = MonoXmlUtils.GetRootNodeFromString(xmlText);
                parse(rootNode);
            }
            catch (System.Exception ex)
            {
                UpdateLog.ERROR_LOG(ex.Message + "\n" + ex.StackTrace);
                UpdateLog.EXCEPTION_LOG(ex);
                return CodeDefine.RET_FAIL;
            }

            return CodeDefine.RET_SUCCESS;
        }

        /// <summary>
        /// 从文件解析
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public int parseLocalVersionXml(string path)
        {
            UpdateLog.INFO_LOG(_TAG + "parseLocalVersionXml(string path):  " + path);

            int ret = CodeDefine.RET_SUCCESS;
            try
            {
                var sp = MonoXmlUtils.LoadXmlEx(path);
                if( sp == null || sp.ToXml() == null ) {
                    UpdateLog.ERROR_LOG(_TAG + "File not exist or invalid: " + path);
                    return CodeDefine.RET_FAIL_PARSE_LOCAL_XML_FILE;
                } 

                dom = sp.ToXml();
                _localVersionXml = path;
                parse(dom);
            }
            catch (System.Exception ex)
            {
                ret = CodeDefine.RET_FAIL_PARSE_LOCAL_XML_FILE;
                
                UpdateLog.ERROR_LOG(_TAG + ex.Message + "\n" + ex.StackTrace);
                UpdateLog.EXCEPTION_LOG(ex);
            }

            return ret;
        }

        /// <summary>
        /// 解析过程
        /// </summary>
        /// <param name="dom"></param>
        private void parse(SecurityElement dom)
        {
            ResourceVersionUrl = parse(dom, "local_info/VersionInfoURL");
            BaseResVersion = parse(dom, "local_info/local_base_res_version");
            PatchResVersion = parse(dom, "local_info/local_patch_res_version");
            LocalAppVersion = parse(dom, "local_info/local_app_version");
            Fid = parse(dom, "local_info/platform_fid");
            Fgi = parse(dom, "local_info/fgi");
            HasCopy = parse(dom, "local_info/hasCopy");
            Developer = parse(dom, "local_info/developer");
            EnableDownload = parse(dom, "local_info/enableDownload");
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="baseVersion"></param>
        /// <param name="patchVersion"></param>
        /// <param name="hasCopy"></param>
        /// <param name="appVersion"></param>
        /// <returns></returns>
        public int save(string baseVersion="", string patchVersion="", string hasCopy="", string appVersion="")
        {
            UpdateLog.INFO_LOG(_TAG + "save()");
            int ret = CodeDefine.RET_SUCCESS;
            bool hasChange = false;
            try
            {
                if (!"".Equals(baseVersion) && baseVersion != _baseResVersion)
                {
                    _baseResVersion = baseVersion;
                    set( dom, "local_info/local_base_res_version", baseVersion );
                    hasChange = true;
                }
                if (!"".Equals(patchVersion)&& patchVersion != _patchResVersion)
                {
                    _patchResVersion = patchVersion;
                    hasChange = true;
                    set(dom, "local_info/local_patch_res_version", patchVersion);
                }
                if (!"".Equals(hasCopy) && hasCopy != _hasCopy)
                {
                    _hasCopy = hasCopy;
                    hasChange = true;
                    set(dom, "local_info/hasCopy", hasCopy);
                }
                if (!"".Equals(appVersion) && appVersion != _localAppVersion)
                {
                    _localAppVersion = appVersion;
                    hasChange = true;
                    set(dom, "local_info/local_app_version", appVersion);
                }
                if (hasChange)
                {
                    MonoXmlUtils.SaveXml(_localVersionXml, dom);
                }
                else
                {
                    UpdateLog.DEBUG_LOG("没有改动，不保存localversion.xml");
                }
            }
            catch (System.Exception ex)
            {
                ret = CodeDefine.RET_FAIL_SAVE_LOCAL_XML_FILE;
                UpdateLog.ERROR_LOG(_TAG + ex.Message + "\n" + ex.StackTrace);
                UpdateLog.EXCEPTION_LOG(ex);
            }

            return ret;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="localVersion"></param>
        /// <returns></returns>
        public int save(LocalVersionXml localVersion)
        {
            UpdateLog.INFO_LOG(_TAG + "save()");
            int ret = CodeDefine.RET_SUCCESS;
            try
            {
                set( dom, "local_info/local_base_res_version", localVersion.BaseResVersion );
                set( dom, "local_info/local_patch_res_version", localVersion.PatchResVersion );
                set( dom, "local_info/hasCopy", localVersion.HasCopy );
                set( dom, "local_info/local_app_version", localVersion.LocalAppVersion );
                MonoXmlUtils.SaveXml(_localVersionXml, dom);
            }
            catch (System.Exception ex)
            {
                ret = CodeDefine.RET_FAIL_SAVE_LOCAL_XML_FILE;
                UpdateLog.ERROR_LOG(_TAG + ex.Message + "\n" + ex.StackTrace);
                UpdateLog.EXCEPTION_LOG(ex);
            }

            return ret;
        }

        private string parse(SecurityElement dom, string domPath)
        {
            if( dom == null || string.IsNullOrEmpty( domPath ) ) {
                return string.Empty;
            }
            SecurityElement node = dom;
            var subpaths = domPath.Split( '/' );
            for( int i = 1; node != null && i < subpaths.Length; ++i ) {
                var tag = subpaths[i];
                node = node.SearchForChildByTag( tag );
            }
            return node != null ? node.Text : string.Empty;
        }

        private bool set(SecurityElement dom, string domPath, string value) {
            if( dom == null || string.IsNullOrEmpty( domPath ) ) {
                return false;
            }
            SecurityElement node = dom;
            var subpaths = domPath.Split( '/' );
            for( int i = 1; node != null && i < subpaths.Length; ++i ) {
                var tag = subpaths[i];
                node = node.SearchForChildByTag( tag );
            }
            if( node != null ) {
                node.Text = value;
                return true;
            }
            return false;
        }
    }
}
