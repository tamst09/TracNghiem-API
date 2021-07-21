using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.Catalog.Question
{
    public class AddListQuestionRequest
    {
        public AddListQuestionRequest()
        {
            Questions = new List<AddQuestionRequest>();
        }
        public List<AddQuestionRequest> Questions { get; set; }
    }
}
