using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TN.WebApplication.Models
{
    public class Category
    {
        public int ID { get; set; }
        public string CategoryName { get; set; }    //tên lĩnh vực hay môn học
        public List<Exam> Exams { get; set; }
    }
}
