using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok.Models;
using Pustok.ViewModels;

namespace Pustok.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        public DataContext _dataContext { get; }

        private UserManager<AppUser> _userManager;

        public HeaderViewComponent(DataContext dataContext,UserManager<AppUser> userManager)
        {
            _dataContext = dataContext;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            
            List<BasketViewModel> basketItems = new List<BasketViewModel>();
            List<CheckoutItemViewModel> checkoutItems = new List<CheckoutItemViewModel>();
            List<BasketItem> userBasketItems = new List<BasketItem>();
            CheckoutItemViewModel checkoutItem = null;
            AppUser user = null;

            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }

            if(user == null)
            {
                string basketItemsStr = HttpContext.Request.Cookies["Basket"];

                if (basketItemsStr != null)
                {
                    basketItems = JsonConvert.DeserializeObject<List<BasketViewModel>>(basketItemsStr);
                    foreach (var item in basketItems)
                    {
                        checkoutItem = new CheckoutItemViewModel
                        {
                            Book = _dataContext.Books.Include(x => x.BookImage).FirstOrDefault(x => x.Id == item.BookId),
                            Count = item.Count
                        };
                        checkoutItems.Add(checkoutItem);
                    }
                }
            }
            else
            {
                userBasketItems = _dataContext.BasketItems.Include(b=>b.Book).Include(x=>x.Book.BookImage).Where(x=>x.AppUserId == user.Id && !x.IsDeleted).ToList();
                if(userBasketItems is not null)
                {
                    foreach (var item in userBasketItems)
                    {
                        checkoutItem = new CheckoutItemViewModel
                        {
                            Book = item.Book,
                            Count = item.Count
                        };
                        checkoutItems.Add(checkoutItem);
                    }
                }
            }

            

            return View(await Task.FromResult(checkoutItems));
        }

    }
}
