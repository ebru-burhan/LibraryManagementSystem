using Library.Entity.Abstract;

public class BookCategory : BaseEntity
{
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}