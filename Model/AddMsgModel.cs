using System;
using System.Collections.Generic;
using System.Text;

namespace Wechat.Core.Model
{
    public class AddMsgModel
    {
        public string MsgId { get; set; }
        public string FromUserName { get; set; }
        public string ToUserName { get; set; }
        public int MsgType { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
        public int ImgStatus { get; set; }
        public UInt64 CreateTime { get; set; }
        public int VoiceLength { get; set; }
        public int PlayLength { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public string MediaId { get; set; }
        public string Url { get; set; }
        public int AppMsgType { get; set; }
        public int StatusNotifyCode { get; set; }
        public string StatusNotifyUserName { get; set; }
        public RecommendModel RecommendInfo { get; set; }
        public int ForwardFlag { get; set; }
        public AppModel AppInfo { get; set; }
        public int HasProductId { get; set; }
        public string Ticket { get; set; }
        public int ImgHeight { get; set; }
        public int ImgWidth { get; set; }
        public int SubMsgType { get; set; }
        public UInt64 NewMsgId { get; set; }
    }
}
