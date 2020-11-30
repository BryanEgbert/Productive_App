using Microsoft.EntityFrameworkCore;

namespace BackEnd.Models
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        public DbSet<UserModel> UserModels { get; set; }
    }
}