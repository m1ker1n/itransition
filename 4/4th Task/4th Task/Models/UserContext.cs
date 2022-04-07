using Microsoft.EntityFrameworkCore;

namespace _4th_Task.Models
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public UserContext(DbContextOptions<UserContext> options) 
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
