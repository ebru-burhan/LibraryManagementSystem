using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class UserConfiguration : AuditableConfiguration<User> 
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.ToTable("Users");

        // Çekirdek Kişisel Bilgiler
        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(50);

        // TC Kimlik No hem 11 karakter olmalı hem de sistemde benzersiz olmalı
        builder.Property(x => x.IdentityNumber)
            .IsRequired()
            .HasMaxLength(11);
        builder.HasIndex(x => x.IdentityNumber).IsUnique();

        // Hesap Bilgileri
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(100);
        builder.HasIndex(x => x.Email).IsUnique();

        builder.Property(x => x.PasswordHash).IsRequired();
        builder.Property(x => x.PasswordSalt).IsRequired();

        // İletişim Bilgileri (Zorunlu değiller ama uzunluk sınırı şart)
        builder.Property(x => x.PhoneNumber)
            .IsRequired(false)
            .HasMaxLength(20);

        builder.Property(x => x.Address)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Not: Member ilişkisi (Bire-Bir) genellikle MemberConfiguration içinde yönetilir, çünkü her member bir userdir her user bi member değildir :))
        // o yüzden burayı temiz bırakıyoruz.
    }
}