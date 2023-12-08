using KizspyWebApp.Models;

namespace KizspyWebApp.ViewModels
{
	public class ProductModel
	{
		public int totalProducts { get; set; }
		public int countPages { get; set; }

		public int ITEMS_PER_PAGE { get; set; } = 9;

		public int currentPage { get; set; }

		public List<Product> products { get; set; }
        public List<Guid> SelectedCategoryIds { get; set; }
    }
}
