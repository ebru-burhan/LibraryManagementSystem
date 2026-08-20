using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class LoanConfiguration : AuditableConfiguration<Loan>
{
    public override void Configure(EntityTypeBuilder<Loan> builder)
    {
        base.Configure(builder);
        builder.ToTable("Loans");

        builder.Property(x => x.LoanDate).IsRequired();
        builder.Property(x => x.DueDate).IsRequired();
        builder.Property(x => x.ReturnDate).IsRequired(false);

        // İlişkiler (Geçmiş veriyi korumak için Cascade yerine Restrict kullanıyoruz)
        builder.HasOne(x => x.Member)
            .WithMany(m => m.Loans)
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.BookCopy)
            .WithMany()
            .HasForeignKey(x => x.BookCopyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}