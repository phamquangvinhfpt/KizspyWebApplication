﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using KizspyWebApp.Models;
using KizspyWebApp.Response;
using Microsoft.AspNetCore.Identity;
using App.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using KizspyWebApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace KizspyWebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly KizspyDbContext _context;
    private UserManager<AppUser> _userManager;

    public HomeController(KizspyDbContext context, ILogger<HomeController> logger, UserManager<AppUser> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Payment()
    {
        return View();
    }
    
    public IActionResult LandingPage()
    {
        LandingProductModel model = new LandingProductModel();
        //Get list model mobile product
        model.MobileProduct = _context.Products.Where(x => x.Status == true && x.Categories.Any(c => c.CategoryName.Contains("Điện thoại"))).ToList();
        //Get list model smart watch product
        model.SmartWatchProduct = _context.Products.Where(x => x.Status == true && x.Categories.Any(c => c.CategoryName.Contains("Đồng hồ"))).ToList();
        return View("Landing", model);
    }

    [HttpPost]
    public async Task<IActionResult> GetFilteredProducts(Guid[] categoriesId)
    {
        var model = new ProductModel();
        model.SelectedCategoryIds = categoriesId.ToList();
        model.currentPage = 1;
        var qr = _context.Products.Where(x => x.Status == true).OrderBy(p => p.Name);
        if(categoriesId != null && categoriesId.Length > 0)
        {
            qr = (IOrderedQueryable<Product>)qr.Where(p => p.Categories.Any(c => categoriesId.Contains(c.Id)));
        }
        model.totalProducts = qr.Count();
        model.countPages = (int)Math.Ceiling((double)model.totalProducts / model.ITEMS_PER_PAGE);
        if (model.currentPage < 1)
            model.currentPage = 1;
        if (model.currentPage > model.countPages)
            model.currentPage = model.countPages;
        var qr1 = qr.Skip((model.currentPage - 1) * model.ITEMS_PER_PAGE).Take(model.ITEMS_PER_PAGE).Select(p => new Product
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Qty = p.Qty,
            Description = p.Description,
            Image = p.Image,
            Status = p.Status,
            Categories = p.Categories
        });
        model.products = await qr1.ToListAsync();
        return PartialView("_ProductList", model);
    }

    public async Task<IActionResult> Index([FromQuery(Name = "page")] int currentPage)
    {
        ViewBag.Categories = _context.Categories.ToList();
        //var products = _context.Products.Where(x => x.Status == true).ToList();
        var model = new ProductModel();
        model.currentPage = currentPage;

        var qr = _context.Products.Where(x => x.Status == true).OrderBy(p => p.Name);

        model.totalProducts = await qr.CountAsync();
		model.countPages = (int)Math.Ceiling((double)model.totalProducts / model.ITEMS_PER_PAGE);

		if (model.currentPage < 1)
			model.currentPage = 1;
		if (model.currentPage > model.countPages)
			model.currentPage = model.countPages;

        var qr1 = qr.Skip((model.currentPage - 1) * model.ITEMS_PER_PAGE).Take(model.ITEMS_PER_PAGE)
            .Select(p => new Product
            {
				Id = p.Id,
				Name = p.Name,
				Price = p.Price,
				Qty = p.Qty,
				Description = p.Description,
				Image = p.Image,
				Status = p.Status,
				Categories = p.Categories
			});

        model.products = await qr1.ToListAsync();
		return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public async Task<IActionResult> Payment([FromBody] CassoResponse response)
    {
        var re = Request;
        var headers = re.Headers;

        try
        {
            if (headers["secure-token"] == "phamquangvinh" && headers["Content-Type"] == "application/json")
            {
                foreach (var item in response.Data)
                {
                    if (item != null)
                    {
                        //Add Transaction
                        var transactionEntity = new CassoTransaction
                        {
                            Tid = item.Tid,
                            Description = item.Description,
                            Amount = item.Amount,
                            CusumBalance = item.CusumBalance,
                            When = item.When,
                            BankSubAccId = item.BankSubAccId,
                            SubAccId = item.SubAccId,
                            VirtualAccount = item.VirtualAccount,
                            VirtualAccountName = item.VirtualAccountName,
                            CorresponsiveName = item.CorresponsiveName,
                            CorresponsiveAccount = item.CorresponsiveAccount,
                            CorresponsiveBankId = item.CorresponsiveBankId,
                            CorresponsiveBankName = item.CorresponsiveBankName
                        };
                        await _context.CassoTransactions.AddAsync(transactionEntity);
                        await _context.SaveChangesAsync();
                        var user = _userManager.Users.FirstOrDefault(x => GetKizspyCode(item.Description).Contains(x.Casso_Code));
                        //Add System Transaction
                        if (user != null)
                        {
                            var systemTransactionEntity = new SystemTransaction
                            {
                                CreateAt = DateTime.Now.ToUniversalTime(),
                                CassoTranId = transactionEntity.Id,
                                UserId = Guid.Parse(user.Id),
                                Amount = transactionEntity.Amount ?? 0,
                                TransactionType = "Cộng tiền!",
                                TotalBalance = (decimal)(user.Amount + transactionEntity.Amount),
                                Description = transactionEntity.Description
                            };
                            //dưới 200.000đ trừ 10.000đ
                            if(transactionEntity.Amount <= 200000)
                            {
                                systemTransactionEntity.TotalBalance = (decimal)(user.Amount + transactionEntity.Amount - 10000);
                            }
                            user.Amount = systemTransactionEntity.TotalBalance;
                            await _context.SystemTransactions.AddAsync(systemTransactionEntity);
                            //update user
                            await _userManager.UpdateAsync(user);
                            await _context.SaveChangesAsync();
                            return Ok(systemTransactionEntity);
                        } else
                        {
                            var systemTransactionEntity = new SystemTransaction
                            {
                                CreateAt = DateTime.Now.ToUniversalTime(),
                                CassoTranId = transactionEntity.Id,
                                UserId = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                                Amount = transactionEntity.Amount ?? 0,
                                TransactionType = "Cộng tiền!",
                                TotalBalance = (decimal)(transactionEntity.Amount),
                                Description = transactionEntity.Description
                            };
                            await _context.SystemTransactions.AddAsync(systemTransactionEntity);
                            await _context.SaveChangesAsync();
                            return Ok(systemTransactionEntity);
                        }
                    }
                }
            }    
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        return StatusCode(StatusCodes.Status400BadRequest);
    }

	[HttpPost]
    public async Task<IActionResult> CassoAsync()
    {
		string bankAccId = "0948190073";
		string apiKey = "<phamquangvinh"; // Replace with your API key
		using (HttpClient client = new HttpClient())
		{
			client.DefaultRequestHeaders.Add("Content-Type", "application/json");
			client.DefaultRequestHeaders.Add("Authorization", apiKey);
			var requestBody = new
			{
				bank_acc_id = bankAccId
			};
			var response = await client.PostAsJsonAsync("https://oauth.casso.vn/v2/sync", requestBody);
			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadAsStringAsync();
				return Ok(result);
			}
			else
			{
				return StatusCode(StatusCodes.Status400BadRequest);
			}
		}
	}

	public static string GetKizspyCode(string description)
    {
        // regex pattern: Kizspy\s([a-zA-Z0-9\s]+)
        Regex regex = new Regex(@"Kizspy\s([a-zA-Z0-9\s]+)");
        Match match = regex.Match(description);

        if (match.Success)
        {
            string kizspyCode = "Kizspy " + match.Groups[1].Value.Replace(" ", "");
            return kizspyCode;
        }

        return "";
    }
}
