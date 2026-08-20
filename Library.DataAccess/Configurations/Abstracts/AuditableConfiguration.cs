using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Library.Entity.Abstract;

namespace Library.DataAccess.Configurations.Abstracts;

public abstract class AuditableConfiguration<T> : CreationAuditedConfiguration<T> where T : AuditableEntity
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        base.Configure(builder); // Id ve CreatedAt kurallarını getir

        // Geriye sadece Auditable'a özel kurallar kaldı
        builder.Property(x => x.IsDeleted)
               .IsRequired()
               .HasDefaultValue(false);

        //is deleted true diyince gene görünür  silinenler o yüzden global query filter yapcaz 
        //toList() dediğim anda Ef core arka planda sql e otomatik olarak where isDeleted = 0 şartını ekler

        builder.HasQueryFilter(x => !x.IsDeleted); // TODO: COK ONEMLİ--> generic repo yaparken delete i değiştircez.20.08 notunda var.


        //eğer admin falan silinenleri isterse de örn;   _context.Users.IgnoreQueryFilters().ToList(); kullancaz

    }
}