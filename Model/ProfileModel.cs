using System;
using System.Collections.Generic;
using System.Text;

namespace Wechat.Core.Model
{
    public class ProfileModel
    {
        public int BitFlag { get; set; }
        public BuffModel UserName { get; set; }
        public BuffModel NickName { get; set; }
        public BuffModel BindEmail { get; set; }
        public BuffModel BindMobile { get; set; }
        public int BindUin { get; set; }
        public int Status { get; set; }
        public int Sex { get; set; }
        public int PersonalCard { get; set; }
        public string Alias { get; set; }
        public int HeadImgUpdateFlag { get; set; }
        public string HeadImgUrl { get; set; }
        public string Signature { get; set; }
    }
}
