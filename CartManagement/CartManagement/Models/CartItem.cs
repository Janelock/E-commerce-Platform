using System.ComponentModel.DataAnnotations;

namespace CartManagement.Models
{
    public class Cart
    {
        [Key]
        public int CartItemId { get; set; }
        public string OwnerEmail { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int TotalPrice { get; set; }
    }
}
