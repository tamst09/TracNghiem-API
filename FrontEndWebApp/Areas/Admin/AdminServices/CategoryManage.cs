using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Category;
using TN.ViewModels.Common;
using TN.ViewModels.Settings;

namespace FrontEndWebApp.Areas.Admin.AdminServices
{
    public class CategoryManage : ICategoryManage
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient _httpClient;

        public CategoryManage(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(ConstStrings.BASE_URL_API);
        }

        public async Task<ResponseBase<Category>> Create(Category model, string accessToken)
        {
            if (accessToken != null)
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/categories/", httpContent);
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                ResponseBase<Category> category = JsonConvert.DeserializeObject<ResponseBase<Category>>(body);
                return category;
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseBase<string>> Delete(int id, string accessToken)
        {
            if (accessToken != null)
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.DeleteAsync("/api/categories/"+id.ToString());
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                ResponseBase<string> deleteResult = JsonConvert.DeserializeObject<ResponseBase<string>>(body);
                return deleteResult;
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseBase<string>> DeleteRange(DeleteRangeModel<int> lstId, string accessToken)
        {
            if (accessToken != null)
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var json = JsonConvert.SerializeObject(lstId);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/categories/deleterange", httpContent);
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                ResponseBase<string> deleteResult = JsonConvert.DeserializeObject<ResponseBase<string>>(body);
                return deleteResult;
            }
            else
            {
                return null;
            }
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
            var response = await _httpClient.GetAsync("/api/categories/exams/"+id.ToString());
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                ResponseBase<List<Exam>> lstExam = JsonConvert.DeserializeObject<ResponseBase<List<Exam>>>(body);
                return lstExam;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return new ResponseBase<List<Exam>>() { StatusCode = "401" };
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseBase<Category>> GetByID(int id)
        {
            var response = await _httpClient.GetAsync("/api/categories/"+id.ToString());
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                ResponseBase<Category> resultConverted = JsonConvert.DeserializeObject<ResponseBase<Category>>(result);
                return resultConverted;
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseBase<Category>> Update(int id, Category model, string accessToken)
        {
            if (accessToken != null)
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("/api/categories/"+id.ToString(), httpContent);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                ResponseBase<Category> resultConverted = JsonConvert.DeserializeObject<ResponseBase<Category>>(result);
                return resultConverted;
            }
            else
            {
                return null;
            }
        }
    }
}
