using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using School.Audit.Models;

namespace School.Audit.Db
{
    public class DbAuditConfiguration : IEntityTypeConfiguration<AuditItem>
    {
        public void Configure(EntityTypeBuilder<AuditItem> builder)
        {
            builder.ToTable("AuditItem");

            builder.HasKey(i => i.Id);
            builder.Property(i => i.OperationType).HasConversion<string>();
        }
    }
}