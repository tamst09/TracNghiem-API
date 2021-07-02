using FrontEndWebApp.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Common;
using TN.ViewModels.Settings;

namespace FrontEndWebApp.Areas.User.Services
{
    public class FavoriteExamService : IFavoriteExamService
    {
        private readonly CallApiService _apiService;

        public FavoriteExamService(CallApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<bool> Add(int userId, int examId, string accessToken)
        {
            var httpContent = JsonConvert.SerializeObject(new { userId = userId, examId = examId });
            var response = await _apiService.PostAsync($"/api/FavoriteExam", accessToken, httpContent);
            return response.Success;
        }

        public async Task<bool> Delete(int userId, int examId, string accessToken)
        {
            var httpContent = JsonConvert.SerializeObject(new { userId = userId, examId = examId });
            var response = await _apiService.PostAsync($"/api/FavoriteExam/remove", accessToken, httpContent);
            return response.Success;
        }

        public async Task<ResponseBase<List<Exam>>> GetExams(int userId, string accessToken)
        {
            var response = await _apiService.GetAsync($"/api/FavoriteExam?userId={userId}", accessToken);
            if (response.Success)
            {
                ResponseBase<List<Exam>> exams = JsonConvert.DeserializeObject<ResponseBase<List<Exam>>>(response.ContentBody);
                return exams;
            }
            return new ResponseBase<List<Exam>>()
            {
                success = response.Success,
                msg = response.StatusCode.ToString()
            };
        }
    }
}
