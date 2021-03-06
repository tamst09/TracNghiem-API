using FrontEndWebApp.Areas.Admin.AdminServices;
using FrontEndWebApp.Areas.User.Services;
using FrontEndWebApp.Exceptions;
using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
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
            services.AddSession();
            services.AddHttpContextAccessor();
            // Dependency Injection
            services.AddTransient<IApiHelper, ApiHelper>();
            // DI of Admin
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUserManage, UserManage>();
            services.AddScoped<ICategoryManage, CategoryManage>();
            services.AddScoped<IExamManage, ExamManage>();
            services.AddScoped<IQuestionManage, QuestionManage>();
            // DI of User
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IFavoriteExamService, FavoriteExamService>();
            services.AddSingleton<SessionService>();

            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    //options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    //options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //}).AddCookie(cookieoptions =>
            //{
            //    cookieoptions.LoginPath = new PathString("/Account/Login");
            //    cookieoptions.LogoutPath = new PathString("/Account/Login");
            //    cookieoptions.AccessDeniedPath = new PathString("/Views/Account/AccessDenied");
            //    // thoi gian cookie het han
            //    cookieoptions.ExpireTimeSpan = TimeSpan.FromDays(3);
            //    // tu dong gia han cookie neu co request gui di
            //    cookieoptions.SlidingExpiration = true;
            //    cookieoptions.Cookie.Name = "Asp_Authentication";
            //});

            services.AddControllersWithViews();
            services.AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

                        if (exceptionHandlerPathFeature?.Error is UnauthorizedException)
                        {
                            context.Response.Redirect("/Account/Login");
                        }
                        else if(exceptionHandlerPathFeature?.Error is ForbidenException)
                        {
                            context.Response.Redirect("/Account/AccessDenied.cshtml");
                        }
                        else
                        {
                            context.Response.Redirect("/Home/Error");
                        }
                    });
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseSession();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
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
