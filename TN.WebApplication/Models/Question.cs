using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.WebApplication.Models.Enums;

namespace TN.WebApplication.Models
{
    public class Question
    {
        public int ID { get; set; }
        public string QuesContent { get; set; } //Nội dung câu hỏi
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
        public string ImgURL { get; set; }
        public Opt Answer { get; set; } //{A,B,C,D}
        public int STT { get; set; }    //STT trong bài thi
        public Exam Exam { get; set; }
        public int ExamID { get; set; }
        public List<Result> Results { get; set; }
    }
}
