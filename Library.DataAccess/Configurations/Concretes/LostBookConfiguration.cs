using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class LostBookConfiguration : AuditableConfiguration<LostBook>
{
    public override void Configure(EntityTypeBuilder<LostBook> builder)
    {
        base.Configure(builder);
        builder.ToTable("LostBooks");

        // Virgülden sonra 2 hane (Kuruş) hassasiyeti
        builder.Property(x => x.BookValue)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.HasOne(x => x.Member)
            .WithMany()
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.BookCopy)
            .WithMany()
            .HasForeignKey(x => x.BookCopyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}