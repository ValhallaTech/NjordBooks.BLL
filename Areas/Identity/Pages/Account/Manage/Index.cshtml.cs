using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NjordBooks.BLL.Data;
using NjordBooks.BLL.Extensions;
using NjordBooks.BLL.Models;
using NjordBooks.BLL.Services;

namespace NjordBooks.BLL.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<NjordBooksUser> userManager;
        private readonly SignInManager<NjordBooksUser> signInManager;
        private readonly INjordBooksFileService fileService;
        private readonly NjordBooksContext context;

        public IndexModel(
            UserManager<NjordBooksUser> userManager,
            SignInManager<NjordBooksUser> signInManager,
            INjordBooksFileService fileService,
            NjordBooksContext context )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.fileService = fileService;
            this.context = context;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [StringLength( 50 )]
            [Display( Name = "First Name" )]
            public string FirstName { get; set; }

            [StringLength( 50 )]
            [Display( Name = "Last Name" )]
            public string LastName { get; set; }

            [Phone]
            [Display( Name = "Phone number" )]
            public string PhoneNumber { get; set; }

            [Display( Name = "Avatar" )]
            [NotMapped]
            [DataType( DataType.Upload )]
            [MaxFileSize( 2 * 1024 * 1024 )]
            [AllowedExtensions( new string[] { ".jpg", ".png" } )]
            public IFormFile Avatar { get; set; }
        }

        private async Task LoadAsync( NjordBooksUser user )
        {
            string userName = await this.userManager.GetUserNameAsync( user ).ConfigureAwait( false );
            string phoneNumber = await this.userManager.GetPhoneNumberAsync( user ).ConfigureAwait( false );

            this.Username = userName;

            this.Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync( )
        {
            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            if ( user == null )
            {
                return this.NotFound( $"Unable to load user with ID '{this.userManager.GetUserId( this.User )}'." );
            }

            await this.LoadAsync( user ).ConfigureAwait( false );
            return this.Page( );
        }

        public async Task<IActionResult> OnPostAsync( )
        {
            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            if ( user == null )
            {
                return this.NotFound( $"Unable to load user with ID '{this.userManager.GetUserId( this.User )}'." );
            }

            if ( !this.ModelState.IsValid )
            {
                await this.LoadAsync( user ).ConfigureAwait( false );
                return this.Page( );
            }
            // First Name
            if ( this.Input.FirstName != null )
            {
                user.FirstName = this.Input.FirstName;
                await this.context.SaveChangesAsync( ).ConfigureAwait( false );
            }
            // Last Name
            if ( this.Input.LastName != null )
            {
                user.LastName = this.Input.LastName;
                await this.context.SaveChangesAsync( ).ConfigureAwait( false );
            }
            // Phone Number
            string phoneNumber = await this.userManager.GetPhoneNumberAsync( user ).ConfigureAwait( false );
            if ( this.Input.PhoneNumber != phoneNumber )
            {
                IdentityResult setPhoneResult = await this.userManager.SetPhoneNumberAsync( user, this.Input.PhoneNumber ).ConfigureAwait( false );
                if ( !setPhoneResult.Succeeded )
                {
                    this.StatusMessage = "Unexpected error when trying to set phone number.";
                    return this.RedirectToPage( );
                }
            }
            // Avatar
            if ( this.Input.Avatar != null )
            {
                user.FileData = await this.fileService.ConvertFileToByteArrayAsync( this.Input.Avatar ).ConfigureAwait( false );
                user.FileName = this.Input.Avatar.FileName;
                await this.context.SaveChangesAsync( ).ConfigureAwait( false );
            }
            await this.signInManager.RefreshSignInAsync( user ).ConfigureAwait( false );
            this.StatusMessage = "Your profile has been updated";
            return this.RedirectToPage( );
        }
    }
}
