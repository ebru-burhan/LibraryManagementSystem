using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class AuditLogConfiguration : CreationAuditedConfiguration<AuditLog>
{
    public override void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        base.Configure(builder);

        builder.ToTable("AuditLogs");

        builder.Property(x => x.ActionType)
            .IsRequired()
            .HasMaxLength(50); // Create, Update, Delete, Login vb.

        builder.Property(x => x.TableName)
            .IsRequired()
            .HasMaxLength(100);

        // JSON formatında tutulacak veriler sınırsız MAX
        builder.Property(x => x.OldValues)
            .IsRequired(false);

        builder.Property(x => x.NewValues)
            .IsRequired(false);

        // IPv6 max 45 karakter olur
        builder.Property(x => x.IpAddress)
            .IsRequired()
            .HasMaxLength(45);
    }
}