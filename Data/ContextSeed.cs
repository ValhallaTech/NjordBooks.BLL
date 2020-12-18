using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NjordBooks.BLL.Data.Enums;
using NjordBooks.BLL.Models;
using NjordBooks.BLL.Services;

namespace NjordBooks.BLL.Data
{
    public static class ContextSeed
    {
        public static async Task SeedDataBaseAsync( NjordBooksContext context, UserManager<NjordBooksUser> userManager,
                                                    RoleManager<IdentityRole> roleManager,
                                                    INjordBooksFileService fileService )
        {
            await SeedRolesAsync( roleManager ).ConfigureAwait( false );
            await SeedUsersAsync( userManager, fileService ).ConfigureAwait( false );
        }

        private static async Task SeedRolesAsync( RoleManager<IdentityRole> roleManager )
        {
            await roleManager.CreateAsync( new IdentityRole( nameof( Roles.Admin ) ) ).ConfigureAwait( false );
            await roleManager.CreateAsync( new IdentityRole( nameof( Roles.Head ) ) ).ConfigureAwait( false );
            await roleManager.CreateAsync( new IdentityRole( nameof( Roles.Member ) ) ).ConfigureAwait( false );
            await roleManager.CreateAsync( new IdentityRole( nameof( Roles.New ) ) ).ConfigureAwait( false );
        }

        private static async Task SeedUsersAsync( UserManager<NjordBooksUser> userManager,
                                                  INjordBooksFileService fileService )
        {
            #region Admin

            NjordBooksUser user = new NjordBooksUser
            {
                UserName = "valhallatechtest+administrator@gmail.com",
                Email = "valhallatechtest+administrator@gmail.com",
                FirstName = "Fred",
                LastName = "Smith",
                FileData = await fileService.GetDefaultAvatarFileBytesAsync( )
                                                                  .ConfigureAwait( false ),
                FileName = fileService.GetDefaultAvatarFileName( ),
                EmailConfirmed = true
            };

            try
            {
                NjordBooksUser test = await userManager.FindByEmailAsync( user.Email ).ConfigureAwait( false );

                if ( test == null )
                {
                    await userManager.CreateAsync( user, "rh5eX3*DV!ou" ).ConfigureAwait( false );
                    await userManager.AddToRoleAsync( user, nameof( Roles.Admin ) ).ConfigureAwait( false );
                }
            }
            catch ( Exception ex )
            {
                Console.WriteLine( "========= ERROR ============" );
                Console.WriteLine( $"Error Seeding {user.Email}" );
                Console.WriteLine( ex.Message );
                Console.WriteLine( "============================" );

                throw;
            }

            #endregion

            #region Head

            user = new NjordBooksUser
            {
                UserName = "valhallatechtest+developer@gmail.com",
                Email = "valhallatechtest+developer@gmail.com",
                FirstName = "Bill",
                LastName = "Williams",
                FileData = await fileService.GetDefaultAvatarFileBytesAsync( ).ConfigureAwait( false ),
                FileName = fileService.GetDefaultAvatarFileName( ),
                EmailConfirmed = true
            };

            try
            {
                NjordBooksUser test = await userManager.FindByEmailAsync( user.Email ).ConfigureAwait( false );

                if ( test == null )
                {
                    await userManager.CreateAsync( user, "rh5eX3*DV!ou" ).ConfigureAwait( false );
                    await userManager.AddToRoleAsync( user, nameof( Roles.Head ) ).ConfigureAwait( false );
                }
            }
            catch ( Exception ex )
            {
                Console.WriteLine( "========= ERROR ============" );
                Console.WriteLine( $"Error Seeding {user.Email}" );
                Console.WriteLine( ex.Message );
                Console.WriteLine( "============================" );

                throw;
            }

            #endregion

            #region Member

            user = new NjordBooksUser
            {
                UserName = "valhallatechtest+projectmanager@gmail.com",
                Email = "valhallatechtest+projectmanager@gmail.com",
                FirstName = "Nugz",
                LastName = "McNugz",
                FileData = await fileService.GetDefaultAvatarFileBytesAsync( ).ConfigureAwait( false ),
                FileName = fileService.GetDefaultAvatarFileName( ),
                EmailConfirmed = true
            };

            try
            {
                NjordBooksUser test = await userManager.FindByEmailAsync( user.Email ).ConfigureAwait( false );

                if ( test == null )
                {
                    await userManager.CreateAsync( user, "rh5eX3*DV!ou" ).ConfigureAwait( false );
                    await userManager.AddToRoleAsync( user, nameof( Roles.Member ) ).ConfigureAwait( false );
                }
            }
            catch ( Exception ex )
            {
                Console.WriteLine( "========= ERROR ============" );
                Console.WriteLine( $"Error Seeding {user.Email}" );
                Console.WriteLine( ex.Message );
                Console.WriteLine( "============================" );

                throw;
            }

            #endregion

            #region New

            user = new NjordBooksUser
            {
                UserName = "valhallatechtest+submitter@gmail.comt",
                Email = "valhallatechtest+submitter@gmail.com",
                FirstName = "Nil",
                LastName = "Nullable",
                FileData = await fileService.GetDefaultAvatarFileBytesAsync( ).ConfigureAwait( false ),
                FileName = fileService.GetDefaultAvatarFileName( ),
                EmailConfirmed = true
            };

            try
            {
                NjordBooksUser test = await userManager.FindByEmailAsync( user.Email ).ConfigureAwait( false );

                if ( test == null )
                {
                    await userManager.CreateAsync( user, "rh5eX3*DV!ou" ).ConfigureAwait( false );
                    await userManager.AddToRoleAsync( user, nameof( Roles.New ) ).ConfigureAwait( false );
                }
            }
            catch ( Exception ex )
            {
                Console.WriteLine( "========= ERROR ============" );
                Console.WriteLine( $"Error Seeding {user.Email}" );
                Console.WriteLine( ex.Message );
                Console.WriteLine( "============================" );

                throw;
            }

            #endregion
        }
    }
}
