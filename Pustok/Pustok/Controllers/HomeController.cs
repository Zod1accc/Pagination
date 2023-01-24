using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok.Models;
using Pustok.ViewModels;

namespace Pustok.Controllers
{
    public class HomeController : Controller
    {
        private DataContext _dataContext { get; }

        private UserManager<AppUser> _userManager;

        public HomeController(DataContext dataContext,UserManager<AppUser> userManager)
        {
            _dataContext = dataContext;
            _userManager = userManager;
        }


        public IActionResult Index()
        {
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Sliders = _dataContext.Sliders.OrderBy(x=>x.Order).ToList(),
                Features = _dataContext.Features.ToList(),
                Brands = _dataContext.Brands.ToList(),
                Books = _dataContext.Books.Include(c=>c.Genre).Include(v=>v.Author).ToList(),
                FeaturedBooks = _dataContext.Books.Include(x=>x.BookImage).Include(x=>x.Author).Where(x=>x.IsFeatured).ToList(),
                NewBooks = _dataContext.Books.Include(x=>x.BookImage).Include(x=>x.Author).Where(x=>x.IsNew).ToList(),
                DiscountBooks = _dataContext.Books.Include(x=>x.BookImage).Include(x=>x.Author).Where(x=>x.DiscountPrice > 0).ToList(),
            };

            return View(homeViewModel);
        }


        public async Task<IActionResult> AddToBasket(int id)
        {
            if (!_dataContext.Books.Any(x => x.Id == id)) return NotFound();
    
            List<BasketViewModel> basketItems= new List<BasketViewModel>();
            BasketViewModel basketItem = null;
            AppUser user = null;
            string BasketStr = HttpContext.Request.Cookies["Basket"];

            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }

            if (user == null)
            {
                if (BasketStr != null)
                {
                    basketItems = JsonConvert.DeserializeObject<List<BasketViewModel>>(BasketStr);

                    basketItem = basketItems.FirstOrDefault(x => x.BookId == id);

                    if (basketItem != null) basketItem.Count++;
                    else
                    {
                        basketItem = new BasketViewModel
                        {
                            BookId = id,
                            Count = 1
                        };
                        basketItems.Add(basketItem);
                    }
                }
                else
                {
                    basketItem = new BasketViewModel
                    {
                        BookId = id,
                        Count = 1
                    };

                    basketItems.Add(basketItem);
                }


                string basketItemsStr = JsonConvert.SerializeObject(basketItems);

                HttpContext.Response.Cookies.Append("Basket", basketItemsStr);
            }
            else
            {
                BasketItem userBasketItems = _dataContext.BasketItems.FirstOrDefault(b=>b.AppUserId == user.Id && b.BookId == id && b.IsDeleted == false);
                if (userBasketItems != null) userBasketItems.Count++;
                else
                {
                    var userbasketItem = new BasketItem
                    {
                        BookId = id,
                        AppUserId = user.Id,
                        Count = 1,
                        IsDeleted = false
                    };
                    _dataContext.BasketItems.Add(userbasketItem);
                }
            }

            _dataContext.SaveChanges();
            
