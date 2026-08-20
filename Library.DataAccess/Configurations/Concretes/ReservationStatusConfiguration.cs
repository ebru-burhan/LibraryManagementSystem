using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class ReservationStatusConfiguration : LookupConfiguration<ReservationStatus>
{
    public override void Configure(EntityTypeBuilder<ReservationStatus> builder)
    {
        base.Configure(builder);
        builder.ToTable("ReservationStatuses");
    }
}