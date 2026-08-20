using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.System;

namespace Library.DataAccess.Configurations.Concretes;
public class SettingConfiguration : AuditableConfiguration<Setting>
{
    public override void Configure(EntityTypeBuilder<Setting> builder)
    {
        // 1. Üst sınıftaki ortak kuralları (Id, ExternalId, CreatedAt, IsDeleted) uygula
        base.Configure(builder);


        // 2. Sadece Setting tablosuna özel kurallar
        // Tablo Adı
        builder.ToTable("Settings");

        //Kolon Kısıtlamaları (Kritik nokta: Bunları yazmazsak SQL hepsini NVARCHAR(MAX) yapar, bu da performansı bitirir)

        builder.Property(x => x.Key)
            .IsRequired() // Boş geçilemez (NOT NULL)
            .HasMaxLength(100); // SQL'de NVARCHAR(100) olur

        builder.Property(x => x.Value)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Description)
            .IsRequired(false) // Zorunlu değil (NULL)
            .HasMaxLength(1000);
    }
}