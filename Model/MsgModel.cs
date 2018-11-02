using System;
using System.Collections.Generic;
using System.Text;

namespace Wechat.Core.Model
{
    public class MsgModel
    {

        /// <summary>
        /// 消息发送方
        /// </summary>
        public string FromUserName
        {
            get;
            set;
        }
        /// <summary>
        /// 消息接收方
        /// </summary>
        public string ToUserName
        {
            set;
            get;
        }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content
        {
            get;
            set;
        }
        /// <summary>
        /// 消息类型
        /// </summary>
        public int MsgType
        {
            get;
            set;
        }
    }
}
