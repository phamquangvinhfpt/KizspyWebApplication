namespace KizspyWebApp.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public double Price { get; set; }
        public int Qty { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool Status { get; set; }
        public List<Category> Categories { get; set; } = new();
    }
}