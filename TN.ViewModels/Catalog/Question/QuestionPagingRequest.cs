using System;
using System.Collections.Generic;
using System.Text;
using TN.ViewModels.Common;

namespace TN.ViewModels.Catalog.Question
{
    public class QuestionPagingRequest : PagingRequestBase
    {
        public string keyword { get; set; } // dung de tim kiem
        public int ExamID { get; set; } // phan loai
    }
}
