using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontEndWebApp.Services
{
    public class SessionService
    {
        public string AvatarUrl { get; set; }
        public string ExamImgUrl { get; set; }
        public int UserId { get; set; }

        public SessionService(
            string avatarUrl = "http://res.cloudinary.com/tam-tht/image/upload/v1626518463/englishQuiz/avatarUser/default_avatar.png",
            string examImgUrl = "https://res.cloudinary.com/tam-tht/image/upload/v1626518636/englishQuiz/coverExams/default_cover.jpg")
        {
            AvatarUrl = avatarUrl;
            ExamImgUrl = examImgUrl;
        }
    }
}
