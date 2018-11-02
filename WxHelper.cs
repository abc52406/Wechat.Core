using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wechat.Core.Model;
using Wechat.Core.Response;
using Wechat.Core.Service;

namespace Wechat.Core
{
    public class WxHelper:IDisposable
    {
        private BaseService baseService;
        private LoginService loginService;
        private WXService wxService;

        /// <summary>
        /// 登录状态
        /// </summary>
        public LoginState State { get; private set; }

        /// <summary>
        /// 登录用户信息
        /// </summary>
        public SelfUserModel SelfUser { get; private set; }

        /// <summary>
        /// 最近联系人信息
        /// </summary>
        public List<FriendUserModel> LastConnectUser { get; private set; }

        /// <summary>
        /// 所有联系人信息
        /// </summary>
        public List<FriendUserModel> AllConnectUser { get; private set; }

        public WxHelper() {
            baseService = new BaseService();
            loginService = new LoginService(baseService);
            State= LoginState.Unlogin;
        }

        #region 网页微信登录
        /// <summary>
        /// 网页微信登录
        /// </summary>
        /// <param name="scanCallback">用户扫码回调，会传入用户头像base64字符串</param>
        /// <param name="LoginCallback">用户登录成功回调</param>
        /// <returns>登录扫一扫二维码Base64字符串</returns>
        public async Task<string> Login(Action<string> ScanCallback, Action LoginCallback,Action<MsgModel> GetMsgCallback, Action<string> LoginErrorCallback)
        {
            if (State == LoginState.Logined)
            {
                throw new Exception("已登录之后，不允许重新登录");
            }
            State = LoginState.Unlogin;
            var result = await loginService.GetQRBase64Async();
            var next = Task.Factory.StartNew(async () =>
            {
                try
                {
                    string login_result = null;
                    while (true)  //循环判断手机扫面二维码结果
                    {
                        login_result = loginService.LoginCheck();
                        if (!login_result.ToLower().StartsWith("http")) //已扫描 未登录
                        {
                            State = LoginState.Scaned;
                            var scanNotice = Task.Factory.StartNew(() =>
                            {
                                ScanCallback(login_result);
                            });
                        }
                        else if (!string.IsNullOrEmpty(login_result))  //已完成登录
                        {
                            //访问登录跳转URL
                            await loginService.GetSidUidAsync(login_result);
                            wxService = new WXService(baseService, loginService.SKey, loginService.Pass_Ticket);
                            State = LoginState.Logined;

                            var init_result = wxService.WxInit();  //登录初始化
                            #region 初始化个人信息和最近聊天列表
                            if (init_result != null)
                            {
                                SelfUser = init_result.User;
                                LastConnectUser = init_result.ContactList;
                            }
                            else
                            {
                                init_result = wxService.WxInit();  //登录初始化
                                if (init_result != null)
                                {
                                    SelfUser = init_result.User;
                                    LastConnectUser = init_result.ContactList;
                                }
                                else
                                {
                                    LoginErrorCallback("初始化登录用户信息失败，请重新登录");
                                    break;
                                }
                            }
                            #endregion
                            #region 初始化通讯录
                            var contact_result = wxService.GetContact(); //通讯录
                            if (contact_result != null)
                            {
                                AllConnectUser = contact_result.MemberList.OrderBy(c => c.ShowPinYin).ToList();
                            }
                            else {
                                contact_result = wxService.GetContact(); //通讯录
                                if (contact_result != null)
                                {
                                    AllConnectUser = contact_result.MemberList.OrderBy(c => c.ShowPinYin).ToList();
                                }
                                else
                                {
                                    LoginErrorCallback("初始化通讯录失败，请重新登录");
                                    break;
                                }
                            }
                            #endregion

                            var loginNotice = Task.Factory.StartNew(() =>
                            {
                                LoginCallback();
                            });
                            #region 循环拉取微信消息
                            string sync_flag = "";
                            WxSyncResponse sync_result;
                            while (true)
                            {
                                sync_flag =await wxService.WxSyncCheck();  //同步检查
                                if (sync_flag == null)
                                {
                                    continue;
                                }
                                //这里应该判断 sync_flag中selector的值
                                else //有消息
                                {
                                    sync_result =await wxService.WxSync();  //进行同步
                                    if (sync_result != null)
                                    {
                                        if (sync_result.AddMsgCount>0)
                                        {
                                            foreach (var m in sync_result.AddMsgList)
                                            {
                                                GetMsgCallback(new MsgModel() {
                                                    FromUserName=m.FromUserName,
                                                    ToUserName=m.ToUserName,
                                                    Content=m.Content,
                                                    MsgType=m.MsgType,
                                                });
                                            }
                                        }
                                    }
                                }
                                System.Threading.Thread.Sleep(10);
                            }
                            #endregion
                        }
                    }
                }
                catch (Exception ex) {
                    LoginErrorCallback(ex.Message);
                }
            });
            return result;
        }
        #endregion

