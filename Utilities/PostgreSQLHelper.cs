using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NjordBooks.BLL.Data;
using NjordBooks.BLL.Models;
using NjordBooks.BLL.Services;
using Npgsql;

namespace NjordBooks.BLL.Utilities
{
    public static class PostgreSQLHelper
    {
        public static string GetConnectionString( IConfiguration configuration )
        {
            // the default connection string will come from appsettings.json like usual
            string connectionString = configuration.GetConnectionString( "DefaultConnection" );

            // It will be automatically overwritten if we are running on Heroku
            string databaseUrl = Environment.GetEnvironmentVariable( "DATABASE_URL" );

            return string.IsNullOrEmpty( databaseUrl ) ? connectionString : BuildConnectionString( databaseUrl );
        }

        public static string BuildConnectionString( string databaseUrl )
        {
            // Provides an object representation of a uniform resource identifier (URI) and easy access to the parts of the URI.
            Uri databaseUri = new Uri( databaseUrl );
            string[] userInfo = databaseUri.UserInfo.Split( ':' );

            //Provides a simple way to create and manage the contents of connection strings used by the NpgsqlConnection class.
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart( '/' )
            };

            return builder.ToString( );
        }

        public static async Task ManageData( IHost host )
        {
            try
            {
                //This technique is used to obtain references to services
                // normally I would just inject these services but you cant use a constructor in a static class
                using IServiceScope svcScope = host.Services.CreateScope( );
                IServiceProvider svcProvider = svcScope.ServiceProvider;

                //The service will run your migrations
                NjordBooksContext dbContextSvc = svcProvider.GetRequiredService<NjordBooksContext>( );
                await dbContextSvc.Database.MigrateAsync( ).ConfigureAwait( false );

                IServiceProvider services = svcScope.ServiceProvider;
                NjordBooksContext context = services.GetRequiredService<NjordBooksContext>( );
                UserManager<NjordBooksUser> userManager = services.GetRequiredService<UserManager<NjordBooksUser>>( );
                RoleManager<IdentityRole> roleManager = services.GetRequiredService<RoleManager<IdentityRole>>( );
                INjordBooksFileService fileService = services.GetRequiredService<INjordBooksFileService>( );
                await ContextSeed.SeedDataBaseAsync( context, userManager, roleManager, fileService )
                                 .ConfigureAwait( false );
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"Exception while running Manage Data => {ex}" );
            }
        }
    }
}
