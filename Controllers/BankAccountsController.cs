using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NjordBooks.BLL.Data;
using NjordBooks.BLL.Models;

namespace NjordBooks.BLL.Controllers
{
    [Authorize]
    public class BankAccountsController : Controller
    {
        private readonly NjordBooksContext           context;
        private readonly UserManager<NjordBooksUser> userManager;

        public BankAccountsController( NjordBooksContext context, UserManager<NjordBooksUser> userManager )
        {
            this.context     = context;
            this.userManager = userManager;
        }

        [Authorize( Roles = "Admin,Head,Member" )]
        // GET: BankAccounts
        public async Task<IActionResult> Index( )
        {
            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            IIncludableQueryable<BankAccount, NjordBooksUser> applicationDbContext = this.context.BankAccount
                .Where( x => x.HouseholdId == user.HouseholdId )
                .Include( b => b.Household )
                .Include( u => u.NjordBooksUser );

            return this.View( await applicationDbContext.ToListAsync( ).ConfigureAwait( false ) );
        }

        [Authorize( Roles = "Admin,Head,Member" )]
        // GET: BankAccounts/Details/5
        public async Task<IActionResult> Details( int? id )
        {
            if ( id == null )
            {
                return this.NotFound( );
            }

            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            BankAccount bankAccount = await this.context.BankAccount
                                                .Include( b => b.Household )
                                                .FirstOrDefaultAsync( x => x.HouseholdId == user.HouseholdId
                                                                        && x.Id          == id )
                                                .ConfigureAwait( false );

            if ( bankAccount == null )
            {
                return this.NotFound( );
            }

            return this.View( bankAccount );
        }

        [Authorize( Roles = "Admin,Head,Member" )]
        // GET: BankAccounts/Create
        public async Task<IActionResult> Create( )
        {
            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            this.ViewData["HouseholdId"] = user.HouseholdId;

            return this.View( );
        }

        // POST: BankAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize( Roles = "Admin,Head,Member" )]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( [Bind( "Id,HouseholdId,NjordBooksUserId,Name,Type,StartingBalance,CurrentBalance" )]
                                                 BankAccount bankAccount )
        {
            bankAccount.NjordBooksUserId = this.userManager.GetUserId( this.User );
            bankAccount.CurrentBalance   = bankAccount.StartingBalance;

            if ( this.ModelState.IsValid )
            {
                this.context.Add( bankAccount );
                await this.context.SaveChangesAsync( ).ConfigureAwait( false );

                return this.RedirectToAction( "Dashboard", "Households" );
            }

            this.ViewData["HouseholdId"] =
                new SelectList( this.context.Household, "Id", "Name", bankAccount.HouseholdId );

            return this.View( bankAccount );
        }

        // GET: BankAccounts/Edit/5
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Edit( int? id )
        {
            if ( id == null )
            {
                return this.NotFound( );
            }

            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            BankAccount bankAccount = await this.context.BankAccount
                                                .FirstOrDefaultAsync( x => x.HouseholdId == user.HouseholdId
                                                                        && x.Id          == id )
                                                .ConfigureAwait( false );

            if ( bankAccount == null )
            {
                return this.NotFound( );
            }

            this.ViewData["HouseholdId"] =
                new SelectList( this.context.Household, "Id", "Name", bankAccount.HouseholdId );

            return this.View( bankAccount );
        }

        // POST: BankAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize( Roles = "Admin,Head,Member" )]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( int id, [Bind( "Id,HouseholdId,NjordBooksUserId,Name,Type,StartingBalance,CurrentBalance" )]
                                               BankAccount bankAccount )
        {
            if ( id != bankAccount.Id )
            {
                return this.NotFound( );
            }

            if ( this.ModelState.IsValid )
            {
                try
                {
                    this.context.Update( bankAccount );
                    await this.context.SaveChangesAsync( ).ConfigureAwait( false );
                }
                catch ( DbUpdateConcurrencyException ) when ( !this.BankAccountExists( bankAccount.Id ) )
                {
                    return this.NotFound( );
                }

                return this.RedirectToAction( nameof( this.Index ) );
            }

            this.ViewData["HouseholdId"] =
                new SelectList( this.context.Household, "Id", "Name", bankAccount.HouseholdId );

            return this.View( bankAccount );
        }

        // GET: BankAccounts/Delete/5
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Delete( int? id )
        {
            if ( id == null )
            {
                return this.NotFound( );
            }

            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            BankAccount bankAccount = await this.context.BankAccount
                                                .FirstOrDefaultAsync( x => x.HouseholdId == user.HouseholdId
                                                                        && x.Id          == id )
                                                .ConfigureAwait( false );

            if ( bankAccount == null )
            {
                return this.NotFound( );
            }

            return this.View( bankAccount );
        }

        // POST: BankAccounts/Delete/5
        [Authorize( Roles = "Admin,Head,Member" )]
        [HttpPost]
        [ActionName( "Delete" )]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed( int id )
        {
            BankAccount bankAccount = await this.context.BankAccount.FindAsync( id ).ConfigureAwait( false );
            this.context.BankAccount.Remove( bankAccount );
            await this.context.SaveChangesAsync( ).ConfigureAwait( false );

            return this.RedirectToAction( "Dashboard", "Households" );
        }

        private bool BankAccountExists( int id )
        {
            return this.context.BankAccount.Any( e => e.Id == id );
        }
    }
}
