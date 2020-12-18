using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NjordBooks.BLL.Data;
using NjordBooks.BLL.Data.Enums;
using NjordBooks.BLL.Models;
using NjordBooks.BLL.Models.ViewModels;
using NjordBooks.BLL.Services;

namespace NjordBooks.BLL.Controllers
{
    [Authorize]
    public class HouseHoldsController : Controller
    {
        private readonly NjordBooksContext context;
        private readonly UserManager<NjordBooksUser> userManager;
        private readonly SignInManager<NjordBooksUser> signInManager;
        private readonly INjordBooksHouseholdService houseHoldService;

        public HouseHoldsController( NjordBooksContext context, UserManager<NjordBooksUser> userManager,
                                     SignInManager<NjordBooksUser> signInManager,
                                     INjordBooksHouseholdService houseHoldService )
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.houseHoldService = houseHoldService;
        }

        // GET: Households
        [Authorize( Roles = "Admin" )]
        public async Task<IActionResult> Index( ) =>
            this.View( await this.context.Household.ToListAsync( ).ConfigureAwait( false ) );

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Join( int id )
        {
            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            user.HouseholdId = id;
            IList<string> roles = await this.userManager.GetRolesAsync( user ).ConfigureAwait( false );
            await this.userManager.RemoveFromRolesAsync( user, roles ).ConfigureAwait( false );
            await this.userManager.AddToRoleAsync( user, nameof( Roles.Member ) ).ConfigureAwait( false );
            await this.context.SaveChangesAsync( ).ConfigureAwait( false );

            // Authentication
            await this.signInManager.RefreshSignInAsync( user ).ConfigureAwait( false );

            return this.RedirectToAction( "Index", "Home" );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Leave( )
        {
            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );

            if ( this.User.IsInRole( nameof( Roles.Head ) ) )
            {
                List<NjordBooksUser> members = await this.houseHoldService.ListHouseHoldMembersAsync( user.HouseholdId )
                                                         .ConfigureAwait( false );

                if ( members.Count > 0 )
                {
                    this.TempData["Script"] = "CantLeave()";

                    return this.RedirectToAction( "Dashboard" );
                }

                // Delete Household
                Household houseHold = await this.context.Household
                                                .FirstOrDefaultAsync( hh => hh.Id == user.HouseholdId )
                                                .ConfigureAwait( false );
                this.context.Household.Remove( houseHold );
            }

            // No Household
            user.HouseholdId = null;

            // Role Reset
            IList<string> roles = await this.userManager.GetRolesAsync( user ).ConfigureAwait( false );
            await this.userManager.RemoveFromRolesAsync( user, roles ).ConfigureAwait( false );
            await this.userManager.AddToRoleAsync( user, nameof( Roles.New ) ).ConfigureAwait( false );
            await this.context.SaveChangesAsync( ).ConfigureAwait( false );

            // Authentication
            await this.signInManager.RefreshSignInAsync( user ).ConfigureAwait( false );

            return this.RedirectToAction( "Index", "Home" );
        }

        // GET: Households/Details/5
        [Authorize( Roles = "Admin" )]
        public async Task<IActionResult> Details( int? id )
        {
            if ( id == null )
            {
                return this.NotFound( );
            }

            Household houseHold = await this.context.Household
                                            .FirstOrDefaultAsync( m => m.Id == id )
                                            .ConfigureAwait( false );

            if ( houseHold == null )
            {
                return this.NotFound( );
            }

            return this.View( houseHold );
        }

        // GET: Households/Create
        public IActionResult Create( ) => this.View( );

