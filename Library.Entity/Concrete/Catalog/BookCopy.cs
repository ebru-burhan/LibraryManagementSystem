using Library.Entity.Abstract;
using Library.Entity.Concrete.Lookups;

namespace Library.Entity.Concrete.Catalog;

public class BookCopy : AuditableEntity
{
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;

    public string Barcode { get; set; } = null!; //LM-100234 unique olmalı ama.
    public string ShelfLocation { get; set; } = null!; // Raf 

    // Kitabın anlık durumu (Rafta, Ödünçte, Tamirde, Kayıp)
    //foreign key
    public int StatusId { get; set; }
    // EF, ilişkileri anlaması için gereken Navigation Property
    public BookStatus Status { get; set; } //(Lookup tablosunda
}