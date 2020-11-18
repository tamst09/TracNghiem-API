using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.Common
{
    public class PagedResultVM<T>
    {
        public int TotalRecord { get; set; }
        public List<T> Items { get; set; }
    }
}
