﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using NjordBooks.BLL.Data.Enums;
using NjordBooks.BLL.Extensions;
using NjordBooks.BLL.Models;
using NjordBooks.BLL.Services;

namespace NjordBooks.BLL.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<NjordBooksUser> signInManager;
        private readonly UserManager<NjordBooksUser> userManager;
        private readonly ILogger<RegisterModel> logger;
        private readonly IEmailSender emailSender;
        private readonly INjordBooksFileService fileService;

        public RegisterModel( UserManager<NjordBooksUser> userManager,
                              SignInManager<NjordBooksUser> signInManager,
                              ILogger<RegisterModel> logger,
                              IEmailSender emailSender,
                              INjordBooksFileService fileService )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.emailSender = emailSender;
            this.fileService = fileService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [Display( Name = "First Name" )]
            public string FirstName { get; set; }

            [Required]
            [Display( Name = "Last Name" )]
            public string LastName { get; set; }

            [Display( Name = "Avatar" )]
            [NotMapped]
            [DataType( DataType.Upload )]
            [MaxFileSize( 2 * 1024 * 1024 )]
            [AllowedExtensions( new string[] { ".jpg", ".png" } )]
            public IFormFile Avatar { get; set; }

            [Required]
            [EmailAddress]
            [Display( Name = "Email" )]
            public string Email { get; set; }

            [Required]
            [StringLength( 100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                           MinimumLength = 6 )]
            [DataType( DataType.Password )]
            [Display( Name = "Password" )]
            public string Password { get; set; }

            [DataType( DataType.Password )]
            [Display( Name = "Confirm password" )]
            [Compare( "Password", ErrorMessage = "The password and confirmation password do not match." )]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync( string returnUrl = null )
        {
            this.ReturnUrl = returnUrl;
            this.ExternalLogins =
                ( await this.signInManager.GetExternalAuthenticationSchemesAsync( ).ConfigureAwait( false ) ).ToList( );
        }

        public async Task<IActionResult> OnPostAsync( string returnUrl = null )
        {
            returnUrl = returnUrl ?? this.Url.Content( "~/" );
            this.ExternalLogins =
                ( await this.signInManager.GetExternalAuthenticationSchemesAsync( ).ConfigureAwait( false ) ).ToList( );

            if ( !this.ModelState.IsValid ) return this.Page( );

            byte[] fileData;
            string fileName;

            if ( this.Input.Avatar != null )
            {
                fileData = await this.fileService.ConvertFileToByteArrayAsync( this.Input.Avatar )
                                     .ConfigureAwait( false );
                fileName = this.Input.Avatar.FileName;
            }
            else
            {
                fileData = await this.fileService.GetDefaultAvatarFileBytesAsync( ).ConfigureAwait( false );
                fileName = this.fileService.GetDefaultAvatarFileName( );
            }

            NjordBooksUser user = new NjordBooksUser
            {
                FirstName = this.Input.FirstName,
                LastName = this.Input.LastName,
                FileName = fileName,
                FileData = fileData,
                UserName = this.Input.Email,
                Email = this.Input.Email
            };
            IdentityResult result =
                await this.userManager.CreateAsync( user, this.Input.Password ).ConfigureAwait( false );
            await this.userManager.AddToRoleAsync( user, nameof( Roles.New ) ).ConfigureAwait( false );

            if ( result.Succeeded )
            {
                this.logger.LogInformation( "User created a new account with password." );

                string code = await this.userManager.GenerateEmailConfirmationTokenAsync( user )
                                        .ConfigureAwait( false );
                code = WebEncoders.Base64UrlEncode( Encoding.UTF8.GetBytes( code ) );
                string callbackUrl = this.Url.Page( "/Account/ConfirmEmail",
                                                    pageHandler: null,
                                                    values: new
                                                    {
                                                        area = "Identity",
                                                        userId = user.Id,
                                                        code = code,
                                                        returnUrl = returnUrl
                                                    },
                                                    protocol: this.Request.Scheme );

                await this.emailSender.SendEmailAsync( this.Input.Email, "Confirm your email",
                                                       $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode( callbackUrl )}'>clicking here</a>." )
                          .ConfigureAwait( false );

                if ( this.userManager.Options.SignIn.RequireConfirmedAccount )
                {
                    return this.RedirectToPage( "RegisterConfirmation",
                                                new { email = this.Input.Email, returnUrl = returnUrl } );
                }
                else
                {
                    await this.signInManager.SignInAsync( user, isPersistent: false ).ConfigureAwait( false );

                    return this.LocalRedirect( returnUrl );
                }
            }

            foreach ( IdentityError error in result.Errors )
            {
                this.ModelState.AddModelError( string.Empty, error.Description );
            }

            // If we got this far, something failed, redisplay form
            return this.Page( );
        }
    }
}
