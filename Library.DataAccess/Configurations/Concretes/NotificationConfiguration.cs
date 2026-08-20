using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class NotificationConfiguration : CreationAuditedConfiguration<Notification>
{
    public override void Configure(EntityTypeBuilder<Notification> builder)
    {
        base.Configure(builder);

        builder.ToTable("Notifications");

        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.IsRead)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.ReadAt)
            .IsRequired(false);

        // İlişki: Bir Bildirim Bir Kullanıcıya Aittir
        builder.HasOne(n => n.User)
            .WithMany() // User sınıfında ICollection<Notification> yok, bu yüzden burası boş kalıyor.
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Eğer kullanıcı sistemden (hard delete ile) silinirse bildirimleri de temizlensin.
    }
}