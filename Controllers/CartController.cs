using App.Data;
using App.Models;
using KizspyWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KizspyWebApp.Controllers
{
	public class CartController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly KizspyDbContext _context;
		private UserManager<AppUser> _userManager;

		public CartController(KizspyDbContext context, ILogger<HomeController> logger, UserManager<AppUser> userManager)
		{
			_logger = logger;
			_context = context;
			_userManager = userManager;
		}


		[HttpPost]
		public async Task<IActionResult> AddProductToCart(Guid productId)
		{
			//admin can't add product to cart
			if (User.IsInRole(RoleName.Administrator))
			{
				return BadRequest("Admin can't add product to cart");
			}
			//check product exist
			var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);
			if (product == null)
			{
                return NotFound();
            }
			//Add product to cart
			var cart = await _context.Carts.FirstOrDefaultAsync(x => x.AppUserId == _userManager.GetUserId(User));
			if (cart == null)
			{
				cart = new Cart
				{
					AppUserId = _userManager.GetUserId(User)
				};
				_context.Carts.Add(cart);
			}

			//Add cart item
			var cartItem = await _context.CartItems.FirstOrDefaultAsync(x => x.ProductId == productId && x.CartId == cart.Id);
			if (cartItem == null)
			{
				cartItem = new CartItem
				{
					ProductId = productId,
					CartId = cart.Id,
					ProductName = product.Name,
				};
				//Quantity + 1
				cartItem.Quantity = 1;
				//Calculate price
				cartItem.Price = cartItem.Quantity * product.Price;
				_context.CartItems.Add(cartItem);
			} else
			{
				//Add Product to cart
				cartItem.ProductId = productId;
				cartItem.CartId = cart.Id;
				cartItem.ProductName = product.Name;
				//Quantity + 1
                cartItem.Quantity += 1;
                //Calculate price
                cartItem.Price = cartItem.Quantity * product.Price;
				//update cart item
				_context.CartItems.Update(cartItem);
			}

			//return list cart item
			await _context.SaveChangesAsync();
			//return cart dang json
			return Ok(cart.cartItems);
		}

		[HttpGet]
		[Authorize(Roles = RoleName.Member)]
		public IActionResult ViewCart()
		{
			var cart = _context.Carts.Include(x => x.cartItems).FirstOrDefault(x => x.AppUserId == _userManager.GetUserId(User));
			return View(cart);
		}

		[HttpPost]
		[Route("/RemoveProductFromCart")]
		public async Task<IActionResult> RemoveProductFromCart(Guid productId)
		{
            try
			{
                var cart = await _context.Carts.FirstOrDefaultAsync(x => x.AppUserId == _userManager.GetUserId(User));
                if (cart == null)
				{
                    return NotFound("cart not found");
                }
                var cartItem = await _context.CartItems.FirstOrDefaultAsync(x => x.ProductId == productId && x.CartId == cart.Id);
                if (cartItem == null)
				{
                    return NotFound("cart item not found");
                }
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                return Ok(cart.cartItems);
            } catch (Exception e)
			{
                return BadRequest(e.Message);
            }
        }

		[HttpGet]
		[Route("/GetCartCount")]
		public async Task<IActionResult> GetCartCount()
		{
			var cart = await _context.Carts.FirstOrDefaultAsync(x => x.AppUserId == _userManager.GetUserId(User));
			if (cart == null)
			{
				//return count = 0
				return Json(new { count = 0 });
			}
			var count = await _context.CartItems.CountAsync(x => x.CartId == cart.Id);
			return Json(new { count = count });
		}

		[HttpPost]
		[Route("/CreateSubscription")]
		public async Task<IActionResult> CreateSubscription(string[] productIds, int[] qtys, decimal total)
		{
			try
			{
				//check product quantity is enough
				for (int i = 0; i < productIds.Length; i++)
				{
					var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == Guid.Parse(productIds[i]));
					if (product == null)
					{
						return Json(new { status = 400, message = "Product not found" });
					}
					if (product.Qty < qtys[i])
					{
						return Json(new { status = 400, message = "Product quantity is not enough" });
					}
				}

				//check user amount >= total
				var user = await _userManager.GetUserAsync(User);
				if (user.Amount < total)
				{
					return Json(new { status = 400, message = "Your amount is not enough" });
				}
				//create order
				var order = new Order
				{
					CreateTime = DateTime.Now,
					TotalPrice = total,
					Status = true,
					AppUserId = _userManager.GetUserId(User)
				};
				_context.Orders.Add(order);
				//create order detail
				for (int i = 0; i < productIds.Length; i++)
				{
					var orderDetail = new OrderDetail
					{
						OrderId = order.Id,
						ProductId = Guid.Parse(productIds[i]),
						Quantity = qtys[i]
					};
					//Product Name
					var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == orderDetail.ProductId);
					//Price in order detail = price in product
					orderDetail.Price = product.Price;
					//Calculate total price
					orderDetail.TotalPrice = orderDetail.Quantity * orderDetail.Price;
					orderDetail.ProductName = product.Name;
					_context.OrderDetails.Add(orderDetail);
				}
				//update user amount
				user.Amount -= total;
				_context.Users.Update(user);
				//update product quantity
				for (int i = 0; i < productIds.Length; i++)
				{
					var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == Guid.Parse(productIds[i]));
					product.Qty -= qtys[i];
					//if product quantity = 0 => product status = false
					if (product.Qty == 0)
					{
						product.Status = false;
					}
					_context.Products.Update(product);
				}

				//update cart
				var cart = await _context.Carts.FirstOrDefaultAsync(x => x.AppUserId == _userManager.GetUserId(User));
				if (cart != null)
				{
					_context.Carts.Remove(cart);
				}
				//save change
				await _context.SaveChangesAsync();
				return Json(new { status = 200, message = "Subscription created successfully", orderId = order.Id });
			}
			catch (Exception e)
			{
				return Json (new { status = 400, message = e.Message });
			}
		}
	}
}
