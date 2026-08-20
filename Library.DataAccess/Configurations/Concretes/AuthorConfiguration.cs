using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class AuthorConfiguration : AuditableConfiguration<Author>
{
    public override void Configure(EntityTypeBuilder<Author> builder)
    {
        base.Configure(builder);
        builder.ToTable("Authors");

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Biography)
            .IsRequired(false)
            .HasMaxLength(2000);
    }
}