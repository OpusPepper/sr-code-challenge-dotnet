using challenge.Models;
using Microsoft.EntityFrameworkCore;

namespace challenge.Data
{
    public class CompensationContext : DbContext
    {
        public CompensationContext(DbContextOptions<CompensationContext> options) : base(options)
        {

        }

        public DbSet<CompensationDb> Compensations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CompensationDb>()
                .HasKey(k => k.EmployeeId);
        }
    }
}
