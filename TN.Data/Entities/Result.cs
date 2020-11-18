using System;
using System.Collections.Generic;
using System.Text;

namespace TN.Data.Entities
{
    public class Result
    {
        public AppUser AppUser { get; set; }
        public int UserID { get; set; }
        public Question Question { get; set; }
        public int QuestionID { get; set; }
        public string OptChoose { get; set; }
    }
}
