using Library.Entity.Abstract;
using Library.Entity.Constants;



namespace Library.Entity.Concrete.System;

public class AuditLog : CreationAuditedEntity
{
    public string ActionType { get; set; } = null!; // Create, Update, Delete, Login 
    public string TableName { get; set; } = null!;  // İşlemin yapıldığı tablo (Örn: "Books", "Users")

    // Eski ve yeni değerleri JSON formatında string olarak tutmak en esnek yöntemdir.
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }

    public string IpAddress { get; set; } = null!; // İşlemi yapan cihazın IP adresi
}