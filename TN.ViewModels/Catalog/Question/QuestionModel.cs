using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.Catalog.Question
{
    public class QuestionModel
    {
        public int ID { get; set; }
        public string QuesContent { get; set; } //Nội dung câu hỏi
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
        public string ImgURL { get; set; }
        public string Answer { get; set; } //{A,B,C,D}
        public int STT { get; set; }    //STT trong bài thi
        public int ExamID { get; set; }
    }
}
