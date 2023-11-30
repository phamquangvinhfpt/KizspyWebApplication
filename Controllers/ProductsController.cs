using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KizspyWebApp.Models;
using KizspyWebApp.Services;
using KizspyWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using App.Data;
using X.PagedList;
using KizspyWebApp.Data;

namespace KizspyWebApp.Controllers
{
    //[Authorize]
    public class ProductsController : Controller
    {
        private readonly KizspyDbContext _context;
        private readonly IPhotoService _photoService;

        public ProductsController(KizspyDbContext context, IPhotoService photoService)
        {
            _context = context;
            _photoService = photoService;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return _context.Products != null ?
                          View(await _context.Products.Include("Categories").ToListAsync()) :
                          Problem("Entity set 'KizspyDbContext.Products'  is null.");
        }

        [HttpPost]
        public async Task<IActionResult> getProducts()
        {
            var products = await _context.Products.Include("Categories").ToListAsync();
            return Json(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = RoleName.Administrator + "," + RoleName.Editor)]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories
                              .Select(x => new SelectListItem
                              {
                                  Value = x.Id.ToString(),
                                  Text = x.CategoryName
                              })
                              .ToList();
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Administrator + "," + RoleName.Editor)]
        public async Task<IActionResult> Create(ProductViewModel product)
        {
            if (ModelState.IsValid)
            {
                product.Id = Guid.NewGuid();
                var result = await _photoService.AddPhotoAsync(product.Image);
                var Product = new Product
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Qty = product.Qty,
                    Description = product.Description,
                    Image = result.Url.ToString(),
                    Categories = product.Categories
                };
                if (Product.Qty > 0)
                {
                    Product.Status = true;
                }
                _context.Products.Add(Product);

                foreach (var catId in product.CategoryIds)
                {
                    var productCategory = new ProductCategory
                    {
                        ProductId = product.Id,
                        CategoryId = catId
                    };

                    _context.ProductCategories.Add(productCategory);
                    }
                    await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = RoleName.Administrator + "," + RoleName.Editor)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var productViewModel = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Qty = product.Qty,
                Description = product.Description,
                Image = null,
                Status = product.Status
            };
            //Get all categories
            var categories = await _context.Categories.ToListAsync();
            //Get all categories for this product
            var productCategories = await _context.ProductCategories.Where(x => x.ProductId == id).ToListAsync();
            //viewbag
            ViewBag.Categories = categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.CategoryName,
                Selected = productCategories.Any(y => y.CategoryId == x.Id)
            }).ToList();
            return View(productViewModel);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = RoleName.Administrator + "," + RoleName.Editor)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ProductViewModel product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _photoService.AddPhotoAsync(product.Image);
                    var Product = new Product
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Qty = product.Qty,
                        Description = product.Description,
                        Image = result.Url.ToString(),
                        Categories = product.Categories
                    };
                    if (Product.Qty > 0)
                    {
                        Product.Status = true;
                    }

                    _context.Update(Product);

                    //Update productcategories
                    var productCategories = await _context.ProductCategories.Where(x => x.ProductId == id).ToListAsync();
                    _context.ProductCategories.RemoveRange(productCategories);
                    foreach (var catId in product.CategoryIds)
                    {
                        var productCategory = new ProductCategory
                        {
                            ProductId = product.Id,
                            CategoryId = catId
                        };
                        _context.ProductCategories.Add(productCategory);
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = RoleName.Administrator + "," + RoleName.Editor)]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [Authorize(Roles = RoleName.Administrator + "," + RoleName.Editor)]
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Products == null)
            {
                return Json(new { success = false, message = "Entity set 'KizspyDbContext.Products'  is null." });
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            } else
            {
                return Json(new { success = false, message = "Product not found." });
            }
            
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Delete success." });
        }

        private bool ProductExists(Guid id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public IActionResult Paging(int page=1)
        {
            page=page<1?1:page;
            int pageSize = 3;
            var products = _context.Products.Include("Categories").ToPagedList(page,pageSize);
            return View(products);
        }
    }
}
