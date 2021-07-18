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

        public SessionService(string avatarUrl = "/images/cover/user/default_avatar.png", string examImgUrl = "/images/cover/exam/default_cover.jpg")
        {
            AvatarUrl = avatarUrl;
            ExamImgUrl = examImgUrl;
        }
    }
}
