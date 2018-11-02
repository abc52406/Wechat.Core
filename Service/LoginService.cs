using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.DrawingCore;
using System.Threading.Tasks;

namespace Wechat.Core.Service
{
    /// <summary>
    /// 微信登录服务类
    /// </summary>
    public class LoginService : IDisposable
    {
        public string Pass_Ticket = "";
        public string SKey = "";
        private string _session_id = null;

        //获取会话ID的URL
        private static readonly string _session_id_url = "https://login.weixin.qq.com/jslogin?appid=wx782c26e4c19acffb";
        //获取二维码的URL
        private static readonly string _qrcode_url = "https://login.weixin.qq.com/qrcode/"; //后面增加会话id
        //判断二维码扫描情况   200表示扫描登录  201表示已扫描未登录  其它表示未扫描
        private static readonly string _login_check_url = "https://login.weixin.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid="; //后面增加会话id

        private BaseService Service;

        public LoginService(BaseService service)
        {
            Service = service;
        }

        #region 获取登录二维码
        /// <summary>
        /// 获取登录二维码
        /// 网页版微信登录第一步
        /// </summary>
        /// <returns></returns>
        public Image GetQRCode()
        {
            byte[] bytes = Service.DownloadBytes(_session_id_url);
            _session_id = Encoding.UTF8.GetString(bytes).Split(new string[] { "\"" }, StringSplitOptions.None)[1];
            bytes = Service.DownloadBytes(_qrcode_url + _session_id);

            return Image.FromStream(new MemoryStream(bytes));
        }
        /// <summary>
        /// 获取登录二维码
        /// 网页版微信登录第一步
        /// </summary>
        /// <returns></returns>
        public async Task<Image> GetQRCodeAsync()
        {
            byte[] bytes = await Service.DownloadBytesAsync(_session_id_url);
            _session_id = Encoding.UTF8.GetString(bytes).Split(new string[] { "\"" }, StringSplitOptions.None)[1];
            bytes = await Service.DownloadBytesAsync(_qrcode_url + _session_id);
            return Image.FromStream(new MemoryStream(bytes));
        }

        /// <summary>
        /// 获取登录二维码
        /// 网页版微信登录第一步
        /// </summary>
        /// <returns></returns>
        public string GetQRBase64()
        {
            byte[] bytes = Service.DownloadBytes(_session_id_url);
            _session_id = Encoding.UTF8.GetString(bytes).Split(new string[] { "\"" }, StringSplitOptions.None)[1];
            bytes = Service.DownloadBytes(_qrcode_url + _session_id);

            return Convert.ToBase64String(bytes);
        }
        /// <summary>
        /// 获取登录二维码
        /// 网页版微信登录第一步
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetQRBase64Async()
        {
            byte[] bytes = await Service.DownloadBytesAsync(_session_id_url);
            _session_id = Encoding.UTF8.GetString(bytes).Split(new string[] { "\"" }, StringSplitOptions.None)[1];
            bytes = await Service.DownloadBytesAsync(_qrcode_url + _session_id);

            return Convert.ToBase64String(bytes);
        }
        #endregion

        #region 登录扫描检测
        /// <summary>
        /// 登录扫描检测
        /// 微信登录第二步，展示给用户登录二维码之后，不断检测用户的扫码状态
        /// </summary>
        /// <returns></returns>
        public string LoginCheck()
        {
            if (_session_id == null)
            {
                return null;
            }
            byte[] bytes = Service.DownloadBytes(_login_check_url + _session_id);
            string login_result = Encoding.UTF8.GetString(bytes);
            if (login_result.Contains("=201")) //已扫描 未登录
            {
                string base64_image = login_result.Split(new string[] { "\'" }, StringSplitOptions.None)[1].Split(',')[1];
                //转成图片
                return base64_image;
            }
            else if (login_result.Contains("=200"))  //已扫描 已登录
            {
                string login_redirect_url = login_result.Split(new string[] { "\"" }, StringSplitOptions.None)[1];
                return login_redirect_url;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 登录扫描检测
        /// 微信登录第二步，展示给用户登录二维码之后，不断检测用户的扫码状态
        /// </summary>
        /// <returns>若以http开头，则是获取登录令牌的跳转地址；若是base64字符串，则是用户扫码后，展示用的用户头像</returns>
        public async Task<string> LoginCheckAsync()
        {
            if (_session_id == null)
            {
                return null;
            }
            byte[] bytes = await Service.DownloadBytesAsync(_login_check_url + _session_id);
            string login_result = Encoding.UTF8.GetString(bytes);
            if (login_result.Contains("=201")) //已扫描 未登录
            {
                string base64_image = login_result.Split(new string[] { "\'" }, StringSplitOptions.None)[1].Split(',')[1];
                //转成图片
                return base64_image;
            }
            else if (login_result.Contains("=200"))  //已扫描 已登录
            {
                string login_redirect_url = login_result.Split(new string[] { "\"" }, StringSplitOptions.None)[1];
                return login_redirect_url;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 获取登录令牌，并存放在cookies中
        /// <summary>
        /// 获取登录令牌，并存放在cookies中
        /// </summary>
        public void GetSidUid(string login_redirect)
        {
            byte[] bytes = Service.DownloadBytes(login_redirect + "&fun=new&version=v2&lang=zh_CN");
            string pass_ticket = Encoding.UTF8.GetString(bytes);
            Pass_Ticket = pass_ticket.Split(new string[] { "pass_ticket" }, StringSplitOptions.None)[1].TrimStart('>').TrimEnd('<', '/');
            SKey = pass_ticket.Split(new string[] { "skey" }, StringSplitOptions.None)[1].TrimStart('>').TrimEnd('<', '/');
        }

        /// <summary>
        /// 获取登录令牌，并存放在cookies中
        /// </summary>
        public async Task GetSidUidAsync(string login_redirect)
        {
            byte[] bytes = await Service.DownloadBytesAsync(login_redirect + "&fun=new&version=v2&lang=zh_CN");
            string pass_ticket = Encoding.UTF8.GetString(bytes);
            Pass_Ticket = pass_ticket.Split(new string[] { "pass_ticket" }, StringSplitOptions.None)[1].TrimStart('>').TrimEnd('<', '/');
            SKey = pass_ticket.Split(new string[] { "skey" }, StringSplitOptions.None)[1].TrimStart('>').TrimEnd('<', '/');
        }
        #endregion

        public void Dispose()
        {
            this.Service = null;
        }
    }
}
