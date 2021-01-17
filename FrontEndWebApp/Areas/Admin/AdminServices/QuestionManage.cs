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
using TN.ViewModels.Catalog.Question;
using TN.ViewModels.Common;
using TN.ViewModels.Settings;

namespace FrontEndWebApp.Areas.Admin.AdminServices
{
    public class QuestionManage : IQuestionManage
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient _httpClient;
        public QuestionManage(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(ConstStrings.BASE_URL_API);
        }

        public async Task<ResponseBase<Question>> Create(QuestionModel model, string accessToken)
        {
            if (accessToken != null)
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Questions/", httpContent);
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                ResponseBase<Question> createResult = JsonConvert.DeserializeObject<ResponseBase<Question>>(body);
                return createResult;
            }
            else
            {
                return null;
            }
        }

        public Task<ResponseBase<string>> Delete(int id, string accessToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseBase<string>> DeleteMany(DeleteRangeModel<int> lstId, string accessToken)
        {
            if (accessToken != null)
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var json = JsonConvert.SerializeObject(lstId);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Questions/DeleteMany", httpContent);
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

        public Task<ResponseBase<List<Question>>> GetAll(string accessToken)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseBase<List<Question>>> GetAllByExamID(int examID, string accessToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseBase<PagedResult<Question>>> GetAllPaging(QuestionPagingRequest model, string accessToken)
        {
            if (accessToken != null)
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Questions/Paged", httpContent);
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                ResponseBase<PagedResult<Question>> pagedResult = JsonConvert.DeserializeObject<ResponseBase<PagedResult<Question>>>(body);
                return pagedResult;
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseBase<Question>> GetByID(int id, string accessToken)
        {
            if (accessToken != null)
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync("api/Questions/"+id.ToString());
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                ResponseBase<Question> question = JsonConvert.DeserializeObject<ResponseBase<Question>>(body);
                return question;
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseBase<string>> GetNumberQuestion(string accessToken)
        {
            if (accessToken != null)
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync("api/Questions/GetNumber");
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                ResponseBase<string> updateResult = JsonConvert.DeserializeObject<ResponseBase<string>>(body);
                return updateResult;
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseBase<string>> Update(QuestionModel model, string accessToken)
        {
            if (accessToken != null)
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("api/Questions/"+model.ID, httpContent);
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                ResponseBase<string> updateResult = JsonConvert.DeserializeObject<ResponseBase<string>>(body);
                return updateResult;
            }
            else
            {
                return null;
            }
        }
    }
}
