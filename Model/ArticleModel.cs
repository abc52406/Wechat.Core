using System;
using System.Collections.Generic;
using System.Text;

namespace Wechat.Core.Model
{
    /// <summary>
    /// 单图文实体
    /// </summary>
    public class ArticleModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string Digest { get; set; }
        /// <summary>
        /// 封面地址
        /// </summary>
        public string Cover { get; set; }
        /// <summary>
        /// 图文地址
        /// </summary>
        public string Url { get; set; }
    }
}
