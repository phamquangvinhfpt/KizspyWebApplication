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

namespace KizspyWebApp.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(ViewData["NameSortParm"] as string) ? "name_desc" : "";
            ViewData["PriceSortParm"] = String.IsNullOrEmpty(ViewData["PriceSortParm"] as string) ? "price_desc" : "";
            ViewData["QtySortParm"] = String.IsNullOrEmpty(ViewData["QtySortParm"] as string) ? "qty_desc" : "";
            ViewData["CurrentFilter"] = searchString;
            var products = from s in _context.Products
						   select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Name.Contains(searchString)
                                                      || s.Description.Contains(searchString));
            }
            switch (sortOrder)
            {
				case "name_desc":
					products = products.OrderByDescending(s => s.Name);
					break;
				case "price_desc":
					products = products.OrderByDescending(s => s.Price);
					break;
				case "qty_desc":
					products = products.OrderByDescending(s => s.Qty);
					break;
				default:
					products = products.OrderBy(s => s.Name);
					break;
			}
            return View(await products.Include("Categories").AsNoTracking().ToListAsync());
            //return _context.Products != null ? 
            //              View(await _context.Products.Include("Categories").ToListAsync()) :
            //              Problem("Entity set 'KizspyDbContext.Products'  is null.");
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
                    Status = product.Status,
                    Categories = product.Categories
                };
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
                Status = product.Status,
                Categories = product.Categories
            };
            return View(productViewModel);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = RoleName.Administrator + "," + RoleName.Editor)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Price,Qty,Description,Image,Status")] ProductViewModel product)
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
                        Status = product.Status,
                        Categories = product.Categories
                    };
                    _context.Update(Product);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'KizspyDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
