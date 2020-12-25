using Newtonsoft.Json;
using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using TN.ViewModels.Catalog.User;
using System.Security.Claims;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using FrontEndWebApp.Settings;
using Microsoft.AspNetCore.Http;
using TN.ViewModels.FacebookAuth;
using TN.Data.Entities;

namespace FrontEndWebApp.Services
{
    public class AuthClientService : IAuthClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IConfiguration _config;

        public AuthClientService(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public async Task<string> Authenticate(LoginModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            HttpClient client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(ConstStrings.BaseUrl);
            var response = await client.PostAsync("/api/users/login", httpContent);
            
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                JwtResponse access_token_obj = JsonConvert.DeserializeObject<JwtResponse>(token);
                return access_token_obj.Access_Token;
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    return "wrong";
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return "notfound";
                else return "error";
            }
        }

        public async Task<UserViewModel> GetUserInfo(int userId, string access_token)
        {
            HttpClient client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(ConstStrings.BaseUrl);
            if (!string.IsNullOrEmpty(access_token))
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + access_token);
            }
            var response = await client.GetAsync("/api/users/"+userId.ToString());
            
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

            HttpClient client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(ConstStrings.BaseUrl);
            var response = await client.PostAsync("/api/users/loginfb", httpContent);

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

            HttpClient client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(ConstStrings.BaseUrl);
            var response = await client.PostAsync("/api/users/register", httpContent);

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

            HttpClient client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(ConstStrings.BaseUrl);
            if (!string.IsNullOrEmpty(access_token))
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + access_token);
            }
            var response = await client.PutAsync("/api/users/UpdateUser/"+uid.ToString(), httpContent);

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

            HttpClient client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(ConstStrings.BaseUrl);
            var response = await client.PutAsync("/api/users/AddPassword/", httpContent);

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
            parameters.ValidAudience = _config["Tokens:Issuer"];
            parameters.ValidIssuer = _config["Tokens:Issuer"];
            parameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:SecretKey"]));
            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(token, parameters, out validatedToken);
            return principal;
        }
    }
}
