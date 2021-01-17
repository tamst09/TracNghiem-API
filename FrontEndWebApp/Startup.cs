using FrontEndWebApp.Areas.Admin.AdminServices;
using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TN.Data.Entities;
using TN.ViewModels.Catalog.User;
using TN.ViewModels.Common;
using TN.ViewModels.Settings;

namespace FrontEndWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddHttpContextAccessor();
            services.AddSession();
            // Dependency Injection
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IUserManage, UserManage>();
            services.AddTransient<ICategoryManage, CategoryManage>();
            services.AddTransient<IExamManage, ExamManage>();
            services.AddTransient<IQuestionManage, QuestionManage>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddCookie(cookieoptions => 
                {
                    cookieoptions.LoginPath = new PathString("/Account/Login");
                    cookieoptions.LogoutPath = new PathString("/Account/Login");
                    cookieoptions.AccessDeniedPath = new PathString("/Views/Account/AccessDenied");
                    // thoi gian cookie het han
                    cookieoptions.ExpireTimeSpan = TimeSpan.FromDays(3);
                    // tu dong gia han cookie neu co request gui di
                    cookieoptions.SlidingExpiration = true;
                    cookieoptions.Cookie.Name = "Asp_Authentication";
                })
                .AddJwtBearer(jwtOptions =>
                {
                    jwtOptions.RequireHttpsMetadata = false;
                    jwtOptions.SaveToken = false;
                    jwtOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Tokens:Issuer"],
                        ValidAudience = Configuration["Tokens:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:SecretKey"]))
                    };
                });
            services.AddAuthorization(options => {
                options.AddPolicy("admin",
                    authBuilder => { 
                        authBuilder.RequireRole("admin");
                    });
                options.AddPolicy("user",
                    authBuilder => {
                        authBuilder.RequireRole("user");
                    });
            });
            services.AddControllersWithViews();
            services.AddHttpClient();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseExceptionHandler("/Home/Error");
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseSession();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                // Kiem tra accessToken va gia han
                var refreshToken = CookieEncoder.DecodeToken(context.Request.Cookies["refresh_token_cookie"]);
                var accessToken = CookieEncoder.DecodeToken(context.Request.Cookies["access_token_cookie"]);
                // Lay refreshToken by accessToken
                if (string.IsNullOrEmpty(refreshToken))
                {
                    if (string.IsNullOrEmpty(accessToken))
                    {
                        context.Response.Cookies.Delete("Asp_Authentication");
                        //await next();
                    }
                    else
                    {
                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri(ConstStrings.BASE_URL_API);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        RefreshAccessTokenRequest RenewAccessTokenRequest = new RefreshAccessTokenRequest();
                        RenewAccessTokenRequest.AccessToken = accessToken;
                        var json = JsonConvert.SerializeObject(RenewAccessTokenRequest);
                        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                        var responseMessage = await client.PostAsync("/api/Users/GetRefreshToken", httpContent);
                        if (responseMessage.IsSuccessStatusCode)
                        {
                            var body = await responseMessage.Content.ReadAsStringAsync();
                            ResponseBase<RefreshToken> refreshTokenOfUser = JsonConvert.DeserializeObject<ResponseBase<RefreshToken>>(body);
                            if (refreshTokenOfUser.data == null || string.IsNullOrEmpty(refreshTokenOfUser.data.Token))
                            {
                                //context.Response.Cookies.Delete("access_token_cookie");
                                context.Response.Cookies.Delete("Asp_Authentication");
                            }
                            else
                            {
                                refreshToken = refreshTokenOfUser.data.Token;
                                context.Response.Cookies.Append("refresh_token_cookie", CookieEncoder.EncodeToken(refreshTokenOfUser.data.Token), new CookieOptions() { Expires = DateTime.UtcNow.AddDays(8), HttpOnly = true, Secure = true });
                            }
                        }
                    }
                }
                // -------------------------------
                var IsPersistent = context.Session.GetInt32("IsPersistent");
                if (IsPersistent == 1)
                {
                    // Gia han
                    SecurityToken validatedToken;
                    TokenValidationParameters parameters = new TokenValidationParameters();
                    parameters.ClockSkew = TimeSpan.FromSeconds(5);
                    parameters.ValidateLifetime = true;
                    parameters.RequireExpirationTime = true;
                    parameters.ValidAudience = Configuration["Tokens:Issuer"];
                    parameters.ValidIssuer = Configuration["Tokens:Issuer"];
                    parameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:SecretKey"]));
                    try
                    {
                        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                        tokenHandler.ValidateToken(accessToken, parameters, out validatedToken);
                        var tokenExpiresAt = validatedToken.ValidTo;
                        if (tokenExpiresAt.Subtract(DateTime.UtcNow).TotalDays < 1)
                        {
                            // gia han bang refreshToken
                            RefreshAccessTokenRequest refreshAccessTokenRequest = new RefreshAccessTokenRequest();
                            refreshAccessTokenRequest.AccessToken = accessToken;
                            refreshAccessTokenRequest.RefreshToken = refreshToken;
                            HttpClient client = new HttpClient();
                            client.BaseAddress = new Uri(ConstStrings.BASE_URL_API);
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                            var refreshTokenJSON = JsonConvert.SerializeObject(refreshAccessTokenRequest);
                            var content = new StringContent(refreshTokenJSON, Encoding.UTF8, "application/json");
                            var response = await client.PostAsync("/api/users/RefreshToken/", content);
                            if (response.IsSuccessStatusCode)
                            {
                                var responseBody = await response.Content.ReadAsStringAsync();
                                ResponseBase<string> newAccessToken = JsonConvert.DeserializeObject<ResponseBase<string>>(responseBody);
                                if (newAccessToken.msg != null)
                                {
                                    context.Response.Cookies.Delete("Asp_Authentication");
                                }
                                accessToken = newAccessToken.data;
                                context.Response.Cookies.Append("access_token_cookie", CookieEncoder.EncodeToken(newAccessToken.data), new CookieOptions() { Expires = DateTime.UtcNow.AddDays(4), HttpOnly = true, Secure = true });
                            }
                        }
                    }
                    catch (Exception)
                    {
                        context.Response.Cookies.Delete("Asp_Authentication");
                    }
                }
                // lay avatar url
                if (context.User != null)
                {
                    var uid = context.User.FindFirst("UserID");
                    if (uid != null)
                    {
                        try
                        {
                            HttpClient client = new HttpClient();
                            client.BaseAddress = new Uri(ConstStrings.BASE_URL_API);
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                            var avatar = await client.GetAsync("/api/users/" + uid.Value);
                            if (avatar.IsSuccessStatusCode)
                            {
                                var body = await avatar.Content.ReadAsStringAsync();
                                ResponseBase<AppUser> user = JsonConvert.DeserializeObject<ResponseBase<AppUser>>(body);
                                if (user == null || !user.data.isActive)
                                {
                                    context.Response.Cookies.Delete("Asp_Authentication");
                                }
                                else
                                {
                                    Global.Avatar_Url = user.data.Avatar;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            context.Response.Cookies.Delete("Asp_Authentication");
                        }
                    }
                }
                else
                {
                    context.Response.Cookies.Delete("Asp_Authentication");
                }
                //=============
                await next();
            });


            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapAreaControllerRoute(
                //    name: "PublicArea",
                //    areaName: "Public",
                //    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapAreaControllerRoute(
                    name: "AdminArea",
                    areaName: "Admin",
                    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapAreaControllerRoute(
                    name: "UserArea",
                    areaName: "User",
                    pattern: "User/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
