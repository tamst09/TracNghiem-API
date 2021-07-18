using System;
using System.Collections.Generic;
using System.Text;

namespace TN.Data.Entities
{
    public class Exam
    {
        public Exam()
        {
            Questions = new List<Question>();
            isActive = true;
        }

        public int ID { get; set; }
        public string ExamName { get; set; }
        public bool isPrivate { get; set; }  //trạng thái public hay private
        public int Time { get; set; }   //tính bằng second
        public string ImageURL { get; set; }
        public DateTime TimeCreated { get; set; }
        public int NumOfAttemps { get; set; }   // số lượt làm
        public Category Category { get; set; }
        public int CategoryID { get; set; }
        public AppUser Owner { get; set; }
        public int OwnerID { get; set; }
        public List<Question> Questions { get; set; }
        public bool isActive { get; set; }
    }
}
