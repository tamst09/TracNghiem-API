using System;
using System.Collections.Generic;
using System.Text;

namespace TN.Data.Entities
{
    public class FavoriteExam
    {
        public AppUser AppUser { get; set; }
        public int UserID { get; set; }
        public Exam Exam { get; set; }
        public int ExamID { get; set; }
    }
}
