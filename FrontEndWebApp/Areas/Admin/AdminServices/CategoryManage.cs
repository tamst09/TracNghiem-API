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

        public async Task<bool> Create(Category model, string accessToken)
        {
            if (accessToken != null)
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/categories/", httpContent);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                if (result != null)
                {
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> Delete(int id, string accessToken)
        {
            if (accessToken != null)
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.DeleteAsync("/api/categories/"+id.ToString());
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                if (result != null)
                {
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteRange(DeleteRangeModel<int> lstId, string accessToken)
        {
            if (accessToken != null)
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var json = JsonConvert.SerializeObject(lstId);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/categories/deleterange", httpContent);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                if (result != null)
                {
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<Category>> GetAll()
        {
            var response = await _httpClient.GetAsync("/api/categories/");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                List<Category> resultConverted = JsonConvert.DeserializeObject<List<Category>>(result);
                return resultConverted;
            }
            else
            {
                return null;
            }
        }

        public async Task<Category> GetByID(int id)
        {
            var response = await _httpClient.GetAsync("/api/categories/"+id.ToString());
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Category resultConverted = JsonConvert.DeserializeObject<Category>(result);
                return resultConverted;
            }
            else
            {
                return null;
            }
        }

        public async Task<Category> Update(int id, Category model, string accessToken)
        {
            if (accessToken != null)
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("/api/categories/"+id.ToString(), httpContent);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Category resultConverted = JsonConvert.DeserializeObject<Category>(result);
                return resultConverted;
            }
            else
            {
                return null;
            }
        }
    }
}
