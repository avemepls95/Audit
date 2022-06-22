using Microsoft.EntityFrameworkCore;
using School.Audit.Db;

namespace School.Audit._Db.Tests
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SomeClass>()
                .ToTable("SomeClass")
                .HasKey(c => c.Id);
            
            modelBuilder.Entity<AnotherClass>()
                .ToTable("AnotherClass")
                .HasKey(c => c.Id);
            
            modelBuilder.ApplyConfiguration(new DbAuditConfiguration());
        }
    }
}