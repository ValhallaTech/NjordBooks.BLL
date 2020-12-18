using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NjordBooks.BLL.Data;
using NjordBooks.BLL.Data.Enums;
using NjordBooks.BLL.Models;

namespace NjordBooks.BLL.Services
{
    public class NjordBooksHouseHoldService : INjordBooksHouseholdService
    {
        private readonly NjordBooksContext context;
        private readonly UserManager<NjordBooksUser> userManager;

        public NjordBooksHouseHoldService( NjordBooksContext context, UserManager<NjordBooksUser> userManager )
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<List<NjordBooksUser>> ListHouseHoldMembersAsync( int? houseHoldId )
        {
            Household houseHold = await this.context.Household.Include( hh => hh.NjordBooksUsers )
                                            .FirstOrDefaultAsync( hh => hh.Id == houseHoldId )
                                            .ConfigureAwait( false );
            List<NjordBooksUser> members = new List<NjordBooksUser>( );

            foreach ( NjordBooksUser member in houseHold.NjordBooksUsers )
            {
                string role = ( await this.userManager.GetRolesAsync( member ).ConfigureAwait( false ) )[0];

                if ( role == nameof( Roles.Member ) )
                {
                    members.Add( member );
                }
            }

            return members;
        }

        public List<Transaction> ListTransactions( Household houseHold )
        {
            List<ICollection<Transaction>> transactions = new List<ICollection<Transaction>>( );

            foreach ( BankAccount bankAccount in houseHold.BankAccounts )
            {
                transactions.Add( bankAccount.Transactions );
            }

            return transactions.SelectMany( t => t ).ToList( );
        }

        public async Task<string> GetRoleAsync( NjordBooksUser user )
        {
            IList<string> roles = await this.userManager.GetRolesAsync( user ).ConfigureAwait( false );

            return roles[0];
        }

        public async Task<string> GetGreetingAsync( NjordBooksUser user )
        {
            Household houseHold = await this.context.Household.FirstOrDefaultAsync( hh => hh.Id == user.HouseholdId )
                                            .ConfigureAwait( false );

            if ( houseHold == null )
            {
                return "Hello";
            }

            return houseHold.Greeting;
        }
    }
}
