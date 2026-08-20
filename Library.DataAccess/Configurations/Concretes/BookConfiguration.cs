using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class BookConfiguration : AuditableConfiguration<Book>
{
    public override void Configure(EntityTypeBuilder<Book> builder)
    {
        base.Configure(builder);
        builder.ToTable("Books");

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ISBN)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(x => x.ISBN)
            .IsUnique(); // ISBN benzersiz olmalı

        builder.Property(x => x.Publisher)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.PublicationYear)
            .IsRequired();

        builder.Property(x => x.PageCount)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired(false)
            .HasMaxLength(2000);

        builder.Property(x => x.CoverImageUrl)
            .IsRequired(false).
            HasMaxLength(500);
    }
}