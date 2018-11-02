using System;
using System.Collections.Generic;
using System.Text;
using Wechat.Core.Model;

namespace Wechat.Core.Response
{
    /// <summary>
    /// 登录后获取信息
    /// </summary>
    public class LoginResponse: CommonResponse
    {
        /// <summary>
        /// 最近对话列表数量
        /// </summary>
        public long Count { get; set; }
        /// <summary>
        /// 最近会话清单
        /// </summary>
        public List<FriendUserModel> ContactList { get; set; }
        /// <summary>
        /// 同步校验参数
        /// </summary>
        public SyncModel SyncKey { get; set; }
        /// <summary>
        /// 登录用户信息
        /// </summary>
        public SelfUserModel User { get; set; }
        public string ChatSet { get; set; }
        public string SKey { get; set; }
        public long ClientVersion { get; set; }
        public long SystemTime { get; set; }
        public int GrayScale { get; set; }
        public int InviteStartCount { get; set; }
        /// <summary>
        /// 公众号消息数量
        /// </summary>
        public int MPSubscribeMsgCount { get; set; }
        /// <summary>
        /// 公众号清单
        /// </summary>
        public List<MpUserModel> MPSubscribeMsgList { get; set; }
        public long ClickReportInterval { get; set; }
    }
}
