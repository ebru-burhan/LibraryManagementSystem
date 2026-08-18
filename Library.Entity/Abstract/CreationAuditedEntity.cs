namespace Library.Entity.Abstract;

public abstract class CreationAuditedEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedByUserId { get; set; }
}