using System;
using System.ComponentModel.DataAnnotations;
using TN.Data.Entities;

namespace TN.ViewModels.Catalog.Exams
{
    public class ExamModel
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Tên bài thi không được để trống")]
        public string ExamName { get; set; }
        public bool isPrivate { get; set; }  //trạng thái public hay private
        public int Time { get; set; }   //tính bằng second
        public string ImageURL { get; set; }
        public DateTime TimeCreated { get; set; }
        public int NumOfAttemps { get; set; }   // số lượt làm
        public int CategoryID { get; set; }
        public int OwnerID { get; set; }
        public bool isActive { get; set; }
    }
}
