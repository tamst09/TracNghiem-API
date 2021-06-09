using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TN.ViewModels.Catalog.User;
using TN.ViewModels.Common;
using TN.ViewModels.Settings;

namespace FrontEndWebApp.Services
{
    public class AccountService : IAccountService
    {
        private IConfiguration _config;
        private HttpClient _client;

        public AccountService(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            this._client = httpClientFactory.CreateClient();
            this._client.BaseAddress = new Uri(ConstStrings.BASE_URL_API);
            _config = config;
        }

        public async Task<ResponseBase<JwtResponse>> Authenticate(LoginModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/users/login", httpContent);
            
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                ResponseBase<JwtResponse> access_token_obj = JsonConvert.DeserializeObject<ResponseBase<JwtResponse>>(token);
                return access_token_obj;
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseBase<UserViewModel>> GetUserInfo(int userId, string access_token)
        {
            if (!string.IsNullOrEmpty(access_token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            }
            var response = await _client.GetAsync("/api/users/"+userId.ToString());
            
            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadAsStringAsync();
                ResponseBase<UserViewModel> userCreated = JsonConvert.DeserializeObject<ResponseBase<UserViewModel>>(user);
                return userCreated;
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseBase<JwtResponse>> LoginFacebook(string accesstoken)
        {
            var json = JsonConvert.SerializeObject(accesstoken);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/users/loginfb", httpContent);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                ResponseBase<JwtResponse> jwt = JsonConvert.DeserializeObject<ResponseBase<JwtResponse>>(result);
                return jwt;
            }
            return null;
        }

        public async Task<ResponseBase<JwtResponse>> LoginGoogle(string accesstoken)
        {
            var json = JsonConvert.SerializeObject(accesstoken);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/users/logingg", httpContent);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                ResponseBase<JwtResponse> jwt = JsonConvert.DeserializeObject<ResponseBase<JwtResponse>>(result);
                return jwt;
            }
            return null;
        }

        public async Task<ResponseBase<JwtResponse>> Register(RegisterModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/users/register", httpContent);

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadAsStringAsync();
                ResponseBase<JwtResponse> userCreated = JsonConvert.DeserializeObject<ResponseBase<JwtResponse>>(user);
                return userCreated;
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseBase<UserViewModel>> UpdateProfile(UserViewModel model, string access_token)
        {
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            if (!string.IsNullOrEmpty(access_token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            }
            var response = await _client.PutAsync("/api/Users", httpContent);

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadAsStringAsync();
                ResponseBase<UserViewModel> uservm = JsonConvert.DeserializeObject<ResponseBase<UserViewModel>>(user);              
                return uservm;
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseBase<UserViewModel>> AddPassword(ResetPasswordModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("/api/users/AddPassword/", httpContent);

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadAsStringAsync();
                ResponseBase<UserViewModel> uservm = JsonConvert.DeserializeObject<ResponseBase<UserViewModel>>(user);
                return uservm;
            }
            else
            {
                return null;
            }
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            IdentityModelEventSource.ShowPII = true;
            SecurityToken validatedToken;
            TokenValidationParameters parameters = new TokenValidationParameters();
            parameters.ValidateLifetime = true;
            parameters.RequireExpirationTime = true;
            parameters.ValidAudience = _config["Tokens:Issuer"];
            parameters.ValidIssuer = _config["Tokens:Issuer"];
            parameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:SecretKey"]));
            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(token, parameters, out validatedToken);
            var tokenExpiresAt = validatedToken.ValidTo;
            if(DateTime.UtcNow > tokenExpiresAt)
            {
                return null;
            }
            return principal;
        }

        public bool ValidateLifeTimeToken(string token)
        {
            //IdentityModelEventSource.ShowPII = true;
            SecurityToken validatedToken;
            TokenValidationParameters parameters = new TokenValidationParameters();
            parameters.ValidateLifetime = true;
            parameters.RequireExpirationTime = true;
            parameters.ValidAudience = _config["Tokens:Issuer"];
            parameters.ValidIssuer = _config["Tokens:Issuer"];
            parameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:SecretKey"]));
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, parameters, out validatedToken);
            var tokenExpiresAt = validatedToken.ValidTo;
            if (DateTime.UtcNow > tokenExpiresAt)
            {
                return false;
            }
            return true;
        }

        public async Task<ResponseBase<string>> GetResetPasswordCode(ForgotPasswordModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");            
            try
            {
                var response = await _client.PostAsync("api/Users/getresetcode", httpContent);
                if (response.IsSuccessStatusCode)
                {
                    string responseResult = await response.Content.ReadAsStringAsync();
                    ResponseBase<string> resetPassCode = JsonConvert.DeserializeObject<ResponseBase<string>>(responseResult);
                    return resetPassCode;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<ResponseBase<string>> ResetPassword(string resetCode, ResetPasswordModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                var response = await _client.PostAsync("api/Users/resetpass/", httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseResult = await response.Content.ReadAsStringAsync();
                    ResponseBase<string> changePassResult = JsonConvert.DeserializeObject<ResponseBase<string>>(responseResult);
                    return changePassResult;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ResponseBase<string>> ChangePassword(string userID, ChangePasswordModel model, string accessToken)
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                var response = await _client.PostAsync("api/Users/ChangePass/"+userID, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseResult = await response.Content.ReadAsStringAsync();
                    ResponseBase<string> changePassResult = JsonConvert.DeserializeObject<ResponseBase<string>>(responseResult);
                    return changePassResult;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
