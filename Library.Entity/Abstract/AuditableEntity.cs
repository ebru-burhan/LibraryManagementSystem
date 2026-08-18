namespace Library.Entity.Abstract;


public abstract class AuditableEntity : CreationAuditedEntity, ISoftDelete
{
    // Sadece Dış Güvenlik (Guid), Güncelleme ve Silme (Soft Delete) özelliklerini ekliyoruz.
    public Guid ExternalId { get; set; } = Guid.NewGuid();

    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}