using System.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NjordBooks.BLL.Models;
using NjordBooks.BLL.Utilities;
using Npgsql;

namespace NjordBooks.BLL.Data
{
    public class NjordBooksContext : IdentityDbContext<NjordBooksUser>
    {
        private readonly IConfiguration configuration;

        public NjordBooksContext( DbContextOptions<NjordBooksContext> options, IConfiguration configuration )
            : base( options ) =>
            this.configuration = configuration;

        #region Phase 1 - BLL

        public DbSet<Attachment>   Attachment   { get; set; }
        public DbSet<BankAccount>  BankAccount  { get; set; }
        public DbSet<Category>     Category     { get; set; }
        public DbSet<CategoryItem> CategoryItem { get; set; }
        public DbSet<Household>    Household    { get; set; }
        public DbSet<Invitation>   Invitation   { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Transaction>  Transaction  { get; set; }
        public DbSet<History>      History      { get; set; }

        #endregion

        #region Phase 2 - API

        public string CallPostgresFunction( string funcName )
        {
            NpgsqlConnection connection =
                new NpgsqlConnection( PostgreSQLHelper.GetConnectionString( this.configuration ) );
            connection.Open( );

            // Build Command To Call PostgreSQL Function
            using NpgsqlCommand cmd = new NpgsqlCommand( funcName, connection )
                                      {
                                          CommandType = CommandType.StoredProcedure
                                      };

            // Execute Query
            using NpgsqlDataReader reader    = cmd.ExecuteReader( );
            DataTable dataTable = new DataTable( );
            dataTable.Load( reader );

            // Returns Either Serialized Json Or an Empty String
            connection.Close( );

            return dataTable.Rows.Count > 0 ? JsonConvert.SerializeObject( dataTable ) : string.Empty;
        }
    }
}

//public List<Household> GetAllHouseholdData( IConfiguration configuration )
//{
//    // Open connection string
//    var connString = new NpgsqlConnection( configuration.GetConnectionString( "DefaultConnection" ) );
//    connString.Open( );

//    // New up List<PortalUser>
//    var allHouseholds = new List<Household>( );

//    // Tell npgsql what function to call
//    using var cmd = new NpgsqlCommand( "getallhouseholds", connString )
//                    {
//                        cmd.CommandType = CommandType.StoredProcedure
//                    };

//        // Store data into a NpgsqlDataReader
//        using ( var reader = cmd.ExecuteReader( ) );
//        {
//            var dataTable = new DataTable( );
//            dataTable.Load( reader );

//            if ( dataTable.Rows.Count > 0 )
//            {
//                var serializedObjects = JsonSerializer.Serialize( dataTable );
//                allHouseholds.AddRange( ( List<Household> )
//                                        JsonSerializer.Deserialize<DataTable>( serializedObjects,
//                                            typeof( List<Household> ) ) );
//            }
//        }
//        connString.Close( );
//    }

//    return allHouseholds;
//}

#endregion
