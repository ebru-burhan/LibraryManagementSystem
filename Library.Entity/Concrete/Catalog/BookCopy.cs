using Library.Entity.Abstract;

public class BookCopy : AuditableEntity
{
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;

    public string Barcode { get; set; } = null!; //LM-100234 unique olmalı ama.
    public string ShelfLocation { get; set; } = null!; // Raf 

    // Kitabın anlık durumu (Rafta, Ödünçte, Tamirde, Kayıp)
    public int StatusId { get; set; }
    // public LookupStatus Status { get; set; } (Lookup tablosu ile bağlanacak) /// bu kısmı anlamadım
}