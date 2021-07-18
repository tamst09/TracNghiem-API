using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.Common
{
    // ==== Nhan ket qua sau khi phan trang ==== //
    public class PagedResult<T> : PagedResultBase
    {
        public PagedResult()
        {
            Items = new List<T>();
        }

        public List<T> Items { get; set; }
    }
}
