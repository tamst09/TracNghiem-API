using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.WebApplication.Models.Enums;

namespace TN.WebApplication.Models
{
    public class Exam
    {
        public int ID { get; set; }
        public string ExamName { get; set; }
        public ExamStatus Status { get; set; }  //trạng thái public hay private
        public int? Time { get; set; }   //tính bằng second
        public string ImageURL { get; set; }
        public DateTime TimeCreated { get; set; }
        public int NumOfAttemps { get; set; }   // số lượt làm
        public Category Category { get; set; }
        public int CategoryID { get; set; }
        public AppUser AppUser { get; set; }    //owner
        public int UserID { get; set; }
        public List<Question> Questions { get; set; }
        public List<HistoryExam> HistoryExams { get; set; }
    }
}
