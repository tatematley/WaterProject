using Microsoft.EntityFrameworkCore;

namespace WaterProject.API.data
{
    public class WaterDbContext : DbContext
    {
        public WaterDbContext(DbContextOptions<WaterDbContext> options) : base(options)
        {
        }
        
        public DbSet<Project> Projects { get; set; }
    }
}


