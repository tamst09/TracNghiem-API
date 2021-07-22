using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.Catalog.Result
{
    public class EditResultRequest
    {
        public int UserId { get; set; }
        public int QuestionId { get; set; }
        public string Choice { get; set; }
    }
}
