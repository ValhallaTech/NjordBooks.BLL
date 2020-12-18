using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NjordBooks.BLL.Data;
using NjordBooks.BLL.Models;

namespace NjordBooks.BLL.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly NjordBooksContext context;
        private readonly UserManager<NjordBooksUser> userManager;

        public CategoriesController( NjordBooksContext context, UserManager<NjordBooksUser> userManager )
        {
            this.context = context;
            this.userManager = userManager;
        }

        // GET: Categories
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Index( )
        {
            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Category, Household> applicationDbContext = this.context.Category
                                                                                                                  .Where( x => x.HouseholdId == user.HouseholdId )
                                                                                                                  .Include( c => c.Household );
            return this.View( await applicationDbContext.ToListAsync( ).ConfigureAwait( false ) );
        }

        // GET: Categories/Details/5
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Details( int? id )
        {
            if ( id == null )
            {
                return this.NotFound( );
            }

            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            Category category = await this.context.Category
                                          .Include( c => c.Household )
                                          .Where( x => x.HouseholdId == user.HouseholdId )
                                          .FirstOrDefaultAsync( m => m.Id == id )
                                          .ConfigureAwait( false );
            if ( category == null )
            {
                return this.NotFound( );
            }

            return this.View( category );
        }

        // GET: Categories/Create
        [Authorize( Roles = "Admin,Head,Member" )]
        public IActionResult Create( ) => this.View( );

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Create( [Bind( "Id,HouseholdId,Name,Description" )] Category category )
        {
            if ( this.ModelState.IsValid )
            {
                this.context.Add( category );
                await this.context.SaveChangesAsync( ).ConfigureAwait( false );
                this.TempData["Script"] = $"NewColor('{category.Name}')";
                return this.RedirectToAction( "Dashboard", "Households" );
            }
            return this.View( category );
        }

        // GET: Categories/Edit/5
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Edit( int? id )
        {
            if ( id == null )
            {
                return this.NotFound( );
            }

            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            Category category = await this.context.Category
                                          .Where( x => x.HouseholdId == user.HouseholdId )
                                          .FirstOrDefaultAsync( x => x.Id == id )
                                          .ConfigureAwait( false );
            if ( category == null )
            {
                return this.NotFound( );
            }
            return this.View( category );
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Edit( int id, [Bind( "Id,HouseholdId,Name,Description" )] Category category )
        {
            if ( id != category.Id )
            {
                return this.NotFound( );
            }

            if ( this.ModelState.IsValid )
            {
                try
                {
                    this.context.Update( category );
                    await this.context.SaveChangesAsync( ).ConfigureAwait( false );
                }
                catch ( DbUpdateConcurrencyException ) when ( !this.CategoryExists( category.Id ) )
                {
                    return this.NotFound( );
                }
                return this.RedirectToAction( nameof( this.Index ) );
            }
            return this.View( category );
        }

        // GET: Categories/Delete/5
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Delete( int? id )
        {
            if ( id == null )
            {
                return this.NotFound( );
            }

            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            Category category = await this.context.Category
                                          .Include( c => c.Household )
                                          .Where( x => x.HouseholdId == user.HouseholdId )
                                          .FirstOrDefaultAsync( x => x.Id == id )
                                          .ConfigureAwait( false );
            if ( category == null )
            {
                return this.NotFound( );
            }

            return this.View( category );
        }

        // POST: Categories/Delete/5
        [HttpPost]
        [ActionName( "Delete" )]
        [ValidateAntiForgeryToken]
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> DeleteConfirmed( int id )
        {
            Category category = await this.context.Category.FindAsync( id ).ConfigureAwait( false );
            this.context.Category.Remove( category );
            await this.context.SaveChangesAsync( ).ConfigureAwait( false );
            return this.RedirectToAction( nameof( this.Index ) );
        }

        private bool CategoryExists( int id )
        {
            return this.context.Category.Any( e => e.Id == id );
        }
    }
}
