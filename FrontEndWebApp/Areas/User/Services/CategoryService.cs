using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Common;
using TN.ViewModels.Settings;

namespace FrontEndWebApp.Areas.User.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient _httpClient;

        public CategoryService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(ConstStrings.BASE_URL_API);
        }

        public async Task<ResponseBase<List<Category>>> GetAll()
        {
            var response = await _httpClient.GetAsync("/api/categories/");
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                ResponseBase<List<Category>> lstCategory = JsonConvert.DeserializeObject<ResponseBase<List<Category>>>(body);
                return lstCategory;
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseBase<List<Exam>>> GetAllExams(int id, string accessToken)
        {
            if (accessToken != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            var response = await _httpClient.GetAsync("/api/categories/exams/" + id.ToString());
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                ResponseBase<List<Exam>> lstExam = JsonConvert.DeserializeObject<ResponseBase<List<Exam>>>(body);
                return lstExam;
            }
            else
            {
                return null;
            }
        }
    }
}
