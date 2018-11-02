using System;
using System.Collections.Generic;
using System.Text;
using Wechat.Core.Model;

namespace Wechat.Core.Response
{
    /// <summary>
    /// 拉取联系人返回信息
    /// </summary>
    public class ContactResponse: CommonResponse
    {
        /// <summary>
        /// 最近对话列表数量
        /// </summary>
        public Int64 MemberCount { get; set; }
        /// <summary>
        /// 最近会话清单
        /// </summary>
        public List<FriendUserModel> MemberList { get; set; }
        public int Seq { get; set; }
    }
}
