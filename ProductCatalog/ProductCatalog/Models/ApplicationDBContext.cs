using Microsoft.EntityFrameworkCore;

namespace ProductCatalog.Models{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext (DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = default!;  
        public DbSet<User> Users { get; set; } = default!;  

    }


}