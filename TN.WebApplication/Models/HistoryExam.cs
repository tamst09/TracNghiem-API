using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.WebApplication.Models.Enums;

namespace TN.WebApplication.Models
{
    public class HistoryExam
    {
        public AppUser AppUser { get; set; }
        public int UserID { get; set; }
        public Exam Exam { get; set; }
        public int ExamID { get; set; }
        public HistoryStatus Status { get; set; }
        public int TimeRemain { get; set; }
    }
}
