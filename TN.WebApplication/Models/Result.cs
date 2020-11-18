using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.WebApplication.Models.Enums;

namespace TN.WebApplication.Models
{
    public class Result
    {
        public AppUser AppUser { get; set; }
        public int UserID { get; set; }
        public Question Question { get; set; }
        public int QuestionID { get; set; }
        public Opt OptChoose { get; set; }
    }
}
