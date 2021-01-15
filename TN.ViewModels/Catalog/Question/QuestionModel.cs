using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TN.ViewModels.Catalog.Question
{
    public class QuestionModel
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Không được để trống")]
        public string QuesContent { get; set; } //Nội dung câu hỏi
        [Required(ErrorMessage = "Không được để trống")]
        public string Option1 { get; set; }
        [Required(ErrorMessage = "Không được để trống")]
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
        public string ImgURL { get; set; }
        [Required(ErrorMessage = "Không được để trống")]
        public string Answer { get; set; } //{A,B,C,D}
        public int STT { get; set; }    //STT trong bài thi
        [Required(ErrorMessage = "Không được để trống")]
        public int ExamID { get; set; }
        public bool isActive { get; set; }
    }
}
