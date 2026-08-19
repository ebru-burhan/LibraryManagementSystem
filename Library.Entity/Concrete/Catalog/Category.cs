using Library.Entity.Abstract;
namespace Library.Entity.Concrete.Catalog;

public class Category : AuditableEntity
{
    public string Name { get; set; } = null!;

    // Alt Kategori parent child process gibi 
    public int? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();

    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
}