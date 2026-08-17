namespace Library.Entity.Abstract;

public abstract class BaseEntity : IEntity, ISoftDelete
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}