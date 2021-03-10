using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement_Mvc
{
    public class Startup
    { 
        //manage Nuget: AspNetCore.Authentication.Google
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(15);
            });

        //Lỗi 400: redirect_uri_mismatch
        //The redirect URI in the request, https://localhost:44395/signin-google, does not match the ones authorized for the OAuth client. To update the authorized redirect URIs, visit:
        //https://console.developers.google.com/apis/credentials/oauthclient/${your_client_id}?project=${your_project_number}
         
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                 
            })     
                .AddCookie(options =>
                {
                    options.LoginPath = "/Home/GoogleLogin";
                })
                // id:     943482371704-h7me7hn4h3kkr3n7ooo58jc127q861ed.apps.googleusercontent.com
                // secret: chZYJ8bF5TRKIvbTdRv25CDP
                .AddGoogle(options =>
                {
                    options.ClientId = "943482371704-h7me7hn4h3kkr3n7ooo58jc127q861ed.apps.googleusercontent.com";
                    options.ClientSecret = "chZYJ8bF5TRKIvbTdRv25CDP"; 
                });
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession();  
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default", 
                    pattern: "{controller=Home}/{action=Login}/{id?}");
            });
        }
    }
}
