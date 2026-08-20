using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class CategoryConfiguration : AuditableConfiguration<Category>
{
    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        base.Configure(builder);

        builder.ToTable("Categories");

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(x => x.Name).IsUnique(); // Aynı isimde iki kategori olmasın

        // Parent-Child
        builder.HasOne(c => c.ParentCategory)
               .WithMany(c => c.SubCategories)
               .HasForeignKey(c => c.ParentCategoryId)
               .OnDelete(DeleteBehavior.Restrict); // Üst kategori silindiğinde altındakiler otomatik silinmesin, hata versin
        // TODO: üst kategori silinmeye çalışıldığında alt kategorileri yoksa sil diye bişiler eklicez sanırım repoya 
    }
}