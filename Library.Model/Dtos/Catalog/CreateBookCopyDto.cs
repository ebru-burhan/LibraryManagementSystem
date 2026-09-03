public class CreateBookCopyDto
{
    public int BookId { get; set; } // Doğrudan int ID alıyoruz
    public string Barcode { get; set; } = null!;
    public string ShelfLocation { get; set; } = null!;
}