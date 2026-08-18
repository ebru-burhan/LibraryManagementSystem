namespace Library.Entity.Abstract;

public abstract class ImmutableEntity : BaseEntity
{
    // Güncelleme veya silinme takibi (Soft Delete) yok log tablolar için
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedByUserId { get; set; }
}