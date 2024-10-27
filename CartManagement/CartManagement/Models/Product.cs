using System.ComponentModel.DataAnnotations;

namespace CartManagement.Models
{
    public class Product
    { // AND SO ON
        [Key]
        public int ProdId { get; set; }
        public string ProdName { get; set; }
        public string ProdDesc { get; set; }
        public int ProdPrice { get; set; }
    }
}