        #region 初始化所有头像
        /// <summary>
        /// 初始化所有头像
        /// 由于所有联系人和最近会话中的头像数据量较大，因此需要单独调用函数进行初始化
        /// </summary>
        /// <returns></returns>
        public async Task InitHeadImg()
        {
            if (State != LoginState.Logined)
            {
                throw new Exception("未登录状态下无法初始化头像");
            }
            if (string.IsNullOrEmpty(SelfUser.HeadImgUrl))
            {
                SelfUser.HeadImgUrl = await wxService.GetHeadImgBase64Async(SelfUser.UserName);
            }
            for (int i = 0; i < AllConnectUser.Count(); i++)
            {
                var item = AllConnectUser[i];
                if (string.IsNullOrEmpty(item.HeadImgUrl))
                {
                    item.HeadImgUrl = await wxService.GetHeadImgBase64Async(item.UserName);
                }
            }
            for (int i = 0; i < LastConnectUser.Count(); i++)
            {
                var item = LastConnectUser[i];
                if (string.IsNullOrEmpty(item.HeadImgUrl))
                {
                    var connect = AllConnectUser.Where(c => c.UserName == item.UserName).FirstOrDefault();
                    if (connect == null)
                        item.HeadImgUrl = await wxService.GetHeadImgBase64Async(item.UserName);
                    else
                        item.HeadImgUrl = connect.HeadImgUrl;
                }
            }
        }
        #endregion

        /// <summary>
        /// 发消息
        /// </summary>
        /// <param name="userName">接收者</param>
        /// <param name="content">消息内容</param>
        /// <returns></returns>
        public async Task<BaseModel> SendMsg(string userName, string content)
        {
            var result = await wxService.SendMsgAsync(content, SelfUser.UserName, userName, 1);
            return result.BaseResponse;
        }

        /// <summary>
        /// 创建群聊
        /// </summary>
        /// <param name="userNames">初始成员</param>
        /// <returns>聊天室ID</returns>
        public async Task<CreateRoomModel> CreateRoom(IList<string> userNames)
        {
            var result = await wxService.CreateRoomAsync(userNames.ToList());
            return new CreateRoomModel()
            {
                ErrMsg = result.BaseResponse.ErrMsg,
                Ret = result.BaseResponse.Ret,
                ChatRoomName = result.ChatRoomName,
            };
        }

        /// <summary>
        /// 拉人进群聊
        /// </summary>
        /// <param name="roomName">聊天室ID</param>
        /// <param name="userNames">成员</param>
        /// <returns></returns>
        public async Task<BaseModel> AddMember(string roomName, IList<string> userNames) {
            var result= await wxService.AddMemberAsync(roomName, userNames.ToList());
            return result.BaseResponse;
        }

        /// <summary>
        /// 踢出群聊成员
        /// </summary>
        /// <param name="roomName">聊天室ID</param>
        /// <param name="userNames">成员</param>
        /// <returns></returns>
        public async Task<BaseModel> DeleteMember(string roomName, IList<string> userNames)
        {
            var result = await wxService.DeleteMemberAsync(roomName, userNames.ToList());
            return result.BaseResponse;
        }

        public void Dispose()
        {
            this.baseService = null;
            this.loginService.Dispose();
            this.loginService = null;
            this.wxService.Dispose();
            this.wxService = null;
        }
    }
}
