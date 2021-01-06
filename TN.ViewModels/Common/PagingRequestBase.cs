using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.Common
{
    // ===== class cha dung de ke thua ===== //
    public class PagingRequestBase
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
