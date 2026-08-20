using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class PenaltyTypeConfiguration : LookupConfiguration<PenaltyType>
{
    public override void Configure(EntityTypeBuilder<PenaltyType> builder)
    {
        base.Configure(builder);
        builder.ToTable("PenaltyTypes");
    }
}