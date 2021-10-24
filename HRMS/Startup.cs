using HRMS.Data;
using HRMS.Data.Core;
using HRMS.Repository;
using HRMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace HRMS
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("SqlConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireLowercase = bool.Parse(Configuration["SecurityConfig:Password:RequireLowercase"]);
                options.Password.RequireUppercase = bool.Parse(Configuration["SecurityConfig:Password:RequireUppercase"]);
                options.Password.RequireDigit = bool.Parse(Configuration["SecurityConfig:Password:RequireDigit"]);
                options.Password.RequiredLength = int.Parse(Configuration["SecurityConfig:Password:RequiredLength"]);
            }).AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddErrorDescriber<IdentityErrorDescriber>()
                .AddDefaultTokenProviders();

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddControllersWithViews();

            services.AddRazorPages()
                .AddRazorRuntimeCompilation();
            services.AddSignalR();

            services.Configure<DataProtectionTokenProviderOptions>(o =>
            {
                o.TokenLifespan = TimeSpan.FromHours(1);
            });

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            services.AddScoped<IDDLRepo, DDLRepo>();
            services.AddScoped<IFunctionRepo, FunctionRepo>();

            services.ConfigureExternalCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            services.Configure<PasswordHasherOptions>(options =>
            {
                options.IterationCount = 12000;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            IList<CultureInfo> supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("sq-AL")
                {
                    DateTimeFormat = new DateTimeFormatInfo { DateSeparator="/" },
                    NumberFormat = new NumberFormatInfo { CurrencyDecimalDigits = 2, CurrencyGroupSeparator =",", CurrencyDecimalSeparator = ".", NumberGroupSeparator = ",", NumberDecimalSeparator = "." }
                },
                new CultureInfo("en-GB")
                {
                    DateTimeFormat = new DateTimeFormatInfo { DateSeparator="/" },
                    NumberFormat = new NumberFormatInfo { CurrencyDecimalDigits = 2, CurrencyGroupSeparator =",", CurrencyDecimalSeparator = ".", NumberGroupSeparator = ",", NumberDecimalSeparator = "." }
                },
            };

            var culture = new CultureInfo("sq_AL");
            culture.NumberFormat.NumberDecimalSeparator = ".";
            culture.NumberFormat.NumberGroupSeparator = ",";
            culture.DateTimeFormat.DateSeparator = "/";

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(culture),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseMigrationsEndPoint();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseStatusCodePagesWithRedirects("/Error/{0}");
            app.UseHttpsRedirection();

            app.UseExceptionHandler();

            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = r =>
                {
                    string path = r.File.PhysicalPath;
                    if (path.EndsWith(".jpeg") || path.EndsWith(".jpg") || path.EndsWith(".png") || path.EndsWith(".svg") || path.EndsWith(".css") || path.EndsWith(".js"))
                    {
                        var maxAge = new TimeSpan(7, 0, 0, 0);
                        r.Context.Response.Headers.Append("Cache-Control", "max-age=" + maxAge.TotalSeconds.ToString("0"));
                    }
                }
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCaching();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
