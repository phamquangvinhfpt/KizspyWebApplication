using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KizspyWebApp.Models
{
    public class SystemTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime CreateAt { get; set; }
        public int CassoTranId { get; set; }
        public Guid? UserId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public decimal TotalBalance { get; set; }
        public string Description { get; set; }
    }
}
