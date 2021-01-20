using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TN.Data.Entities;

namespace FrontEndWebApp
{
    public static class Global 
    { 
        public static string Avatar_Url = "/images/cover/user/default_avatar.png";
        public static string ExamImg_Url = "/images/cover/exam/default_cover.jpg";
        public static string AccessToken = "";
        public static string RefreshToken = "";
        public static List<Result> results = new List<Result>();
        public static List<Question> questions = new List<Question>();
    } 
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
