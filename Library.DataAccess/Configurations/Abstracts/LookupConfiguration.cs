using Library.Entity.Abstract;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Abstracts;

public abstract class LookupConfiguration<T> : AuditableConfiguration<T> where T : LookupEntity
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        
        base.Configure(builder);

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