using Babelon.Infrastucture;
using Babelon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Babelon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductsController : Controller
    {

        private readonly DataContext _Context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductsController(DataContext Context, IWebHostEnvironment webHostEnvironment)
        {
            _Context = Context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index(int p = 1)
        {
            int pageSize = 3;
            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)_Context.Products.Count() / pageSize);
            return View(await _Context.Products.OrderByDescending(p => p.Id)
                .Include(p => p.Category)
                .Skip((p - 1) * pageSize)
                .Take(pageSize).ToListAsync());

        }

        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_Context.Categories,"Id","Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.Categories = new SelectList(_Context.Categories, "Id", "Name", product.CategoryId);

            if (ModelState.IsValid)
            {
                product.Slug = product.Name.ToLower().Replace(" ","-");

                var slug = await _Context.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The product already exists.");
                    return View(product);
                }

                if (product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;

                    string filePath = Path.Combine(uploadsDir, imageName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    product.Image = imageName;
                }

                _Context.Add(product);
                await _Context.SaveChangesAsync();

                TempData["Success"] = "The product has been created!";

                return RedirectToAction("Index");
            }

            return View(product);
        }

        public async Task<IActionResult> Edit(long id)
        {
            Product product = await _Context.Products.FindAsync(id);
            ViewBag.Categories = new SelectList(_Context.Categories, "Id", "Name",product.CategoryId);
            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            ViewBag.Categories = new SelectList(_Context.Categories, "Id", "Name", product.CategoryId);

            if (ModelState.IsValid)
            {
                product.Slug = product.Name.ToLower().Replace(" ", "-");

                var slug = await _Context.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The product already exists.");
                    return View(product);
                }

                if (product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;

                    string filePath = Path.Combine(uploadsDir, imageName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    product.Image = imageName;
                }

                _Context.Update(product);
                await _Context.SaveChangesAsync();

                TempData["Success"] = "The product has been Edited!";

            }

            return View(product);
        }
        public async Task<IActionResult> Delete(long id)
        {
            Product product = await _Context.Products.FindAsync(id);
            if (!string.Equals(product.Image, "noimage.png"))
            {
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                string oldImagePath = Path.Combine(uploadsDir,product.Image);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            _Context.Products.Remove(product);
            await _Context.SaveChangesAsync();
            TempData["Success"] = "The product has been Deleted!";

            return RedirectToAction("Index");
            
        }
    }
}
