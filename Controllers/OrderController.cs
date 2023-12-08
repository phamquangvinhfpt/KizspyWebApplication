using App.Models;
using KizspyWebApp.Models;
using KizspyWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KizspyWebApp.Controllers
{
	[Authorize]
	public class OrderController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly KizspyDbContext _context;
		private UserManager<AppUser> _userManager;

		public OrderController(KizspyDbContext context, ILogger<HomeController> logger, UserManager<AppUser> userManager)
		{
			_logger = logger;
			_context = context;
			_userManager = userManager;
		}
		public IActionResult Index()
		{
			OrderModel model = new OrderModel();
			//get all model of user
			var orders = _context.Orders.Where(x => x.AppUserId == _userManager.GetUserId(User)).ToList();
			model.Orders = orders;
			//get all order detail of orders
			var orderDetails = _context.OrderDetails.Where(x => x.Order.AppUserId == _userManager.GetUserId(User)).ToList();
			model.OrderDetails = orderDetails;
			return View(model);
		}

		[HttpGet]
		[Route("Order/Detail/{id}")]
		public IActionResult Detail(Guid id)
		{
            OrderDetailModel model = new OrderDetailModel();
			//get order
			var order = _context.Orders.FirstOrDefault(x => x.Id == id);
			if (order == null)
			{
				return NotFound();
			}
			model.Order = order;
			//get order detail
			var orderDetails = _context.OrderDetails.Where(x => x.OrderId == id).ToList();
			model.OrderDetails = orderDetails;
			return View(model);
        }
	}
}