        // POST: Households/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( [Bind( "Id,Name,Greeting,Established" )]
                                                 Household houseHold )
        {
            if ( !this.ModelState.IsValid ) return this.View( houseHold );

            this.context.Add( houseHold );
            await this.context.SaveChangesAsync( ).ConfigureAwait( false );

            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            user.HouseholdId = houseHold.Id;

            IList<string> roles = await this.userManager.GetRolesAsync( user ).ConfigureAwait( false );
            await this.userManager.RemoveFromRolesAsync( user, roles ).ConfigureAwait( false );
            await this.userManager.AddToRoleAsync( user, nameof( Roles.Head ) ).ConfigureAwait( false );
            await this.context.SaveChangesAsync( ).ConfigureAwait( false );

            // Authentication
            await this.signInManager.SignOutAsync( ).ConfigureAwait( false );
            await this.signInManager.SignInAsync( user, isPersistent: false ).ConfigureAwait( false );

            this.TempData["Script"] = "Wizard()";

            return this.RedirectToAction( nameof( this.Dashboard ) );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetUp( string bank, AccountType accountType, decimal startBalance,
                                                string categoryName, string catDesc, string itemName, string itemDesc,
                                                decimal target )
        {
            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );

            if ( user.HouseholdId is { } )
            {
                BankAccount bankAccount = new BankAccount
                {
                    HouseholdId = ( int )user.HouseholdId,
                    NjordBooksUserId = user.Id,
                    Name = bank,
                    Type = accountType,
                    StartingBalance = startBalance,
                    CurrentBalance = startBalance
                };
                await this.context.AddAsync( bankAccount ).ConfigureAwait( false );
            }

            await this.context.SaveChangesAsync( ).ConfigureAwait( false );

            if ( user.HouseholdId is { } )
            {
                Category category = new Category
                {
                    HouseholdId = ( int )user.HouseholdId,
                    Name = categoryName,
                    Description = catDesc
                };
                await this.context.AddAsync( category ).ConfigureAwait( false );
                await this.context.SaveChangesAsync( ).ConfigureAwait( false );

                CategoryItem item = new CategoryItem
                {
                    CategoryId = category.Id,
                    Name = itemName,
                    Description = itemDesc,
                    TargetAmount = target,
                    ActualAmount = 0
                };
                await this.context.AddAsync( item ).ConfigureAwait( false );
            }

            await this.context.SaveChangesAsync( ).ConfigureAwait( false );

            return this.RedirectToAction( "Dashboard" );
        }

        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Dashboard( string year, string month )
        {
            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            Household houseHold = await this.context.Household
                                            .Include( hh => hh.BankAccounts )
                                            .ThenInclude( ba => ba.Transactions )
                                            .ThenInclude( t => t.BankAccount )
                                            .Include( hh => hh.BankAccounts )
                                            .ThenInclude( ba => ba.Transactions )
                                            .ThenInclude( t => t.CategoryItem )
                                            .Include( hh => hh.BankAccounts )
                                            .ThenInclude( ba => ba.Transactions )
                                            .ThenInclude( t => t.NjordBooksUser )
                                            .FirstOrDefaultAsync( hh => hh.Id == user.HouseholdId )
                                            .ConfigureAwait( false );

            List<Transaction> transactions = this.houseHoldService.ListTransactions( houseHold );
            List<Transaction> allTransactions = transactions;

            // By Year Filter
            if ( year != null )
            {
                // DateTimeOffset.Parse needs a valid datetime format
                int filterYear = 0;
                transactions = transactions.Where( t => t.Created.Year == filterYear ).ToList( );
            }

            // By Month Filter
            if ( month != null )
            {
                // DateTimeOffset.Parse needs a valid datetime format
                int filterMonth = 0;
                filterMonth = DateTimeOffset.Parse( $"{filterMonth} 1, 2009" ).Month;
                transactions = transactions.Where( t => t.Created.Month == filterMonth ).ToList( );
            }

            HouseholdDashboardVM model = new HouseholdDashboardVM
            {
                Occupants = await this.context.Users
                                                                   .Where( u => u.HouseholdId == user.HouseholdId )
                                                                   .ToListAsync( )
                                                                   .ConfigureAwait( false ),
                Accounts = houseHold.BankAccounts,
                Transactions = transactions
            };

            List<Category> categories = this.context.Category
                                            .Include( c => c.CategoryItems )
                                            .Where( c => c.HouseholdId == houseHold.Id )
                                            .ToList( );
            List<CategoryItem> items = categories.SelectMany( category => category.CategoryItems ).ToList( );

            List<BankAccount> bankAccounts =
                this.context.BankAccount.Where( ba => ba.HouseholdId == houseHold.Id ).ToList( );

            // Only The Years That Have Transactions
            List<string> years = new List<string>( );

            foreach ( Transaction transaction in allTransactions.Where( transaction =>
                                                                            !years.Contains( transaction.Created.Year
                                                                                .ToString( ) ) ) )
            {
                years.Add( transaction.Created.Year.ToString( ) );
            }

            List<string> months = new List<string>
                                  {
                                      "January", "February", "March", "April", "May", "June", "July", "August",
                                      "September", "October", "November", "December"
                                  };
            this.ViewData["Years"] = new SelectList( years, year ?? DateTimeOffset.Now.Year.ToString( ) );
            this.ViewData["Months"] = new SelectList( months, month ?? DateTimeOffset.Now.Month.ToString( ) );

            this.ViewData["CategoryId"] = new SelectList( categories, "Id", "Name" );
            this.ViewData["BankAccountId"] = new SelectList( bankAccounts, "Id", "Name" );
            this.ViewData["CategoryItemId"] = new SelectList( items, "Id", "Name" );

            return this.View( model );
        }

        // GET: Households/Edit/5
        [Authorize( Roles = "Admin,Head" )]
        public async Task<IActionResult> Edit( int? id )
        {
            if ( id == null )
            {
                return this.NotFound( );
            }

            Household houseHold = await this.context.Household.FindAsync( id ).ConfigureAwait( false );

            if ( houseHold == null )
            {
                return this.NotFound( );
            }

            return this.View( houseHold );
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize( Roles = "Admin,Head" )]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( int id, [Bind( "Id,Name,Greeting,Established" )]
                                               Household houseHold )
        {
            if ( id != houseHold.Id )
            {
                return this.NotFound( );
            }

            if ( this.ModelState.IsValid )
            {
                try
                {
                    this.context.Update( houseHold );
                    await this.context.SaveChangesAsync( ).ConfigureAwait( false );
                }
                catch ( DbUpdateConcurrencyException ) when ( !this.HouseholdExists( houseHold.Id ) )
                {
                    return this.NotFound( );
                }

                return this.RedirectToAction( nameof( this.Index ) );
            }

            return this.View( houseHold );
        }

        // GET: Households/Delete/5
        [Authorize( Roles = "Admin" )]
        public async Task<IActionResult> Delete( int? id )
        {
            if ( id == null )
            {
                return this.NotFound( );
            }

            Household houseHold = await this.context.Household
                                            .FirstOrDefaultAsync( m => m.Id == id )
                                            .ConfigureAwait( false );

            if ( houseHold == null )
            {
                return this.NotFound( );
            }

            return this.View( houseHold );
        }

        // POST: Households/Delete/5
        [Authorize( Roles = "Admin,Head" )]
        [HttpPost]
        [ActionName( "Delete" )]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed( int id )
        {
            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            user.HouseholdId = null;
            Household houseHold = await this.context.Household.FindAsync( id ).ConfigureAwait( false );
            this.context.Household.Remove( houseHold );
            await this.context.SaveChangesAsync( ).ConfigureAwait( false );

            return this.RedirectToAction( nameof( this.Index ) );
        }

        private bool HouseholdExists( int id )
        {
            return this.context.Household.Any( e => e.Id == id );
        }
    }
}
