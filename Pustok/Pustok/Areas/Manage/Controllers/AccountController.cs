using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok.Areas.Manage.ViewModels;
using Pustok.Models;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class AccountController : Controller
    {
        private UserManager<AppUser> _userManage;
        private SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager)
        {
            _userManage = userManager;
            _signInManager = signInManager;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginViewModel adminLogin)
        {
            if (!ModelState.IsValid) return View();
            var user =  await _userManage.FindByNameAsync(adminLogin.Username);

            if(user == null)
            {
                ModelState.AddModelError("", "Username or Password incorrect");
                return View();
            }

            var passwordResult = await _signInManager.PasswordSignInAsync(user, adminLogin.Password, false, false);

            if(!passwordResult.Succeeded)
            {
                ModelState.AddModelError("", "Username or Password incorrect");
                return View();
            }

            return RedirectToAction("index", "dashboard");
        }

        public async Task< IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
              await _signInManager.SignOutAsync();

            }

            return RedirectToAction("login", "account");
        }
    }
}
