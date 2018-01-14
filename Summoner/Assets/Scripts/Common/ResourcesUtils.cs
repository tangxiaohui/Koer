using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public static class ResourcesUtils
    {
        public static string GetAssetRealPath(string originPathWithExt)
        {
            if(DevelopSetting.IsUsePersistent)
                return Common.StringUtils.CombineString(PathUtils.PERSISTENT_DATA_PATH, originPathWithExt);
            else
                return Common.StringUtils.CombineString(PathUtils.STREAMING_ASSET_PATH, originPathWithExt);
        }
    }
}
