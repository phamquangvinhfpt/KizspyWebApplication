using KizspyWebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace KizspyWebApp.ViewModels
{
    public class ProductViewModel
    {
        public Guid Id { get; set; }
        [Required (ErrorMessage = "Name is required")]
        public string Name { get; set; } = null!;
        [Required (ErrorMessage = "Price is required")]
        [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public double Price { get; set; }
        [Required (ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Qty { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public bool Status { get; set; }
        public List<Guid> CategoryIds { get; set; }
        public List<Category> Categories { get; set; } = new();
    }
}
