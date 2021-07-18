using FrontEndWebApp.Exceptions;
using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.FavoriteExam;
using TN.ViewModels.Common;
using TN.ViewModels.Settings;

namespace FrontEndWebApp.Areas.User.Services
{
    public class FavoriteExamService : IFavoriteExamService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string accessToken;

        public FavoriteExamService(IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(ConstStrings.BASE_URL_API);
            _httpContextAccessor = httpContextAccessor;
            accessToken = _httpContextAccessor.HttpContext.Request.Cookies["access_token_cookie"];
        }

        public async Task<ResponseBase<bool>> Add(AddFavoriteExamRequest addFavoriteExamRequest)
        {
            var json = JsonConvert.SerializeObject(addFavoriteExamRequest);
            if (!string.IsNullOrEmpty(accessToken))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.PostAsync("/api/FavoriteExam", new StringContent(json, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                var dataJson = await response.Content.ReadAsStringAsync();
                ResponseBase<bool> dataObject = JsonConvert.DeserializeObject<ResponseBase<bool>>(dataJson);
                return dataObject;
            }
            else
            {
                return new ResponseBase<bool>(success: false, msg: $"Error: {response.StatusCode}", data: false);
            }
        }

        public async Task<ResponseBase<bool>> Delete(DeleteFavoriteExamRequest deleteFavoriteExamRequest)
        {
            var json = JsonConvert.SerializeObject(deleteFavoriteExamRequest);
            if (!string.IsNullOrEmpty(accessToken))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.PostAsync("/api/FavoriteExam/Remove", new StringContent(json, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                var dataJson = await response.Content.ReadAsStringAsync();
                ResponseBase<bool> dataObject = JsonConvert.DeserializeObject<ResponseBase<bool>>(dataJson);
                return dataObject;
            }
            else
            {
                return new ResponseBase<bool>(success: false, msg: $"Error: {response.StatusCode}", data: false);
            }
        }

        public async Task<ResponseBase<List<Exam>>> GetExams(GetAllFavoriteRequest getAllFavoriteRequest)
        {
            //if (!string.IsNullOrEmpty(accessToken))
            //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync($"/api/FavoriteExam?userId={getAllFavoriteRequest.userId}");
            if (response.IsSuccessStatusCode)
            {
                var dataJson = await response.Content.ReadAsStringAsync();
                ResponseBase<List<Exam>> exams = JsonConvert.DeserializeObject<ResponseBase<List<Exam>>>(dataJson);
                return exams;
            }
            else if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedException();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new ForbidenException();
            }
            else
            {
                return new ResponseBase<List<Exam>>(success: false, msg: $"Error: {response.StatusCode}", data: null);
            }
        }
    }
}
