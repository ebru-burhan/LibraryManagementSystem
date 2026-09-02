using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Membership;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class MemberConfiguration : AuditableConfiguration<Member>
{
    public override void Configure(EntityTypeBuilder<Member> builder)
    {
        base.Configure(builder);

        builder.ToTable("Members");

        // MemberNumber zorunlu ve unique olacak
        builder.Property(x => x.MemberNumber)
            .IsRequired()
            .HasMaxLength(30); //LUM-2023-001

        builder.HasIndex(x => x.MemberNumber).IsUnique();

        builder.HasOne(m => m.Status)
            .WithMany()
            .HasForeignKey(m => m.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        //  one to one
        // Her üye bir kullanıcıdır ama her kullanıcı bir üye olmak zorunda değildir
        builder.HasOne(m => m.User)
            .WithOne(u => u.Member) // User sınıfındaki "public Member? Member" alanına bağlanır
            .HasForeignKey<Member>(m => m.UserId) // Foreign Key, Member tablosunda durur!
            .OnDelete(DeleteBehavior.Cascade); // Eğer User silinirse, Member kaydı da silinsin

        // Her üye tek bir başvurudan gelir; her başvuru en fazla bir üye üretir.
        // Restrict: başvuru silinince üye kaydı sessizce yok olmasın.
        builder.HasOne(m => m.MembershipApplication)
            .WithOne(ma => ma.Member)
            .HasForeignKey<Member>(m => m.MembershipApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => m.MembershipApplicationId).IsUnique();

        // Not: Loans, Reservations ve Penalties (one to many) 
        // kendi configuration sınıfları (LoanConfiguration vb.) yazılırken ele alınacak.
    }
}