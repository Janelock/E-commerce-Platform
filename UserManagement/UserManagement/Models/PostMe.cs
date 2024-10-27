using System.ComponentModel.DataAnnotations;

namespace UserManagement.Models
{
    public class PostMe
    {
        [Key]
        public int ID { get; set; }
        public string Post { get; set; }
    }
}
