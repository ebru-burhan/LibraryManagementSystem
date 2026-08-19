using Library.Entity.Abstract;

namespace Library.Entity.Concrete.Catalog;

public class Book : AuditableEntity
{
    public string Title { get; set; } = null!;
    public string ISBN { get; set; } = null!;
    public string Publisher { get; set; } = null!; 
    public int PublicationYear { get; set; } 
    public int PageCount { get; set; } 
    public string? Description { get; set; } 
    public string? CoverImageUrl { get; set; } 

    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();

    // Bir kitabın birden fazla fiziksel kopyası olabilir
    public ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();
}