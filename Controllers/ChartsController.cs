using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NjordBooks.BLL.Data;
using NjordBooks.BLL.Models;
using NjordBooks.BLL.Models.Charts;

namespace NjordBooks.BLL.Controllers
{
    [Authorize]
    public class ChartsController : Controller
    {
        private readonly NjordBooksContext context;
        private readonly UserManager<NjordBooksUser> userManager;

        public ChartsController( NjordBooksContext context, UserManager<NjordBooksUser> userManager )
        {
            this.context = context;
            this.userManager = userManager;
        }

        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<JsonResult> Categories( )
        {
            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            Household houseHold = await this.context.Household
                                            .Include( hh => hh.Categories )
                                            .ThenInclude( c => c.CategoryItems )
                                            .FirstOrDefaultAsync( hh => hh.Id == user.HouseholdId )
                                            .ConfigureAwait( false );

            List<BudgetBreakDownPieChartData> list = (
                from category in houseHold.Categories
                let total = category.CategoryItems.Sum( item => item.ActualAmount )
                select new BudgetBreakDownPieChartData { Name = category.Name, Total = total } ).ToList( );

            return this.Json( list );
        }

        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<JsonResult> Items( )
        {
            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            Household houseHold = await this.context.Household.Include( hh => hh.Categories )
                                            .ThenInclude( c => c.CategoryItems )
                                            .FirstOrDefaultAsync( hh => hh.Id == user.HouseholdId )
                                            .ConfigureAwait( false );
            List<CategoryItem> items = houseHold.Categories.SelectMany( c => c.CategoryItems ).ToList( );
            List<CategoryItemsBarChartData> list = new List<CategoryItemsBarChartData>( );
            items.ForEach( item => list.Add( new CategoryItemsBarChartData
            {
                Category = item.Category.Name,
                Name = item.Name,
                Goal = item.TargetAmount,
                Reality = item.ActualAmount
            } ) );

            return this.Json( list );
        }

        //[Authorize(Roles = "Admin,Head,Member")]
        public async Task<JsonResult> History( )
        {
            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            Household houseHold = await this.context.Household
                                            .Include( hh => hh.BankAccounts )
                                            .ThenInclude( ba => ba.Histories )
                                            .FirstOrDefaultAsync( hh => hh.Id == user.HouseholdId )
                                            .ConfigureAwait( false );

            List<Line> lines = new List<Line>( );
            DateTimeOffset oldest = DateTimeOffset.Now;
            DateTimeOffset newest = DateTimeOffset.Now;

            foreach ( BankAccount account in houseHold.BankAccounts )
            {
                Line line = new Line
                {
                    Name = account.Name,
                    Xcords = new List<string>( ),
                    Ycords = new List<decimal>( )
                };

                // Oldest To Newest
                List<History> histories = account.Histories.OrderBy( h => h.Date ).ToList( );

                foreach ( History history in histories )
                {
                    oldest = this.UpdateOldestDate( history.Date, oldest );
                    newest = this.UpdateNewestDate( history.Date, newest );
                    string date = history.Date.ToString( "MM/dd/yyyy" );
                    line.Xcords.Add( date );
                    line.Ycords.Add( history.Balance );
                }

                lines.Add( line );
            }

            List<string> dates = this.PopulateDates( oldest, newest );
            Chart chart = new Chart
            {
                Dates = dates,
                Lines = lines
            };

            return this.Json( chart );
        }

        // Start Date
        private DateTimeOffset UpdateOldestDate( DateTimeOffset date, DateTimeOffset oldest ) => date < oldest ? date : oldest;

        // End Date
        private DateTimeOffset UpdateNewestDate( DateTimeOffset date, DateTimeOffset newest ) => date > newest ? date : newest;

        // List From Start To End
        private List<string> PopulateDates( DateTimeOffset oldest, DateTimeOffset newest )
        {
            List<string> dates = new List<string>( );

            while ( oldest < newest )
            {
                dates.Add( oldest.ToString( "MM/dd/yyyy" ) );
                oldest = oldest.AddDays( 1 );
            }

            dates.Add( newest.ToString( "MM/dd/yyyy" ) );

            return dates.Distinct( ).ToList( );
        }
    }
}
