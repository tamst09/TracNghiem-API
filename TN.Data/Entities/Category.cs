using System;
using System.Collections.Generic;
using System.Text;

namespace TN.Data.Entities
{
    public class Category
    {
        public Category()
        {
            isActive = true;
            Exams = new List<Exam>();
        }

        public int ID { get; set; }
        public string CategoryName { get; set; }    //tên chủ đề
        public List<Exam> Exams { get; set; }
        public bool isActive { get; set; }
    }
}
