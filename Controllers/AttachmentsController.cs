using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NjordBooks.BLL.Data;
using NjordBooks.BLL.Models;

namespace NjordBooks.BLL.Controllers
{
    [Authorize]
    public class AttachmentsController : Controller
    {
        private readonly NjordBooksContext context;

        public AttachmentsController( NjordBooksContext context ) => this.context = context;

        // GET: Attachments
        [Authorize( Roles = "Admin,Head" )]
        public async Task<IActionResult> Index( )
        {
            Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Attachment, Household> applicationDbContext = this.context.Attachment.Include( a => a.Household );
            return this.View( await applicationDbContext.ToListAsync( ).ConfigureAwait( false ) );
        }

        // GET: Attachments/Details/5
        [Authorize( Roles = "Admin,Head" )]
        public async Task<IActionResult> Details( int? id )
        {
            if ( id == null )
            {
                return this.NotFound( );
            }

            Attachment attachment = await this.context.Attachment
                                              .Include( a => a.Household )
                                              .FirstOrDefaultAsync( m => m.Id == id )
                                              .ConfigureAwait( false );
            if ( attachment == null )
            {
                return this.NotFound( );
            }

            return this.View( attachment );
        }

        // GET: Attachments/Create
        [Authorize( Roles = "Admin,Head" )]
        public IActionResult Create( )
        {
            this.ViewData["HouseholdId"] = new SelectList( this.context.Set<Household>( ), "Id", "Name" );
            return this.View( );
        }

        // POST: Attachments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Create( [Bind( "Id,HouseholdId,FileName,Description,ContentType,FileData" )] Attachment attachment )
        {
            if ( this.ModelState.IsValid )
            {
                this.context.Add( attachment );
                await this.context.SaveChangesAsync( ).ConfigureAwait( false );
                return this.RedirectToAction( nameof( this.Index ) );
            }

            this.ViewData["HouseholdId"] = new SelectList( this.context.Set<Household>( ), "Id", "Name", attachment.HouseholdId );
            return this.View( attachment );
        }

        // GET: Attachments/Edit/5
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Edit( int? id )
        {
            if ( id == null )
            {
                return this.NotFound( );
            }

            Attachment attachment = await this.context.Attachment.FindAsync( id ).ConfigureAwait( false );
            if ( attachment == null )
            {
                return this.NotFound( );
            }

            this.ViewData["HouseholdId"] = new SelectList( this.context.Set<Household>( ), "Id", "Name", attachment.HouseholdId );
            return this.View( attachment );
        }

        // POST: Attachments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize( Roles = "Admin,Head,Member" )]
        public async Task<IActionResult> Edit( int id, [Bind( "Id,HouseholdId,FileName,Description,ContentType,FileData" )] Attachment attachment )
        {
            if ( id != attachment.Id )
            {
                return this.NotFound( );
            }

            if ( this.ModelState.IsValid )
            {
                try
                {
                    this.context.Update( attachment );
                    await this.context.SaveChangesAsync( ).ConfigureAwait( false );
                }
                catch ( DbUpdateConcurrencyException ) when ( !this.AttachmentExists( attachment.Id ) )
                {
                    return this.NotFound( );
                }
                return this.RedirectToAction( nameof( this.Index ) );
            }

            this.ViewData["HouseholdId"] = new SelectList( this.context.Set<Household>( ), "Id", "Name", attachment.HouseholdId );
            return this.View( attachment );
        }

        // GET: Attachments/Delete/5
        [Authorize( Roles = "Admin,Head" )]
        public async Task<IActionResult> Delete( int? id )
        {
            if ( id == null )
            {
                return this.NotFound( );
            }

            Attachment attachment = await this.context.Attachment
                                              .Include( a => a.Household )
                                              .FirstOrDefaultAsync( m => m.Id == id )
                                              .ConfigureAwait( false );
            if ( attachment == null )
            {
                return this.NotFound( );
            }

            return this.View( attachment );
        }

        // POST: Attachments/Delete/5
        [HttpPost]
        [ActionName( "Delete" )]
        [ValidateAntiForgeryToken]
        [Authorize( Roles = "Admin,Head" )]
        public async Task<IActionResult> DeleteConfirmed( int id )
        {
            Attachment attachment = await this.context.Attachment.FindAsync( id ).ConfigureAwait( false );
            this.context.Attachment.Remove( attachment );
            await this.context.SaveChangesAsync( ).ConfigureAwait( false );
            return this.RedirectToAction( nameof( this.Index ) );
        }

        private bool AttachmentExists( int id )
        {
            return this.context.Attachment.Any( e => e.Id == id );
        }
    }
}
