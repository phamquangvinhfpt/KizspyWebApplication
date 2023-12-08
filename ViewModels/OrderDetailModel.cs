using KizspyWebApp.Models;

namespace KizspyWebApp.ViewModels
{
    public class OrderDetailModel
    {
        public Order Order { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
