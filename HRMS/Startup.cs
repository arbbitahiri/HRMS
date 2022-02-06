using HRMS.Data;
using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Repository;
using HRMS.Services;
using HRMS.Utilities.General;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace HRMS;
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromSeconds(10);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("SqlConnection")));
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddDbContext<HRMSContext>(options =>
            options.UseSqlServer(Configuration["ConnectionStrings:SqlConnection"]));

        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Password.RequireLowercase = bool.Parse(Configuration["SecurityConfiguration:Password:RequiredLowercase"]);
            options.Password.RequireUppercase = bool.Parse(Configuration["SecurityConfiguration:Password:RequiredUppercase"]);
            options.Password.RequireDigit = bool.Parse(Configuration["SecurityConfiguration:Password:RequiredDigit"]);
            options.Password.RequiredLength = int.Parse(Configuration["SecurityConfiguration:Password:RequiredLength"]);
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

        services.AddMvc(setup =>
        {
            setup.EnableEndpointRouting = false;
        }).AddRazorRuntimeCompilation();

        services.AddTransient<IEmailSender, EmailSender>();
        services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
        services.AddScoped<IDDLRepository, DDLRepository>();
        services.AddScoped<IFunctionRepository, FunctionRepo>();

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

        services.Configure<IISServerOptions>(options =>
        {
            options.MaxRequestBodySize = long.MaxValue;
        });

        services.AddResponseCaching();
        services.AddResponseCompression();
        services.AddMemoryCache();
    }

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

        app.UseRewriter(new RewriteOptions().AddRedirectToWww().AddRedirectToHttps());

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
            app.UseHsts();
        }
        app.UseStatusCodePagesWithRedirects("/Error/{0}");
        app.UseHttpsRedirection();

        app.UseExceptionHandlerMiddleware();

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
                pattern: "{controller=Home}/{action=Index}/{ide?}");
            endpoints.MapRazorPages();
        });
    }
}
