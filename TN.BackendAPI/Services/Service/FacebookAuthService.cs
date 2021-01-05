using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TN.BackendAPI.Services.IServices;
using TN.ViewModels.FacebookAuth;
using TN.ViewModels.Settings;

namespace TN.BackendAPI.Services.Service
{
    public class FacebookAuthService : IFacebookAuth
    {
        private const string TokenValidationUrl = "https://graph.facebook.com/debug_token?input_token={0}&access_token={1}|{2}";
        private const string UserInfoUrl = "https://graph.facebook.com/me?fields=first_name,last_name,email&access_token={0}";
        private readonly FBAuthSettings _fbAuthSettings;
        private readonly IHttpClientFactory _httpClientFactory;

        public FacebookAuthService(IHttpClientFactory httpClientFactory, FBAuthSettings facebookAuthSettings)
        {
            _httpClientFactory = httpClientFactory;
            _fbAuthSettings = facebookAuthSettings;
        }

        public async Task<FacebookUserInfoResult> GetUserInfoAsync(string accesstoken)
        {
            var formatedUrl = string.Format(UserInfoUrl, accesstoken);
            var result = await _httpClientFactory.CreateClient().GetAsync(formatedUrl);
            result.EnsureSuccessStatusCode();
            var responseAsString = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacebookUserInfoResult>(responseAsString);
        }

        public async Task<FacebookTokenValidationResult> ValidateAccessTokenAsync(string accesstoken)
        {
            var formatedUrl = string.Format(TokenValidationUrl, accesstoken, _fbAuthSettings.AppID, _fbAuthSettings.AppSecret);
            var result = await _httpClientFactory.CreateClient().GetAsync(formatedUrl);
            result.EnsureSuccessStatusCode();
            var responseAsString = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacebookTokenValidationResult>(responseAsString);
        }
    }
}
