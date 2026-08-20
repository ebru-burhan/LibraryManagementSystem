using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Interactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class BookReviewConfiguration : AuditableConfiguration<BookReview>
{
    public override void Configure(EntityTypeBuilder<BookReview> builder)
    {
        base.Configure(builder);

        builder.ToTable("BookReviews");

        // Rating sadece 1, 2, 3, 4 veya 5 olabilir. SQL seviyesinde kısıtlama!
        builder.ToTable(t => t.HasCheckConstraint("CK_BookReview_Rating", "[Rating] >= 1 AND [Rating] <= 5"));
        builder.Property(x => x.Rating).IsRequired();

        builder.Property(x => x.Comment)
            .IsRequired(false) // Yorum yapmak zorunlu değil, sadece puan verebilir
            .HasMaxLength(1000);

        // İlişkiler
        builder.HasOne(br => br.Member)
            .WithMany() // Member sınıfında List<BookReview> yok İÇİ BOŞ KALCAK AMA DURSUN
            .HasForeignKey(br => br.MemberId)
            .OnDelete(DeleteBehavior.Cascade); // Üye silinirse yorumları da gitsin

        builder.HasOne(br => br.Book)
            .WithMany()
            .HasForeignKey(br => br.BookId)
            .OnDelete(DeleteBehavior.Cascade); // Kitap silinirse yorumları da gitsin
    }
}