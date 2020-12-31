using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.Common
{
    public class ExamPagingRequest : PagingRequestBase
    {
        public string keyword { get; set; } // dung de tim kiem
        public int? CategoryID { get; set; } // phan loai
    }
}
