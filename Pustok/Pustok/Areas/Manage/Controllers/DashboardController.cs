using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok.Models;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("Manage")]
    //[Authorize]
    public class DashboardController : Controller
    {
        private UserManager<AppUser> _userManage;
        private RoleManager<IdentityRole> _roleManage;

        public DashboardController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManage)
        {
            _userManage = userManager;
            _roleManage = roleManage;
        }
        public IActionResult Index()
        {
            return View();
        }



        //public async Task<IActionResult> AddRole()
        //{
        //    var user = await _userManage.FindByNameAsync("Zod1accc");

        //    await _userManage.AddToRoleAsync(user, "SuperAdmin");

        //    return Ok("Added");
        //}






        //public async Task<IActionResult> CreateRole()
        //{
        //    IdentityRole ıdentityRole = new IdentityRole("SuperAdmin");
        //    IdentityRole ıdentityRole1 = new IdentityRole("Admin");
        //    IdentityRole ıdentityRole2= new IdentityRole("Member");

        //    await _roleManage.CreateAsync(ıdentityRole);
        //    await _roleManage.CreateAsync(ıdentityRole1);
        //    await _roleManage.CreateAsync(ıdentityRole2);

        //    return Ok("Created");
        //}



        //public async Task< IActionResult> Create()
        //{
        //    AppUser user = new AppUser
        //    {
        //        Fullname = "Ferhad Aslan",
        //        UserName = "Zod1accc"
        //    };

        //    var result = await _userManage.CreateAsync(user,"Admin123");

        //    return Ok(result);
        //}
    }
}