            return Ok();
        }

        public IActionResult GetBasket()
        {
            List<BasketViewModel> Basket = new List<BasketViewModel>();

            string basketItems = HttpContext.Request.Cookies["Basket"];

            if(basketItems != null)
            {
                
                Basket = JsonConvert.DeserializeObject<List<BasketViewModel>>(basketItems);

            }


            return Json(Basket);
        }

        public async Task<IActionResult> Checkout()
        {
            List<BasketViewModel> basketItems = new List<BasketViewModel>();
            List<CheckoutItemViewModel> checkoutItems = new List<CheckoutItemViewModel>();
            List<BasketItem> userBasketItems= new List<BasketItem>();
            CheckoutItemViewModel checkoutItem = new CheckoutItemViewModel();
            AppUser user = null;
            OrderViewModel orderViewModel= null;
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
                            Book = _dataContext.Books.FirstOrDefault(x => x.Id == item.BookId),
                            Count = item.Count
                        };
                        checkoutItems.Add(checkoutItem);
                    }
                }
            }
            else
            {
                userBasketItems = _dataContext.BasketItems.Include(x=>x.Book).Where(b => b.AppUserId == user.Id && !b.IsDeleted).ToList();

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

            orderViewModel = new OrderViewModel
            {
                checkoutItemViewModels = checkoutItems,
            };

           
            return View(orderViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Order(OrderViewModel orderVM)
        {
			List<BasketViewModel> basketItems = new List<BasketViewModel>();
			List<CheckoutItemViewModel> checkoutItems = new List<CheckoutItemViewModel>();
			List<BasketItem> userBasketItems = new List<BasketItem>();
			CheckoutItemViewModel checkoutItem = null;
            double totalPrice = 0;
			AppUser user = null;
            OrderItem orderItem = null;
            if (!ModelState.IsValid) return View("Checkout");

			if (User.Identity.IsAuthenticated)
			{
				user = await _userManager.FindByNameAsync(User.Identity.Name);

			}

            Order order = new Order
            {
                Fullname = orderVM.Fullname,
                Address1 = orderVM.Address1,
                Address2 = orderVM.Address2,
                Country = orderVM.Country,
                Email = orderVM.Email,
                Note = orderVM.Note,
                PhoneNumber = orderVM.PhoneNumber,
                ZipCode = orderVM.ZipCode,
                CreatedTime = DateTime.UtcNow.AddHours(4),
                AppUserId = user?.Id
            };

			if (user == null)
			{
				string basketItemsStr = HttpContext.Request.Cookies["Basket"];
				if (basketItemsStr != null)
				{
					basketItems = JsonConvert.DeserializeObject<List<BasketViewModel>>(basketItemsStr);
					foreach (var item in basketItems)
					{
                        Book book = _dataContext.Books.FirstOrDefault(x => x.Id == item.BookId);
                        if (book == null) return NotFound();
                        orderItem = new OrderItem
                        {
                            Book = book,
                            BookName = book.Name,
                            CostPrice = book.CostPrice,
                            DiscountPrice = book.DiscountPrice,
                            Count = item.Count,
                            SalePrice = book.SellPrice * (1 - (book.DiscountPrice / 100))
                        };
                        totalPrice += orderItem.SalePrice * item.Count;
                        order.OrderItems.Add(orderItem); 
					}
                    
                    Response.Cookies.Append("Basket", "", new CookieOptions()
                    {
                        Expires = DateTime.Now.AddDays(-1)
                    });
                }
            }
			else
			{
				userBasketItems = _dataContext.BasketItems.Include(x => x.Book).Where(b => b.AppUserId == user.Id && !b.IsDeleted).ToList();

				if (userBasketItems is not null)
				{
					foreach (var item in userBasketItems)
					{
						Book book = _dataContext.Books.FirstOrDefault(x => x.Id == item.BookId);
						if (book == null) return NotFound();
						orderItem = new OrderItem
						{
							Book = book,
							BookName = book.Name,
							CostPrice = book.CostPrice,
							DiscountPrice = book.DiscountPrice,
							Count = item.Count,
							SalePrice = book.SellPrice * (1 - (book.DiscountPrice / 100))
						};
						totalPrice += orderItem.SalePrice * item.Count;
						order.OrderItems.Add(orderItem);
                        item.IsDeleted = true;
					}
				}
			}

            order.TotalPrice = totalPrice;
            _dataContext.Orders.Add(order);
            _dataContext.SaveChanges();

            return RedirectToAction("index", "home");
        }


        public async Task<IActionResult> DeleteItemToBasket(int id)
        {
            List<BasketViewModel> basketItems = new List<BasketViewModel>();
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

                    var findItem = basketItems.FirstOrDefault(x => x.BookId == id);

                    if (findItem == null) return NotFound();
                    else
                    {
                        if (findItem.Count > 1)
                        {
                            findItem.Count--;
                        }
                        else
                        {
                            basketItems.Remove(findItem);
                        }
                    }

                }

                string basketStr = JsonConvert.SerializeObject(basketItems);

                HttpContext.Response.Cookies.Append("Basket", basketStr);
            }
            else
            {
                BasketItem userBasketItem = _dataContext.BasketItems.FirstOrDefault(b => b.AppUserId == user.Id && b.BookId == id);
                if(userBasketItem == null) return NotFound();

                if(userBasketItem.Count > 1) userBasketItem.Count--; 
                else
                {
                    _dataContext.BasketItems.Remove(userBasketItem);
                }
            }
            _dataContext.SaveChanges();

            
            
            return Ok();
        }





        //public IActionResult SetCookie(int id)
        //{
        //    List<int> bookIds = new List<int>();
            

        //    string check = HttpContext.Request.Cookies["BookName"];

        //    if(check != null)
        //    {
        //        bookIds = JsonConvert.DeserializeObject<List<int>>(check);

        //        bookIds.Add(id);

        //    }
        //    else
        //    {
        //        bookIds.Add(id);
        //    }


        //    string bookIdStr = JsonConvert.SerializeObject(bookIds);


        //    HttpContext.Response.Cookies.Append("BookName", bookIdStr);

        //    return Content("Added Cookie");
        //}

        //public IActionResult GetCookie()
        //{
        //    List<int> bookIds= new List<int>();

        //    string name =  HttpContext.Request.Cookies["BookName"];
        //    if (name == null) return Json(bookIds);

        //    bookIds = JsonConvert.DeserializeObject<List<int>>(name);
           
        //    return Json(bookIds);
        //}
    }
}