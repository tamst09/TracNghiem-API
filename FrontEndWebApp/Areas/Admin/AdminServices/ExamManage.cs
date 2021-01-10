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

namespace FrontEndWebApp.Areas.Admin.AdminServices
{
    public class ExamManage : IExamManage
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient _httpClient;

        public ExamManage(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(ConstStrings.BASE_URL_API);
        }

        public async Task<List<Exam>> GetAll(string accessToken)
        {
            if (accessToken != null)
            {
                //_httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            var response = await _httpClient.GetAsync("/api/exams/");
            if (response.IsSuccessStatusCode)
            {
                var resultContent = await response.Content.ReadAsStringAsync();
                List<Exam> exams = JsonConvert.DeserializeObject<List<Exam>>(resultContent);
                return exams;
            }
            else
            {
                return null;
            }
        }

        public Task<PagedResult<Exam>> GetAllPaging(ExamPagingRequest model, string accessToken)
        {
            throw new NotImplementedException();
        }

        public async Task<Exam> GetByID(int id, string accessToken)
        {
            if (accessToken != null)
            {
                //_httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            var response = await _httpClient.GetAsync("/api/exams/"+id.ToString());
            if (response.IsSuccessStatusCode)
            {
                var resultContent = await response.Content.ReadAsStringAsync();
                Exam exam = JsonConvert.DeserializeObject<Exam>(resultContent);
                return exam;
            }
            else
            {
                return null;
            }
        }

        public Task<bool> Update(Exam request, string accessToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int examID, string accessToken)
        {
            throw new NotImplementedException();
        }

        public Task<Exam> Create(Exam request, int userID, string accessToken)
        {
            throw new NotImplementedException();
        }

        public Task<int> IncreaseAttemps(int examID, string accessToken)
        {
            throw new NotImplementedException();
        }
    }
}
