using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class LoanStatusConfiguration : LookupConfiguration<LoanStatus>
{
    public override void Configure(EntityTypeBuilder<LoanStatus> builder)
    {
        base.Configure(builder);
        builder.ToTable("LoanStatuses");
    }
}