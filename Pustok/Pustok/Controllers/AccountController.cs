using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok.Models;
using Pustok.ViewModels.Member;

namespace Pustok.Controllers
{
    public class AccountController : Controller
    {
		private UserManager<AppUser> _userManager;
		private SignInManager<AppUser> _signInManager;

		public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
         }
        public IActionResult Index()
        {
            return View();
        }

        
        public IActionResult Register()
        {
            return View();
        }

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            if (!ModelState.IsValid) return View();

            var member = await _userManager.FindByNameAsync(registerVM.Username);

            if (member != null)
            {
                ModelState.AddModelError("Username", "Username has taken!");
                return View();
            }

            member  = await _userManager.FindByEmailAsync(registerVM.Email);

            if(member != null)
            {
                ModelState.AddModelError("Email", "Email has taken!");
                return View();
            }

            member = new AppUser
            {
                Fullname = registerVM.Fullname,
                UserName = registerVM.Username,
                Email = registerVM.Email
            };

            var result = await _userManager.CreateAsync(member,registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                    return View();
                }
            }

            var rolResult = await _userManager.AddToRoleAsync(member,"Member");

            if(!rolResult.Succeeded)
            {
                foreach (var err in rolResult.Errors)
                {
                    ModelState.AddModelError("",err.Description);
                    return View();  
                }
            }

            await _signInManager.SignInAsync(member,isPersistent: false);

            return RedirectToAction("index", "home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = await _userManager.FindByNameAsync(loginVM.Username);

            if (user == null)
            {
                ModelState.AddModelError("", "Username or password incorrect!");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or password incorrect!");
                return View();
            }

            await _signInManager.SignInAsync(user,isPersistent: false);

            return RedirectToAction("index", "home");
        }

        public IActionResult Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                _signInManager.SignOutAsync();
            }
            
            return RedirectToAction("index", "home");
        }
    }
}
