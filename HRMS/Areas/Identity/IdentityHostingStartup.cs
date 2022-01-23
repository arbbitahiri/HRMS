using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(HRMS.Areas.Identity.IdentityHostingStartup))]
namespace HRMS.Areas.Identity;

public class IdentityHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
        });
    }
}
