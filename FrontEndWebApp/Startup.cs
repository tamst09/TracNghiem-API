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
using System;
using System.Net.Http;
using System.Text;
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
                    cookieoptions.ExpireTimeSpan = TimeSpan.FromHours(3);
                    // tu dong gia han cookie neu co request gui di
                    cookieoptions.SlidingExpiration = true;
                    cookieoptions.Cookie.Name = "asp.authentication";
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

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseSession();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            //app.UseStatusCodePagesWithRedirects("/Account/AccessDenied");
            //app.Use(async (context, next) =>
            //{
            //    var token = TokenUtils.DecodeToken(context.Request.Cookies["access_token_cookie"]);
            //    if (!string.IsNullOrEmpty(token))
            //    {
            //        context.Request.HttpContext.Request.Headers.Add("Authorization", "Bearer " + token);
            //        context.Request.Headers.Add("Authorization", "Bearer " + token);
            //    };
            //    await next();
            //});
            app.Use(async (context, next) =>
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(ConstStrings.BASE_URL_API);
                var token = Services.Encoder.DecodeToken(context.Request.Cookies["access_token_cookie"]);
                if (token != null)
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                if (context.User == null)
                {
                    await next();
                }
                var uid = context.User.FindFirst("UserID");
                
                if (uid!=null)
                {
                    var validAccount = await client.GetAsync("/api/users/getStatus/" + uid.Value);
                    if (!validAccount.IsSuccessStatusCode)
                    {
                        context.Response.Cookies.Delete("access_token_cookie");
                        context.Response.Cookies.Delete("asp.authentication");
                        await next();
                    }
                }
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
