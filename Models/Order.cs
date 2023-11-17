using App.Models;

namespace KizspyWebApp.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public double TotalPrice { get; set; }
        public bool Status { get; set; }
        public OrderDetail? orderDetail { get; set; }
        public string AppUserId { get; set; } //Required foreign key property
        public AppUser User { get; set; } = null!; //Required reference navigation to principal
    }
}