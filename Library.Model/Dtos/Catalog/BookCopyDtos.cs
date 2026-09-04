namespace Library.Model.Dtos.Catalog; // Projendeki namespace yapısına göre düzenleyebilirsin

public class CreateBookCopyDto
{
    public Guid BookId { get; set; }
    public string Barcode { get; set; } = null!;
    public string ShelfLocation { get; set; } = null!;
}

// 3. Listeleme (List) İşlemi İçin DTO (UI'da tabloda göstereceğimiz alanlar)
public class BookCopyListDto : BaseExternalDto
{
    public string Barcode { get; set; } = null!;

    public string BookTitle { get; set; } = null!; //(Gelecekte Book'tan joinlenecek)
    public DateTime CreatedAt { get; set; }
    public string StatusName { get; set; } = null!;

    public List<string> BookAuthorsNameList { get; set; } = new(); //sadece adları olacak kitap adı altında yazarlar daha kçk 
}

public class BookCopyDetailDto : BookCopyListDto
{
    //yine book dan fotograf description falan filan alınabilir ilerde 

    public List<BookAuthorDto> BookAuthors { get; set; } = new();   // Yazar bilgisi (Gelecekte Book'tan joinlenecek)

    public string ShelfLocation { get; set; } = null!;// Ne zaman eklendiği vb.
}

