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
using System.Text;

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
                    cookieoptions.LoginPath = new PathString("/Auth/Login");
                    cookieoptions.LogoutPath = new PathString("/Auth/Login");
                    cookieoptions.AccessDeniedPath = new PathString("/Views/Auth/AccessDenied");
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
            services.AddTransient<IAuthClient, AuthClientService>();
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
            //app.UseStatusCodePagesWithRedirects("/Auth/AccessDenied");
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
