using System;
using System.Collections.Generic;
using System.Text;

namespace Wechat.Core.Model
{
    public class SyncModel
    {
        public long Count { get; set; }
        public List<SyncItemModel> List { get; set; }
    }
    public class SyncItemModel {

        public Int64 Key { get; set; }
        public Int64 Val { get; set; }
    }
}
