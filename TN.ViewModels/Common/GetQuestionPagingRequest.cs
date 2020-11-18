using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.Common
{
    public class GetQuestionPagingRequest : PagingRequestBase
    {
        public string keyword { get; set; }
        public int? ExamID { get; set; }
    }
}
