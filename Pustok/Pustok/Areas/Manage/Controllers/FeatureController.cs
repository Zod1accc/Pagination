using Microsoft.AspNetCore.Mvc;
using Pustok.Helpers;
using Pustok.Models;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class FeatureController : Controller
    {
        private DataContext _dataContext { get; }
        public FeatureController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }


        public IActionResult Index(int page = 1)
        {
            //List<Feature> features = _dataContext.Features.ToList();

            var query = _dataContext.Features.AsQueryable();
            var paginatedFeatures = PaginatedList<Feature>.Create(query, 3, page);
            return View(paginatedFeatures);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Feature feature) 
        {
            _dataContext.Features.Add(feature);
            _dataContext.SaveChanges();

            return RedirectToAction("index");
        }


        public IActionResult Update(int id)
        {
            Feature feature = _dataContext.Features.Find(id);

            if (feature == null) return View("Error");

            return View(feature);

        }

        [HttpPost]
        public IActionResult Update(Feature feature)
        {
            Feature existFeature = _dataContext.Features.Find(feature.Id);

            if (existFeature == null) return View("Error");

            existFeature.Name = feature.Name;
            existFeature.Desc = feature.Desc;
            existFeature.Icon = feature.Icon;

            _dataContext.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Feature feature = _dataContext.Features.Find(id);

            _dataContext.Features.Remove(feature);
            _dataContext.SaveChanges();

            return RedirectToAction("index");
        }
    }
}
