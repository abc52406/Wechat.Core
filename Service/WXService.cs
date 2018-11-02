using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.DrawingCore;
using Wechat.Core.Response;
using Newtonsoft.Json.Linq;

namespace Wechat.Core.Service
{
    /// <summary>
    /// 微信主要业务逻辑服务类
    /// </summary>
    public class WXService : IDisposable
    {
        #region 私有变量
        /// <summary>
        /// 通讯校验信息
        /// </summary>
        private Dictionary<string, string> _syncKey = new Dictionary<string, string>();
        /// <summary>
        /// 微信初始化url
        /// </summary>
        private static readonly string _init_url = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxinit?r=1377482058764";
        /// <summary>
        /// 获取好友头像
        /// </summary>
        private static readonly string _geticon_url = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgeticon?username=";
        /// <summary>
        /// 获取群聊（组）头像
        /// </summary>
        private static readonly string _getheadimg_url = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetheadimg?username=";
        /// <summary>
        /// 获取好友列表
        /// </summary>
        private static readonly string _getcontact_url = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetcontact";
        /// <summary>
        /// 同步检查url
        /// </summary>
        private string _synccheck_url = "https://webpush.weixin.qq.com/cgi-bin/mmwebwx-bin/synccheck?sid={0}&uin={1}&synckey={2}&r={3}&skey={4}&deviceid={5}";
        /// <summary>
        /// 同步url
        /// </summary>
        private static readonly string _sync_url = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsync?sid=";
        /// <summary>
        /// 发送消息url
        /// </summary>
        private static readonly string _sendmsg_url = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsendmsg?sid=";
        /// <summary>
        /// 创建群聊url
        /// </summary>
        private static readonly string _addroom_url = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxcreatechatroom?pass_ticket={0}&r={1}";
        /// <summary>
        /// 删除群聊成员url
        /// </summary>
        private static readonly string _delmember_url = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxupdatechatroom?fun=delmember&pass_ticket={0}";
        /// <summary>
        /// 添加群聊成员url
        /// </summary>
        private static readonly string _addmember_url = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxupdatechatroom?fun=addmember&pass_ticket={0}";
        
        private static readonly string _addroom_msg_json = "{{\"BaseRequest\": {{\"Uin\": {0},\"Sid\": \"{1}\",\"Skey\": \"{2}\",\"DeviceID\": \"e441551176\"}},\"MemberCount\": {3},\"MemberList\": {4},\"Topic\": \"\"}}";

        private static readonly string _delmem_msg_json = "{{\"BaseRequest\": {{\"Uin\": {0},\"Sid\": \"{1}\",\"Skey\": \"{2}\",\"DeviceID\": \"e441551176\"}},\"ChatRoomName\": \"{3}\",\"DelMemberList\": \"{4}\"}}";

        private static readonly string _addmem_msg_json = "{{\"BaseRequest\": {{\"Uin\": \"{0}\",\"Sid\": \"{1}\",\"Skey\": \"{2}\",\"DeviceID\": \"e441551176\"}},\"ChatRoomName\": \"{3}\",\"AddMemberList\": \"{4}\"}}";

        private BaseService Service;
        private string SKey;
        private string PassTicket;
        #endregion

        public WXService(BaseService service, string sKey, string passTicket)
        {
            Service = service;
            SKey = sKey;
            PassTicket = passTicket;
        }

