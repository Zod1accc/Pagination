using Microsoft.AspNetCore.Identity;
using Pustok.Models;

namespace Pustok.Areas.Manage.Services
{
    public class LayoutService
    {
        private UserManager<AppUser> _userManager;
        private IHttpContextAccessor _httPContext;

        public LayoutService(UserManager<AppUser> userManager, IHttpContextAccessor httpContext )
        {
            _userManager  = userManager;
            _httPContext = httpContext;
        }

        public async Task<AppUser> GetUser()
        {
            if(_httPContext.HttpContext.User.Identity.IsAuthenticated)
            {
                
                var user = await _userManager.FindByNameAsync(_httPContext.HttpContext.User.Identity.Name);

                return user;
            }
            return null;
        }
    }
}
