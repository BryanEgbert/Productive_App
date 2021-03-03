using Global.Protos;
using Microsoft.EntityFrameworkCore;

namespace BackEnd
{
    public class UserContext:DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }

        public DbSet<UserInfo> UserDb { get; set; }
        public DbSet<ToDoItemList> ToDoListDb  { get; set; }
        public DbSet<ToDoStructure> ToDoDb { get; set; }
    }
}