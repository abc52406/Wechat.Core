using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Wechat.Core.Service
{
    /// <summary>
    /// 访问http服务器类
    /// </summary>
    public class BaseService
    {
        private static readonly string BaseUrl = "https://wx.qq.com/";
        private static readonly string AcceptHeadName = "Accept-Language";
        private static readonly string AcceptHeadValue = "zh-cn,en-us;q=0.8,zh-hk;q=0.6,ja;q=0.4,zh;q=0.2";
        private static readonly string UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.87 Safari/537.36";
        /// <summary>
        /// 访问服务器时的cookies
        /// </summary>
        private CookieContainer CookiesContainer = new CookieContainer();

        /// <summary>
        /// 向服务器发送get请求  返回服务器回复数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string DownloadString(string url)
        {
            var buf = DownloadBytes(url);
            return buf == null ? "" : Encoding.UTF8.GetString(buf, 0, buf.Length);
        }

        /// <summary>
        /// 向服务器发送get请求  返回服务器回复数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> DownloadStringAsync(string url)
        {
            var buf = await DownloadBytesAsync(url);
            return buf == null ? "" : Encoding.UTF8.GetString(buf, 0, buf.Length);
        }

        /// <summary>
        /// 向服务器发送get请求  返回服务器回复数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public byte[] DownloadBytes(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "get";
                request.UserAgent = UserAgent;
                request.Headers.Add(AcceptHeadName, AcceptHeadValue);
                request.Referer = BaseUrl;
                request.CookieContainer = CookiesContainer;  //启用cookie

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream response_stream = response.GetResponseStream();

                int count = (int)response.ContentLength;
                int offset = 0;
                byte[] buf = new byte[count];
                while (count > 0)  //读取返回数据
                {
                    int n = response_stream.Read(buf, offset, count);
                    if (n == 0) break;
                    count -= n;
                    offset += n;
                }
                return buf;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 向服务器发送get请求  返回服务器回复数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<byte[]> DownloadBytesAsync(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "get";
                request.UserAgent = UserAgent;
                request.Headers.Add(AcceptHeadName, AcceptHeadValue);
                request.Referer = BaseUrl;
                request.CookieContainer = CookiesContainer;  //启用cookie

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream response_stream = response.GetResponseStream();

                int count = (int)response.ContentLength;
                int offset = 0;
                byte[] buf = new byte[count];
                while (count > 0)  //读取返回数据
                {
                    int n = await response_stream.ReadAsync(buf, offset, count);
                    if (n == 0) break;
                    count -= n;
                    offset += n;
                }
                return buf;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 向服务器发送get请求  返回服务器回复数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public T DownloadModel<T>(string url)
        {
            byte[] bytes = DownloadBytes(url);
            string str = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(str);
        }

        /// <summary>
        /// 向服务器发送get请求  返回服务器回复数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<T> DownloadModelAsync<T>(string url)
        {
            byte[] bytes = await DownloadBytesAsync(url);
            string str = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(str);
        }

        /// <summary>
        /// 向服务器发送post请求 返回服务器回复数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public string UploadString(string url, string body)
        {
            var buf = UploadBytes(url, body);
            return buf == null ? "" : Encoding.UTF8.GetString(buf, 0, buf.Length);
        }

        /// <summary>
        /// 向服务器发送post请求 返回服务器回复数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<string> UploadStringAsync(string url, string body)
        {
            var buf = await UploadBytesAsync(url, body);
            return buf == null ? "" : Encoding.UTF8.GetString(buf, 0, buf.Length);
        }
        /// <summary>
        /// 向服务器发送post请求 返回服务器回复数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public byte[] UploadBytes(string url, string body)
        {
            try
            {
                byte[] request_body = Encoding.UTF8.GetBytes(body);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "post";
                request.UserAgent = UserAgent;
                request.Headers.Add(AcceptHeadName, AcceptHeadValue);
                request.Referer = BaseUrl;
                request.ContentLength = request_body.Length;
                request.CookieContainer = CookiesContainer;  //启用cookie
                Stream request_stream = request.GetRequestStream();
                request_stream.Write(request_body, 0, request_body.Length);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream response_stream = response.GetResponseStream();

                int count = (int)response.ContentLength;
                int offset = 0;
                byte[] buf = new byte[count];
                while (count > 0)  //读取返回数据
                {
                    int n = response_stream.Read(buf, offset, count);
                    if (n == 0) break;
                    count -= n;
                    offset += n;
                }
                return buf;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 向服务器发送post请求 返回服务器回复数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<byte[]> UploadBytesAsync(string url, string body)
        {
            try
            {
                byte[] request_body = Encoding.UTF8.GetBytes(body);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "post";
                request.UserAgent = UserAgent;
                request.Headers.Add(AcceptHeadName, AcceptHeadValue);
                request.Referer = BaseUrl;
                request.ContentLength = request_body.Length;
                request.CookieContainer = CookiesContainer;  //启用cookie
                Stream request_stream = request.GetRequestStream();
                request_stream.Write(request_body, 0, request_body.Length);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream response_stream = response.GetResponseStream();

                int count = (int)response.ContentLength;
                int offset = 0;
                byte[] buf = new byte[count];
                while (count > 0)  //读取返回数据
                {
                    int n = await response_stream.ReadAsync(buf, offset, count);
                    if (n == 0) break;
                    count -= n;
                    offset += n;
                }
                return buf;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取指定cookie
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Cookie GetCookie(string name)
        {
            List<Cookie> cookies = GetAllCookies(CookiesContainer);
            foreach (Cookie c in cookies)
            {
                if (c.Name == name)
                {
                    return c;
                }
            }
            return null;
        }

        private List<Cookie> GetAllCookies(CookieContainer cc)
        {
            List<Cookie> lstCookies = new List<Cookie>();

            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) lstCookies.Add(c);
            }
            return lstCookies;
        }
    }
}