        #region 微信初始化
        /// <summary>
        /// 微信初始化
        /// </summary>
        /// <returns></returns>
        public LoginResponse WxInit()
        {
            string init_json = "{{\"BaseRequest\":{{\"Uin\":\"{0}\",\"Sid\":\"{1}\",\"Skey\":\"\",\"DeviceID\":\"e1615250492\"}}}}";
            Cookie sid = Service.GetCookie("wxsid");
            Cookie uin = Service.GetCookie("wxuin");

            if (sid != null && uin != null)
            {
                init_json = string.Format(init_json, uin.Value, sid.Value);
                string init_str = Service.UploadString(_init_url + "&pass_ticket=" + PassTicket, init_json);

                LoginResponse init_result = JsonConvert.DeserializeObject<LoginResponse>(init_str);

                foreach (var synckey in init_result.SyncKey.List)  //同步键值
                {
                    _syncKey.Add(synckey.Key.ToString(), synckey.Val.ToString());
                }
                return init_result;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 获取微信头像
        /// <summary>
        /// 获取微信头像
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string GetHeadImgBase64(string username)
        {
            if (username.Contains("@@"))  //讨论组
            {
                byte[] bytes = Service.DownloadBytes(_getheadimg_url + username);
                return Convert.ToBase64String(bytes);
            }
            else
            {
                byte[] bytes = Service.DownloadBytes(_geticon_url + username);
                return Convert.ToBase64String(bytes);
            }
        }
        /// <summary>
        /// 获取微信头像
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<string> GetHeadImgBase64Async(string username)
        {
            if (username.Contains("@@"))  //讨论组
            {
                byte[] bytes = await Service.DownloadBytesAsync(_getheadimg_url + username);
                return Convert.ToBase64String(bytes);
            }
            else
            {
                byte[] bytes = await Service.DownloadBytesAsync(_geticon_url + username);
                return Convert.ToBase64String(bytes);
            }
        }
        #endregion

        #region 获取好友列表
        /// <summary>
        /// 获取好友列表
        /// </summary>
        /// <returns></returns>
        public ContactResponse GetContact()
        {
            return Service.DownloadModel<ContactResponse>(_getcontact_url);
        }
        #endregion

        #region 微信同步检测
        /// <summary>
        /// 微信同步检测
        /// </summary>
        /// <returns></returns>
        public async Task<string> WxSyncCheck()
        {
            string sync_key = "";
            foreach (KeyValuePair<string, string> p in _syncKey)
            {
                sync_key += p.Key + "_" + p.Value + "%7C";
            }
            sync_key = sync_key.TrimEnd('%','7','C');

            Cookie sid = Service.GetCookie("wxsid");
            Cookie uin = Service.GetCookie("wxuin");

            if (sid != null && uin != null)
            {
                _synccheck_url = string.Format(_synccheck_url, sid.Value, uin.Value, sync_key, (long)(DateTime.Now.ToUniversalTime() - new System.DateTime(1970, 1, 1)).TotalMilliseconds, SKey.Replace("@", "%40"), "e1615250492");

                byte[] bytes =await Service.DownloadBytesAsync(_synccheck_url +"&_=" + DateTime.Now.Ticks);
                if (bytes != null)
                {
                    return Encoding.UTF8.GetString(bytes);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 微信同步
        /// <summary>
        /// 微信同步
        /// </summary>
        /// <returns></returns>
        public async Task<WxSyncResponse> WxSync()
        {
            string sync_json = "{{\"BaseRequest\" : {{\"DeviceID\":\"e1615250492\",\"Sid\":\"{1}\", \"Skey\":\"{5}\", \"Uin\":\"{0}\"}},\"SyncKey\" : {{\"Count\":{2},\"List\":[{3}]}},\"rr\" :{4}}}";
            Cookie sid = Service.GetCookie("wxsid");
            Cookie uin = Service.GetCookie("wxuin");

            string sync_keys = "";
            foreach (var p in _syncKey)
            {
                sync_keys += "{\"Key\":" + p.Key + ",\"Val\":" + p.Value + "},";
            }
            sync_keys = sync_keys.TrimEnd(',');
            sync_json = string.Format(sync_json, uin.Value, sid.Value, _syncKey.Count, sync_keys, (long)(DateTime.Now.ToUniversalTime() - new System.DateTime(1970, 1, 1)).TotalMilliseconds, SKey);

            if (sid != null && uin != null)
            {
                byte[] bytes = await Service.UploadBytesAsync(_sync_url + sid.Value + "&lang=zh_CN&skey=" + SKey + "&pass_ticket=" + PassTicket, sync_json);
                string sync_str = Encoding.UTF8.GetString(bytes);
                var sync_resul = JsonConvert.DeserializeObject<WxSyncResponse>(sync_str);

                if (sync_resul.SyncKey.Count > 0)
                {
                    _syncKey.Clear();
                    foreach (var key in sync_resul.SyncKey.List)
                    {
                        _syncKey.Add(key.Key.ToString(), key.Val.ToString());
                    }
                }
                return sync_resul;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 发送消息
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="type"></param>
        public async Task<CreateRoomResponse> SendMsgAsync(string msg, string from, string to, int type)
        {
            string msg_json = "{{" +
            "\"BaseRequest\":{{" +
                "\"DeviceID\" : \"e441551176\"," +
                "\"Sid\" : \"{0}\"," +
                "\"Skey\" : \"{6}\"," +
                "\"Uin\" : \"{1}\"" +
            "}}," +
            "\"Msg\" : {{" +
                "\"ClientMsgId\" : {8}," +
                "\"Content\" : \"{2}\"," +
                "\"FromUserName\" : \"{3}\"," +
                "\"LocalID\" : {9}," +
                "\"ToUserName\" : \"{4}\"," +
                "\"Type\" : {5}" +
            "}}," +
            "\"rr\" : {7}" +
            "}}";

            Cookie sid = Service.GetCookie("wxsid");
            Cookie uin = Service.GetCookie("wxuin");

            if (sid != null && uin != null)
            {
                msg_json = string.Format(msg_json, sid.Value, uin.Value, msg, from, to, type, SKey, DateTime.Now.Millisecond, DateTime.Now.Millisecond, DateTime.Now.Millisecond);

                byte[] bytes =await Service.UploadBytesAsync(_sendmsg_url + sid.Value + "&lang=zh_CN&pass_ticket="+PassTicket, msg_json);
                
                return JsonConvert.DeserializeObject<CreateRoomResponse>(Encoding.UTF8.GetString(bytes));
            }
            return null;
        }
        #endregion

        #region 创建群聊
        /// <summary>
        /// 创建群聊
        /// </summary>
        /// <param name="userNames">群成员</param>
        /// <returns></returns>
        public async Task<CreateRoomResponse> CreateRoomAsync(List<string> userNames)
        {
            Cookie sid = Service.GetCookie("wxsid");
            Cookie uin = Service.GetCookie("wxuin");

            byte[] bytes = await Service.UploadBytesAsync(string.Format(_addroom_url, PassTicket
                , (long)(DateTime.Now.ToUniversalTime() - new System.DateTime(1970, 1, 1)).TotalMilliseconds)
                , string.Format(_addroom_msg_json, uin.Value, sid.Value, SKey, userNames.Count()
                , JsonConvert.SerializeObject(userNames.Select(c => new { UserName = c }).ToList(), Formatting.None)));
            string room_str = Encoding.UTF8.GetString(bytes);

            return JsonConvert.DeserializeObject<CreateRoomResponse>(room_str);
        }
        #endregion

        #region 增加群聊成员
        /// <summary>
        /// 增加群聊成员
        /// </summary>
        /// <param name="roomName">房间名</param>
        /// <param name="userNames">成员</param>
        public async Task<CommonResponse> AddMemberAsync(string roomName, List<string> userNames)
        {
            Cookie sid = Service.GetCookie("wxsid");
            Cookie uin = Service.GetCookie("wxuin");
            byte[] bytes = await Service.UploadBytesAsync(string.Format(_addmember_url, PassTicket)
                , string.Format(_addmem_msg_json, uin.Value, sid.Value, SKey, roomName
                , string.Join(",", userNames)));
            string delmem_str = Encoding.UTF8.GetString(bytes);

            return JsonConvert.DeserializeObject<CommonResponse>(delmem_str);
        }
        #endregion

        #region 移除群聊成员
        /// <summary>
        /// 移除群聊成员
        /// </summary>
        /// <param name="roomName">房间名</param>
        /// <param name="userNames">成员</param>
        /// <returns></returns>
        public async Task<CommonResponse> DeleteMemberAsync(string roomName, List<string> userNames)
        {
            Cookie sid = Service.GetCookie("wxsid");
            Cookie uin = Service.GetCookie("wxuin");
            byte[] bytes =await Service.UploadBytesAsync(string.Format(_delmember_url, PassTicket)
                , string.Format(_delmem_msg_json, uin.Value, sid.Value, SKey, roomName
                , string.Join(",", userNames)));
            string delmem_str = Encoding.UTF8.GetString(bytes);

            return JsonConvert.DeserializeObject< CommonResponse>(delmem_str);
        }
        #endregion

        public void Dispose()
        {
            this.Service = null;
        }

    }
}
