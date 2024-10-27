using Microsoft.EntityFrameworkCore;

namespace CartManagement.Models{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext (DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = default!;  
        public DbSet<User> Users { get; set; } = default!;  
        public DbSet<Cart> Carts { get; set; } = default!;  
// DB TABLESS ONLYY
    }


}