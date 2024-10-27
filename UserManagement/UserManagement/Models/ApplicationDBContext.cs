using Microsoft.EntityFrameworkCore;

namespace UserManagement.Models{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext (DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }

        public DbSet<PostMe> PostMe { get; set; } = default!;
        public DbSet<User> Users { get; set; } = default!;            

    }


}