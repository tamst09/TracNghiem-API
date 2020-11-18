using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.Common
{
    public class GetExamPagingRequest : PagingRequestBase
    {
        public string keyword { get; set; }
        public int? CategoryID { get; set; }
    }
}
