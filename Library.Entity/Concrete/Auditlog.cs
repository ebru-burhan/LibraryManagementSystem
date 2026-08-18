namespace Library.Entity.Concrete;

using Library.Entity.Abstract;

public class AuditLog : CreationAuditedEntity
{
    // TODO: daha action type tablo olur heralde string olasın da sonra bak
    public string ActionType { get; set; } = null!; // Create, Update, Delete, Login 
    public string TableName { get; set; } = null!;  // İşlemin yapıldığı tablo (Örn: "Books", "Users")

    // Eski ve yeni değerleri JSON formatında string olarak tutmak en esnek yöntemdir.
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }

    public string IpAddress { get; set; } = null!; // İşlemi yapan cihazın IP adresi
}