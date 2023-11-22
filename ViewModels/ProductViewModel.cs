using KizspyWebApp.Models;

namespace KizspyWebApp.ViewModels
{
    public class ProductViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Price { get; set; } = null!;
        public int Qty { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public bool Status { get; set; }
        public List<Guid> CategoryIds { get; set; }
        public List<Category> Categories { get; set; } = new();
    }
}
