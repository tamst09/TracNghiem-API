using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.Catalog.FavoriteExam
{
    public class AddFavoriteExamRequest
    {
        public int userId { get; set; }
        public int examId { get; set; }
    }
}
