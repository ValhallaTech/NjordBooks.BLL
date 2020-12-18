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
    public class NotificationsController : Controller
    {
        private readonly NjordBooksContext context;
        private readonly UserManager<NjordBooksUser> userManager;

        public NotificationsController( NjordBooksContext context, UserManager<NjordBooksUser> userManager )
        {
            this.context = context;
            this.userManager = userManager;
        }

        // GET: Notifications
        [Authorize( Roles = "Admin,Head" )]
        public async Task<IActionResult> Index( )
        {
            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Notification, Household> applicationDbContext =
                this.context.Notification
                    .Where( x => x.HouseholdId == user.HouseholdId )
                    .Include( n => n.Household );

            return this.View( await applicationDbContext.ToListAsync( ).ConfigureAwait( false ) );
        }

        // GET: Notifications/Details/5
        [Authorize( Roles = "Admin,Head" )]
        public async Task<IActionResult> Details( int? id )
        {
            if ( id == null )
            {
                return this.NotFound( );
            }

            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            Notification notification = await this.context.Notification
                                                  .Include( n => n.Household )
                                                  .Where( x => x.HouseholdId == user.HouseholdId )
                                                  .FirstOrDefaultAsync( m => m.Id == id )
                                                  .ConfigureAwait( false );

            if ( notification == null )
            {
                return this.NotFound( );
            }

            notification.IsRead = true;
            this.context.Update( notification );
            await this.context.SaveChangesAsync( ).ConfigureAwait( false );

            return this.View( notification );
        }

        // GET: Notifications/Create
        [Authorize( Roles = "Admin" )]
        public IActionResult Create( ) => this.View( );

        // POST: Notifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize( Roles = "Admin,Head" )]
        public async Task<IActionResult> Create( [Bind( "Id,HouseholdId,Created,Subject,Body,IsRead" )]
                                                 Notification notification )
        {
            if ( this.ModelState.IsValid )
            {
                this.context.Add( notification );
                await this.context.SaveChangesAsync( ).ConfigureAwait( false );

                return this.RedirectToAction( nameof( this.Index ) );
            }

            return this.View( notification );
        }

        // GET: Notifications/Delete/5
        [Authorize( Roles = "Admin,Head" )]
        public async Task<IActionResult> Delete( int? id )
        {
            if ( id == null )
            {
                return this.NotFound( );
            }

            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            Notification notification = await this.context.Notification
                                                  .Include( n => n.Household )
                                                  .Where( x => x.HouseholdId == user.HouseholdId )
                                                  .FirstOrDefaultAsync( m => m.Id == id )
                                                  .ConfigureAwait( false );

            if ( notification == null )
            {
                return this.NotFound( );
            }

            return this.View( notification );
        }

        // POST: Notifications/Delete/5
        [HttpPost]
        [ActionName( "Delete" )]
        [ValidateAntiForgeryToken]
        [Authorize( Roles = "Admin,Head" )]
        public async Task<IActionResult> DeleteConfirmed( int id )
        {
            Notification notification = await this.context.Notification.FindAsync( id ).ConfigureAwait( false );
            this.context.Notification.Remove( notification );
            await this.context.SaveChangesAsync( ).ConfigureAwait( false );

            return this.RedirectToAction( nameof( this.Index ) );
        }

        private bool NotificationExists( int id )
        {
            return this.context.Notification.Any( e => e.Id == id );
        }
    }
}
