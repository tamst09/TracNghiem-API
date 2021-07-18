using FrontEndWebApp.Exceptions;
using Microsoft.AspNetCore.Http;
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
    public class ApiHelper : IApiHelper
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _accessToken;
        public ApiHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(ConstStrings.BASE_URL_API);
            _httpContextAccessor = httpContextAccessor;
            _accessToken = CookieEncoder.DecodeToken(_httpContextAccessor.HttpContext.Request.Cookies["access_token_cookie"]);
        }

        public async Task<ResponseBase<TResponse>> NonBodyQueryAsync<TResponse>(HttpMethod httpMethod, string url) where TResponse : class
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(httpMethod, $"{ConstStrings.BASE_URL_API}{url}");
            if (!string.IsNullOrEmpty(_accessToken))
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            var response = await _httpClient.SendAsync(httpRequestMessage);
            if (response.IsSuccessStatusCode)
            {
                var bodyJson = await response.Content.ReadAsStringAsync();
                ResponseBase<TResponse> content = JsonConvert.DeserializeObject<ResponseBase<TResponse>>(bodyJson);
                return content;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedException();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new ForbidenException();
            }
            else
            {
                return new ResponseBase<TResponse>(success: false, msg: $"Error: {response.StatusCode}", data: null);
            }
        }

        public async Task<ResponseBase<TResponse>> QueryAsync<TRequest, TResponse>(HttpMethod httpMethod, string url, TRequest request) where TRequest:class where TResponse: class
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(httpMethod, $"{ConstStrings.BASE_URL_API}{url}");
            httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            if (!string.IsNullOrEmpty(_accessToken))
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var response = await _httpClient.SendAsync(httpRequestMessage);
            if (response.IsSuccessStatusCode)
            {
                var bodyJson = await response.Content.ReadAsStringAsync();
                ResponseBase<TResponse> content = JsonConvert.DeserializeObject<ResponseBase<TResponse>>(bodyJson);
                return content;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedException();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new ForbidenException();
            }
            else
            {
                return new ResponseBase<TResponse>(success: false, msg: $"Error: {response.StatusCode}", data: null);
            }
        }

        public async Task<ResponseBase> CommandAsync<TRequest>(HttpMethod httpMethod, string url, TRequest request) where TRequest : class
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(httpMethod, $"{ConstStrings.BASE_URL_API}{url}");
            httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            if (!string.IsNullOrEmpty(_accessToken))
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var response = await _httpClient.SendAsync(httpRequestMessage);
            if (response.IsSuccessStatusCode)
            {
                var bodyJson = await response.Content.ReadAsStringAsync();
                ResponseBase content = JsonConvert.DeserializeObject<ResponseBase>(bodyJson);
                return content;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedException();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new ForbidenException();
            }
            else
            {
                return new ResponseBase(success: false, msg: $"Error: {response.StatusCode}");
            }
        }

        public async Task<ResponseBase> NonBodyCommandAsync(HttpMethod httpMethod, string url)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(httpMethod, $"{ConstStrings.BASE_URL_API}{url}");
            
            if (!string.IsNullOrEmpty(_accessToken))
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var response = await _httpClient.SendAsync(httpRequestMessage);
            if (response.IsSuccessStatusCode)
            {
                var bodyJson = await response.Content.ReadAsStringAsync();
                ResponseBase content = JsonConvert.DeserializeObject<ResponseBase>(bodyJson);
                return content;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedException();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new ForbidenException();
            }
            else
            {
                return new ResponseBase(success: false, msg: $"Error: {response.StatusCode}");
            }
        }
    }
}
