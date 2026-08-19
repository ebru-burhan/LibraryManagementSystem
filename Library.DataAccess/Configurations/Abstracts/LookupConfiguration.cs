using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Abstract;
using Library.Entity.Concrete.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations; // Klasör yapına göre namespace'i güncelleyebilirsin

public abstract class LookupConfiguration<T> : BaseConfiguration<T> where T : LookupEntity
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        // 1. Önce üst sınıfın (BaseConfiguration) Id kuralını çalıştır
        base.Configure(builder);

        // 2. Tüm lookup tablolarında ortak olan standart kurallar
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(30);

        // Kod alanı sistem genelinde benzersiz olmalı (Örn: İki tane aynı 'AVAILABLE' kodu olamaz)
        //BookStatuses tablosunda AVAILABLE kodlu bir satır olabilir.
       //LoanStatuses tablosunda da AVAILABLE kodlu bir satır olabilir. unique olması iki tabloyu etkilemez o kolonda 2 tane availbale olmaz demek
        builder.HasIndex(x => x.Code).IsUnique();
    }
}