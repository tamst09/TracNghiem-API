using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TN.ViewModels.Common;
using TN.ViewModels.Settings;

namespace FrontEndWebApp.Services
{
    public class CallApiService
    {
        private readonly HttpClient _httpClient;

        public CallApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(ConstStrings.BASE_URL_API);
        }

        public async Task<string> GetAsync(string url, string accessToken)
        {
            var response = await _httpClient.GetAsync(url);
            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                return body;
            }
            return null;
        }

        public async Task<string> PostAsync(string url, string accessToken, string modelJson)
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            HttpContent httpContent = null;
            if (!string.IsNullOrEmpty(modelJson))
            {
                httpContent = new StringContent(modelJson, Encoding.UTF8, "application/json");
            }
            var response = await _httpClient.PostAsync(url, httpContent);
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                return body;
            }
            return null;
        }
    }
}
