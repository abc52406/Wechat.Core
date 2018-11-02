using System;
using System.Collections.Generic;
using System.Text;

namespace Wechat.Core.Model
{
    public class RecommendModel
    {
        public string UserName { get; set; }
        public string NickName { get; set; }
        public UInt64 QQNum { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Content { get; set; }
        public string Signature { get; set; }
        public string Alias { get; set; }
        public int Scene { get; set; }
        public int VerifyFlag { get; set; }
        public int AttrStatus { get; set; }
        public string Sex { get; set; }
        public string Ticket { get; set; }
        public Int64 OpCode { get; set; }
    }
}
