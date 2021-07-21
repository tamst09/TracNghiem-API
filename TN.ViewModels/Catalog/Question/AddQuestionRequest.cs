using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.Catalog.Question
{
    public class AddQuestionRequest
    {
        public string QuesContent { get; set; } 
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
        public string Answer { get; set; }
        public int ExamID { get; set; }
    }
}
