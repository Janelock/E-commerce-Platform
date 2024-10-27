using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Models
{
    public class Product
    {
        [Key]
        public int ProdId { get; set; }
        public string ProdName { get; set; }
        public string ProdDesc { get; set; }
        public int ProdPrice { get; set; }
    }
}
