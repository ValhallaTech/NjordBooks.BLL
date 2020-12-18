using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NjordBooks.BLL.Data;
using NjordBooks.BLL.Models;
using NjordBooks.BLL.Models.ViewModels;

namespace NjordBooks.BLL.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly NjordBooksContext context;
        private readonly ILogger<HomeController> logger;
        private readonly UserManager<NjordBooksUser> userManager;

        public HomeController( NjordBooksContext context, ILogger<HomeController> logger,
                               UserManager<NjordBooksUser> userManager )
        {
            this.context = context;
            this.logger = logger;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index( )
        {
            NjordBooksUser user = await this.userManager.GetUserAsync( this.User ).ConfigureAwait( false );
            string houseHoldName = null;

            if ( user.HouseholdId != null )
            {
                houseHoldName = this.context.Household.FirstOrDefault( u => u.Id == user.HouseholdId )?.Name;
            }

            LobbyVM model = new LobbyVM
            {
                Role = ( await this.userManager.GetRolesAsync( user ).ConfigureAwait( false ) )[0],
                Household = houseHoldName
            };

            if ( user.HouseholdId != null )
            {
                return this.RedirectToAction( "Dashboard", "Households" );
            }

            return this.View( model );
        }

        public IActionResult Privacy( ) => this.View( );

        [ResponseCache( Duration = 0, Location = ResponseCacheLocation.None, NoStore = true )]
        public IActionResult Error( ) => this.View( new ErrorViewModel
        {
            RequestId = Activity.Current?.Id
                                                                 ?? this.HttpContext.TraceIdentifier
        } );
    }
}
