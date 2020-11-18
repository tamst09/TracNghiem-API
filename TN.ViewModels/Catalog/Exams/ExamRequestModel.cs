using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.Catalog.Exams
{
    public class ExamRequestModel
    {
        public int ID { get; set; }
        public string ExamName { get; set; }
        public bool Status { get; set; }  //trạng thái public hay private
        public int? Time { get; set; }   //tính bằng second
        public string ImageURL { get; set; }
        public DateTime TimeCreated { get; set; }
        public int CategoryID { get; set; }
    }
}
