using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class BookStatusConfiguration : LookupConfiguration<BookStatus>
{
    public override void Configure(EntityTypeBuilder<BookStatus> builder)
    {
        base.Configure(builder);
        builder.ToTable("BookStatuses");
    }
}