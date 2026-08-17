namespace Library.Entity.Abstract;

public abstract class AuditableEntity : BaseEntity, IAuditable
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedByUserId { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
}