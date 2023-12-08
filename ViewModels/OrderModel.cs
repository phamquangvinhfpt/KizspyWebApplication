using KizspyWebApp.Models;

namespace KizspyWebApp.ViewModels
{
	public class OrderModel
	{
		public List<Order> Orders { get; set; }
		public List<OrderDetail> OrderDetails { get; set; }
	}
}
