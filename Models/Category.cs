namespace KizspyWebApp.Models
{
    public class Category
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; }
        public List<Product> Products { get; set; } = new();
    }
}