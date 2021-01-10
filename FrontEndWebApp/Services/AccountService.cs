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

        public async Task<JwtResponse> Authenticate(LoginModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/users/login", httpContent);
            
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                JwtResponse access_token_obj = JsonConvert.DeserializeObject<JwtResponse>(token);
                return access_token_obj;
            }
            else
            {
                var responseResult = await response.Content.ReadAsStringAsync();
                JwtResponse obj = JsonConvert.DeserializeObject<JwtResponse>(responseResult);
                return obj;
            }
        }

        public async Task<UserViewModel> GetUserInfo(int userId, string access_token)
        {
            if (!string.IsNullOrEmpty(access_token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            }
            var response = await _client.GetAsync("/api/users/"+userId.ToString());
            
            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadAsStringAsync();
                UserViewModel userCreated = JsonConvert.DeserializeObject<UserViewModel>(user);
                return userCreated;
            }
            else
            {
                return null;
            }
        }

        public async Task<JwtResponse> LoginFacebook(string accesstoken)
        {
            var json = JsonConvert.SerializeObject(accesstoken);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/users/loginfb", httpContent);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                JwtResponse userResult = JsonConvert.DeserializeObject<JwtResponse>(result);
                return userResult;
            }
            return null;
        }

        public async Task<JwtResponse> Register(RegisterModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/users/register", httpContent);

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadAsStringAsync();
                JwtResponse userCreated = JsonConvert.DeserializeObject<JwtResponse>(user);
                return userCreated;
            }
            else
            {
                var errs = await response.Content.ReadAsStringAsync();
                JwtResponse err = JsonConvert.DeserializeObject<JwtResponse>(errs);
                return err;
            }
        }

        public async Task<UserViewModel> UpdateProfile(int uid, UserViewModel model, string access_token)
        {
            model.Id = uid;
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            if (!string.IsNullOrEmpty(access_token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            }
            var response = await _client.PutAsync("/api/users/UpdateUser/"+uid.ToString(), httpContent);

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadAsStringAsync();
                UserViewModel uservm = JsonConvert.DeserializeObject<UserViewModel>(user);              
                return uservm;
            }
            else
            {
                return null;
            }
        }

        public async Task<UserViewModel> AddPassword(ResetPasswordModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("/api/users/AddPassword/", httpContent);

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadAsStringAsync();
                UserViewModel uservm = JsonConvert.DeserializeObject<UserViewModel>(user);
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
            return principal;
        }

        public async Task<string> GetResetPasswordCode(ForgotPasswordModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");            
            try
            {
                var response = await _client.PostAsync("api/Users/getresetcode", httpContent);
                if (response.IsSuccessStatusCode)
                {
                    string responseResult = await response.Content.ReadAsStringAsync();
                    return responseResult;
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
        public async Task<bool> ChangePassword(string resetCode, ResetPasswordModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                var response = await _client.PostAsync("api/Users/resetpass/", httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseResult = await response.Content.ReadAsStringAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
