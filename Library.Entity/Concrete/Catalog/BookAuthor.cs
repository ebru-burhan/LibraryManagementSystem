using Library.Entity.Abstract;
namespace Library.Entity.Concrete.Catalog;

public class BookAuthor : BaseEntity
{

    // many to many için bir kitabın 1den fazla yazarı olabiliyor. ara ablo ile bağladık.
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;
}

