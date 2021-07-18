using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.Catalog.Question
{
    public class GetQuestionsByExamRequest
    {
        public GetQuestionsByExamRequest(int examId)
        {
            ExamId = examId;
        }

        public int ExamId { get; set; }
    }
}
