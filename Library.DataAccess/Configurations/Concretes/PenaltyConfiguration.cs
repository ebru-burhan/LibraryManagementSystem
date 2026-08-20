using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class PenaltyConfiguration : AuditableConfiguration<Penalty>
{
    public override void Configure(EntityTypeBuilder<Penalty> builder)
    {
        base.Configure(builder);
        builder.ToTable("Penalties");

        // Finansal hassasiyet
        builder.Property(x => x.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.PaidDate).IsRequired(false);

        builder.HasOne(x => x.Member)
            .WithMany(m => m.Penalties)
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Loan)
            .WithMany(l => l.Penalties)
            .HasForeignKey(x => x.LoanId)
            .IsRequired(false) // Ceza her zaman bir ödünç işlemine bağlı olmayabilir (Örn: İçeride gürültü yapma cezası)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PenaltyType)
            .WithMany()
            .HasForeignKey(x => x.PenaltyTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}