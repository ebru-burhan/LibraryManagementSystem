using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Interactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class MemberReadingStatusConfiguration : AuditableConfiguration<MemberReadingStatus>
{
    public override void Configure(EntityTypeBuilder<MemberReadingStatus> builder)
    {
        base.Configure(builder);

        builder.ToTable("MemberReadingStatuses");

        // Bir üyenin bir kitap için sadece bir okuma durumu satırı olabilir
        builder.HasIndex(mrs => new { mrs.MemberId, mrs.BookId }).IsUnique();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50); // "CurrentlyReading", "Read" gibi sabit metinler için yeterli

        builder.Property(x => x.StartedAt).IsRequired(false);
        builder.Property(x => x.CompletedAt).IsRequired(false);

        builder.HasOne(mrs => mrs.Member)
            .WithMany()
            .HasForeignKey(mrs => mrs.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(mrs => mrs.Book)
            .WithMany()
            .HasForeignKey(mrs => mrs.BookId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}