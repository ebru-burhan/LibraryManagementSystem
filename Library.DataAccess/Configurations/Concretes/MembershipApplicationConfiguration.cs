using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Membership;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class MembershipApplicationConfiguration : AuditableConfiguration<MembershipApplication>
{
    public override void Configure(EntityTypeBuilder<MembershipApplication> builder)
    {
        base.Configure(builder);

        builder.ToTable("MembershipApplications");

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(50);

  

        builder.Property(x => x.IdentityNumber)
            .IsRequired()
            .HasMaxLength(11);
        
        builder.HasIndex(x => x.IdentityNumber).IsUnique();

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Address)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(x => x.PictureUrl)
       .HasMaxLength(500)
       .IsRequired(false);


        // Başvuru Durumu İlişkisi (one to many)
        builder.HasOne(ma => ma.ApplicationStatus)
            .WithMany() // ApplicationStatus sınıfında List<MembershipApplication> tutmana gerek yok, tek yönlü yeterli
            .HasForeignKey(ma => ma.ApplicationStatusId)
            .OnDelete(DeleteBehavior.Restrict); // Durum silinirse, başvurular etkilenmesin (hata versin)



        // User İlişkisi
        builder.HasOne(ma => ma.User)
            .WithMany() // Bir User'ın iptal edilenlerle birlikte geçmişte birden fazla başvurusu olabilir
            .HasForeignKey(ma => ma.UserId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
