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
using TN.ViewModels.FacebookAuth;
using TN.Data.Entities;

namespace FrontEndWebApp.Services
{
    public class UserClient : IUserClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IConfiguration _config;

        public UserClient(IHttpClientFactory httpClientFactory, IConfiguration config)
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

        public async Task<AppUser> GetUserInfo(int userId)
        {
            HttpClient client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(ConstStrings.BaseUrl);
            var response = await client.GetAsync("/api/users/"+userId.ToString());

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadAsStringAsync();
                AppUser userCreated = JsonConvert.DeserializeObject<AppUser>(user);
                return userCreated;
            }
            else
            {
                return null;
            }
        }

        public async Task<CreateFacebookUserResult> LoginFacebook(string accesstoken)
        {
            var json = JsonConvert.SerializeObject(accesstoken);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            HttpClient client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(ConstStrings.BaseUrl);
            var response = await client.PostAsync("/api/users/createfacebookuser", httpContent);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                CreateFacebookUserResult userResult = JsonConvert.DeserializeObject<CreateFacebookUserResult>(result);
                return userResult;
            }
            return null;
        }

        public async Task<AppUser> Register(RegisterModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            HttpClient client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(ConstStrings.BaseUrl);
            var response = await client.PostAsync("/api/users/register", httpContent);

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadAsStringAsync();
                AppUser userCreated = JsonConvert.DeserializeObject<AppUser>(user);
                return userCreated;
            }
            else
            {
                return null;
            }
        }

        public async Task<AppUser> UpdateProfile(int uid, RegisterModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            HttpClient client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(ConstStrings.BaseUrl);
            string id = Convert.ToString(uid);
            var response = await client.PutAsync("/api/users/UpdateUser/"+id, httpContent);

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadAsStringAsync();
                AppUser userUpdated = JsonConvert.DeserializeObject<AppUser>(user);
                return userUpdated;
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
