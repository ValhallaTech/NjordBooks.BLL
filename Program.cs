using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NjordBooks.BLL.Utilities;

namespace NjordBooks.BLL
{
    public static class Program
    {
        public static async Task Main( string[] args )
        {
            IHost host = CreateHostBuilder( args ).Build( );
            await PostgreSQLHelper.ManageData( host ).ConfigureAwait( false );
            await host.RunAsync( ).ConfigureAwait( false );
        }

        public static IHostBuilder CreateHostBuilder( string[] args ) =>
            Host.CreateDefaultBuilder( args )
                .ConfigureWebHostDefaults( webBuilder =>
                                           {
                                               webBuilder.CaptureStartupErrors( true );
                                               webBuilder.UseSetting( WebHostDefaults.DetailedErrorsKey, "true" );
                                               webBuilder.UseStartup<Startup>( );
                                           } );
    }
}
