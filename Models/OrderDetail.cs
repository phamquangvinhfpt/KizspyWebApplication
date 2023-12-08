namespace KizspyWebApp.Models
{
    public class OrderDetail
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public Order Order { get; set; } = null!; //Required reference navigation to principal
    }
}