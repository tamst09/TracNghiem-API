using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Exams;
using TN.ViewModels.Common;
using TN.ViewModels.Settings;

namespace FrontEndWebApp.Areas.User.Services
{
    public class ExamService : IExamService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ExamService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ResponseBase<Exam>> Create(ExamModel model, int userID, string accessToken)
        {
            HttpClient _httpClient = _httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(ConstStrings.BASE_URL_API);
            if (accessToken != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"/api/exams?userID={userID}", httpContent);
            if (response.IsSuccessStatusCode)
            {
                var resultContent = await response.Content.ReadAsStringAsync();
                ResponseBase<Exam> exam = JsonConvert.DeserializeObject<ResponseBase<Exam>>(resultContent);
                return exam;
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseBase<Exam>> Delete(int id, string accessToken, string userID)
        {
            HttpClient _httpClient = _httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(ConstStrings.BASE_URL_API);
            if (accessToken != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            var response = await _httpClient.DeleteAsync($"/api/exams?id={id}&userID={userID}");
            if (response.IsSuccessStatusCode)
            {
                var resultContent = await response.Content.ReadAsStringAsync();
                ResponseBase<Exam> exam = JsonConvert.DeserializeObject<ResponseBase<Exam>>(resultContent);
                return exam;
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseBase<List<Exam>>> GetAll(string accessToken, string userID)
        {
            HttpClient _httpClient = _httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(ConstStrings.BASE_URL_API);
            if (accessToken != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            var response = await _httpClient.GetAsync($"/api/exams/{userID}");
            if (response.IsSuccessStatusCode)
            {
                var resultContent = await response.Content.ReadAsStringAsync();
                ResponseBase<List<Exam>> exams = JsonConvert.DeserializeObject<ResponseBase<List<Exam>>>(resultContent);
                return exams;
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseBase<PagedResult<Exam>>> GetAllPaging(ExamPagingRequest model,string accessToken, string userID)
        {
            HttpClient _httpClient = _httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(ConstStrings.BASE_URL_API);
            if (accessToken != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"/api/exams/Paged?userID={userID}", httpContent);
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                ResponseBase<PagedResult<Exam>> lstExam = JsonConvert.DeserializeObject<ResponseBase<PagedResult<Exam>>>(body);
                lstExam.success = true;
                return lstExam;
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseBase<List<Exam>>> GetOwned(string accessToken, string userID)
        {
            HttpClient _httpClient = _httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(ConstStrings.BASE_URL_API);
            if (accessToken != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            var response = await _httpClient.GetAsync($"/api/exams/Owned/{userID}");
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                ResponseBase<List<Exam>> lstExam = JsonConvert.DeserializeObject<ResponseBase<List<Exam>>>(body);
                lstExam.success = true;
                return lstExam;
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseBase<Exam>> GetByID(int id, string accessToken, string userID)
        {
            HttpClient _httpClient = _httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(ConstStrings.BASE_URL_API);
            if (accessToken != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            var response = await _httpClient.GetAsync($"/api/exams?id={id}&userID={userID}");
            if (response.IsSuccessStatusCode)
            {
                var resultContent = await response.Content.ReadAsStringAsync();
                ResponseBase<Exam> exam = JsonConvert.DeserializeObject<ResponseBase<Exam>>(resultContent);
                return exam;
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseBase<int>> IncreaseAttemps(int id, string accessToken)
        {
            HttpClient _httpClient = _httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(ConstStrings.BASE_URL_API);
            if (accessToken != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            var response = await _httpClient.PostAsync($"/api/exams/IncreaseAttemp/{id}", null);
            if (response.IsSuccessStatusCode)
            {
                var resultContent = await response.Content.ReadAsStringAsync();
                ResponseBase<int> attemps = JsonConvert.DeserializeObject<ResponseBase<int>>(resultContent);
                return attemps;
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseBase<Exam>> Update(int id, ExamModel model, string accessToken, string userID)
        {
            HttpClient _httpClient = _httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(ConstStrings.BASE_URL_API);
            if (accessToken != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/exams?id={id}&userID={userID}", httpContent);
            if (response.IsSuccessStatusCode)
            {
                var resultContent = await response.Content.ReadAsStringAsync();
                ResponseBase<Exam> exam = JsonConvert.DeserializeObject<ResponseBase<Exam>>(resultContent);
                return exam;
            }
            else
            {
                return null;
            }
        }
    }
}
