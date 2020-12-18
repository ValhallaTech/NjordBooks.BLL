using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NjordBooks.BLL.Data;
using NjordBooks.BLL.Models;

namespace NjordBooks.BLL.Controllers
{
    [Authorize]
    public class CategoryItemsController : Controller
    {
        private readonly NjordBooksContext context;
        private readonly UserManager<NjordBooksUser> userManager;

        public CategoryItemsController( NjordBooksContext context, UserManager<NjordBooksUser> userManager )
        {
            this.context = context;
            this.userManager = userManager;
        }

        // GET: CategoryItems
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Index( )
        {
            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            Household houseHold = await this.context.Household
                                            .Include( hh => hh.Categories )
                                            .ThenInclude( c => c.CategoryItems )
                                            .FirstOrDefaultAsync( hh => hh.Id == user.HouseholdId )
                                            .ConfigureAwait( false );

            List<CategoryItem> items = houseHold.Categories.SelectMany( c => c.CategoryItems ).ToList( );

            return this.View( items );
        }

        // GET: CategoryItems/Details/5
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Details( int? id )
        {
            if ( id == null )
            {
                return this.NotFound( );
            }

            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            Household houseHold = await this.context.Household
                                            .Include( hh => hh.Categories )
                                            .ThenInclude( c => c.CategoryItems )
                                            .FirstOrDefaultAsync( hh => hh.Id == user.HouseholdId )
                                            .ConfigureAwait( false );

            CategoryItem categoryItem = houseHold.Categories.SelectMany( c => c.CategoryItems )
                                                 .FirstOrDefault( m => m.Id == id );

            if ( categoryItem == null )
            {
                return this.NotFound( );
            }

            return this.View( categoryItem );
        }

        // GET: CategoryItems/Create
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Create( )
        {
            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            Household houseHold = await this.context.Household
                                            .Include( hh => hh.Categories )
                                            .ThenInclude( c => c.CategoryItems )
                                            .FirstOrDefaultAsync( hh => hh.Id == user.HouseholdId )
                                            .ConfigureAwait( false );
            this.ViewData["CategoryId"] = new SelectList( houseHold.Categories, "Id", "Name" );

            return this.View( );
        }

        // POST: CategoryItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Create( [Bind( "Id,CategoryId,Name,Description,TargetAmount,ActualAmount" )]
                                                 CategoryItem categoryItem )
        {
            if ( this.ModelState.IsValid )
            {
                this.context.Add( categoryItem );
                await this.context.SaveChangesAsync( ).ConfigureAwait( false );

                return this.RedirectToAction( "Dashboard", "Households" );
            }

            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            Household houseHold = await this.context.Household
                                            .Include( hh => hh.Categories )
                                            .ThenInclude( c => c.CategoryItems )
                                            .FirstOrDefaultAsync( hh => hh.Id == user.HouseholdId )
                                            .ConfigureAwait( false );
            this.ViewData["CategoryId"] = new SelectList( houseHold.Categories, "Id", "Id", categoryItem.CategoryId );

            return this.View( categoryItem );
        }

        // GET: CategoryItems/Edit/5
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Edit( int? id )
        {
            if ( id == null )
            {
                return this.NotFound( );
            }

            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            Household houseHold = await this.context.Household
                                            .Include( hh => hh.Categories )
                                            .ThenInclude( c => c.CategoryItems )
                                            .FirstOrDefaultAsync( hh => hh.Id == user.HouseholdId )
                                            .ConfigureAwait( false );

            CategoryItem categoryItem = houseHold.Categories.SelectMany( c => c.CategoryItems )
                                                 .FirstOrDefault( m => m.Id == id );

            if ( categoryItem == null )
            {
                return this.NotFound( );
            }

            this.ViewData["CategoryId"] = new SelectList( houseHold.Categories, "Id", "Name", categoryItem.CategoryId );

            return this.View( categoryItem );
        }

        // POST: CategoryItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Edit(
            int id, [Bind( "Id,CategoryId,Name,Description,TargetAmount,ActualAmount" )]
            CategoryItem categoryItem )
        {
            if ( id != categoryItem.Id )
            {
                return this.NotFound( );
            }

            if ( this.ModelState.IsValid )
            {
                try
                {
                    this.context.Update( categoryItem );
                    await this.context.SaveChangesAsync( ).ConfigureAwait( false );
                }
                catch ( DbUpdateConcurrencyException ) when ( !this.CategoryItemExists( categoryItem.Id ) )
                {
                    return this.NotFound( );
                }

                return this.RedirectToAction( nameof( this.Index ) );
            }

            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            Household houseHold = await this.context.Household
                                            .Include( hh => hh.Categories )
                                            .ThenInclude( c => c.CategoryItems )
                                            .FirstOrDefaultAsync( hh => hh.Id == user.HouseholdId )
                                            .ConfigureAwait( false );
            this.ViewData["CategoryId"] = new SelectList( houseHold.Categories, "Id", "Id", categoryItem.CategoryId );

            return this.View( categoryItem );
        }

        // GET: CategoryItems/Delete/5
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Delete( int? id )
        {
            if ( id == null )
            {
                return this.NotFound( );
            }

            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            Household houseHold = await this.context.Household
                                            .Include( hh => hh.Categories )
                                            .ThenInclude( c => c.CategoryItems )
                                            .FirstOrDefaultAsync( hh => hh.Id == user.HouseholdId )
                                            .ConfigureAwait( false );

            CategoryItem categoryItem = houseHold.Categories.SelectMany( c => c.CategoryItems )
                                                 .FirstOrDefault( m => m.Id == id );

            if ( categoryItem == null )
            {
                return this.NotFound( );
            }

            return this.View( categoryItem );
        }

        // POST: CategoryItems/Delete/5
        [HttpPost]
        [ActionName( "Delete" )]
        [ValidateAntiForgeryToken]
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> DeleteConfirmed( int id )
        {
            List<Transaction> transactions = await this.context.Transaction.Where( t => t.CategoryItemId == id )
                                                       .ToListAsync( )
                                                       .ConfigureAwait( false );

            foreach ( Transaction transaction in transactions )
            {
                this.context.Transaction.Remove( transaction );
            }

            CategoryItem categoryItem = await this.context.CategoryItem.FindAsync( id ).ConfigureAwait( false );
            this.context.CategoryItem.Remove( categoryItem );
            await this.context.SaveChangesAsync( ).ConfigureAwait( false );

            return this.RedirectToAction( "Dashboard", "Households" );
        }

        private bool CategoryItemExists( int id )
        {
            return this.context.CategoryItem.Any( e => e.Id == id );
        }
    }
}
