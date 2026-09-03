namespace Library.Model.Dtos.Catalog;

public class BookListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string ISBN { get; set; } = null!;
    public string Publisher { get; set; } = null!;
    public int PublicationYear { get; set; }
    public string? CoverImageUrl { get; set; }
    //Yazar listesi BookAuthor ara tablosundan geldiği için  query ile 
    // Yazarları tutacağımız liste
    public List<string> Authors { get; set; } = new List<string>();

}