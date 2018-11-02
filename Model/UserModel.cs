using System;
using System.Collections.Generic;
using System.Text;

namespace Wechat.Core.Model
{
    /// <summary>
    /// 基础用户信息
    /// </summary>
    public class BaseUserModel
    {
        public Int64 Uin { get; set; }
        /// <summary>
        /// 用户ID，filehelper是文件传输助手，是特例
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 昵称首字母拼音
        /// </summary>
        public string PYInitial { get; set; }
        /// <summary>
        /// 昵称全拼
        /// </summary>
        public string PYQuanPin { get; set; }
        /// <summary>
        /// 备注首字母拼音
        /// </summary>
        public string RemarkPYInitial { get; set; }
        /// <summary>
        /// 备注全拼
        /// </summary>
        public string RemarkPYQuanPin { get; set; }
    }
    /// <summary>
    /// 自己用户信息
    /// </summary>
    public class SelfUserModel: BaseUserModel
    {
        /// <summary>
        /// 头像
        /// </summary>
        public string HeadImgUrl { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string RemarkName { get; set; }
        public int HideInputBarFlag { get; set; }
        public int StarFriend { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; set; }
        /// <summary>
        /// 个性签名
        /// </summary>
        public string Signature { get; set; }
        public int AppAccountFlag { get; set; }
        public int VerifyFlag { get; set; }
        public int ContactFlag { get; set; }
        public int WebWxPluginSwitch { get; set; }
        public int HeadImgFlag { get; set; }
        public int SnsFlag { get; set; }
    }
    /// <summary>
    /// 好友用户信息
    /// </summary>
    public class FriendUserModel: BaseUserModel
    {
        /// <summary>
        /// 头像
        /// </summary>
        public string HeadImgUrl { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string RemarkName { get; set; }
        public int ContactFlag { get; set; }
        /// <summary>
        /// 性别，1是男，2是女，0是未知
        /// </summary>
        public int Sex { get; set; }
        /// <summary>
        /// 个性签名
        /// </summary>
        public string Signature { get; set; }
        /// <summary>
        /// 0,自己和好友
        /// 56,微信团队
        /// 24,订阅号服务号
        /// 8,订阅号企业号和企业号子应用
        /// 28，订阅号服务号
        /// </summary>
        public int VerifyFlag { get; set; }
        public int StarFriend { get; set; }
        public int AppAccountFlag { get; set; }
        /// <summary>
        /// 4，微信团队
        /// 0，公众号企业号
        /// 33，自己
        /// 其他乱七八糟
        /// </summary>
        public Int64 AttrStatus { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 微信号
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// 0是公众号
        /// </summary>
        public int SnsFlag { get; set; }
        public string DisplayName { get; set; }
        public string KeyWord { get; set; }

        /// <summary>
        /// 显示的拼音全拼
        /// </summary>
        public string ShowPinYin
        {
            get
            {
                return (RemarkPYQuanPin == null || RemarkPYQuanPin == "") ? PYQuanPin : RemarkPYQuanPin;
            }
        }
    }
    /// <summary>
    /// 群聊会话用户
    /// </summary>
    public class MemberUserModel: BaseUserModel {
        public int AttrStatus { get; set; }
        public int MemberStatus { get; set; }
        public string DisplayName { get; set; }
        public string KeyWord { get; set; }
    }
    /// <summary>
    /// 公众号用户
    /// </summary>
    public class MpUserModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 图文数量
        /// </summary>
        public int MPArticleCount { get; set; }
        /// <summary>
        /// 图文列表
        /// </summary>
        public List<ArticleModel> MPArticleList { get; set; }
        /// <summary>
        /// 推送时间
        /// </summary>
        public Int64 Time { get; set; }
    }
}
