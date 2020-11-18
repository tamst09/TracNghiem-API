using System;

namespace TN.ViewModels.Catalog.Exams
{
    public class ExamViewModel
    {
        public string ExamName { get; set; }
        public bool Status { get; set; }  //trạng thái public hay private
        public int? Time { get; set; }   //tính bằng second
        public string ImageURL { get; set; }
        public DateTime TimeCreated { get; set; }
        public int NumOfAttemps { get; set; }   // số lượt làm
        public int CategoryID { get; set; }
    }
}
