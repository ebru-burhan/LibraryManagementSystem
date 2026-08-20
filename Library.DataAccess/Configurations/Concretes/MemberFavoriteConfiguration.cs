using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Interactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class MemberFavoriteConfiguration : AuditableConfiguration<MemberFavorite>
{
    public override void Configure(EntityTypeBuilder<MemberFavorite> builder)
    {
        base.Configure(builder);
        builder.ToTable("MemberFavorites");

        // Bir üye aynı kitabı ikinci kez favoriye ekleyemez
        builder.HasIndex(mf => new { mf.MemberId, mf.BookId }).IsUnique();

        builder.HasOne(mf => mf.Member)
            .WithMany()
            .HasForeignKey(mf => mf.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(mf => mf.Book)
            .WithMany()
            .HasForeignKey(mf => mf.BookId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}