using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class ReservationConfiguration : AuditableConfiguration<Reservation>
{
    public override void Configure(EntityTypeBuilder<Reservation> builder)
    {
        base.Configure(builder);
        builder.ToTable("Reservations");

        builder.Property(x => x.QueueNumber).IsRequired();

        // Aynı üye, aynı kitabı aynı anda iki kere rezerve edemesin
        builder.HasIndex(x => new { x.MemberId, x.BookId, x.StatusId })
            .HasFilter("[StatusId] = 1") // Sadece 'Bekliyor' statüsünde olanlar için bu benzersizlik geçerli olsun
            .IsUnique();

        builder.HasOne(x => x.Member)
            .WithMany(m => m.Reservations)
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Book)
            .WithMany()
            .HasForeignKey(x => x.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}