using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class MembershipTypeConfiguration : LookupConfiguration<MembershipType>
{
    public override void Configure(EntityTypeBuilder<MembershipType> builder)
    {
        base.Configure(builder);
        builder.ToTable("MembershipTypes");
    }
}
