using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class BookAuthorConfiguration : BaseConfiguration<BookAuthor>
{
    public override void Configure(EntityTypeBuilder<BookAuthor> builder)
    {
        base.Configure(builder);

        builder.ToTable("BookAuthors");

        // Bir kitaba aynı yazar birden fazla kez eklenemez
        builder.HasIndex(ba => new { ba.BookId, ba.AuthorId }).IsUnique();

        builder.HasOne(ba => ba.Book).WithMany(b => b.BookAuthors)
               .HasForeignKey(ba => ba.BookId).OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ba => ba.Author).WithMany(a => a.BookAuthors)
               .HasForeignKey(ba => ba.AuthorId).OnDelete(DeleteBehavior.Restrict);
    }
}