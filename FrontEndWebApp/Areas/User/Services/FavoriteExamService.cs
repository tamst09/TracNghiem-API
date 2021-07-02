using FrontEndWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TN.Data.Entities;
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

        public Task<bool> Add(int userId, int examId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int userId, int examId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Exam>> GetExams(int userId)
        {
            HttpClient client = _httpClientFactory.CreateClient();
            client.BaseAddress()
            var response = await _httpClient.GetAsync("/api/categories/");
        }
    }
}
