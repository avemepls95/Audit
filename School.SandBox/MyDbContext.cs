using Microsoft.EntityFrameworkCore;
using School.Audit.Db;
using School.SandBox.Models;

namespace School.SandBox
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
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