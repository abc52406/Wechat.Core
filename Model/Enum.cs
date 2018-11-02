using System;
using System.Collections.Generic;
using System.Text;

namespace Wechat.Core.Model
{
    /// <summary>
    /// 登录状态
    /// </summary>
    public enum LoginState
    {
        /// <summary>
        /// 未登录
        /// </summary>
        Unlogin=0,
        /// <summary>
        /// 已扫描
        /// </summary>
        Scaned=1,
        /// <summary>
        /// 已登录
        /// </summary>
        Logined=2,
    }
}
