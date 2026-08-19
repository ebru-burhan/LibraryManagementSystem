using Library.Entity.Abstract;

public class Author : AuditableEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Biography { get; set; }

    // direk yazarı koymama sebebim editor falan eklenirse yazılmış kodu değiştirmeyiz
    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}