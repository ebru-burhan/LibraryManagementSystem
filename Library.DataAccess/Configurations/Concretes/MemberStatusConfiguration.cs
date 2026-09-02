using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class MemberStatusConfiguration : LookupConfiguration<MemberStatus>
{
    public override void Configure(EntityTypeBuilder<MemberStatus> builder)
    {
        base.Configure(builder);
        builder.ToTable("MemberStatuses");
    }
}
