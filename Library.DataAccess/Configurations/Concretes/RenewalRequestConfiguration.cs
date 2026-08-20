using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class RenewalRequestConfiguration : AuditableConfiguration<RenewalRequest>
{
    public override void Configure(EntityTypeBuilder<RenewalRequest> builder)
    {
        base.Configure(builder);
        builder.ToTable("RenewalRequests");

        builder.Property(x => x.NewDueDate).IsRequired();

        builder.HasOne(x => x.Loan)
            .WithMany(l => l.RenewalRequests)
            .HasForeignKey(x => x.LoanId)
            .OnDelete(DeleteBehavior.Cascade); // Ödünç işlemi tamamen (Hard Delete) silinirse, talebi de silebiliriz.
    }
}