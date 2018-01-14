using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;
using System.Security;
using UpdateSystem.Flow;
using UpdateSystem.Log;
using UpdateSystem.MonoXml;

namespace UpdateSystem.Xml
{
    /// <summary>
    /// 服务器端配置的xml，用于资源更新
    /// </summary>
    public partial class ResourceVersionXml
    {
        string _TAG = "ResourceVersion.cs ";

        //解析xml
        public int parseResouceVersionXml(string path)
        {
            UpdateLog.INFO_LOG(_TAG + "parseResouceVersionXml(string path):" + path);

            int ret = CodeDefine.RET_SUCCESS;

            try
            {
                var sp = MonoXmlUtils.LoadXmlEx(path);
                if( sp == null || sp.ToXml() == null ) {
                    return CodeDefine.RET_FAIL_PARSE_RES_XML_FILE;
                }

                var dom = sp.ToXml();

                //正式流程
                parse(dom, "ResourceVersion/VersionBase", NormalFollow.VersionModelBaseList);
                parse(dom, "ResourceVersion/VersionPatch", NormalFollow.VersionModelPatchList);
                NormalFollow.AppVersion = parse(dom, "ResourceVersion/CodeVersion_last/Version");
                NormalFollow.ClientUrl = parse(dom, "ResourceVersion/CodeVersion_last/url");
                NormalFollow.AppSize = parse(dom, "ResourceVersion/CodeVersion_last/size");
                NormalFollow.Language = parse(dom, "ResourceVersion/loginSever/language");
                bool enable = true;
                if (!Boolean.TryParse(parse(dom, "ResourceVersion/ForceUpdate"), out enable))
                {
                    enable = true;
                }
                NormalFollow.EnableForceUpdate = enable;
                NormalFollow.PatchVersion = GetMaxPatchVersion(NormalFollow.VersionModelPatchList);

                //测试流程
                parse(dom, "ResourceVersion/test_tag/VersionBase", TestFollow.VersionModelBaseList);
                parse(dom, "ResourceVersion/test_tag/VersionPatch", TestFollow.VersionModelPatchList);
                TestFollow.AppVersion = parse(dom, "ResourceVersion/test_tag/app_current_version");
                TestFollow.ClientUrl = parse(dom, "ResourceVersion/test_tag/app_update_url");
                TestFollow.AppSize = parse(dom, "ResourceVersion/test_tag/test_size");
                TestFollow.Language = parse(dom, "ResourceVersion/test_tag/language");
                if (!Boolean.TryParse(parse(dom, "ResourceVersion/test_tag/ForceUpdate"), out enable))
                {
                    enable = true;
                }
                TestFollow.EnableForceUpdate = enable;
                TestFollow.PatchVersion = GetMaxPatchVersion(TestFollow.VersionModelPatchList);

                //白名单 mac地址
                var macList = parseNodes( dom, "ResourceVersion/test_tag/legal_client_machine_list/legal_client_machine" );
                for (int i = 0; macList != null && i < macList.Length; i++) {
                    if( macList[i] == null ) {
                        continue;
                    }
                    WhiteCode.Add(macList[i].Text);
                }

                //白名单 用户名
                var userList = parseNodes( dom, "ResourceVersion/test_tag/legal_client_user_list/legal_client_user" );
                for( int i = 0; userList != null && i < userList.Length; i++ ) {
                    if( userList[i] == null ) {
                        continue;
                    }
                    WhiteUsers.Add( userList[i].Text );
                }

                //白名单 ip地址
                var ipList = parseNodes( dom, "ResourceVersion/test_tag/legal_client_ip_list/legal_client_ip" );
                for( int i = 0; ipList != null && i < ipList.Length; i++ ) {
                    if( ipList[i] == null ) {
                        continue;
                    }
                    WhiteIp.Add( ipList[i].Text );
                }
            }
            catch (System.Exception ex)
            {
                ret = CodeDefine.RET_FAIL_PARSE_RES_XML_FILE;
                UpdateLog.ERROR_LOG(_TAG + ex.Message + "/n" + ex.StackTrace);
                UpdateLog.EXCEPTION_LOG(ex);
            }


            return ret;
        }

        private string GetMaxPatchVersion(List<VersionModel> verList)
        {
            int count = verList.Count;
            if (count > 0)
            {
                return verList[count - 1].ToVersion;
            }

            return "";
        }

        //解析节点
        private void parse(SecurityElement dom, string domPath, List<VersionModel> versionList)
        {
            var nodes = parseNodes( dom, domPath );
            if( nodes == null ) {
                return;
            }
            foreach( SecurityElement item in nodes ) {
                if( item == null ) {
                    continue;
                }

                VersionModel model = new VersionModel();
                var fromNode = item.SearchForChildByTag( "FromVersion" );
                if( fromNode != null ) {
                    model.FromVersion = fromNode.Text;
                }
                var ToVersion = item.SearchForChildByTag( "ToVersion" );
                if( ToVersion != null ) {
                    model.ToVersion = ToVersion.Text;
                }
                var PatchFile = item.SearchForChildByTag( "PatchFile" );
                if( PatchFile != null ) {
                    model.ResourceUrl = PatchFile.Text;
                }
                var PatchFileMD5 = item.SearchForChildByTag( "PatchFileMD5" );
                if( PatchFileMD5 != null ) {
                    model.Md5 = PatchFileMD5.Text;
                }
                var FileSize = item.SearchForChildByTag( "FileSize" );
                if( FileSize != null ) {
                    model.FileSize = FileSize.Text;
                }
                model.Stage = model.ToVersion;

                var mapFile = item.SearchForChildByTag( "mapFile" );
                if( mapFile != null ) {
                    model.Map_url = mapFile.Text;
                }
                var mapFileMD5 = item.SearchForChildByTag( "mapFileMD5" );
                if( mapFileMD5 != null ) {
                    model.Map_md5 = mapFileMD5.Text;
                }
                var mapFileSize = item.SearchForChildByTag( "mapFileSize" );
                if( mapFileSize != null ) {
                    model.Map_size = mapFileSize.Text;
                }

                versionList.Add( model );
            }
        }

        private string parse(SecurityElement dom, string domPath) {
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

        private SecurityElement[] parseNodes(SecurityElement dom, string domPath) {
            if( dom == null || string.IsNullOrEmpty( domPath ) ) {
                return null;
            }
            SecurityElement node = dom;
            List<SecurityElement> retval = null;
            var subpaths = domPath.Split( '/' );
            for( int i = 1; node != null && i < subpaths.Length; ++i ) {
                var tag = subpaths[i];
                if( i + 1 >= subpaths.Length &&
                    node != null && node.Children != null ) {
                    foreach( SecurityElement item in node.Children ){
                        if( item == null ) {
                            continue;
                        }
                        if( item.Tag == tag ) {
                            if( retval == null ) {
                                retval = new List<SecurityElement>();
                            }
                            retval.Add( item );
                        }
                    }
                    break;
                }
                node = node.SearchForChildByTag( tag );
            }
            return retval != null ? retval.ToArray() : null;
        }
    }
}
