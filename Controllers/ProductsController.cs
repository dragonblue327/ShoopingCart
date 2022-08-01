using Babelon.Infrastucture;
using Babelon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Babelon.Controllers
{
    public class ProductsController : Controller
    {
        private readonly DataContext _Context;
        public ProductsController(DataContext Context)
        {
            _Context = Context;
        }
        public async Task<IActionResult> Index(string categorySlug = "", int p = 1)
        {
            int pageSize = 3;
            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.CategorySlug = categorySlug;

            if (categorySlug == "")
            {
                ViewBag.TotalPages = (int)Math.Ceiling((decimal)_Context.Products.Count()/pageSize);

                return View(await _Context.Products.OrderByDescending(p => p.Id).Skip((p-1) * pageSize).Take(pageSize).ToListAsync());
            }

            Category category = await _Context.Categories.Where(c => c.Slug == categorySlug).FirstOrDefaultAsync();

            if (category == null) return RedirectToAction("Index");
            var productsByCategory = _Context.Products.Where(p => p.CategoryId == category.Id);
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)_Context.Products.Count() / pageSize);
            return View(await productsByCategory.OrderByDescending(p => p.Id).Skip((p - 1) * pageSize).Take(pageSize).ToListAsync());
        }
    }
}
