using Library.DataAccess.Configurations.Abstracts;
using Library.Entity.Concrete.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Concretes;

public class BookCopyConfiguration : AuditableConfiguration<BookCopy>
{
    public override void Configure(EntityTypeBuilder<BookCopy> builder)
    {
        base.Configure(builder);

        builder.ToTable("BookCopies");

        builder.Property(x => x.Barcode).IsRequired().HasMaxLength(50);
        builder.HasIndex(x => x.Barcode).IsUnique();

        //kitabın raftaki yeri sabit ödünç verdin geri getirdiler nere koycan aha buraya
        builder.Property(x => x.ShelfLocation).IsRequired().HasMaxLength(50);



        builder.HasOne(bc => bc.Book)
               .WithMany(b => b.BookCopies)
               .HasForeignKey(bc => bc.BookId)
               .OnDelete(DeleteBehavior.Cascade); // Kitap silinirse fiziksel kopyaları da silinsin

        builder.HasOne(bc => bc.Status)
               .WithMany() // Status (Lookup) tarafında listeye gerek yok
               .HasForeignKey(bc => bc.StatusId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict); 
        // Statü sistemden silinemez statüler auditable soft delete ama dbden silinebiirdi burda onu engelledik restrict ile.
    }
}