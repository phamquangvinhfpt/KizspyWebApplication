using KizspyWebApp.ViewModels;

namespace KizspyWebApp.Response
{
    public class CassoResponse
    {
        public int Error { get; set; }
        public List<TransactionViewModel> Data { get; set; }
        public CassoResponse()
        {
            Error = 0;
            Data = new List<TransactionViewModel>();
        }
    }
}
