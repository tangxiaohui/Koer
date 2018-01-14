using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Threading;
using UpdateSystem.Log;


namespace UpdateSystem.Trans
{
    /// <summary>
    /// 
    /// </summary>
    public class TransPCResource : TransResource
    {
        public override void BeginTransRes()
        {
            base.BeginTransRes();

            _success = true;
        }
    }
}