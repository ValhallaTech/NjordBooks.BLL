using Microsoft.AspNetCore.Hosting;
using NjordBooks.BLL.Areas.Identity;

[assembly: HostingStartup( typeof( IdentityHostingStartup ) )]
namespace NjordBooks.BLL.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure( IWebHostBuilder builder )
        {
            builder.ConfigureServices( ( context, services ) =>
            {
            } );
        }
    }
}