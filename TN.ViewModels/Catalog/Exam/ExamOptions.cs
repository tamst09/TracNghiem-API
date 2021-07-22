using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.Catalog.Exam
{
    public class ExamOptions
    {
        public int Minute { get; set; }
        public int Second { get; set; }
        public int CountQuestion { get; set; }
        public bool NoCountDown { get; set; }
        public int ExamId { get; set; }
    }
}
