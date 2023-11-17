namespace KizspyWebApp.Models
{
    public class OrderDetail
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid CartId { get; set; }
        public Order Order { get; set; } = null!; //Required reference navigation to principal
    }
}