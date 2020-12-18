using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
    public class InvitationsController : Controller
    {
        private readonly NjordBooksContext context;
        private readonly IEmailSender emailService;
        private readonly SignInManager<NjordBooksUser> signInManager;
        private readonly UserManager<NjordBooksUser> userManager;
        private readonly INjordBooksFileService fileService;

        public InvitationsController( NjordBooksContext context, IEmailSender emailService,
                                      SignInManager<NjordBooksUser> signInManager,
                                      UserManager<NjordBooksUser> userManager, INjordBooksFileService fileService )
        {
            this.context = context;
            this.emailService = emailService;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.fileService = fileService;
        }

        // GET: Invitations
        [Authorize( Roles = "Admin,Head" )]
        public async Task<IActionResult> Index( )
        {
            IIncludableQueryable<Invitation, Household> applicationDbContext =
                this.context.Invitation.Include( i => i.Household );

            return this.View( await applicationDbContext.ToListAsync( ).ConfigureAwait( false ) );
        }

        // GET: Invitations/Details/5
        [Authorize( Roles = "Admin,Head" )]
        public async Task<IActionResult> Details( int? id )
        {
            if ( id == null )
            {
                return this.NotFound( );
            }

            Invitation invitation = await this.context.Invitation
                                              .Include( i => i.Household )
                                              .FirstOrDefaultAsync( m => m.Id == id )
                                              .ConfigureAwait( false );

            if ( invitation == null )
            {
                return this.NotFound( );
            }

            return this.View( invitation );
        }

        [Authorize( Roles = "Admin,Head" )]
        // GET: Invitations/Create
        public IActionResult Create( )
        {
            this.ViewData["HouseholdId"] = new SelectList( this.context.Household, "Id", "Name" );

            return this.View( );
        }

        // POST: Invitations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize( Roles = "Admin,Head" )]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( [Bind( "Id,HouseholdId,Created,Expires,Accepted,EmailTo,Subject,Body,Code" )]
                                                 Invitation invitation )
        {
            if ( this.ModelState.IsValid )
            {
                // prevent inviting user already in household
                NjordBooksUser user = await this.context.Users.FirstOrDefaultAsync( u => u.Email == invitation.EmailTo )
                                                .ConfigureAwait( false );

                if ( user?.HouseholdId != null )
                {
                    this.TempData["Script"] = "CantInvite()";

                    return this.RedirectToAction( "Dashboard", "Households" );
                }

                // Create Invitation Record
                this.context.Add( invitation );
                await this.context.SaveChangesAsync( ).ConfigureAwait( false );

                // Construct Email
                string acceptUrl = this.Url.Action( "Accept", "Invitations",
                                                    new { email = invitation.EmailTo, code = invitation.Code },
                                                    this.Request.Scheme );
                string declineUrl = this.Url.Action( "Decline", "Invitations", new { code = invitation.Code },
                                                     this.Request.Scheme );
                string houseHoldName = ( await this.context.Household
                                                   .FirstOrDefaultAsync( hh => hh.Id == invitation.HouseholdId )
                                                   .ConfigureAwait( false ) ).Name;
                string emailBody =
                    $"<h3>You are invited to join the <em>{houseHoldName}</em> household.</h3>"
                  + $"<p>{invitation.Body}</p>"
                  + "<p>If you already have an account you can copy and paste this code when you click to 'join'</p>"
                  + $"<b>{invitation.Code.ToString( ).ToUpper( )}</b></p><br />"
                  + $"<p>Or click <a href='{HtmlEncoder.Default.Encode( acceptUrl )}'>Accept</a>"
                  + " or "
                  + $"<a href='{HtmlEncoder.Default.Encode( declineUrl )}'> Deny</a>.</p>";

                // Send Email
                await this.emailService.SendEmailAsync( invitation.EmailTo, invitation.Subject, emailBody )
                          .ConfigureAwait( false );

                // Sweet Alert
                this.TempData["Script"] = "CanInvite()";

                return this.RedirectToAction( "Dashboard", "Households" );
            }

            this.ViewData["HouseholdId"] =
                new SelectList( this.context.Household, "Id", "Name", invitation.HouseholdId );

            return this.View( invitation );
        }

        [AllowAnonymous]
        public async Task<IActionResult> Accept( string email, string code )
        {
            // Validation
            Invitation invitation = await this.context.Invitation
                                              .FirstOrDefaultAsync( i => i.Code.ToString( ) == code.ToLower( ) )
                                              .ConfigureAwait( false );

            if ( invitation?.Accepted != false || DateTimeOffset.Now > invitation.Expires )
            {
                return this.NotFound( );
            }

            // No account found
            NjordBooksUser user = await this.context.Users.FirstOrDefaultAsync( u => u.Email == email )
                                            .ConfigureAwait( false );

            if ( user == null )
            {
                this.TempData["Email"] = email;
                this.TempData["Code"] = code;

                return this.View( );
            }

            // Account Found
            invitation.Accepted = true;
            user.HouseholdId = invitation.HouseholdId;
            IList<string> roles = await this.userManager.GetRolesAsync( user ).ConfigureAwait( false );
            await this.userManager.RemoveFromRolesAsync( user, roles ).ConfigureAwait( false );
            await this.userManager.AddToRoleAsync( user, nameof( Roles.Member ) ).ConfigureAwait( false );
            await this.signInManager.SignInAsync( user, false ).ConfigureAwait( false );
            await this.context.SaveChangesAsync( ).ConfigureAwait( false );

            return this.RedirectToAction( "Dashboard", "Households" );
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Accept( string email, string code, string firstName, string lastName,
                                                 IFormFile avatar, string password )
        {
            Invitation invitation = await this.context.Invitation.FirstOrDefaultAsync( i => i.Code.ToString( ) == code )
                                              .ConfigureAwait( false );
            byte[] fileData;
            string fileName;

            if ( avatar != null )
            {
                fileData = await this.fileService.ConvertFileToByteArrayAsync( avatar ).ConfigureAwait( false );
                fileName = avatar.FileName;
            }
            else
            {
                fileData = await this.fileService.GetDefaultAvatarFileBytesAsync( ).ConfigureAwait( false );
                fileName = this.fileService.GetDefaultAvatarFileName( );
            }

            NjordBooksUser user = new NjordBooksUser
            {
                FirstName = firstName,
                LastName = lastName,
                FileName = fileName,
                FileData = fileData,
                UserName = email,
                Email = email,
                HouseholdId = invitation.HouseholdId,
                EmailConfirmed = true
            };
            invitation.Accepted = true;
            // where the magic happens
            await this.userManager.CreateAsync( user, password ).ConfigureAwait( false );

            await this.userManager.AddToRoleAsync( user, nameof( Roles.Member ) ).ConfigureAwait( false );
            await this.signInManager.SignInAsync( user, false ).ConfigureAwait( false );
            await this.context.SaveChangesAsync( ).ConfigureAwait( false );

            return this.RedirectToAction( "Dashboard", "Households" );
        }

        [AllowAnonymous]
        public async Task<IActionResult> Decline( string code )
        {
            // ensure signed out
            await this.signInManager.SignOutAsync( ).ConfigureAwait( false );

            Invitation invitation = await this.context.Invitation.FirstOrDefaultAsync( i => i.Code.ToString( ) == code )
                                              .ConfigureAwait( false );

            if ( invitation?.Accepted != false || DateTimeOffset.Now > invitation.Expires )
            {
                return this.NotFound( );
            }

            invitation.Accepted = true;
            await this.context.SaveChangesAsync( ).ConfigureAwait( false );

            this.TempData["HouseHoldName"] = ( await this.context.Household
                                                         .FirstOrDefaultAsync( hh => hh.Id == invitation.HouseholdId )
                                                         .ConfigureAwait( false ) ).Name;

            return this.View( );
        }

        // GET: Invitations/Edit/5
        [Authorize( Roles = "Admin,Head" )]
        public async Task<IActionResult> Edit( int? id )
        {
            if ( id == null )
            {
                return this.NotFound( );
            }

            Invitation invitation = await this.context.Invitation.FindAsync( id ).ConfigureAwait( false );

            if ( invitation == null )
            {
                return this.NotFound( );
            }

            this.ViewData["HouseholdId"] =
                new SelectList( this.context.Household, "Id", "Name", invitation.HouseholdId );

            return this.View( invitation );
        }

        // POST: Invitations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize( Roles = "Admin,Head" )]
        public async Task<IActionResult> Edit(
            int id, [Bind( "Id,HouseholdId,Created,Expires,Accepted,EmailTo,Subject,Body,Code" )]
            Invitation invitation )
        {
            if ( id != invitation.Id )
            {
                return this.NotFound( );
            }

            if ( this.ModelState.IsValid )
            {
                try
                {
                    this.context.Update( invitation );
                    await this.context.SaveChangesAsync( ).ConfigureAwait( false );
                }
                catch ( DbUpdateConcurrencyException )
                {
                    if ( !this.InvitationExists( invitation.Id ) )
                    {
                        return this.NotFound( );
                    }

                    throw;
                }

                return this.RedirectToAction( nameof( this.Index ) );
            }

            this.ViewData["HouseholdId"] =
                new SelectList( this.context.Household, "Id", "Name", invitation.HouseholdId );

            return this.View( invitation );
        }

        // GET: Invitations/Delete/5
        [Authorize( Roles = "Admin,Head" )]
        public async Task<IActionResult> Delete( int? id )
        {
            if ( id == null )
            {
                return this.NotFound( );
            }

            Invitation invitation = await this.context.Invitation
                                              .Include( i => i.Household )
                                              .FirstOrDefaultAsync( m => m.Id == id )
                                              .ConfigureAwait( false );

            if ( invitation == null )
            {
                return this.NotFound( );
            }

            return this.View( invitation );
        }

        // POST: Invitations/Delete/5
        [Authorize( Roles = "Admin,Head" )]
        [HttpPost]
        [ActionName( "Delete" )]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed( int id )
        {
            Invitation invitation = await this.context.Invitation.FindAsync( id ).ConfigureAwait( false );
            this.context.Invitation.Remove( invitation );
            await this.context.SaveChangesAsync( ).ConfigureAwait( false );

            return this.RedirectToAction( nameof( this.Index ) );
        }

        private bool InvitationExists( int id )
        {
            return this.context.Invitation.Any( e => e.Id == id );
        }
    }
}
