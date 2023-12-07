using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using KizspyWebApp.Models;
using KizspyWebApp.Response;
using Microsoft.AspNetCore.Identity;
using App.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        return View("Landing");
    }

    public IActionResult Index()
    {
        ViewBag.Categories = _context.Categories.ToList();
        var products = _context.Products.Where(x => x.Status == true).ToList();
        return View(products);
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
