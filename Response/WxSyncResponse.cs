using System;
using System.Collections.Generic;
using System.Text;
using Wechat.Core.Model;

namespace Wechat.Core.Response
{
    /// <summary>
    /// 更新会话返回结果
    /// </summary>
    public class WxSyncResponse: CommonResponse
    {
        public SyncModel SyncKey { get; set; }
        /// <summary>
        /// 收到的新消息数量
        /// </summary>
        public int AddMsgCount { get; set; }
        /// <summary>
        /// 收到的新消息详情
        /// </summary>
        public List<AddMsgModel> AddMsgList { get; set; }
        //public int ModContactCount { get; set; }
        //public List<ModContactModel> ModContactList { get; set; }
        //public int DelContactCount { get; set; }
        //public List<DelContactModel> DelContactList { get; set; }
        //public int ModChatRoomMemberCount { get; set; }
        //public List<ModChatRoomMemberModel> ModChatRoomMemberList { get; set; }
        //public ProfileModel ProfileModel { get; set; }
        //public int ContinueFlag { get; set; }
        //public int SKey { get; set; }
    }
}
