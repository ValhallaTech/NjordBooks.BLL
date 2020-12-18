using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NjordBooks.BLL.Data;
using NjordBooks.BLL.Data.Enums;
using NjordBooks.BLL.Models;
using NjordBooks.BLL.Services;

namespace NjordBooks.BLL.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly NjordBooksContext context;
        private readonly UserManager<NjordBooksUser> userManager;
        private readonly INjordBooksNotificationService notificationService;

        public TransactionsController( NjordBooksContext context, UserManager<NjordBooksUser> userManager,
                                       INjordBooksNotificationService notificationService )
        {
            this.context = context;
            this.userManager = userManager;
            this.notificationService = notificationService;
        }

        // GET: Transactions
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Index( )
        {
            IIncludableQueryable<Transaction, NjordBooksUser> applicationDbContext =
                this.context.Transaction
                    .Include( t => t.BankAccount )
                    .Include( t => t.CategoryItem )
                    .Include( t => t.NjordBooksUser );

            return this.View( await applicationDbContext.ToListAsync( ).ConfigureAwait( false ) );
        }

        // GET: Transactions
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Transactions( int id )
        {
            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            BankAccount bankAccount = await this.context.BankAccount
                                                .Include( ba => ba.Transactions )
                                                .ThenInclude( t => t.CategoryItem )
                                                .Include( ba => ba.Transactions )
                                                .ThenInclude( t => t.NjordBooksUser )
                                                .FirstOrDefaultAsync( x => x.HouseholdId == user.HouseholdId
                                                                        && x.Id == id )
                                                .ConfigureAwait( false );

            if ( bankAccount == null )
            {
                return this.NotFound( );
            }

            return this.View( "Index", bankAccount.Transactions );
        }

        // GET: Transactions/Details/5
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Details( int? id )
        {
            if ( id == null )
            {
                return this.NotFound( );
            }

            Transaction transaction = await this.context.Transaction
                                                .Include( t => t.BankAccount )
                                                .Include( t => t.CategoryItem )
                                                .Include( t => t.NjordBooksUser )
                                                .FirstOrDefaultAsync( m => m.Id == id )
                                                .ConfigureAwait( false );

            if ( transaction == null )
            {
                return this.NotFound( );
            }

            return this.View( transaction );
        }

        // GET: Transactions/Create
        [Authorize( Roles = "Admin,Head,Member" )]
        public IActionResult Create( )
        {
            this.ViewData["BankAccountId"] = new SelectList( this.context.BankAccount, "Id", "Name" );
            this.ViewData["CategoryItemId"] = new SelectList( this.context.CategoryItem, "Id", "Name" );
            this.ViewData["NjordBooksUserId"] = new SelectList( this.context.Users, "Id", "FullName" );

            return this.View( );
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Create( [Bind( "Id,CategoryItemId,BankAccountId,NjordBooksUserId,Created,Type,Memo,Amount,IsDeleted" )]
                                                 Transaction transaction )
        {
            transaction.NjordBooksUserId = this.userManager.GetUserId( this.User );
            BankAccount bankAccount = await this.context.BankAccount
                                                .Include( ba => ba.Transactions )
                                                .ThenInclude( t => t.CategoryItem )
                                                .FirstOrDefaultAsync( ba => ba.Id == transaction.BankAccountId )
                                                .ConfigureAwait( false );
            CategoryItem categoryItem = await this.context.CategoryItem
                                                  .FirstOrDefaultAsync( ci => ci.Id == transaction.CategoryItemId )
                                                  .ConfigureAwait( false );

            if ( this.ModelState.IsValid )
            {
                if ( transaction.Type == TransactionType.Deposit )
                {
                    bankAccount.CurrentBalance += transaction.Amount;
                }
                else if ( transaction.Type == TransactionType.Withdrawal )
                {
                    bankAccount.CurrentBalance -= transaction.Amount;

                    if ( categoryItem == null )
                    {
                        return this.NotFound( );
                    }

                    categoryItem.ActualAmount += transaction.Amount;
                    this.context.Update( categoryItem );
                }

                // so that only one history per day
                History history = await this.context.History
                                            .FirstOrDefaultAsync( h => h.BankAccount == bankAccount
                                                                    && h.Date.Day == transaction.Created.Day )
                                            .ConfigureAwait( false );

                if ( history == null )
                {
                    history = new History
                              {
                                  BankAccountId = transaction.BankAccountId,
                                  Balance       = ( decimal )bankAccount.CurrentBalance,
                                  Date          = transaction.Created
                              };
                    this.context.Add( history );
                }
                else
                {
                    history.BankAccountId = transaction.BankAccountId;
                    history.Balance = ( decimal )bankAccount.CurrentBalance;
                    history.Date = transaction.Created;
                    this.context.Update( history );
                }

                this.context.Add( transaction );
                this.context.Update( bankAccount );
                await this.context.SaveChangesAsync( ).ConfigureAwait( false );

                if ( !( bankAccount.CurrentBalance < 0 ) ) return this.RedirectToAction( "Dashboard", "Households" );

                await this.notificationService.NotifyOverdraft( transaction.NjordBooksUserId, bankAccount )
                          .ConfigureAwait( false );
                this.TempData["Script"] = "Overdraft()";

                return this.RedirectToAction( "Dashboard", "Households" );
            }

            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            Household houseHold = await this.context.Household
                                            .Include( hh => hh.BankAccounts )
                                            .ThenInclude( ba => ba.Transactions )
                                            .Include( hh => hh.Categories )
                                            .ThenInclude( c => c.CategoryItems )
                                            .FirstOrDefaultAsync( hh => hh.Id == user.HouseholdId )
                                            .ConfigureAwait( false );
            ICollection<BankAccount> banks = houseHold.BankAccounts;
            List<CategoryItem> items = houseHold.Categories.SelectMany( c => c.CategoryItems ).ToList( );

            this.ViewData["BankAccountId"] = new SelectList( banks, "Id", "Name", transaction.BankAccountId );
            this.ViewData["CategoryItemId"] = new SelectList( items, "Id", "Name", transaction.CategoryItemId );

            return this.View( transaction );
        }

        // GET: Transactions/Edit/5
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Edit( int? id )
        {
            if ( id == null )
            {
                return this.NotFound( );
            }

            Transaction transaction = await this.context.Transaction.FindAsync( id ).ConfigureAwait( false );

            if ( transaction == null )
            {
                return this.NotFound( );
            }

            this.ViewData["BankAccountId"] =
                new SelectList( this.context.BankAccount, "Id", "Id", transaction.BankAccountId );
            this.ViewData["CategoryItemId"] =
                new SelectList( this.context.CategoryItem, "Id", "Id", transaction.CategoryItemId );
            this.ViewData["NjordBooksUserId"] =
                new SelectList( this.context.Users, "Id", "Id", transaction.NjordBooksUserId );

            return this.View( transaction );
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Edit( int id, [Bind( "Id,CategoryItemId,BankAccountId,NjordBooksUserId,Created,Type,Memo,Amount,IsDeleted" )]
                                               Transaction transaction )
        {
            if ( id != transaction.Id )
            {
                return this.NotFound( );
            }

            if ( this.ModelState.IsValid )
            {
                try
                {
                    transaction.NjordBooksUserId = this.userManager.GetUserId( this.User );
                    this.context.Update( transaction );
                    await this.context.SaveChangesAsync( ).ConfigureAwait( false );
                }
                catch ( DbUpdateConcurrencyException ) when ( !this.TransactionExists( transaction.Id ) )
                {
                    return this.NotFound( );
                }

                return this.RedirectToAction( "Dashboard", "Households" );
            }

            this.ViewData["BankAccountId"] =
                new SelectList( this.context.BankAccount, "Id", "Id", transaction.BankAccountId );
            this.ViewData["CategoryItemId"] =
                new SelectList( this.context.CategoryItem, "Id", "Id", transaction.CategoryItemId );
            this.ViewData["NjordBooksUserId"] =
                new SelectList( this.context.Users, "Id", "Id", transaction.NjordBooksUserId );

            return this.View( transaction );
        }

        // GET: Transactions/Delete/5
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Delete( int? id )
        {
            if ( id == null )
            {
                return this.NotFound( );
            }

            Transaction transaction = await this.context.Transaction
                                                .Include( t => t.BankAccount )
                                                .Include( t => t.CategoryItem )
                                                .Include( t => t.NjordBooksUser )
                                                .FirstOrDefaultAsync( m => m.Id == id )
                                                .ConfigureAwait( false );

            if ( transaction == null )
            {
                return this.NotFound( );
            }

            return this.View( transaction );
        }

        // POST: Transactions/Delete/5
        [HttpPost]
        [ActionName( "Delete" )]
        [ValidateAntiForgeryToken]
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> DeleteConfirmed( int id )
        {
            Transaction transaction = await this.context.Transaction.FindAsync( id ).ConfigureAwait( false );
            this.context.Transaction.Remove( transaction );
            await this.context.SaveChangesAsync( ).ConfigureAwait( false );

            return this.RedirectToAction( "Dashboard", "Households" );
        }

        private bool TransactionExists( int id )
        {
            return this.context.Transaction.Any( e => e.Id == id );
        }
    }
}
