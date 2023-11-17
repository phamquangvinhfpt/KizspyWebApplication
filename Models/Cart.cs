using App.Models;
namespace KizspyWebApp.Models
{
    public class Cart
    {
        public Guid Id { get; set; }
        public string AppUserId { get; set; } //Required foreign key property
        public AppUser AppUser { get; set; } = null!; //Required reference navigation to principal
        public ICollection<CartItem> cartItems { get; set; } = new List<CartItem>(); //Collection navigation to dependent
    }
}