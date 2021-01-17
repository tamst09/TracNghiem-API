using System;
using System.Collections.Generic;
using System.Text;
using TN.ViewModels.Common;

namespace TN.ViewModels.Catalog.Exams
{
    public class ExamPagingRequest : PagingRequestBase
    {
        public string keyword { get; set; } // dung de tim kiem
        public int CategoryID { get; set; } // phan loai
    }
}
