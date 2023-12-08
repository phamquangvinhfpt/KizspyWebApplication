namespace KizspyWebApp.Models
{
    public class CartItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public Guid CartId { get; set; } //Required foreign key property
        public Cart Cart { get; set; } = null!; //Required reference navigation to principal
    }
}