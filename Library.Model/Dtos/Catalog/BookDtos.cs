namespace Library.Model.Dtos.Catalog;


public class BookListDto : BaseExternalDto
{
    public string Title { get; set; } = null!;

    //Yazar listesi BookAuthor ara tablosundan geldiği için  query ile 
    // Yazarları tutacağımız liste
    public List<string> AuthorsNameList { get; set; } = new List<string>();
}

public class BookCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}

public class BookAuthorDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
}

public class BookDetailDto : BookListDto
{
    public string ISBN { get; set; } = null!;
    public string Publisher { get; set; } = null!;
    public int PublicationYear { get; set; }
    public string? CoverImageUrl { get; set; }
    // Tıklanabilir (Guid taşıyan) yazar listesi detayda yer alıyor
    public List<BookAuthorDto> Authors { get; set; } = new();
    public List<BookCategoryDto> Categories { get; set; } = new();
}

public class CreateBookDto
{   
    public string Title { get; set; } = null!;
    public string ISBN { get; set; } = null!;
    public string Publisher { get; set; } = null!;
    public int PublicationYear { get; set; }
    public int PageCount { get; set; }

    // Kitap eklerken yazarların Guid'lerini React'ten alacağız
    public List<Guid> AuthorIds { get; set; } = new();
    public List<Guid> CategoryIds { get; set; } = new();
}