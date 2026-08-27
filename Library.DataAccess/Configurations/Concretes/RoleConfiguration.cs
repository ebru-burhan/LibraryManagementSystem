using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class RoleConfiguration : AuditableConfiguration<Role>
{
    public override void Configure(EntityTypeBuilder<Role> builder)
    {
        // Id, CreatedAt, IsDeleted vb. tüm kurallar otomatik gelir
        base.Configure(builder);

        builder.ToTable("Roles");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .IsRequired(false)
            .HasMaxLength(250);


        //Permissions ayarı!!!!
        // Yetkiler virgülle ayrılmış uzun bir metin olabileceği için uzunluğu artırdık
        builder.Property(x => x.Permissions)
            .IsRequired(false)
            .HasMaxLength(1000);


        // Rol adı benzersiz olmalı
        builder.HasIndex(x => x.Name).IsUnique();
    }
}