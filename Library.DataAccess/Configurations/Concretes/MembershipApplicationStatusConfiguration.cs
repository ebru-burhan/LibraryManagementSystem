using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class MembershipApplicationStatusConfiguration : LookupConfiguration<MembershipApplicationStatus>
{
    public override void Configure(EntityTypeBuilder<MembershipApplicationStatus> builder)
    {
        base.Configure(builder);
        builder.ToTable("MembershipApplicationStatuses");
    }
}