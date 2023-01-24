using Microsoft.AspNetCore.Mvc;
using Pustok.Helpers;
using Pustok.Models;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class BrandController : Controller
    {
        private DataContext _dataContext { get; }
        public BrandController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }


        public IActionResult Index(int page = 1)
        {
            //List<Brand> brands = _dataContext.Brands.ToList();
            var query = _dataContext.Brands.AsQueryable();

            var paginatedBrands = PaginatedList<Brand>.Create(query, 3, page);

            return View(paginatedBrands);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Brand brand)
        {

            _dataContext.Brands.Add(brand);
            _dataContext.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Update(int id)
        {
            Brand brand = _dataContext.Brands.Find(id);
            if (brand == null) return View("Error");

            return View(brand);
        }

        [HttpPost]
        public IActionResult Update(Brand brand)
        {
            Brand existbrand = _dataContext.Brands.Find(brand.Id);
            if (brand == null) return View("Error");
            existbrand.Image = brand.Image;
            _dataContext.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Brand brand = _dataContext.Brands.Find(id);
            if (brand == null) return View("Error");

            _dataContext.Brands.Remove(brand);
            _dataContext.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
