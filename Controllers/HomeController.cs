using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using KizspyWebApp.Models;
using KizspyWebApp.Response;
using Microsoft.AspNetCore.Identity;
using App.Models;

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
    public IActionResult Index()
    {
        return View();
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
                        var user = _userManager.Users.FirstOrDefault(x => x.Casso_Code == item.Description);
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
                            user.Amount = systemTransactionEntity.TotalBalance;
                            await _context.SystemTransactions.AddAsync(systemTransactionEntity);
                            _userManager.UpdateAsync(user);
                            await _context.SaveChangesAsync();
                        }
                    }    
                }
                return Ok(new SystemTransaction());
            }    
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        return StatusCode(StatusCodes.Status400BadRequest);
    }
}
