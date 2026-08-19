using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Library.Entity.Abstract;

namespace Library.DataAccess.Configurations;

public abstract class AuditableConfiguration<T> : CreationAuditedConfiguration<T> where T : AuditableEntity
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        base.Configure(builder); // Id ve CreatedAt kurallarını getir

        // Geriye sadece Auditable'a özel kurallar kaldı
        builder.Property(x => x.IsDeleted)
               .IsRequired()
               .HasDefaultValue(false);
    }
}