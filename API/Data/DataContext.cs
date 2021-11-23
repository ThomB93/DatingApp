using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        public DataContext() : base()
        {

        }

        public DbSet<AppUser> Users { get; set; }
    }
}
