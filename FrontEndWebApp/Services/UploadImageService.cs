using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace FrontEndWebApp.Services
{
    public class UploadImageService
    {
        private static UploadImageService _instance;
        private readonly Account _acc;
        protected UploadImageService()
        {
            _acc = new Account { ApiKey = "271384337327975", ApiSecret = "JbpNWjA0CON1WqtZg6IN3BWo8-U", Cloud = "tam-tht" };
        }
        public static UploadImageService Instance()
        {
            if (_instance == null)
            {
                _instance = new UploadImageService();
            }
            return _instance;
        }
        public string Upload(string userName, string filePath)
        {
            Cloudinary _cloudinary = new Cloudinary(_acc);
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(filePath),
                Overwrite = true,
                PublicId = $"englishQuiz/avatarUser/{userName}"
            };
            var uploadResult = _cloudinary.Upload(uploadParams);
            if(uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                // return default avatar image path
                return "https://res.cloudinary.com/tam-tht/image/upload/v1626518463/englishQuiz/avatarUser/default_avatar.png";
            }
            return uploadResult.SecureUrl.AbsoluteUri;
        }
    }
}
