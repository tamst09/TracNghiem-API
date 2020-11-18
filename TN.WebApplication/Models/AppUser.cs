using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TN.WebApplication.Models
{
    public class AppUser : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public DateTime DoB { get; set; }
        public List<HistoryExam> HistoryExams { get; set; }
        public List<Result> Results { get; set; }
        public List<Exam> Exams { get; set; }
    }
}
