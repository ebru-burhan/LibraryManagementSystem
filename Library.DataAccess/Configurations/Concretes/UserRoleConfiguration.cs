using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class UserRoleConfiguration : BaseConfiguration<UserRole> 
{
    public override void Configure(EntityTypeBuilder<UserRole> builder)
    {
        base.Configure(builder);

        builder.ToTable("UserRoles");

        // Kritik Kural: Aynı kullanıcıya aynı rol birden fazla kez verilemez!
        builder.HasIndex(ur => new { ur.UserId, ur.RoleId }).IsUnique();

        // İlişkiler
        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Kullanıcı silinirse rolleri de uçsun

        builder.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Restrict); // İçi dolu olan (kullanıcısı olan) rol silinemez
    }
}