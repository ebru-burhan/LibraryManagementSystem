using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class BookCategoryConfiguration : BaseConfiguration<BookCategory>
{
    public override void Configure(EntityTypeBuilder<BookCategory> builder)
    {
        base.Configure(builder);

        builder.ToTable("BookCategories");

        // Bir kitaba aynı kategori birden fazla kez eklenemez
        builder.HasIndex(bc => new { bc.BookId, bc.CategoryId }).IsUnique();

        builder.HasOne(bc => bc.Book).WithMany(b => b.BookCategories)
               .HasForeignKey(bc => bc.BookId).OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bc => bc.Category).WithMany(c => c.BookCategories)
               .HasForeignKey(bc => bc.CategoryId).OnDelete(DeleteBehavior.Restrict);
    }
}
